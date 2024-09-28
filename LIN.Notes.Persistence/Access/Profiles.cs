using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Access;

public class Profiles(DataContext context)
{

    /// <summary>
    /// Crear nuevo perfil.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async Task<ReadOneResponse<ProfileModel>> Create(AuthModel<ProfileModel> data)
    {

        data.Profile.ID = 0;

        // Ejecución (Transacción)
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                await context.Profiles.AddAsync(data.Profile);
                context.SaveChanges();


                transaction.Commit();


                return new(Responses.Success, data.Profile);

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                    return new(Responses.ExistAccount);
            }
        }
        return new();
    }


    /// <summary>
    /// Obtener un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async Task<ReadOneResponse<ProfileModel>> Read(int id)
    {

        // Ejecución
        try
        {

            var res = await Query.Profiles.Read(id, context).FirstOrDefaultAsync();

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, res);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtener perfiles.
    /// </summary>
    /// <param name="ids">Id de los perfiles.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async Task<ReadAllResponse<ProfileModel>> Read(List<int> ids)
    {

        // Ejecución
        try
        {

            var res = await Query.Profiles.Read(ids, context).ToListAsync();

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, res);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtener perfiles.
    /// </summary>
    /// <param name="ids">Id de los perfiles.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async Task<ReadAllResponse<ProfileModel>> ReadByAccounts(List<int> ids)
    {

        // Ejecución
        try
        {

            var res = await Query.Profiles.ReadByAccounts(ids, context).ToListAsync();

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, res);
        }
        catch (Exception)
        {
        }

        return new();
    }


    /// <summary>
    /// Obtener perfil.
    /// </summary>
    /// <param name="id">Id de la cuenta.</param>
    /// <param name="context">Contexto de base de datos.</param>
    public async Task<ReadOneResponse<ProfileModel>> ReadByAccount(int id)
    {

        // Ejecución
        try
        {

            var res = await Query.Profiles.ReadByAccount(id, context).FirstOrDefaultAsync();

            // Si no existe el modelo
            if (res == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, res);
        }
        catch (Exception)
        {
        }

        return new();
    }

}