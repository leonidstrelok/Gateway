using ChatService.API.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Interfaces;

namespace ChatService.API.DAL;

public static class ServiceCollectionExtensions
{
    public static void AddDalServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(p =>
        {
            p.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IApplicationDbContext>(p => p.GetService<AppDbContext>()!);
    }
}