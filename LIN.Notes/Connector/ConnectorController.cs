namespace LIN.Notes.Connector;

[Route("[Controller]")]
public class ConnectorController(Profiles profiles, Persistence.Access.Notes notes) : ControllerBase
{

    /// <summary>
    /// Obtiene las notas asociadas.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet]
    public async Task<HttpReadAllResponse<NoteDataModel>> ReadAll([FromHeader] string token)
    {
        var auth = await LIN.Access.Auth.Controllers.Authentication.Login(token);

        var profile = await profiles.ReadByAccount(auth.Model.Id);

        // Obtiene la lista de notas.
        var result = await notes.ReadAll(profile.Model.Id);

        return result;
    }

}