using LIN.Notes.Persistence.Access;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LIN.Notes.Persistence.Extensions;

public static class PersistenceExtensions
{

    /// <summary>
    /// Agregar servicios de persistence.
    /// </summary>
    /// <param name="services">Services.</param>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddDbContextPool<DataContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("release"));
        });

        services.AddScoped<NoteAccess, NoteAccess>();
        services.AddScoped<Access.Notes, Access.Notes>();
        services.AddScoped<Access.Profiles, Access.Profiles>();

        return services;
    }


    /// <summary>
    /// Habilitar el servicio de base de datos.
    /// </summary>
    public static IApplicationBuilder UseDataBase(this IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        try
        {
            var context = scope.ServiceProvider.GetService<DataContext>();
            context?.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<DataContext>>();
            logger?.LogCritical(ex, "Error al crear la estructura de la base de datos");
        }
        return app;
    }

}