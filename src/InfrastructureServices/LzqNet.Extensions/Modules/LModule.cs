namespace LzqNet.Extensions.Modules;

public abstract class LModule
{
    public virtual void PreConfigureServices(WebApplicationBuilder builder)
    {
    }

    public virtual void ConfigureServices(WebApplicationBuilder builder)
    {
    }

    public virtual void PostConfigureServices(WebApplicationBuilder builder)
    {
    }

    public virtual void OnPreApplicationInitialization(WebApplication app)
    {
    }

    public virtual void OnApplicationInitialization(WebApplication app)
    {
    }

    public virtual void OnPostApplicationInitialization(WebApplication app)
    {
    }

    public virtual void OnApplicationShutdown(WebApplication app)
    {
    }
}
