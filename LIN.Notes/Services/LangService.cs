using LIN.Types.Enumerations;

namespace LIN.Notes.Services;

public class LangService(IConfiguration configuration)
{

    /// <summary>
    /// Servicio idiomas.
    /// </summary>
    public async Task<Languages> Lang(string content)
    {

        try
        {
            HttpClient client = new();

            client.DefaultRequestHeaders.Add("apiKey", configuration["lin:key"]);

            StringContent stringContent = new(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

            var result = await client.PostAsync("https://api.cloud.services.linplatform.com/IA/predict/lang", stringContent);


            var ss = await result.Content.ReadAsStringAsync();


            var fin = JsonConvert.DeserializeObject<ReadOneResponse<Languages>>(ss);
            return fin.Model;
        }
        catch { }
        return Languages.Undefined;

    }

}