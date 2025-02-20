using App.Context;
using App.Services;
using Microsoft.EntityFrameworkCore;

namespace App.Main;

public class Program
{
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
    // Les services
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<RespGenerator>();

    // Configuration de PostgreSQL
        builder.Services.AddDbContext<MyDbContext>(opt =>
            opt.UseNpgsql(
                builder.Configuration.GetConnectionString("Default")
            )
        );
    // Ajouter les contrôleurs
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.WebHost.UseUrls("http://0.0.0.0:5000");

        var app = builder.Build();

    // Configuration des middlewares
        if (!app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = "swagger"; // Accès via URL racine
            });
            // app.UseHsts();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = "swagger"; // Accès via URL racine
            });
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
