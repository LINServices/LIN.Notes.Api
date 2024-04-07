using LIN.Notes;

namespace LIN.Notes.Data;


public partial class NoteAccess
{


    /// <summary>
    /// Crear acceso a inventario.
    /// </summary>
    /// <param name="model">Modelo.</param>
    public async static Task<CreateResponse> Create(NoteAccessDataModel model)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var rs = await Create(model, context);
        context.CloseActions(connectionKey);
        return rs;
    }



    /// <summary>
    /// Obtener una invitación.
    /// </summary>
    /// <param name="id">Id de la invitación.</param>
    public async static Task<ReadOneResponse<Notificacion>> Read(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var rs = await Read(id, context);
        context.CloseActions(connectionKey);
        return rs;
    }



    /// <summary>
    /// Obtiene la lista de invitaciones a un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    public async static Task<ReadAllResponse<Notificacion>> ReadAll(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var rs = await ReadAll(id, context);
        context.CloseActions(connectionKey);
        return rs;
    }



    /// <summary>
    /// Cambia el estado de una invitación.
    /// </summary>
    /// <param name="id">Id de la invitación.</param>
    /// <param name="estado">Nuevo estado.</param>
    public async static Task<ResponseBase> UpdateState(int id, NoteAccessState estado)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await UpdateState(id, estado, context);
        context.CloseActions(connectionKey);
        return res;
    }






    /// <summary>
    /// Obtener los integrantes.
    /// </summary>
    /// <param name="inventario">Id del inventario.</param>
    public async static Task<ReadAllResponse<Tuple<NoteAccessDataModel, ProfileModel>>> ReadMembers(int inventario)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadMembers(inventario, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Eliminar a alguien de un inventario.
    /// </summary>
    /// <param name="inventario">Id del inventario</param>
    /// <param name="profile">Id del perfil.</param>
    public async static Task<ResponseBase> DeleteSomeOne(int inventario, int profile)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await DeleteSomeOne(inventario, profile, context);
        context.CloseActions(connectionKey);
        return res;
    }



}