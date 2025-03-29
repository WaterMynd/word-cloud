using WordCloud;

class Program
{
    // FileManager and WordManager can be singletons
    static FileManager file_manager = new("words_report.txt");
    static WordManager word_manager = new();
    static SemaphoreSlim semaphore = new(20);
    static string[] urls_list = new string[100];
    static HttpClient client = new();

    /*
     * Gets the text from a url
     */
    static async Task<string> scrap_url_text(string url)
    {
        string text = string.Empty;

        Console.WriteLine("Scrapping url: " + url);

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            text = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting text: " + ex.Message);
        }

        return text;
    }

    /*
     * Processes the url, gets the text, cleans it and adds each word to the word repository
     */
    static async Task process_url(string url)
    {
        string text = await scrap_url_text(url);
        Console.WriteLine(text);
        string cleanedText = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        string[] words_list = cleanedText.Split(' ');

        foreach (string word in words_list)
        {
            word_manager.increment_word(word);
        }
    }

    static async Task Main()
    {
        // Populate list with urls (using a python api to test)
        for (int i = 0; i <= 99; i++)
        {
            urls_list[i] = $"http://localhost:5031/api/text/{i}";
        }
        
        // The number of concurrent threads can be changed in the semaphore
        List<Task> tasks = new List<Task>();

        foreach (string url in urls_list)
        {
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await process_url(url);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        Console.WriteLine("All threads started");

        await Task.WhenAll(tasks);

        word_manager.clean_single_words();
        word_manager.create_report(file_manager); // The file manager can be a singleton
        
        Console.WriteLine("Report generated");
    }
}