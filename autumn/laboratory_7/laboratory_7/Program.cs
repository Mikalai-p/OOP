using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections;
//Исключения where T: new(), ...
public interface ICollectionOperations<T>
{
    void Add(T item);
    bool Remove(T item);
    IEnumerable<T> Find(Predicate<T> predicate);
    void ShowAll();
}


public class CollectionType<T> : ICollectionOperations<T>
{
    private List<T> items;

    public Production Production { get; set; }

    public class Developer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }

        public Developer(int id, string fullName, string department)
        {
            Id = id;
            FullName = fullName;
            Department = department;
        }

        public override string ToString()
        {
            return $"Developer: {FullName} (ID: {Id}, Dept: {Department})";
        }
    }

    public Developer Dev { get; set; }

    public CollectionType()
    {
        items = new List<T>();
        InitializeNestedObjects();
    }

    public CollectionType(IEnumerable<T> collection)
    {
        items = new List<T>(collection);
        InitializeNestedObjects();
    }

    private void InitializeNestedObjects()
    {
        Production = new Production { Id = 1, OrganizationName = "IT Solutions Inc." };
        Dev = new Developer(101, "Pinchuk Nikolai", "Software Development");
    }

    public void Add(T item)
    {
        try
        {
            if (item == null && typeof(T).IsClass)
                throw new ArgumentNullException(nameof(item), "Cannot add null item");

            items.Add(item);
            Console.WriteLine($"Item added successfully. Total items: {items.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding item: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Add operation completed");
        }
    }

    public bool Remove(T item)
    {
        try
        {
            if (item == null && typeof(T).IsClass)
                throw new ArgumentNullException(nameof(item), "Cannot remove null item");

            bool removed = items.Remove(item);
            Console.WriteLine(removed ? "Item removed successfully" : "Item not found");
            return removed;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
            return false;
        }
        finally
        {
            Console.WriteLine("Remove operation completed");
        }
    }

    public IEnumerable<T> Find(Predicate<T> predicate)
    {
        try
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");

            return items.FindAll(predicate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finding items: {ex.Message}");
            return new List<T>();
        }
    }

    public void ShowAll()
    {
        Console.WriteLine($"Collection contains {items.Count} items:");
        foreach (var item in items)
        {
            Console.WriteLine($"  {item}");
        }
    }

    
    public void SaveToJsonFile(string filePath)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine($"Collection saved to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public static CollectionType<T> LoadFromJsonFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            string jsonString = File.ReadAllText(filePath);
            List<T> items = JsonSerializer.Deserialize<List<T>>(jsonString);
            return new CollectionType<T>(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
            return new CollectionType<T>();
        }
    }

    
    public T this[int index]
    {
        get => items[index];
        set => items[index] = value;
    }

    public static CollectionType<T> operator +(CollectionType<T> list, T item)
    {
        CollectionType<T> result = new CollectionType<T>(list.items);
        result.items.Add(item);
        return result;
    }

    public static CollectionType<T> operator +(T item, CollectionType<T> list)
    {
        CollectionType<T> result = new CollectionType<T>(list.items);
        result.items.Insert(0, item);
        return result;
    }

    public static CollectionType<T> operator --(CollectionType<T> list)
    {
        if (list.items.Count > 0)
        {
            CollectionType<T> result = new CollectionType<T>(list.items);
            result.items.RemoveAt(0);
            return result;
        }
        return new CollectionType<T>(list.items);
    }

    public override string ToString()
    {
        return $"CollectionType<{typeof(T).Name}>: [{string.Join(", ", items)}]";
    }
}


public class Document : ICloneable, IComparable<Document>
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Pages { get; set; }

    public Document() { }

    public Document(string title, string author, int pages)
    {
        Title = title;
        Author = author;
        Pages = pages;
    }

    public virtual void Print()
    {
        Console.WriteLine($"Document: {Title} by {Author}, {Pages} pages");
    }

    public object Clone()
    {
        return new Document(Title, Author, Pages);
    }

    public int CompareTo(Document other)
    {
        return Pages.CompareTo(other.Pages);
    }

    public override string ToString()
    {
        return $"{Title} by {Author} ({Pages} pages)";
    }
}

public class Book : Document
{
    public string Genre { get; set; }

    public Book() { }

    public Book(string title, string author, int pages, string genre)
        : base(title, author, pages)
    {
        Genre = genre;
    }

    public override void Print()
    {
        Console.WriteLine($"Book: {Title} by {Author}, {Pages} pages, Genre: {Genre}");
    }

    public override string ToString()
    {
        return $"{Title} by {Author} ({Pages} pages, {Genre})";
    }
}


public class Production
{
    public int Id { get; set; }
    public string OrganizationName { get; set; }

    public override string ToString()
    {
        return $"Production: {OrganizationName} (ID: {Id})";
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== ТЕСТИРОВАНИЕ ОБОБЩЕННОЙ КОЛЛЕКЦИИ ===");

        TestWithInt();
        TestWithString();
        TestWithDocument();
        TestFileOperations();
    }

    static void TestWithInt()
    {
        Console.WriteLine("\n--- Тестирование с типом int ---");

        var intCollection = new CollectionType<int>();
        intCollection.Add(1);
        intCollection.Add(2);
        intCollection.Add(3);

        intCollection.ShowAll();

        var evenNumbers = intCollection.Find(x => x % 2 == 0);
        Console.WriteLine("Even numbers: " + string.Join(", ", evenNumbers));
    }

    static void TestWithString()
    {
        Console.WriteLine("\n--- Тестирование с типом string ---");

        var stringCollection = new CollectionType<string>();
        stringCollection.Add("Hello");
        stringCollection.Add("World");
        stringCollection.Add("C#");
        stringCollection.Add("Programming");

        stringCollection.ShowAll();

        var longStrings = stringCollection.Find(s => s.Length > 3);
        Console.WriteLine("Strings longer than 3 chars: " + string.Join(", ", longStrings));

        
        stringCollection.Remove("C#");
        stringCollection.ShowAll();
    }

    static void TestWithDocument()
    {
        Console.WriteLine("\n--- Тестирование с пользовательским классом Document ---");

        var docCollection = new CollectionType<Document>();
        docCollection.Add(new Document("C# Programming", "John Doe", 300));
        docCollection.Add(new Book("CLR via C#", "Jeffrey Richter", 800, "Programming"));
        docCollection.Add(new Document("ASP.NET Core", "Jane Smith", 450));

        docCollection.ShowAll();

        
        var longDocs = docCollection.Find(d => d.Pages > 400);
        Console.WriteLine("Documents with more than 400 pages:");
        foreach (var doc in longDocs)
        {
            Console.WriteLine($"  {doc}");
        }

        
        Console.WriteLine(docCollection.Production);
        Console.WriteLine(docCollection.Dev);

        string filePath1 = "collectionbook.json";

        docCollection.SaveToJsonFile(filePath1);
    }

    static void TestFileOperations()
    {
        Console.WriteLine("\n--- Тестирование операций с файлами ---");

        
        var collection = new CollectionType<string>();
        collection.Add("First");
        collection.Add("Second");
        collection.Add("Third");

        string filePath = "collection.json";
        collection.SaveToJsonFile(filePath);

        
        var loadedCollection = CollectionType<string>.LoadFromJsonFile(filePath);
        Console.WriteLine("Loaded collection:");
        loadedCollection.ShowAll();

        
    }
}