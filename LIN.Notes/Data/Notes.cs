namespace LIN.Notes.Data;


public partial class Notes
{


    /// <summary>
    /// Crear nueva nota.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public async static Task<CreateResponse> Create(NoteDataModel data)
    {

        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var response = await Create(data, context);
        context.CloseActions(connectionKey);
        return response;
    }



    /// <summary>
    /// Obtiene una nota.
    /// </summary>
    /// <param name="id">Id de la nota</param>
    public async static Task<ReadOneResponse<NoteDataModel>> Read(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var response = await Read(id, context);
        context.CloseActions(connectionKey);

        return response;

    }



    /// <summary>
    /// Obtiene la lista de notas asociados a un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    public async static Task<ReadAllResponse<NoteDataModel>> ReadAll(int id)
    {

        // Obtiene la conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var response = await ReadAll(id, context);
        context.CloseActions(connectionKey);
        return response;

    }



    /// <summary>
    /// Actualizar la información de una nota.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="name">Nuevo nombre.</param>
    /// <param name="description">Nueva descripción.</param>
    public async static Task<ResponseBase> Update(int id, string name, string description)
    {

        // Conexión
        (Conexión context, string connectionKey) = Conexión.GetOneConnection();

        var response = await Update(id, name, description, context);
        context.CloseActions(connectionKey);
        return response;
    }




}