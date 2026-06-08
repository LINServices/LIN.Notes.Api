using LIN.Types.Enumerations;

namespace LIN.Notes.Controllers;

[LocalToken]
[Route("movements")]
public class MovementController(Persistence.Access.Movements movements) : ControllerBase
{
    /// <summary>
    /// Crea un movimiento de control de gastos.
    /// </summary>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] MovementDataModel modelo, [FromHeader] string token)
    {
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        if (modelo is null)
            return new() { Response = Responses.InvalidParam };

        modelo.ProfileId = tokenInfo.ProfileId;

        return await movements.Create(modelo);
    }


    /// <summary>
    /// Obtiene todos los movimientos del perfil.
    /// </summary>
    [HttpGet("read/all")]
    public async Task<HttpReadAllResponse<MovementDataModel>> ReadAll([FromHeader] string token)
    {
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        return await movements.ReadAll(tokenInfo.ProfileId);
    }


    /// <summary>
    /// Elimina un movimiento.
    /// </summary>
    [HttpDelete]
    public async Task<HttpResponseBase> Delete([FromQuery] int id, [FromHeader] string token)
    {
        var tokenInfo = HttpContext.Items[token] as JwtInformation ?? new();

        // Verificar que el movimiento existe y pertenece al perfil solicitante.
        var ownerResponse = await movements.GetProfileId(id);
        if (ownerResponse.Response != Responses.Success)
            return new() { Response = Responses.NotRows };

        if (ownerResponse.Model != tokenInfo.ProfileId)
            return new() { Response = Responses.Unauthorized };

        return await movements.Delete(id);
    }
}
