using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace BookTracker
{
    internal class Program
    {
        static readonly string TitleURL = $"https://openlibrary.org/search.json?q=";
        static readonly string AuthorURL = $"https://openlibrary.org/search/authors.json?q=";

        static async Task Main(string[] args)
        {
            bool continueProgram = true;

            do
            {
                Console.WriteLine("Benvenuto in BookTracker. Scrivi 1 per cercare per titolo, 2 per autore, 3 per ISBN. Scrivi 0 per uscire.");

                switch (Console.ReadLine())
                {
                    case "0":
                        continueProgram = false;
                        break;
                    case "1":
                        await SearchByTitle();
                        break;
                    case "2":
                        await SearchByAuthor();
                        break;
                    case "3":
                        await SearchByISBN();
                        break;
                }
            }
            while (continueProgram);
        }

        static async Task SearchByTitle()
        {
            Console.WriteLine("Inserisci il titolo:");
            string text = SanitizeInput(Console.ReadLine());
            string URL = TitleURL + text;
            string response = await SearchOnWeb(URL);

            JsonDocument document = JsonDocument.Parse(response);
            JsonElement root = document.RootElement;
            JsonElement docsArray = root.GetProperty("docs");
            JsonElement numFound = root.GetProperty("numFound");
            Console.WriteLine($"Trovati {numFound.ToString()} valori. Verranno mostrati i primi 10 valori.");
            int count = 0;
            foreach (JsonElement doc in docsArray.EnumerateArray())
            {
                Console.WriteLine($"{doc.GetProperty("title")} - {doc.GetProperty("author_name")}");
                if (++count > 10)
                {
                    break;
                }
            }
        }

        static async Task SearchByAuthor()
        {
            Console.WriteLine("Inserisci l'autore:");
            string text = SanitizeInput(Console.ReadLine());
            string URL = AuthorURL + text;
            string response = await SearchOnWeb(URL);

            JsonDocument document = JsonDocument.Parse(response);
            JsonElement root = document.RootElement;
            JsonElement docsArray = root.GetProperty("docs");
            JsonElement numFound = root.GetProperty("numFound");
            Console.WriteLine($"Trovati {numFound.ToString()} valori. Verranno mostrati i primi 10 valori.");
            int count = 0;
            foreach (JsonElement doc in docsArray.EnumerateArray())
            {
                Console.WriteLine($"{doc.GetProperty("name")} conosciuto per {doc.GetProperty("top_work")}");
                if (++count > 10)
                {
                    break;
                }
            }
        }

        static async Task SearchByISBN()
        {

        }

        static string SanitizeInput(string? input)
        {
            return input == null ? "" : Uri.EscapeDataString(input);
        }

        static async Task<string> SearchOnWeb(string URL)
        {
            Console.WriteLine($"Verrà richiesto questo URL: {URL}");

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            //HttpResponseMessage response = await client.GetAsync(CreateURL());
            return await client.GetStringAsync(URL);           
        }

        static void ProcessFile()
        {
            string path = @"C:\Users\luca9\Desktop\file.txt";
            string text = File.ReadAllText(path);

            JsonDocument document = JsonDocument.Parse(text);
            JsonElement root = document.RootElement;
            JsonElement docsArray = root.GetProperty("docs");
            int count = 0;
            foreach (JsonElement doc in docsArray.EnumerateArray())
            {
                Console.WriteLine(doc.GetProperty("title"));
                if (++count > 10)
                {
                    break;
                }
            }
        }
    }
}
