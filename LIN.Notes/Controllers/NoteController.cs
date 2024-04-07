namespace LIN.Notes.Controllers;


[Route("notes")]
public class NoteController : ControllerBase
{


    /// <summary>
    /// Crea un nuevo Inventario.
    /// </summary>
    /// <param name="modelo">Modelo del inventario.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost("create")]
    [LocalToken]
    public async Task<HttpCreateResponse> Create([FromBody] NoteDataModel modelo, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Comprobaciones
        if (!modelo.Tittle.Any() || !modelo.Content.Any())
            return new(Responses.InvalidParam);




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
    /// Obtiene los inventarios asociados a un perfil
    /// </summary>
    /// <param name="id">Id de la cuenta</param>
    [HttpGet("read/all")]
    [LocalToken]
    public async Task<HttpReadAllResponse<NoteDataModel>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Obtiene la lista de Id's de inventarios
        var result = await Data.Notes.ReadAll(tokenInfo.ProfileId);

        return result;

    }




    /// <summary>
    /// Obtener un inventario.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("read")]
    [LocalToken]
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


        // Crea el inventario
        var response = await Data.Notes.Read(id);

        // Si no se creo el inventario
        if (response.Response != Responses.Success)
            return response;

        // Retorna
        return response;

    }




    /// <summary>
    /// Actualizar la información de un inventario.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="name">Nuevo nombre.</param>
    /// <param name="description">Nueva descripción.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPatch]
    [LocalToken]
    public async Task<HttpResponseBase> Update([FromQuery] int id, [FromQuery] string name, [FromQuery] string description, [FromHeader] string token)
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
        var response = await Data.Notes.Update(id, name, description);

        // Retorna
        return response;

    }


}