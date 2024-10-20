using Http.Extensions;
using LIN.Access.Logger;
using LIN.Notes.Services.Abstractions;
using LIN.Access.Auth;
using LIN.Notes.Persistence.Extensions;
using LIN.Notes.Persistence.Context;

// Create a builder.
var builder = WebApplication.CreateBuilder(args);

// Services.
builder.Services.AddSignalR();
builder.Services.AddLINHttp();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIam, Iam>();
builder.Services.AddScoped<LangService, LangService>();
builder.Services.AddScoped<HubService, HubService>();
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

builder.Services.AddDatabaseAction(() =>
{
    var context = app.Services.GetRequiredService<DataContext>();
    context.Profiles.Where(x => x.Id == 0).FirstOrDefaultAsync();
    return "Success";
});

app.Run();