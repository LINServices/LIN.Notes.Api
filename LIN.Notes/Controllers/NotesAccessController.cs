namespace LIN.Notes.Controllers;


[Route("Notes/access")]
public class NotesAccessController(IHubContext<NotesHub> hubContext) : ControllerBase
{


    /// <summary>
    /// Hub de contexto.
    /// </summary>
    private readonly IHubContext<NotesHub> _hubContext = hubContext;




    /// <summary>
    /// Crear acceso a nota.
    /// </summary>
    /// <param name="model">Modelo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    [LocalToken]
    public async Task<HttpCreateResponse> Create([FromBody] NoteAccessDataModel model, [FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = model.NoteId,
            Profile = tokenInfo.ProfileId
        });

        // Si no tiene ese rol.
        if (!iam)
            return new()
            {
                Message = "No tienes privilegios en este inventario.",
                Response = Responses.Unauthorized
            };

        // Data.
        model.State = NoteAccessState.OnWait;
        model.Fecha = DateTime.Now;

        // Crear acceso.
        var result = await Data.NoteAccess.Create(model);

        // Si el recurso ya existe.
        if (result.Response == Responses.ResourceExist)
        {
            var update = await Data.NoteAccess.UpdateState(result.LastID, NoteAccessState.OnWait);
            result.Response = update.Response;
        }

        // Si fue correcto.
        if (result.Response == Responses.Success)
        {
            // Realtime.
            string groupName = $"group.{model.ProfileID}";
            string command = $"newInvitation({result.LastID})";
            await _hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });
        }

        // Retorna el resultado
        return new CreateResponse()
        {
            Response = result.Response,
            LastID = result.LastID
        };

    }



    /// <summary>
    /// Obtener una notificación.
    /// </summary>
    /// <param name="id">Id de la notificación.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet]
    [LocalToken]
    public async Task<HttpReadOneResponse<Notificacion>> Read([FromHeader] int id, [FromHeader] string token)
    {

        // Información del token.
        _ = HttpContext.Items[token] as JwtInformation ?? new();



        // Obtiene la lista de Id's de inventarios
        var result = await Data.NoteAccess.Read(id);

        // Retorna el resultado
        return result;

    }



    /// <summary>
    /// Obtiene una lista de accesos asociados a un usuario.
    /// </summary>
    /// <param name="id">Id de la cuenta</param>
    [HttpGet("read/all")]
    [LocalToken]
    public async Task<HttpReadAllResponse<Notificacion>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Obtiene la lista de Id's de inventarios
        var result = await Data.NoteAccess.ReadAll(tokenInfo.ProfileId);

        // Retorna el resultado
        return result;

    }



    /// <summary>
    /// Cambia el acceso a nota por medio de su Id
    /// </summary>
    /// <param name="id">Id del estado de nota</param>
    /// <param name="estado">Nuevo estado del acceso</param>
    [HttpPut("update/state")]
    [LocalToken]
    public async Task<HttpResponseBase> AccessChange([FromHeader] string token, [FromHeader] int id, [FromHeader] NoteAccessState estado)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Comprobaciones
        if (id <= 0 || estado == NoteAccessState.Undefined)
            return new(Responses.InvalidParam);


        var can = await Iam.CanAccept(id, tokenInfo.ProfileId);

        if (!can)
            return new(Responses.Unauthorized);


        // Obtiene la lista de Id's de inventarios
        var result = await Data.NoteAccess.UpdateState(id, estado);

        // Retorna el resultado
        return result;

    }




    /// <summary>
    /// Obtiene la lista de integrantes asociados a una nota.
    /// </summary>
    /// <param name="inventario">Id de la nota.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("members")]
    [LocalToken]
    public async Task<HttpReadAllResponse<IntegrantDataModel>> ReadAll([FromHeader] int inventario, [FromHeader] string token, [FromHeader] string tokenAuth)
    {

        // Comprobaciones
        if (inventario <= 0)
            return new(Responses.InvalidParam);

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();


        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = inventario,
            Profile = tokenInfo.ProfileId
        });

        // Si no tiene ese rol.
        if (!iam)
            return new()
            {
                Message = "No tienes privilegios en este inventario.",
                Response = Responses.Unauthorized
            };


        // Obtiene la lista.
        var result = await Data.NoteAccess.ReadMembers(inventario);


        var map = result.Models.Select(T => T.Item2.AccountID).ToList();

        var users = await Access.Auth.Controllers.Account.Read(map, tokenAuth);


        var i = (from I in result.Models
                 join A in users.Models
                 on I.Item2.AccountID equals A.Id
                 select new IntegrantDataModel
                 {
                     State = I.Item1.State,
                     AccessID = I.Item1.Id,
                     InventoryID = I.Item1.NoteId,
                     Nombre = A.Name,
                     Perfil = A.Profile,
                     ProfileID = I.Item2.ID,
                     Usuario = A.Identity.Unique
                 }).ToList();



        return new(Responses.Success, i);

    }



    /// <summary>
    /// Elimina a alguien de una nota.
    /// </summary>
    /// <param name="inventario">Id de la nota</param>
    /// <param name="usuario">Id del usuario que va a ser eliminado</param>
    /// <param name="token">Token de acceso.</param>
    [HttpDelete]
    [LocalToken]
    public async Task<HttpResponseBase> DeleteSomeOne([FromHeader] int inventario, [FromHeader] int usuario, [FromHeader] string token)
    {

        // Comprobaciones
        if (inventario <= 0 || usuario <= 0)
            return new(Responses.InvalidParam);


        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();


        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = inventario,
            Profile = tokenInfo.ProfileId
        });

        // Si no tiene ese rol.
        if (!iam)
            return new()
            {
                Message = "No tienes privilegios en este inventario.",
                Response = Responses.Unauthorized
            };

        // Obtiene la lista de Id's de inventarios
        var result = await Data.NoteAccess.DeleteSomeOne(inventario, usuario);

        return result;

    }


}