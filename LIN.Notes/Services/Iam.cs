using LIN.Notes.Persistence.Context;
using LIN.Notes.Services.Abstractions;

namespace LIN.Notes.Services;

public class Iam(DataContext context) : IIam
{

    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="profile">Id del perfil.</param>
    public async Task<bool> CanAccept(int id, int profile)
    {

        // Query.
        var access = await (from P in context.AccessNotes
                            where P.Id == id
                            && P.ProfileID == profile
                            && P.State == NoteAccessState.OnWait
                            select new { P.NoteId }).FirstOrDefaultAsync();

        // Si no hay.
        return access != null;

    }


    /// <summary>
    /// Validar IAM.
    /// </summary>
    /// <param name="request">Solicitud.</param>
    public async Task<bool> Validate(IamRequest request)
    {

        switch (request.IamBy)
        {
            case IamBy.Note:
                return await OnNote(request.Id, request.Profile);
            case IamBy.Access:
                return await OnAccess(request.Id, request.Profile);
            default:
                break;
        }

        return false;

    }


    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="accessId">Id del acceso.</param>
    /// <param name="profile">Id del perfil.</param>
    private async Task<bool> OnAccess(int accessId, int profile)
    {
        // Query.
        var inventory = await (from P in context.AccessNotes
                               where P.Id == accessId
                               select P.NoteId).FirstOrDefaultAsync();

        // Rol.
        var rol = await OnNote(inventory, profile);

        // Retornar.
        return rol;
    }


    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="profile">Id del perfil.</param>
    private async Task<bool> OnNote(int id, int profile)
    {
        // Query.
        var access = await (from P in context.AccessNotes
                            where P.NoteId == id && P.ProfileID == profile
                            where P.State == NoteAccessState.Accepted
                            select new { P.ProfileID }).FirstOrDefaultAsync();

        // Si no hay.
        if (access == null)
            return false;

        return true;
    }

}