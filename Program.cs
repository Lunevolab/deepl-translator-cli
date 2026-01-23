using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

class Program
{
    static HttpClient client = new HttpClient();


    static async Task Main(string[] args)
    {
        List<string> myList = new List<string>(args);
        string myString;

        if (args.Length == 0)
        {
            Console.WriteLine("No text entered");
            Console.Write("Enter text to translate: ");
            myString = Console.ReadLine();
        }

        else
        {
            myString = string.Join(" ", myList);
        }

        try
        {
            

            CheckApiKey();
            string translatedString = await GetTranslateAsync(myString);
            Console.WriteLine(translatedString);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

    }

    static void CheckStatusCode(HttpResponseMessage response)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest: throw new Exception("Bad request");
            case HttpStatusCode.Unauthorized: throw new Exception("Problem with API key");
            case HttpStatusCode.Forbidden: throw new Exception("Access forbidden");
            case HttpStatusCode.NotFound: throw new Exception("Endpoint not found");
            case HttpStatusCode.TooManyRequests: throw new Exception("Too many requests");
            case HttpStatusCode.InternalServerError: throw new Exception("Internal server error on DeepL side");
            case HttpStatusCode.ServiceUnavailable: throw new Exception("Service temporarily unavailable");
            default:

                if ((int)response.StatusCode == 456)
                {
                    throw new Exception("Translation limit for the plan has been reached");
                }

                throw new Exception($"Unknown error. Response code: {(int)response.StatusCode}");
        }
    }

    static void CheckApiKey()
    {
        string apiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");

        if (string.IsNullOrEmpty(apiKey)) 
        {
            throw new Exception("API key is missing or incorrect. Check the DEEPL_API_KEY environment variable");
        }
    }

    static async Task<string> GetTranslateAsync(string text)
    {
        string apiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", apiKey);

        var values = new Dictionary<string, string>
        {
            { "text", text },
            { "target_lang", "EN" }
        };

        var content = new FormUrlEncodedContent(values);

        var url = "https://api-free.deepl.com/v2/translate";

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            CheckStatusCode(response);
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();

        // 1. Parse the JSON string into a document
        JsonDocument document = JsonDocument.Parse(jsonResponse);

        // 2. Get the root element
        JsonElement root = document.RootElement;

        // 3. Get the 'translations' array
        JsonElement translations = root.GetProperty("translations");

        // 4. Get the first element of the array
        JsonElement firstTranslation = translations[0];

        // 5. Extract the translated text
        string translatedText = firstTranslation.GetProperty("text").GetString();

        return translatedText;
    }
}