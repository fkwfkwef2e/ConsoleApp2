using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

class Figure
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Figure(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

class FileManager
{
    private readonly string _filePath;

    public FileManager(string filePath)
    {
        _filePath = filePath;
    }

    public Figure Load()
    {
        string fileExtension = Path.GetExtension(_filePath).ToLower();

        if (fileExtension == ".txt")
        {
            string[] lines = File.ReadAllLines(_filePath);
            string[] data = lines[0].Split(',');
            return new Figure(data[0], int.Parse(data[1]), int.Parse(data[2]));
        }
        else if (fileExtension == ".json")
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<Figure>(json);
        }
        else if (fileExtension == ".xml")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Figure));
            using (FileStream fileStream = new FileStream(_filePath, FileMode.Open))
            {
                return (Figure)serializer.Deserialize(fileStream);
            }
        }
        else
        {
            throw new NotSupportedException("Unsupported file format");
        }
    }

    public void Save(Figure figure)
    {
        string fileExtension = Path.GetExtension(_filePath).ToLower();

        if (fileExtension == ".txt")
        {
            string data = $"{figure.Name},{figure.Width},{figure.Height}";
            File.WriteAllText(_filePath, data);
        }
        else if (fileExtension == ".json")
        {
            string json = JsonConvert.SerializeObject(figure);
            File.WriteAllText(_filePath, json);
        }
        else if (fileExtension == ".xml")
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Figure));
            using (FileStream fileStream = new FileStream(_filePath, FileMode.Create))
            {
                serializer.Serialize(fileStream, figure);
            }
        }
        else
        {
            throw new NotSupportedException("Unsupported file format");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the path of the file to open: ");
        string filePath = Console.ReadLine();

        FileManager fileManager = new FileManager(filePath);
        Figure figure = fileManager.Load();

        Console.WriteLine($"Name: {figure.Name}, Width: {figure.Width}, Height: {figure.Height}");

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.F1)
            {
                fileManager.Save(figure);
                Console.WriteLine("File saved");
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}