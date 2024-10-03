using LIN.Types.Emma.Models;

namespace LIN.Notes.Controllers;


[Route("[controller]")]
public class EmmaController(Persistence.Access.Notes notes, Profiles profiles) : ControllerBase
{


    /// <summary>
    /// Consulta para LIN Notes Emma.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="consult">Consulta.</param>
    [HttpPost]
    public async Task<HttpReadOneResponse<ResponseIAModel>> Assistant([FromHeader] string tokenAuth, [FromBody] string consult)
    {

        HttpClient client = new();

        client.DefaultRequestHeaders.Add("token", tokenAuth);
        client.DefaultRequestHeaders.Add("useDefaultContext", true.ToString().ToLower());


        var request = new Types.Models.EmmaRequest
        {
            AppContext = "notes",
            Asks = consult
        };



        StringContent stringContent = new(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var result = await client.PostAsync("http://api.emma.linplatform.com/emma", stringContent);


        var ss = await result.Content.ReadAsStringAsync();


        dynamic? fin = JsonConvert.DeserializeObject(ss);


        // Respuesta
        return new ReadOneResponse<ResponseIAModel>()
        {
            Model = new()
            {
                IsSuccess = true,
                Content = fin?.result
            },
            Response = Responses.Success
        };

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




        var inventories = await notes.ReadAll(profile.Model.ID);




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