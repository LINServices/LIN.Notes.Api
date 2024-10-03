using Http.Extensions;
using LIN.Access.Logger;
using LIN.Notes.Services.Abstractions;
using LIN.Access.Auth;
using LIN.Notes.Persistence.Extensions;

// Create a builder.
var builder = WebApplication.CreateBuilder(args);

// Services.
builder.Services.AddSignalR();
builder.Services.AddLINHttp();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIam, Iam>();
builder.Services.AddScoped<LangService, LangService>();
builder.Services.AddAuthenticationService();
builder.Services.AddPersistence(builder.Configuration);

// Logger.
builder.Services.AddServiceLogging("LIN.NOTES");

var app = builder.Build();

// Agregar logging.
app.UseServiceLogging();

app.UseLINHttp();
app.UseDataBase();

app.MapControllers();

// Rutas de servicios de tiempo real
app.MapHub<NotesHub>("/Realtime/notes");

app.UseRouting();

// Inicio de Jwt
Jwt.Open();

app.Run();