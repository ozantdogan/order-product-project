using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OTD.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AutoRegisterDependencies(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            RegisterImplementations(services, assemblies, "Repository");
            RegisterImplementations(services, assemblies, "Service");

            return services;
        }

        private static void RegisterImplementations(IServiceCollection services, Assembly[] assemblies, string suffix)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var interfaceType in types.Where(t => t.IsInterface && t.Name.EndsWith(suffix)))
                {
                    var implementationType = types.FirstOrDefault(t =>
                        t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

                    if (implementationType != null)
                    {
                        services.AddScoped(interfaceType, implementationType);
                    }
                }
            }
        }
    }
}
