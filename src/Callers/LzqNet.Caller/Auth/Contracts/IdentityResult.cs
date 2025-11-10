namespace LzqNet.Caller.Auth.Contracts;
public class IdentityResult<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
