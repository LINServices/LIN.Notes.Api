using LIN.Notes;
using LIN.Notes.Data;
using LIN.Notes.Hubs;
using LIN.Notes.Services;

var builder = WebApplication.CreateBuilder(args);

try
{

    builder.Services.AddSignalR();


    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOrigin",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });


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

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    try
    {
        // Si la base de datos no existe
        using var scope = app.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
        var res = dataContext.Database.EnsureCreated();
    }
    catch
    { }





    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors("AllowAnyOrigin");
    app.UseAuthentication();
    app.UseAuthorization();



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
}