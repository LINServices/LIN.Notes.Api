namespace LIN.Notes.Data;


public partial class Profiles
{


    /// <summary>
    /// Crear nuevo perfil.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public async static Task<ReadOneResponse<ProfileModel>> Create(AuthModel<ProfileModel> data)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Create(data, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtener un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    public async static Task<ReadOneResponse<ProfileModel>> Read(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(id, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtener perfiles.
    /// </summary>
    /// <param name="ids">Id de los perfiles.</param>
    public async static Task<ReadAllResponse<ProfileModel>> Read(List<int> ids)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await Read(ids, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtener perfiles.
    /// </summary>
    /// <param name="ids">Id de las cuentas.</param>
    public async static Task<ReadAllResponse<ProfileModel>> ReadByAccounts(List<int> ids)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadByAccounts(ids, context);
        context.CloseActions(connectionKey);
        return res;

    }



    /// <summary>
    /// Obtener un perfil.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    public async static Task<ReadOneResponse<ProfileModel>> ReadByAccount(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var res = await ReadByAccount(id, context);
        context.CloseActions(connectionKey);
        return res;

    }


}