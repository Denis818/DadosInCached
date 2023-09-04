using Data.Repository;
using Domain.Interfaces.Repository;
using System.Reflection;

namespace DadosInCached.Configurations.Extensions
{
    public static class ApiDependencyExtension
    {
        public static void AddApiDependencyServices(this IServiceCollection services)
        {
            services.AddDependecyInjectinos();
            services.AddAutoMapperConfig();
        }

        public static void AddDependecyInjectinos(this IServiceCollection services)
        {
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
        }

        public static void AddAutoMapperConfig(this IServiceCollection services)
          => services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}
