using LzqNet.Caller.Msm.Contracts.DingtalkPushConfig.Enums;
using LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;
using LzqNet.Caller.Msm.Contracts.Events;
using LzqNet.Extensions.SqlSugar;
using LzqNet.QueueConsumers.Notifications.CommandHandlers.Contracts;
using LzqNet.QueueConsumers.Notifications.Entities;
using SqlSugar;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LzqNet.QueueConsumers.Notifications.CommandHandlers;

public class DingtalkMessageService : ISingletonDependency
{
    private readonly ISqlSugarClient sqlSugarClient = SqlSugarHelper.Client;

    public async Task ProcessHandleAsync(DingtalkMessageSendQueueEvent @event)
    {
        var entities = await sqlSugarClient.Queryable<DingtalkPushMessageRecordEntity>()
            .Where(a => @event.PushMessageRecordIds.Contains(a.Id))
            .ToListAsync();
        foreach (var entity in entities)
        {
            if (entity.PushConfigType == PushConfigTypeEnum.Group)
            {
                DDGroupMsgRequest dDGroupMsg = new();
                dDGroupMsg.msgtype = "markdown";

                DDMsgContent dMsgContent = new()
                {
                    title = entity.PushConfigName,
                    text = entity.PushContent
                };

                #region 群消息@人
                if (entity.DingtalkUserIds.Contains("all")) // @所有人
                {
                    dDGroupMsg.at = new AtUserInfo { isAtAll = true };
                }

                if ((entity.DingtalkUserIds?.Count > 0) || (entity.DingtalkUserIds?.Count > 0)) // @单个人
                {
                    dDGroupMsg.at = new AtUserInfo
                    {
                        atUserIds = entity.DingtalkUserIds ?? new List<string>(),
                        atMobiles = entity.DingtalkUserIds ?? new List<string>(),
                        isAtAll = false
                    };
                }
                #endregion

                #region 图片推送   暂时先不管图片推送
                //if (row.FineReportUrls?.Count > 0)
                //{
                //    var picUrls = new List<string>();
                //    var filesService = new FilesService();
                //    using var ftpService = new FtpServerHelper("192.168.104.21", "22", "", "ftp_interface", "ftp_interface");

                //    foreach (var fineUrl in row.FineReportUrls)
                //    {
                //        var fileName = $"{Guid.NewGuid():N}.PNG";
                //        var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TempImg", fileName);

                //        LogHelper.Info($"开始下载文件：{tempPath}");

                //        var flag = await filesService.DownLoadFileAsync(fineUrl, tempPath);

                //        LogHelper.Info($"结束下载文件：{tempPath}");

                //        if (flag)
                //        {
                //            var ftpUrl = $"mom/{DateTime.Now:yyyyMMdd}/{fileName}";
                //            LogHelper.Info($"开始上传文件到FTP：{ftpUrl}");

                //            var res = await ftpService.UploadFileAsync(tempPath, ftpUrl, true);

                //            LogHelper.Info($"结束上传文件到FTP：{ftpUrl}");

                //            if (res)
                //            {
                //                LogHelper.Info($"上传文件到FTP成功：{ftpUrl}");
                //                var httpUrl = $"http://183.162.196.231:5006/ftp/{ftpUrl}";
                //                picUrls.Add(httpUrl);
                //            }
                //        }
                //    }

                //    if (picUrls.Count > 0)
                //    {
                //        var picsText = string.Join(" ", picUrls.Select(q => $"![screenshot]({q})"));
                //        dMsgContent.text = $"{picsText}{Environment.NewLine}{dMsgContent.text}";
                //    }
                //}
                #endregion

                dDGroupMsg.markdown = dMsgContent;
                long timep = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string appsec = entity.Sign;
                string sign = await HashHmacAsync(timep.ToString(), appsec);
                string signstr = $"&timestamp={timep}&sign={sign}";

                // 使用TCP方式发送请求
                var url = new Uri(entity.Webhook + signstr);
                var responseJson = await SendTcpRequestAsync(url, dDGroupMsg);
                var pushResponse = JsonSerializer.Deserialize<DDResposeModel>(responseJson);

                if (pushResponse?.errcode == "0")
                {
                    entity.PushStatus = DingtalkPushStatusEnum.Success;
                    entity.PushReturnMessage = JsonSerializer.Serialize(pushResponse);
                }
                else
                {
                    entity.PushStatus = DingtalkPushStatusEnum.Failed;
                    entity.PushReturnMessage = JsonSerializer.Serialize(pushResponse);
                }
            }
        }
    }
    private async Task<string> SendTcpRequestAsync<T>(Uri uri, T requestData, Dictionary<string, string>? headers = null)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(uri.Host, uri.Port);

        using var stream = GetStream(client, uri);
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var jsonData = JsonSerializer.Serialize(requestData);
        var contentBytes = Encoding.UTF8.GetBytes(jsonData);

        // 构建HTTP请求
        var requestBuilder = new StringBuilder();
        requestBuilder.AppendLine($"POST {uri.PathAndQuery} HTTP/1.1");
        requestBuilder.AppendLine($"Host: {uri.Host}");
        requestBuilder.AppendLine("Connection: close");
        requestBuilder.AppendLine("Accept: application/json");

        if (headers != null)
        {
            foreach (var header in headers)
            {
                requestBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
        }

        requestBuilder.AppendLine($"Content-Length: {contentBytes.Length}");
        requestBuilder.AppendLine("Content-Type: application/json");
        requestBuilder.AppendLine();
        requestBuilder.Append(jsonData);

        // 发送请求
        var request = requestBuilder.ToString();
        await writer.WriteAsync(request);
        await writer.FlushAsync();

        // 读取响应
        var response = await reader.ReadToEndAsync();

        // 提取响应体（跳过HTTP头部）
        var bodyStart = response.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        if (bodyStart >= 0)
        {
            return response[(bodyStart + 4)..];
        }

        return response;
    }

    private Stream GetStream(TcpClient client, Uri uri)
    {
        var stream = client.GetStream();

        if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
        {
            var sslStream = new SslStream(stream, false);
            sslStream.AuthenticateAsClient(uri.Host);
            return sslStream;
        }

        return stream;
    }

    // HMAC加密方法（异步版本）
    private async Task<string> HashHmacAsync(string message, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var hashBytes = await hmac.ComputeHashAsync(new MemoryStream(messageBytes));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
