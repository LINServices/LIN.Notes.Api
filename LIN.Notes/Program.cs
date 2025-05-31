using Http.Extensions;
using LIN.Access.Auth;
using LIN.Access.Logger;
using LIN.Notes.Persistence.Context;
using LIN.Notes.Persistence.Extensions;
using LIN.Notes.Services.Abstractions;

// Create a builder.
var builder = WebApplication.CreateBuilder(args);

builder.Host.AddServiceLogging("LIN.NOTES", "");

// Services.
builder.Services.AddSignalR();
builder.Services.AddLINHttp();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IIam, Iam>();
builder.Services.AddScoped<LangService, LangService>();
builder.Services.AddScoped<HubService, HubService>();
builder.Services.AddAuthenticationService(app: builder.Configuration["lin:app"]);
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

app.UseLINHttp();
app.UseDataBase();

app.MapControllers();

// Rutas de servicios de tiempo real
app.MapHub<NotesHub>("/Realtime/notes");

app.UseRouting();

// Inicio de Jwt
Jwt.Open(builder.Configuration);

builder.Services.AddDatabaseAction(() =>
{
    var context = app.Services.GetRequiredService<DataContext>();
    context.Profiles.Where(x => x.Id == 0).FirstOrDefaultAsync();
    return "Success";
});

app.Run();