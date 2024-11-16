namespace LIN.Notes.Services.Abstractions;

public interface IIam
{

    /// <summary>
    /// Validar acceso.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="profile">Id del perfil.</param>
    public Task<bool> CanAccept(int id, int profile);


    /// <summary>
    /// Validar IAM.
    /// </summary>
    /// <param name="request">Solicitud.</param>
    public Task<bool> Validate(IamRequest request);

}