using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Common;
using MusicManager.Repositories.Data;

namespace MusicManager.DAL;

public static class Registrator
{
    public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration) => services
        .AddScoped<IDbInitializer, DbInitializer>()
        .AddDbContext<IApplicationDbContext, MusicManagerDbContext>(options =>
        {
            var dbType = configuration["Type"];
            switch (dbType)
            {
                case "MSSQL":
                    {
                        options
                        .UseSqlServer(configuration.GetConnectionString(dbType))
                        .UseSnakeCaseNamingConvention();
                        return;
                    }
                case "SQLITE":
                    {
                        options
                        .UseSqlite(configuration.GetConnectionString(dbType))
                        .UseSnakeCaseNamingConvention();
                        return;
                    }

                case null: throw new InvalidOperationException("Undefined database type.");
                default: throw new InvalidOperationException($"Database [{dbType}] is not supported.");
            }
        })
        ;
}
