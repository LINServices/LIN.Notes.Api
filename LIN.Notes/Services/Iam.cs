namespace LIN.Notes.Services;


internal class Iam
{


    /// <summary>
    /// Validar IAM.
    /// </summary>
    /// <param name="request">Solicitud.</param>
    public static async Task<bool> Validate(IamRequest request)
    {

        switch (request.IamBy)
        {
            case IamBy.Note:
                return await OnNote(request.Id, request.Profile);

          
            case IamBy.Access:
                return await OnAccess(request.Id, request.Profile);

        }

        return false;

    }



    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="profile">Id del perfil.</param>
    private static async Task<bool> OnNote(int id, int profile)
    {

        // Db.
        var (context, contextKey) = Conexión.GetOneConnection();

        // Query.
        var access = await (from P in context.DataBase.AccessNotes
                            where P.NoteId == id && P.ProfileID == profile
                            where P.State == NoteAccessState.Accepted
                            select new { P.ProfileID }).FirstOrDefaultAsync();

        // Si no hay.
        if (access == null)
            return false;

        return true;
    }



    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="profile">Id del perfil.</param>
    public static async Task<bool> CanAccept(int id, int profile)
    {

        // Db.
        var (context, contextKey) = Conexión.GetOneConnection();

        // Query.
        var access = await (from P in context.DataBase.AccessNotes
                            where P.Id == id && P.ProfileID == profile
                            where P.State == NoteAccessState.OnWait
                            select new { P.NoteId }).FirstOrDefaultAsync();

        // Si no hay.
        return access != null;

    }



    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="accessId">Id del acceso.</param>
    /// <param name="profile">Id del perfil.</param>
    private static async Task<bool> OnAccess(int accessId, int profile)
    {

        // Db.
        var (context, contextKey) = Conexión.GetOneConnection();

        // Query.
        var inventory = await (from P in context.DataBase.AccessNotes
                               where P.Id == accessId
                               select P.NoteId).FirstOrDefaultAsync();

        // Rol.
        var rol = await OnNote(inventory, profile);

        // Retornar.
        return rol;
    }


}