using Microsoft.EntityFrameworkCore;
using RestAuth.Business.Services;
using RestAuth.Data.Context;
using RestAuth.Domain.Interfaces.Repositories;
using RestAuth.Domain.Interfaces.Services;
using RestAuth.Infra.Data.Repositories;

namespace RestAuth.Api.Dependency_Injection
{
    public class InjectionFactory
    {
        private static IServiceCollection Services { get; set; }

        private static string ConnectionString { get; set; }

        public static IServiceCollection Configure(IServiceCollection services,
             IConfiguration configuration)
        {
            Services = services;
            ConnectionString = configuration.GetValue<string>("CONNECTION_STRING");
            Services.AddDbContext<RestAuthContext>(options => options.UseMySql(ConnectionString,
                ServerVersion.AutoDetect(ConnectionString)
                , options =>
                {
                    options.EnableRetryOnFailure(3);
                })
            );

            LoadServicesAndRepositories();

            return Services;
        }

        private static void LoadServicesAndRepositories()
        {
            #region Services

            // Fazer aqui o mapping pra injeção de dependência dos services
            Services.AddScoped<IRoleService, RoleService>();
            Services.AddScoped<IUserService, UserService>();

            Services.AddScoped<IOAuthService, OAuthService>();
            Services.AddScoped<IJWTService, JWTService>();

            #endregion Services

            #region Repositories

            // Fazer aqui o mapping pra injeção de dependência dos repositories
            Services.AddScoped<IRoleRepository, RoleRepository>();
            Services.AddScoped<IUserRepository, UserRepository>();

            #endregion Repositories
        }
    }
}