using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace GatewayService.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
            .Build();

        builder.Services.AddOcelot(configuration);

        var app = builder.Build();

        app.UseWebSockets();
        await app.UseOcelot();
        await app.RunAsync();
    }
}