using ChatService.API.BLL;
using ChatService.API.BLL.Services;
using ChatService.API.DAL;

namespace ChatService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddRouting(options => { options.LowercaseUrls = true; });
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDalServiceCollection(builder.Configuration);
        builder.Services.AddBllServiceCollection(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseWebSockets();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<ChatHub>("/chatHub");

        app.Run();
    }
}