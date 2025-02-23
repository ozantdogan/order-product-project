using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OTD.Repository.Abstract;

namespace OTD.Extensions
{
    public static class DependencyInjection
    {
        public static void RegisterRepositories(this IServiceCollection services)
        {
            var assembly = typeof(IProductRepository).Assembly;

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"));

            foreach (var implType in types)
            {
                var interfaceType = implType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == "I" + implType.Name);

                if (interfaceType != null)
                {
                    services.AddTransient(interfaceType, implType);
                }
            }
        }
    }
}
