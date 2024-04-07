using LIN.Notes;

namespace LIN.Notes.Data.Query;


public class Profiles
{


    /// <summary>
    /// Obtener usuarios
    /// </summary>
    /// <param name="id">Id del usuario</param>
    /// <param name="context">Contexto</param>
    public static IQueryable<ProfileModel> Read(int id, Conexión context)
    {
        // Consulta
        var query = (from U in context.DataBase.Profiles
                     where U.ID == id
                     select U).Take(1);

        return query;

    }


    /// <summary>
    /// Obtener usuarios
    /// </summary>
    /// <param name="id">Id del usuario</param>
    /// <param name="context">Contexto</param>
    public static IQueryable<ProfileModel> Read(List<int> id, Conexión context)
    {
        // Consulta
        var query = from U in context.DataBase.Profiles
                    where id.Contains(U.ID)
                    select U;

        return query;

    }



    /// <summary>
    /// Obtener usuarios
    /// </summary>
    /// <param name="id">Id del usuario</param>
    /// <param name="context">Contexto</param>
    public static IQueryable<ProfileModel> ReadByAccounts(List<int> id, Conexión context)
    {
        // Consulta
        var query = from U in context.DataBase.Profiles
                    where id.Contains(U.AccountID)
                    select U;

        return query;

    }




    /// <summary>
    /// Obtener usuarios
    /// </summary>
    /// <param name="id">Id del usuario</param>
    /// <param name="context">Contexto</param>
    public static IQueryable<ProfileModel> ReadByAccount(int id, Conexión context)
    {
        // Consulta
        var query = (from U in context.DataBase.Profiles
                     where U.AccountID == id
                     select U).Take(1);

        return query;

    }



}