namespace WordCloud;

public class WordManager
{
    private Dictionary<string, int> words_map;
    private string[] stop_words;
    private readonly Lock words_map_lock = new();

    public WordManager()
    {
        words_map = new Dictionary<string, int>();
        // TODO - More stop words can be added
        stop_words = ["a", "for", "the", "and", "is", "in", "to", "of", "it", "that", "with", "on", "as", "are", "an"];
    }

    /*
     * Prints all the words currently in the dictionary
     */
    public void print_words()
    {
        Console.WriteLine($"{words_map.Count} words");
        foreach (var entry in words_map)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
    }

    /*
     * Increments the counter of a word, if it doesn't exist, adds it
     * The words are added in lower case and specified stop words are ignored
     */
    public void increment_word(string word)
    {
        lock (words_map_lock)
        {
            if (!stop_words.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                string word_to_store = word.ToLower();
                if (!words_map.ContainsKey(word_to_store))
                {
                    words_map.Add(word_to_store, 1);
                }
                else
                {
                    words_map[word_to_store]++;
                }
            }
        }
    }

    /*
     * Removes all words with counter of 1 from the dictionary
     */
    public void clean_single_words()
    {
        foreach (var entry in words_map)
        {
            if (entry.Value == 1)
            {
                words_map.Remove(entry.Key);
            }
        }
    }

    /*
     * Creates a report of the words with the frequency and font type
     * Words are ordered starting from most used
     */
    public void create_report(FileManager file_manager)
    {
        string word_string, font_size;
        int maxValue = words_map.Values.Max();
        string filePath = "word_frequencies.txt";
        
        foreach (var entry in words_map.OrderByDescending(e => e.Value))
        {
            if (entry.Value == maxValue)
            {
                font_size = "Huge";
            }
            else if (entry.Value > maxValue * 0.6)
            {
                font_size = "Big";
            }
            else if (entry.Value > maxValue * 0.3)
            {
                font_size = "Normal";
            }
            else
            {
                font_size = "Small";
            }

            word_string = $"word: {entry.Key}, frequency: {entry.Value}, font_size: {font_size}";

            file_manager.Write(word_string);
        }
    }
}
