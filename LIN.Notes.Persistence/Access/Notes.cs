using LIN.Types.Enumerations;
using LIN.Types.Notes.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Access;

public partial class Notes(DataContext context)
{

    /// <summary>
    /// Crea un nueva nota.
    /// </summary>
    /// <param name="data">Modelo de la nota</param>
    /// <param name="context">Contexto de conexión</param>
    public async Task<CreateResponse> Create(NoteDataModel data)
    {

        // Modelo
        data.Id = 0;

        try
        {

            // Guardar los accesos.
            foreach (var a in data.UsersAccess)
            {
                a.Profile = new()
                {
                    Id = a.ProfileID,
                };
                a.Note = data;
                context.Attach(a.Profile);
            }

            // Guardar la nota.
            context.Notes.Add(data);

            await context.SaveChangesAsync();

            // Finaliza
            return new(Responses.Success, data.Id);
        }
        catch (Exception)
        {
            context.Remove(data);
        }
        return new();
    }


    /// <summary>
    /// Obtiene una nota.
    /// </summary>
    /// <param name="id">Id de la nota</param>
    public async Task<ReadOneResponse<NoteDataModel>> Read(int id)
    {

        // Ejecución
        try
        {
            var note = await context.Notes.FirstOrDefaultAsync(T => T.Id == id);

            // Si no existe el modelo
            return note is null ? new(Responses.NotRows) : new(Responses.Success, note);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtiene una nota.
    /// </summary>
    /// <param name="id">Id de la nota</param>
    /// <param name="context">Contexto de conexión</param>
    public async Task<ReadOneResponse<Languages>> ReadLang(int id)
    {

        // Ejecución
        try
        {
            var res = await context.Notes.Select(t => new { t.Id, t.Language }).FirstOrDefaultAsync(T => T.Id == id);

            // Si no existe el modelo
            return res == null ? new(Responses.NotRows) : (ReadOneResponse<Languages>)new(Responses.Success, res.Language);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Eliminar una nota.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="context">Contexto de conexión</param>
    public async Task<ResponseBase> Delete(int id)
    {

        // Ejecución
        try
        {
            var res = await context.AccessNotes.Where(T => T.NoteId == id).ExecuteDeleteAsync();

            // Si no existe el modelo
            return res <= 0 ? new(Responses.NotRows) : new(Responses.Success);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtiene la lista de notas asociados a un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    public async Task<ReadAllResponse<NoteDataModel>> ReadAll(int id)
    {

        // Ejecución
        try
        {

            var res = from AI in context.AccessNotes
                      where AI.ProfileID == id && AI.State == NoteAccessState.Accepted
                      join I in context.Notes on AI.NoteId equals I.Id
                      select new NoteDataModel()
                      {
                          Content = I.Content,
                          Id = I.Id,
                          Tittle = I.Tittle,
                          Color = I.Color,
                          Language = I.Language,
                      };


            var modelos = await res.ToListAsync();

            return modelos != null ? new(Responses.Success, modelos) : new(Responses.NotRows);
        }
        catch (Exception)
        {
        }

        return new();




    }


    /// <summary>
    /// Actualizar la información de una nota.
    /// </summary>
    /// <param name="note">Nueva información.</param>
    /// <param name="context">Contexto de conexión..</param>
    public async Task<ResponseBase> Update(NoteDataModel note)
    {

        // Ejecución
        try
        {

            var res = await (from I in context.Notes
                             where I.Id == note.Id
                             select I).ExecuteUpdateAsync(t => t.SetProperty(a => a.Tittle, note.Tittle).SetProperty(a => a.Content, note.Content).SetProperty(a => a.Color, note.Color).SetProperty(a => a.Language, note.Language));


            return new(Responses.Success);

        }
        catch (Exception)
        {
        }

        return new();

    }




    /// <summary>
    /// Actualizar el color.
    /// </summary>
    /// <param name="id">Id de la nota.</param>
    /// <param name="color">Nuevo color.</param>
    public async Task<ResponseBase> UpdateColor(int id, int color)
    {

        // Ejecución
        try
        {

            var res = await (from I in context.Notes
                             where I.Id == id
                             select I).ExecuteUpdateAsync(t => t.SetProperty(a => a.Color, color));


            return new(Responses.Success);

        }
        catch (Exception)
        {
        }

        return new();

    }



}