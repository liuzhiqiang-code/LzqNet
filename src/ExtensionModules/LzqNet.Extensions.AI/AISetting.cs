namespace LzqNet.AI
{
    public class AISetting
    {  
        /// <summary>
        /// 配置Id
        /// </summary>
        public string ConfigId { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; } = "https://dashscope.aliyuncs.com/compatible-mode/v1/";


        /// <summary>
        /// 账户私钥
        /// </summary>
        public string KeySecret { get; set; } = "*****";

        /// <summary>
        /// 默认模型
        /// </summary>
        public string Model { get; set; } = "deepseek-v3";
    }
}
