namespace LIN.Notes.Data;


public partial class NoteAccess
{


    /// <summary>
    /// Crear acceso a note.
    /// </summary>
    /// <param name="model">Modelo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async static Task<CreateResponse> Create(NoteAccessDataModel model, Conexión context)
    {

        // Ejecución
        try
        {
            // Consultar si ya existe.
            var exist = await (from AI in context.DataBase.AccessNotes
                               where AI.ProfileID == model.ProfileID
                               && AI.NoteId == model.NoteId
                               select AI.Id).FirstOrDefaultAsync();

            // Si ya existe.
            if (exist > 0)
                return new()
                {
                    LastID = exist,
                    Response = Responses.ResourceExist
                };

            model.Id = 0;

            await context.DataBase.AccessNotes.AddAsync(model);

            context.DataBase.SaveChanges();

            return new(Responses.Success)
            {
                LastID = model.Id
            };


        }
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Obtener las invitaciones de un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async static Task<ReadAllResponse<Notification>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta
            var res = from AI in context.DataBase.AccessNotes
                      where AI.ProfileID == id && AI.State == NoteAccessState.OnWait
                      join I in context.DataBase.Notes on AI.NoteId equals I.Id
                      select new Notification()
                      {
                          ID = AI.Id,
                          Fecha = AI.Fecha,
                          NoteName = I.Tittle,
                          //UsuarioInvitador = U.Id,
                          NoteId = I.Id
                      };


            var modelos = await res.ToListAsync();
            if (modelos != null)
                return new(Responses.Success, modelos);

            return new(Responses.NotRows);


        }
        catch (Exception)
        {
        }

        return new();
    }



    /// <summary>
    /// Obtener una invitación.
    /// </summary>
    /// <param name="id">Id de la invitación.</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<Notification>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta
            var res = from AI in context.DataBase.AccessNotes
                      where AI.Id == id && AI.State == NoteAccessState.OnWait
                      join I in context.DataBase.Notes on AI.NoteId equals I.Id
                      select new Notification()
                      {
                          ID = AI.Id,
                          Fecha = AI.Fecha,
                          NoteName = I.Tittle,
                          //UsuarioInvitador = U.Id,
                          NoteId = I.Id
                      };


            var modelos = await res.FirstOrDefaultAsync();
            if (modelos != null)
                return new(Responses.Success, modelos);

            return new(Responses.NotRows);


        }
        catch (Exception )
        {
        }

        return new();
    }



    /// <summary>
    /// Cambia el estado de una invitación.
    /// </summary>
    /// <param name="id">Id de la invitación</param>
    /// <param name="estado">Nuevo estado</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ResponseBase> UpdateState(int id, NoteAccessState estado, Conexión context)
    {

        // Ejecución
        try
        {
            var model = await context.DataBase.AccessNotes.FindAsync(id);

            if (model != null)
            {
                model.State = estado;
                context.DataBase.SaveChanges();
                return new(Responses.Success);
            }

            return new(Responses.NotRows);

        }
        catch (Exception )
        {
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de integrantes de un note.
    /// </summary>
    /// <param name="inventario">Id del note</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<Tuple<NoteAccessDataModel, ProfileModel>>> ReadMembers(int inventario, Conexión context)
    {

        // Ejecución
        try
        {

            // Consulta
            var res = from AI in context.DataBase.AccessNotes
                      where AI.NoteId == inventario
                       &&( AI.State == NoteAccessState.Accepted 
                       || AI.State == NoteAccessState.OnWait)
                      join U in context.DataBase.Profiles on AI.ProfileID equals U.ID
                      select new Tuple<NoteAccessDataModel, ProfileModel>(AI, U);


            var modelos = await res.ToListAsync();

            if (modelos == null)
                return new(Responses.NotRows);

            return new(Responses.Success, modelos);


        }
        catch (Exception )
        {
        }

        return new();
    }



    /// <summary>
    /// Eliminar a alguien de un note.
    /// </summary>
    /// <param name="note">Id del note.</param>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="context">Contexto de conexión.</param>
    public async static Task<ResponseBase> DeleteSomeOne(int note, int profile, Conexión context)
    {

        // Ejecución
        try
        {

            // Actualizar estado.
            var result = await (from AI in context.DataBase.AccessNotes
                                where AI.NoteId == note
                                where AI.ProfileID == profile
                                select AI).ExecuteUpdateAsync(t => t.SetProperty(t => t.State, NoteAccessState.Deleted));

            return new(Responses.Success);

        }
        catch (Exception)
        {
        }

        return new();
    }



}