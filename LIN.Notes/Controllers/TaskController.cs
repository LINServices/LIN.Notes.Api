using LIN.Notes.Services.Abstractions;
using LIN.Types.Enumerations;

namespace LIN.Notes.Controllers;

[LocalToken]
[Route("tasks")]
public class TaskController(IIam Iam, Persistence.Access.Tasks tasks) : ControllerBase
{
    /// <summary>
    /// Crea una tarea para una nota.
    /// </summary>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] TaskDataModel modelo, [FromHeader] string token)
    {
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = modelo.NoteId,
            Profile = tokenInfo.ProfileId
        });

        if (!iam)
            return new() { Response = Responses.Unauthorized };

        return await tasks.Create(modelo);
    }


    /// <summary>
    /// Actualiza una tarea.
    /// </summary>
    [HttpPatch]
    public async Task<HttpResponseBase> Update([FromBody] TaskDataModel modelo, [FromHeader] string token)
    {
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Obtener el NoteId real de la tarea desde BD (no confiar en el cliente).
        var noteIdResponse = await tasks.GetNoteId(modelo.Id);
        if (noteIdResponse.Response != Responses.Success)
            return new() { Response = Responses.NotRows };

        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = noteIdResponse.Model,
            Profile = tokenInfo.ProfileId
        });

        if (!iam)
            return new() { Response = Responses.Unauthorized };

        return await tasks.Update(modelo);
    }


    /// <summary>
    /// Elimina una tarea.
    /// </summary>
    [HttpDelete]
    public async Task<HttpResponseBase> Delete([FromQuery] int id, [FromHeader] string token)
    {
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Obtener el NoteId real de la tarea desde BD (no confiar en el cliente).
        var noteIdResponse = await tasks.GetNoteId(id);
        if (noteIdResponse.Response != Responses.Success)
            return new() { Response = Responses.NotRows };

        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = noteIdResponse.Model,
            Profile = tokenInfo.ProfileId
        });

        if (!iam)
            return new() { Response = Responses.Unauthorized };

        return await tasks.Delete(id);
    }
}
