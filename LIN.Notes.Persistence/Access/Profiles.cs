using Microsoft.EntityFrameworkCore;

namespace LIN.Notes.Persistence.Access;

public class Profiles(DataContext context)
{

    /// <summary>
    /// Crear un nuevo perfil de Notas.
    /// </summary>
    /// <param name="data">Modelo.</param>
    public async Task<ReadOneResponse<ProfileModel>> Create(ProfileModel data)
    {

        // Organizar el modelo.
        data.Id = 0;

        try
        {
            // Agregar el modelo.
            await context.Profiles.AddAsync(data);
            context.SaveChanges();

            return new(Responses.Success, data);

        }
        catch (Exception ex)
        {
            if ((ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") ?? false) || (ex.InnerException?.Message.Contains("duplicate key") ?? false))
                return new(Responses.ExistAccount);
        }

        return new();
    }


    /// <summary>
    /// Obtener un perfil.
    /// </summary>
    /// <param name="id">Id del perfil.</param>
    public async Task<ReadOneResponse<ProfileModel>> Read(int id)
    {

        // Ejecución
        try
        {

            // Obtener el perfil.
            var profile = await Query.Profiles.Read(id, context).FirstOrDefaultAsync();

            // Si no existe el modelo
            if (profile is null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profile);
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
    public async Task<ReadAllResponse<ProfileModel>> Read(List<int> ids)
    {

        // Ejecución
        try
        {

            var profiles = await Query.Profiles.Read(ids, context).ToListAsync();

            // Si no existe el modelo
            if (profiles == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profiles);
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
    public async Task<ReadAllResponse<ProfileModel>> ReadByAccounts(List<int> ids)
    {

        // Ejecución
        try
        {

            var profiles = await Query.Profiles.ReadByAccounts(ids, context).ToListAsync();

            // Si no existe el modelo
            if (profiles is null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profiles);
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
    public async Task<ReadOneResponse<ProfileModel>> ReadByAccount(int id)
    {

        // Ejecución
        try
        {
            var profile = await Query.Profiles.ReadByAccount(id, context).FirstOrDefaultAsync();

            // Si no existe el modelo
            if (profile == null)
                return new(Responses.NotExistProfile);

            return new(Responses.Success, profile);
        }
        catch (Exception)
        {
        }

        return new();
    }

}