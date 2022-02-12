using System.Reflection;
using R.Systems.Shared.Core.Interfaces;

namespace R.Systems.Auth.WebApi.Features.Version;

public class GetVersionHandler : IDependencyInjectionScoped
{
    public string Handle()
    {
        string version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "";
        return version;
    }
}
