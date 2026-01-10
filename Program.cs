using System.Net;
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
            Console.WriteLine("Вы не ввели текст");
            Console.Write("Введите текст для перевода: ");
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
            case HttpStatusCode.BadRequest: throw new Exception("Запрос составлен неправильно");
            case HttpStatusCode.Unauthorized: throw new Exception("Проблема с API-ключом");
            case HttpStatusCode.Forbidden: throw new Exception("Доступ запрещён");
            case HttpStatusCode.NotFound: throw new Exception("Endpoint не найден");
            case HttpStatusCode.TooManyRequests: throw new Exception("Превышен лимит запросов");
            case HttpStatusCode.InternalServerError: throw new Exception("Ошибка на стороне DeepL");
            case HttpStatusCode.ServiceUnavailable: throw new Exception("Сервис временно недоступен");
            default:

                if ((int)response.StatusCode == 456)
                {
                    throw new Exception("Закончился лимит переводов по тарифу");
                }

                throw new Exception($"Неизвестная ошибка. Код ответа: {(int)response.StatusCode}");
        }
    }

    static void CheckApiKey()
    {
        string apiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");

        if (string.IsNullOrEmpty(apiKey)) 
        {
            throw new Exception("ключ пустой или введён неправильно. Проверьте переменную окружения DEEPL_API_KEY");
        }
    }

    static async Task<string> GetTranslateAsync(string text)
    {
        string apiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");

        var values = new Dictionary<string, string>
        {
            { "auth_key", apiKey },
            { "text", text },
            { "source_lang", "EN" },
            { "target_lang", "RU" }
        };

        var content = new FormUrlEncodedContent(values);

        var url = "https://api-free.deepl.com/v2/translate";

        var response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            CheckStatusCode(response);
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();

        // 1. Превращаем строку в дерево
        JsonDocument document = JsonDocument.Parse(jsonResponse);

        // 2. Берём корень
        JsonElement root = document.RootElement;

        // 3. Берём массив translations
        JsonElement translations = root.GetProperty("translations");

        // 4. Берём первый элемент массива
        JsonElement firstTranslation = translations[0];

        // 5. Достаём текст
        string translatedText = firstTranslation.GetProperty("text").GetString();

        return translatedText;
    }
}