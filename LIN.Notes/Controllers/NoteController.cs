using LIN.Notes.Services.Abstractions;
using LIN.Types.Enumerations;

namespace LIN.Notes.Controllers;

[LocalToken]
[Route("notes")]
public class NoteController(IIam Iam, HubService hubService, Persistence.Access.Notes notes, LangService langService) : ControllerBase
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

        // Evitar nulos.
        modelo.Tittle ??= "";
        modelo.Content ??= "";

        // Consulta de idioma.
        var taskLang = langService.Lang(modelo.Content);

        // Validar si existe.
        var exist = modelo.UsersAccess.FirstOrDefault(t => t.ProfileID == tokenInfo.ProfileId);

        if (exist is null)
        {
            modelo.UsersAccess.Add(new()
            {
                Fecha = DateTime.Now,
                State = NoteAccessState.Accepted,
                ProfileID = tokenInfo.ProfileId,
            });
        }

        // Modelo.
        foreach (var access in modelo.UsersAccess)
        {
            access.Fecha = DateTime.Now;
            access.State = tokenInfo.ProfileId == access.ProfileID ? NoteAccessState.Accepted : NoteAccessState.OnWait;
        }

        // Esperar lang.
        await taskLang;

        // Asignar el idioma.
        modelo.Language = taskLang.Result;

        // Crea la nota.
        var response = await notes.Create(modelo);

        // Enviar en Realtime.
        if (response.Response == Responses.Success)
            await hubService.NewNote(tokenInfo.ProfileId, response.LastId);

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
        var result = await notes.ReadAll(tokenInfo.ProfileId);

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
        var response = await notes.Read(id);

        // Si no se creo la nota.
        if (response.Response != Responses.Success)
            return response;

        // Retorna
        return response;

    }


    /// <summary>
    /// Obtener el lenguaje de una nota.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("lang")]
    public async Task<HttpReadOneResponse<Languages>> GetLang([FromQuery] int id, [FromHeader] string token)
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
        var response = await notes.ReadLang(id);

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
    public async Task<HttpReadOneResponse<Languages>> Update([FromBody] NoteDataModel note, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Consulta de idioma.
        var taskLang = langService.Lang(note.Content);

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


        // Esperar lang.
        await taskLang;

        // Asignar el idioma.
        note.Language = taskLang.Result;

        // Actualizar el contenido.
        var response = await notes.Update(note);

        if (response.Response is Responses.Success)
            await hubService.UpdateNote(tokenInfo.ProfileId, note.Id);

        // Retorna
        return new(response.Response, note.Language)
        {
            Message = response.Message
        };

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
        var response = await notes.UpdateColor(id, color);

        // Realtime.
        if (response.Response == Responses.Success)
            await hubService.UpdateNoteColor(tokenInfo.ProfileId, id, color);

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
        var response = await notes.Delete(id);

        if (response.Response is Responses.Success)
            await hubService.DeleteNote(tokenInfo.ProfileId, id);

        // Retorna
        return response;

    }

}