using LIN.Types.Cloud.OpenAssistant.Api;

namespace LIN.Notes.Controllers;

[Route("[controller]")]
public class EmmaController(Persistence.Access.Notes notes, Profiles profiles) : ControllerBase
{

    /// <summary>
    /// Respuesta de Emma al usuario.
    /// </summary>
    /// <param name="tokenAuth">Token de identity.</param>
    /// <param name="consult">Consulta del usuario.</param>
    [HttpPost]
    public async Task<HttpReadOneResponse<AssistantResponse>> Assistant([FromHeader] string tokenAuth, [FromBody] string consult)
    {

        // Cliente HTTP.
        HttpClient client = new();

        // Headers.
        client.DefaultRequestHeaders.Add("token", tokenAuth);
        client.DefaultRequestHeaders.Add("useDefaultContext", true.ToString().ToLower());

        // Modelo de Emma.
        var request = new AssistantRequest()
        {
            App = "notes",
            Prompt = consult
        };

        // Generar el string content.
        StringContent stringContent = new(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Solicitud HTTP.
        var result = await client.PostAsync("https://api.emma.linplatform.com/emma", stringContent);

        // Esperar respuesta.
        var response = await result.Content.ReadAsStringAsync();

        // Objeto.
        var assistantResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ReadOneResponse<AssistantResponse>>(response);

        // Respuesta
        return assistantResponse ?? new(Responses.Undefined);

    }


    /// <summary>
    /// Emma IA.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="consult">Prompt.</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<object>> RequestFromEmma([FromHeader] string tokenAuth, [FromHeader] bool includeMethods)
    {

        // Validar token.
        var response = await Access.Auth.Controllers.Authentication.Login(tokenAuth);


        if (response.Response != Responses.Success)
        {
            return new ReadOneResponse<object>()
            {
                Model = "Este usuario no autenticado en LIN Notes."
            };
        }

        // 
        var profile = await profiles.ReadByAccount(response.Model.Id);


        if (profile.Response != Responses.Success)
        {
            return new ReadOneResponse<object>()
            {
                Model = "Este usuario no tiene una cuenta en LIN Notes."
            };
        }




        var inventories = await notes.ReadAll(profile.Model.Id);




        string final = $$""""

                        puedes leer las notas del usuario y responder de acuerdo a ellas.

                        """";


        foreach (var i in inventories.Models)
        {
            final += $"{i.Content}" + Environment.NewLine;
        }


        final += includeMethods ? """
            Estos son comandos, los cuales debes responder con el formato igual a este:
            
            "#Comando(Propiedades en orden separados por coma si es necesario)"
            
            {
              "name": "#select",
              "description": "Abrir una nota, cuando el usuario se refiera a abrir una nota",
              "example":"#select(0)",
              "parameters": {
                "properties": {
                  "content": {
                    "type": "number",
                    "description": "Id de la nota"
                  }
                }
              }
            }
            
            {
              "name": "#say",
              "description": "Utiliza esta función para decirle algo al usuario como saludos o responder a preguntas.",
              "example":"#say('Hola')",
              "parameters": {
                "properties": {
                  "content": {
                    "type": "string",
                    "description": "contenido"
                  }
                }
              }
            }
            
            IMPORTANTE:
            No en todos los casos en necesario usar comandos, solo úsalos cuando se cumpla la descripción.
            
            NUNCA debes inventar comandos nuevos, solo puedes usar los que ya existen.
            """ : "\nPuedes contestar con la información de las notas del usuario, pero si te piden que hagas algo que no puedes hacer debes responder que en el contexto de la app actual no puedes ejecutar ninguna función";

        return new ReadOneResponse<object>()
        {
            Model = final,
            Response = Responses.Success
        };

    }

}