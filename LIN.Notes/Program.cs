using Http.Extensions;
using LIN.Access.Logger;
using LIN.Notes.Data;
using LIN.Notes.Services.Abstractions;

// Create a builder.
var builder = WebApplication.CreateBuilder(args);

// Services.
builder.Services.AddSignalR();
builder.Services.AddLINHttp();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIam, Iam>();

// Logger.
builder.Services.AddServiceLogging("LIN.NOTES");

// Database.
try
{
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
}
catch (Exception ex)
{
    // Log critico.
    LIN.Access.Logger.Services.Logger.Current.Log(ex, LIN.Access.Logger.Models.LogLevels.Critical);
}


LIN.Access.Auth.Build.SetAuth(builder.Configuration["lin:app"] ?? string.Empty);



var app = builder.Build();

// Agregar logging.
app.UseServiceLogging();

try
{
    // Si la base de datos no existe
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
    var res = dataContext.Database.EnsureCreated();
}
catch (Exception ex)
{
    LIN.Access.Logger.Services.Logger.Current.Log(ex, LIN.Access.Logger.Models.LogLevels.Critical);
}

app.UseLINHttp();


app.MapControllers();

// Rutas de servicios de tiempo real
app.MapHub<NotesHub>("/Realtime/notes");

app.UseRouting();

// Inicia las conexiones
_ = Conexión.StartConnections();

// Inicio de Jwt
Jwt.Open();

LIN.Access.Auth.Build.Init();

app.Run();