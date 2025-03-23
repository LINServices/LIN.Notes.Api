using LIN.Types.Notes.Enumerations;
using LIN.Types.Notes.Transient;
using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Access;

public class NoteAccess(DataContext context)
{

    /// <summary>
    /// Crear acceso a note.
    /// </summary>
    /// <param name="model">Modelo.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async Task<CreateResponse> Create(NoteAccessDataModel model)
    {

        // Ejecución
        try
        {
            // Consultar si ya existe.
            var exist = await (from AI in context.AccessNotes
                               where AI.ProfileID == model.ProfileID
                               && AI.NoteId == model.NoteId
                               select AI.Id).FirstOrDefaultAsync();

            // Si ya existe.
            if (exist > 0)
                return new()
                {
                    LastId = exist,
                    Response = Responses.ResourceExist
                };

            model.Id = 0;

            await context.AccessNotes.AddAsync(model);

            context.SaveChanges();

            return new(Responses.Success)
            {
                LastId = model.Id
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
    public async Task<ReadAllResponse<Notification>> ReadAll(int id)
    {

        // Ejecución
        try
        {

            // Consulta
            var res = from AI in context.AccessNotes
                      where AI.ProfileID == id && AI.State == NoteAccessState.OnWait
                      join I in context.Notes on AI.NoteId equals I.Id
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
    public async Task<ReadOneResponse<Notification>> Read(int id)
    {

        // Ejecución
        try
        {

            // Consulta
            var res = from AI in context.AccessNotes
                      where AI.Id == id && AI.State == NoteAccessState.OnWait
                      join I in context.Notes on AI.NoteId equals I.Id
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
        catch (Exception)
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
    public async Task<ResponseBase> UpdateState(int id, NoteAccessState estado)
    {

        // Ejecución
        try
        {
            var model = await context.AccessNotes.FindAsync(id);

            if (model != null)
            {
                model.State = estado;
                context.SaveChanges();
                return new(Responses.Success);
            }

            return new(Responses.NotRows);

        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtiene la lista de integrantes de un note.
    /// </summary>
    /// <param name="inventario">Id del note</param>
    /// <param name="context">Contexto de conexión</param>
    public async Task<ReadAllResponse<Tuple<NoteAccessDataModel, ProfileModel>>> ReadMembers(int inventario)
    {

        // Ejecución
        try
        {

            // Consulta
            var res = from AI in context.AccessNotes
                      where AI.NoteId == inventario
                       && (AI.State == NoteAccessState.Accepted
                       || AI.State == NoteAccessState.OnWait)
                      join U in context.Profiles on AI.ProfileID equals U.Id
                      select new Tuple<NoteAccessDataModel, ProfileModel>(AI, U);


            var modelos = await res.ToListAsync();

            if (modelos == null)
                return new(Responses.NotRows);

            return new(Responses.Success, modelos);


        }
        catch (Exception)
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
    public async Task<ResponseBase> DeleteSomeOne(int note, int profile)
    {

        // Ejecución
        try
        {

            // Actualizar estado.
            var result = await (from AI in context.AccessNotes
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