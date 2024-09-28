namespace LIN.Notes.Controllers;


[Route("profile")]
public class ProfileController(Profiles profiles) : ControllerBase
{


    /// <summary>
    /// Obtiene un usuario por medio del Id
    /// </summary>
    /// <param name="id">Id del usuario</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<ProfileModel>> Read([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        // Obtiene el usuario
        var response = await profiles.Read(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new ReadOneResponse<ProfileModel>()
            {
                Response = response.Response,
                Model = new()
            };

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Buscar.
    /// </summary>
    /// <param name="pattern">Patron de búsqueda.</param>
    /// <param name="token">Token de acceso Identity.</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<SessionModel<ProfileModel>>> Search([FromQuery] string pattern, [FromHeader] string token)
    {

        // Usuarios
        var users = await Access.Auth.Controllers.Account.Search(pattern, token);

        // Si hubo un error
        if (users.Response != Responses.Success)
            return new(users.Response);

        // Mapear los ids de los usuarios.
        var map = users.Models.Select(T => T.Id).ToList();

        // Obtiene el usuario
        var response = await profiles.ReadByAccounts(map);

        // Unir las respuestas.
        var joins = (from Account in users.Models
                     join Profile in response.Models
                     on Account.Id equals Profile.AccountID
                     select new SessionModel<ProfileModel>
                     {
                         Account = Account,
                         Profile = Profile
                     }).ToList();

        // Retorna el resultado
        return new ReadAllResponse<SessionModel<ProfileModel>>
        {
            Response = Responses.Success,
            Models = joins
        };

    }



}