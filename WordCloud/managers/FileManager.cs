public class FileManager
{
    private string file_path;
    private readonly Lock file_lock = new();

    public FileManager(string file_path)
    {
        this.file_path = file_path;
        
        // Instead of removing the file, the name could have a date
        if (File.Exists(file_path))
        {
            File.Delete(file_path);
        }
    }

    /*
     * Writes a string in the file
     */
    public void Write(string text)
    {
        lock (file_lock)
        {
            using (FileStream fs = new FileStream(file_path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(text);
            }
        }
    }
}