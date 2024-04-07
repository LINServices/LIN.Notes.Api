﻿using LIN.Notes;
using LIN.Notes.Services;

namespace LIN.Notes.Data;


public partial class Notes
{


    /// <summary>
    /// Crea un nuevo inventario.
    /// </summary>
    /// <param name="data">Modelo del inventario</param>
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


                foreach(var a in data.UsersAccess)
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
            catch (Exception ex)
            {
                transaction.Rollback();
                context.DataBase.Remove(data);
                ServerLogger.LogError(ex.Message);
            }
        }


        return new();
    }



    /// <summary>
    /// Obtiene un inventario.
    /// </summary>
    /// <param name="id">Id del inventario</param>
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
        catch (Exception ex)
        {
            ServerLogger.LogError(ex.Message);
        }

        return new();
    }



    /// <summary>
    /// Obtiene la lista de inventarios asociados a un perfil.
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
                          Tittle = I.Tittle
                      };


            var modelos = await res.ToListAsync();

            if (modelos != null)
                return new(Responses.Success, modelos);

            return new(Responses.NotRows);


        }
        catch (Exception ex)
        {
            ServerLogger.LogError(ex.Message);
        }

        return new();




    }



    /// <summary>
    /// Actualizar la información de un inventario.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="name">Nuevo nombre.</param>
    /// <param name="description">Nueva descripción.</param>
    /// <param name="context">Contexto de conexión..</param>
    public async static Task<ResponseBase> Update(int id, string name, string description, Conexión context)
    {

        // Ejecución
        try
        {

            var res = await (from I in context.DataBase.Notes
                             where I.Id == id
                             select I).ExecuteUpdateAsync(t => t.SetProperty(a => a.Tittle, name).SetProperty(a => a.Content, description));


            return new(Responses.Success);

        }
        catch (Exception ex)
        {
            ServerLogger.LogError(ex.Message);
        }

        return new();

    }



  



}