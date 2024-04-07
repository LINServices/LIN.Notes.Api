using LIN.Notes.Services;

namespace LIN.Notes.Controllers;


[Route("auth")]
public class AuthController : ControllerBase
{


    /// <summary>
    /// Iniciar sesión.
    /// </summary>
    /// <param name="user">Usuario único.</param>
    /// <param name="password">Contraseña.</param>
    [HttpGet("credentials")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Validar parámetros.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            return new()
            {
                Message = "Usuario / contraseña vacíos o tienen un formato incorrecto.",
                Response = Responses.InvalidParam
            };

        // Respuesta de Cloud Identity.
        var authResponse = await Access.Auth.Controllers.Authentication.Login(user, password);

        // Autenticación errónea.
        if (authResponse.Response != Responses.Success)
            return new ReadOneResponse<AuthModel<ProfileModel>>
            {
                Message = "Autenticación fallida",
                Response = authResponse.Response
            };

        // Obtiene el perfil.
        var profile = await Data.Profiles.ReadByAccount(authResponse.Model.Id);

        // Segun.
        switch (profile.Response)
        {
            // Correcto.
            case Responses.Success:
                break;

            // Si el perfil no existe.
            case Responses.NotExistProfile:
                {

                    // Crear el perfil.
                    var createResponse = await Data.Profiles.Create(new()
                    {
                        Account = authResponse.Model,
                        Profile = new()
                        {
                            AccountID = authResponse.Model.Id,
                            Creation = DateTime.Now
                        }
                    });

                    // Si hubo un error.
                    if (createResponse.Response != Responses.Success)
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = "Un error grave ocurrió"
                        };

                    // Establecer.
                    profile = createResponse;
                    break;
                }

            // Otros errores.
            default:
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurrió"
                };
        }


        // Genera el token
        var token = Jwt.Generate(profile.Model);

        return new ReadOneResponse<AuthModel<ProfileModel>>
        {
            Response = Responses.Success,
            Message = "Success",
            Model = new()
            {
                Account = authResponse.Model,
                TokenCollection = new()
                {
                    {"identity", authResponse.Token }
                },
                Profile = profile.Model
            },
            Token = token
        };

    }



    /// <summary>
    /// Iniciar sesión de usuario por medio del token.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("token")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> LoginWithToken([FromHeader] string token)
    {

        // Respuesta de autenticación
        var authResponse = await Access.Auth.Controllers.Authentication.Login(token);

        // Autenticación errónea
        if (authResponse.Response != Responses.Success)
            return new()
            {
                Message = "Autenticación fallida",
                Response = authResponse.Response
            };

        // Obtiene el perfil
        var profile = await Data.Profiles.ReadByAccount(authResponse.Model.Id);

        // Validar respuesta.
        switch (profile.Response)
        {

            // Correcto.
            case Responses.Success:
                break;

            // Si el perfil no existe.
            case Responses.NotExistProfile:
                {

                    // Crear el perfil.
                    var createResponse = await Data.Profiles.Create(new()
                    {
                        Account = authResponse.Model,
                        Profile = new()
                        {
                            AccountID = authResponse.Model.Id,
                            Creation = DateTime.Now
                        }
                    });

                    // Validar.
                    if (createResponse.Response != Responses.Success)
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = "Un error grave ocurrió"
                        };

                    // Establecer.
                    profile = createResponse;
                    break;
                }

            // Otros casos.
            default:
                return new()
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurrió"
                };
        }

        // Genera el token
        var tokenGen = Jwt.Generate(profile.Model);

        // Respuesta.
        return new ReadOneResponse<AuthModel<ProfileModel>>
        {
            Response = Responses.Success,
            Message = "Success",
            Model = new()
            {
                Account = authResponse.Model,
                TokenCollection = new()
                {
                    {"identity", authResponse.Token }
                },
                Profile = profile.Model
            },
            Token = tokenGen
        };

    }


}