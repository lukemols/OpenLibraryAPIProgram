using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace BookTracker
{
    internal class Program
    {
        static readonly string TitleURL = $"https://openlibrary.org/search.json?q=";
        static readonly string AuthorURL = $"https://openlibrary.org/search/authors.json?q=";
        static readonly string ISBNURL = $"https://openlibrary.org/isbn/";
        static readonly string WebsiteURL = $"https://openlibrary.org/";

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

            if (response == null)
            {
                Console.WriteLine($"Errore nella ricerca. Riprova ancora, se il problema persiste, prova con un altro libro.");
            }
            else
            {
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
        }

        static async Task SearchByAuthor()
        {
            Console.WriteLine("Inserisci l'autore:");
            string text = SanitizeInput(Console.ReadLine());
            string URL = AuthorURL + text;
            string response = await SearchOnWeb(URL);

            if (response == null)
            {
                Console.WriteLine($"Errore nella ricerca. Riprova ancora, se il problema persiste, prova con un altro libro.");
            }
            else
            {
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
        }

        static async Task SearchByISBN()
        {
            string text = "";
            do
            {
                Console.WriteLine("Inserisci il codice ISBN (10 o 13 cifre):");
                text = SanitizeISBN(Console.ReadLine());
            }
            while (text.Length != 10 && text.Length != 13);
            string URL = ISBNURL + text + ".json";
            Console.WriteLine($"Inizio la ricerca per ISBN: {text}");
            string response = await SearchOnWeb(URL);

            if (response == null)
            {
                Console.WriteLine($"Errore nella ricerca dell'ISBN. Riprova ancora, se il problema persiste, prova con un altro libro.");
            }
            else
            {
                JsonDocument document = JsonDocument.Parse(response);
                JsonElement root = document.RootElement;

                // Find author from id
                string authors = "";
                JsonElement authorsArray = root.GetProperty("authors");
                foreach (JsonElement author in authorsArray.EnumerateArray())
                {
                    URL = WebsiteURL + author.GetProperty("key") + ".json";
                    string authorResponse = await SearchOnWeb(URL);
                    if (authorResponse != null)
                    {
                        JsonDocument authorDocument = JsonDocument.Parse(authorResponse);
                        JsonElement authorRoot = authorDocument.RootElement;

                        if (authors != "")
                        {
                            authors += " - ";
                        }
                        authors += authorRoot.GetProperty("name").ToString();
                    }
                }

                Console.WriteLine($"{root.GetProperty("title")} by {authors}");
            }
        }

        static string SanitizeInput(string? input)
        {
            return input == null ? "" : Uri.EscapeDataString(input);
        }

        static string SanitizeISBN(string? input)
        {
            if (input != null)
            {
                for (int i = input.Length - 1; i >= 0; i--)
                {
                    if (!Char.IsNumber(input[i]))
                    {
                        input = input.Remove(i, 1);
                    }
                }
            }
            return input;
        }

        static async Task<string> SearchOnWeb(string URL)
        {
            Console.WriteLine($"Verrà richiesto questo URL: {URL}");

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            //HttpResponseMessage response = await client.GetAsync(CreateURL());
            try
            {
                return await client.GetStringAsync(URL);
            }
            catch
            {
                return null;
            }
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
