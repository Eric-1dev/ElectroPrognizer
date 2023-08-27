namespace ElectroPrognizer.Services.Models;

public class FileData
{
    public string Name { get; set; }
    public byte[] Content { get; set; }

    public FileData(string name, byte[] content)
    {
        Name = name;
        Content = content;
    }
}
