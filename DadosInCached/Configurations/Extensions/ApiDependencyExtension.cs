using DadosInCached.Context;
using DadosInCached.Interfaces;
using DadosInCached.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DadosInCached.Configurations.Extensions
{
    public static class ApiDependencyExtension
    {
        public static void AddApiDependencyServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDependecyInjectinos();
            services.AddAppDbContext(config);
            services.AddAutoMapperConfig();
        }

        public static void AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options
                => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        }

        public static void AddDependecyInjectinos(this IServiceCollection services)
        {
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
        }

        public static void AddAutoMapperConfig(this IServiceCollection services)
          => services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}
