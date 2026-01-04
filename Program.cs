using System.Text.Json;

class Program
{
    static HttpClient client = new HttpClient();
    static async Task Main(string[] args)
    {
        List<string> myList = new List<string>(args);
        if (args.Length == 0)
        {
            Console.WriteLine("Вы не ввели текст");
            // Тут пишу заглушку для проверки ввода текста через консоль
            Console.WriteLine("Введите текст для перевода");
            string myString = Console.ReadLine();
            string translatedString = await GetTranslateAsync(myString);
            Console.WriteLine(translatedString);
            // Тут заканчивается заглушка
        }

        else
        {
            string myString = string.Join(" ", myList);

            CheckApiKey();

            string translatedString = await GetTranslateAsync(myString);
            Console.WriteLine(translatedString);
        }
    }

    static void CheckApiKey()
    {
        string apiKey = Environment.GetEnvironmentVariable("DEEPL_API_KEY");

        if (apiKey == null)
        {
            Console.WriteLine("apiKey введён не правильно");
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