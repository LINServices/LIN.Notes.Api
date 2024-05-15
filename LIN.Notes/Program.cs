using Http.Extensions;
using LIN.Notes.Data;
using LIN.Notes.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

try
{

    LIN.Access.Logger.Logger.AppName = "LIN.NOTES";

    builder.Services.AddSignalR();

    builder.Services.AddLINHttp();

    // Add services to the container.
    string sqlConnection = string.Empty;

#if RELEASE 
    sqlConnection = builder.Configuration["ConnectionStrings:Somee"] ?? string.Empty;
#elif DEBUG
    sqlConnection = builder.Configuration["ConnectionStrings:Somee"] ?? string.Empty;
#elif LOCAL
    sqlConnection = builder.Configuration["ConnectionStrings:Local"] ?? string.Empty;
#endif

    Conexión.SetStringConnection(sqlConnection);


    if (sqlConnection.Length > 0)
    {
        // SQL Server
        builder.Services.AddDbContext<Context>(options =>
        {
            options.UseSqlServer(sqlConnection);
        });
    }


    LIN.Access.Auth.Build.SetAuth(builder.Configuration["lin:app"] ?? string.Empty);

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IIam, Iam>();

    var app = builder.Build();

    try
    {
        // Si la base de datos no existe
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
        var res = dataContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        _ = LIN.Access.Logger.Logger.Log(ex, 3);
    }

    app.UseLINHttp();


    app.MapControllers();

    // Rutas de servicios de tiempo real
    app.MapHub<NotesHub>("/Realtime/notes");

    app.UseRouting();

    app.MapGet("/", () => "LIN APP Services esta funcionando");


    // Inicia las conexiones
    _ = Conexión.StartConnections();

    // Inicio de Jwt
    Jwt.Open();

    LIN.Access.Auth.Build.Init();


    app.Run();
}
catch (Exception ex)
{
    _ = LIN.Access.Logger.Logger.Log(ex, 4);
}