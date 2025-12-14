using System.Text.Json.Serialization;

/// <summary>
/// 基础响应结果 (仅包含状态码和消息，不包含数据)
/// 用于失败响应或不需要返回数据的成功响应
/// </summary>
public class AdminResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess => Code == 0 ;

    /// <summary>
    /// 状态码 0 成功  其他 失败
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    // 【关键修改】基类中移除 Data 属性，彻底消除二义性。

    // ==========================================
    // 静态工厂方法 (统一入口)
    // ==========================================

    /// <summary>
    /// 1. 返回成功 - 无数据
    /// </summary>
    public static AdminResult Success(string message = "")
    {
        return new AdminResult { Code = 0, Message = message };
    }

    /// <summary>
    /// 2. 返回成功 - 带数据 (泛型)
    /// </summary>
    public static AdminResult<T> Success<T>(T data, string message = "")
    {
        return new AdminResult<T> { Code = 0, Message = message, Data = data };
    }

    /// <summary>
    /// 3. 返回失败 - 无数据 (通常失败不需要带数据)
    /// </summary>
    public static AdminResult Fail(string message, int code = 1)
    {
        return new AdminResult { Code = code, Message = message };
    }

    /// <summary>
    /// 4. 返回失败 - 带特定泛型类型 (为了满足接口返回值类型要求)
    /// 例如：接口定义返回 AdminResult<User>，但发生了错误，需要返回同类型对象
    /// </summary>
    public static AdminResult<T> Fail<T>(string message, int code = 1)
    {
        return new AdminResult<T> { Code = code, Message = message, Data = default };
    }
}

/// <summary>
/// 泛型响应结果 (继承基类，额外增加 Data 字段)
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class AdminResult<T> : AdminResult
{
    /// <summary>
    /// 数据载荷
    /// 这里是唯一的 Data 定义，没有任何隐藏或重写，干净纯粹
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; }
}