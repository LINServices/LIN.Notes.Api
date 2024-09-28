using LIN.Notes.Services.Abstractions;

namespace LIN.Notes.Controllers;

[Route("notes")]
[LocalToken]
public class NoteController(IIam Iam, IHubContext<NotesHub> hubContext) : ControllerBase
{

    /// <summary>
    /// Crea un nueva nota.
    /// </summary>
    /// <param name="modelo">Modelo..</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] NoteDataModel modelo, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Comprobaciones
        if (modelo == null)
            return new(Responses.InvalidParam);


        modelo.Tittle ??= "";
        modelo.Content ??= "";


        var exist = modelo.UsersAccess.FirstOrDefault(t => t.ProfileID == tokenInfo.ProfileId);

        if (exist == null)
        {

            modelo.UsersAccess.Add(new()
            {
                Fecha = DateTime.Now,
                State = NoteAccessState.Accepted,
                ProfileID = tokenInfo.ProfileId,
            });

        }



        // Modelo
        foreach (var access in modelo.UsersAccess)
        {

            access.Fecha = DateTime.Now;
            if (tokenInfo.ProfileId == access.ProfileID)
            {
                access.State = NoteAccessState.Accepted;
            }
            else
            {
                access.State = NoteAccessState.OnWait;
            }
        }

        // Crea el inventario
        var response = await Data.Notes.Create(modelo);

        // Si no se creo el inventario
        if (response.Response != Responses.Success)
            return response;

        // Retorna
        return response;

    }


    /// <summary>
    /// Obtiene las notas asociadas.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<NoteDataModel>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Obtiene la lista de notas.
        var result = await Data.Notes.ReadAll(tokenInfo.ProfileId);

        return result;

    }


    /// <summary>
    /// Obtener una nota.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<NoteDataModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = id,
            Profile = tokenInfo.ProfileId
        });

        // Validar Iam.
        if (!iam)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };


        // Crea la nota.
        var response = await Data.Notes.Read(id);

        // Si no se creo la nota.
        if (response.Response != Responses.Success)
            return response;

        // Retorna
        return response;

    }


    /// <summary>
    /// Actualizar una nota.
    /// </summary>
    /// <param name="note">Nuevo modelo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPatch]
    public async Task<HttpResponseBase> Update([FromBody] NoteDataModel note, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = note.Id,
            Profile = tokenInfo.ProfileId
        });

        // Validar Iam.
        if (!iam)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };

        // Actualizar el contenido.
        var response = await Data.Notes.Update(note);

        hubContext.Clients.Group($"note.{note.Id}").SendAsync("");

        // Retorna
        return response;

    }


    /// <summary>
    /// Actualizar una nota.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="color">Nuevo color.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPatch("color")]
    public async Task<HttpResponseBase> Update([FromQuery] int id, [FromQuery] int color, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = id,
            Profile = tokenInfo.ProfileId
        });

        // Validar Iam.
        if (!iam)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };

        // Actualizar el rol.
        var response = await Data.Notes.UpdateColor(id, color);


        // Realtime.
        if (response.Response == Responses.Success)
        {

            // Realtime.
            string groupName = $"group.{tokenInfo.ProfileId}";
            string command = $"updateColor({id}, {color})";
            await hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });

        }

        // Retorna
        return response;

    }


    /// <summary>
    /// Eliminar una nota.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpDelete]
    public async Task<HttpResponseBase> Delete([FromQuery] int id, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = id,
            Profile = tokenInfo.ProfileId
        });

        // Validar Iam.
        if (!iam)
            return new()
            {
                Response = Responses.Unauthorized,
                Message = "No tienes autorización."
            };


        // Crea la nota.
        var response = await Data.Notes.Delete(id);

        // Realtime.
        if (response.Response == Responses.Success)
        {

            // Realtime.
            string groupName = $"group.{tokenInfo.ProfileId}";
            string command = $"remove({id})";
            await hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });

        }

        // Retorna
        return response;

    }

}