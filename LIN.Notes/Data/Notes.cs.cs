namespace LIN.Notes.Data;


public partial class Notes
{


    /// <summary>
    /// Crea un nueva nota.
    /// </summary>
    /// <param name="data">Modelo de la nota</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<CreateResponse> Create(NoteDataModel data, Conexión context)
    {

        // Modelo
        data.Id = 0;

        // Transacción
        using (var transaction = context.DataBase.Database.BeginTransaction())
        {
            try
            {


                foreach (var a in data.UsersAccess)
                {
                    a.Profile = new()
                    {
                        ID = a.ProfileID,
                    };
                    a.Note = data;
                    context.DataBase.Attach(a.Profile);
                }

                // InventoryId
                context.DataBase.Notes.Add(data);

                // Guarda el inventario
                await context.DataBase.SaveChangesAsync();

                // Finaliza
                transaction.Commit();
                return new(Responses.Success, data.Id);
            }
            catch (Exception)
            {
                transaction.Rollback();
                context.DataBase.Remove(data);
            }
        }


        return new();
    }



    /// <summary>
    /// Obtiene una nota.
    /// </summary>
    /// <param name="id">Id de la nota</param>
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadOneResponse<NoteDataModel>> Read(int id, Conexión context)
    {

        // Ejecución
        try
        {
            var res = await context.DataBase.Notes.FirstOrDefaultAsync(T => T.Id == id);

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotExistAccount);

            return new(Responses.Success, res);
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
    public async static Task<ResponseBase> Delete(int id, Conexión context)
    {

        // Ejecución
        try
        {
            var res = await context.DataBase.AccessNotes.Where(T => T.NoteId == id).ExecuteDeleteAsync();

            // Si no existe el modelo
            if (res <= 0)
                return new(Responses.NotRows);

            return new(Responses.Success);
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
    /// <param name="context">Contexto de conexión</param>
    public async static Task<ReadAllResponse<NoteDataModel>> ReadAll(int id, Conexión context)
    {

        // Ejecución
        try
        {

            var res = from AI in context.DataBase.AccessNotes
                      where AI.ProfileID == id && AI.State == NoteAccessState.Accepted
                      join I in context.DataBase.Notes on AI.NoteId equals I.Id
                      select new NoteDataModel()
                      {
                          Content = I.Content,
                          Id = I.Id,
                          Tittle = I.Tittle,
                          Color = I.Color
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
    /// Actualizar la información de una nota.
    /// </summary>
    /// <param name="note">Nueva información.</param>
    /// <param name="context">Contexto de conexión..</param>
    public async static Task<ResponseBase> Update(NoteDataModel note, Conexión context)
    {

        // Ejecución
        try
        {

            var res = await (from I in context.DataBase.Notes
                             where I.Id == note.Id
                             select I).ExecuteUpdateAsync(t => t.SetProperty(a => a.Tittle, note.Tittle).SetProperty(a => a.Content, note.Content).SetProperty(a => a.Color, note.Color));


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
    /// <param name="context">Contexto.</param>
    public async static Task<ResponseBase> UpdateColor(int id, int color, Conexión context)
    {

        // Ejecución
        try
        {

            var res = await (from I in context.DataBase.Notes
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