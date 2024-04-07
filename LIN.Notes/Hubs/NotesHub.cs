namespace LIN.Notes.Hubs;


public class NotesHub : Hub
{


    /// <summary>
    /// Lista de dispositivos.
    /// </summary>
    public static Dictionary<int, List<DeviceModel>> List { get; set; } = [];



    /// <summary>
    /// Agregar una conexión a su grupo de cuenta.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    public async Task Join(string token, DeviceModel model)
    {

        // Validar.
        if (string.IsNullOrEmpty(model.LocalId))
            return;

        // Información del token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token es invalido.
        if (!tokenInfo.IsAuthenticated)
            return;


        var exist = List.ContainsKey(tokenInfo.ProfileId);

        if (!exist)
        {
            List.Add(tokenInfo.ProfileId, [new DeviceModel()
            {
                Id = Context.ConnectionId,
                Name = model.Name,
                Platform = model.Platform,
               LocalId=model.LocalId,
            }]);
        }
        else
        {
            var any = List[tokenInfo.ProfileId].Any(t => t.LocalId == model.LocalId);

            if (any)
                return;


            List[tokenInfo.ProfileId].Add(model);
        }

        model.Id = Context.ConnectionId;

        // Agregar el grupo.
        string groupName = $"group.{tokenInfo.ProfileId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    }



    /// <summary>
    /// Agregar una conexión a un grupo de inventario.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="inventory">Id del inventario.</param>
    public async Task JoinInventory(string token, int inventory)
    {

        // Información del token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token es invalido.
        if (!tokenInfo.IsAuthenticated)
            return;

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = inventory,
            Profile = tokenInfo.ProfileId
        });

        // Si no tiene ese rol.
        if (!iam)
            return;

        string groupName = $"inventory.{inventory}";

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    }



    /// <summary>
    /// Enviar un comando a los demás dispositivos.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="comando">Comando.</param>
    public async Task SendCommand(string token, CommandModel comando)
    {

        // Información del token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token es invalido.
        if (!tokenInfo.IsAuthenticated)
            return;

        // Envía el comando.
        string group;

        if (comando.Inventory > 0)
            group = $"inventory.{comando.Inventory}";
        else
            group = $"group.{tokenInfo.ProfileId}";

        await Clients.Group(group).SendAsync("#command", comando);

    }




    /// <summary>
    /// Agregar una conexión a un grupo de inventario.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="inventory">Id del inventario.</param>
    public async Task<string> Notification(string token, int inventory)
    {

        // Información del token.
        var tokenInfo = Jwt.Validate(token);

        // Si el token es invalido.
        if (!tokenInfo.IsAuthenticated)
            return "No Auth";

        // Acceso Iam.
        var iam = await Iam.Validate(new IamRequest()
        {
            IamBy = IamBy.Note,
            Id = inventory,
            Profile = tokenInfo.ProfileId
        });

        // Si no tiene ese rol.
        if (!iam)
            return "No Rol";


        var (context, contextKey) = Conexión.GetOneConnection();




        var x = await (from i in context.DataBase.AccessNotes
                       where i.NoteId == inventory
                       where i.State == NoteAccessState.OnWait
                       select new
                       {
                           Profile = i.ProfileID,
                           Id = i.Id,
                       }).ToListAsync();


        foreach (var id in x)
        {
            string groupName = $"group.{id.Profile}";
            string command = $"newInvitation({id.Id})";
            await Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });
        }


        return "Success";

    }




    public async Task SendToDevice(string device, CommandModel command)
    {

        // Envía el comando.
        await Clients.Client(device).SendAsync("#command", command);

    }





    /// <summary>
    /// Evento: Cuando un dispositivo se desconecta.
    /// </summary>
    /// <param name="exception">Excepción</param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {

            await Task.Run(() =>
               {
                   // Obtiene la sesión por el dispositivo
                   var x = List.Values.FirstOrDefault(t => t.Any(t => t.Id == Context.ConnectionId));

                   x?.RemoveAll(t => t.Id == Context.ConnectionId);

               });
        }
        catch
        {
        }
    }


}