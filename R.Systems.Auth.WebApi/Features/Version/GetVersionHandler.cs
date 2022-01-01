using R.Systems.Shared.Core.Interfaces;
using System.Reflection;

namespace R.Systems.Auth.WebApi.Features.Version;

public class GetVersionHandler : IDependencyInjectionScoped
{
    public string Handle()
    {
        string version = Assembly.GetExecutingAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion
            .ToString() ?? "";
        return version;
    }
}
