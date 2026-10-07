using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

[Serializable]
[DataContract]
public class Address
{
    [DataMember]
    public string Street { get; set; }
    [DataMember]
    public string City { get; set; }

    public Address() { }

    public Address(string street, string city)
    {
        Street = street;
        City = city;
    }
}

[Serializable]
[DataContract]
[KnownType(typeof(Employee))]
public class Person
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public int Age { get; set; }

    public Person() { }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

[Serializable]
[DataContract]
public class Employee : Person
{
    [DataMember]
    public string Department { get; set; }
    [DataMember]
    public Address Address { get; set; } 

    [NonSerialized] 
    [System.Text.Json.Serialization.JsonIgnore] 
    [XmlIgnore] 
    public decimal Salary;

    public Employee() { }

    public Employee(string name, int age, string department, Address address, decimal salary)
        : base(name, age)
    {
        Department = department;
        Address = address;
        Salary = salary;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Age: {Age}, Department: {Department}, " +
               $"Address: {Address?.Street}, {Address?.City}, Salary: {Salary}";
    }
}

public interface ISerializer
{
    void Serialize<T>(IEnumerable<T> objects, string filePath);
    IEnumerable<T> Deserialize<T>(string filePath);
}


public class BinarySerializer : ISerializer
{
    public void Serialize<T>(IEnumerable<T> objects, string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Create);
        using var writer = new BinaryWriter(stream);

        var list = objects.ToList();
        writer.Write(list.Count);

        foreach (var obj in list)
        {
            if (obj is Employee emp)
            {
                writer.Write(emp.Name ?? "");
                writer.Write(emp.Age);
                writer.Write(emp.Department ?? "");
                writer.Write(emp.Address?.Street ?? "");
                writer.Write(emp.Address?.City ?? "");
            }
        }
    }

    public IEnumerable<T> Deserialize<T>(string filePath)
    {
        var result = new List<T>();

        using var stream = new FileStream(filePath, FileMode.Open);
        using var reader = new BinaryReader(stream);

        var count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            var name = reader.ReadString();
            var age = reader.ReadInt32();
            var department = reader.ReadString();
            var street = reader.ReadString();
            var city = reader.ReadString();

            var employee = new Employee(
                name, age, department,
                new Address(street, city),
                0 
            );

            result.Add((T)(object)employee);
        }

        return result;
    }
}

public class DataContractSerializer : ISerializer
{
    public void Serialize<T>(IEnumerable<T> objects, string filePath)
    {
        var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(List<T>));
        using var writer = System.Xml.XmlWriter.Create(filePath, new System.Xml.XmlWriterSettings { Indent = true });
        serializer.WriteObject(writer, objects.ToList());
    }

    public IEnumerable<T> Deserialize<T>(string filePath)
    {
        var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(List<T>));
        using var reader = System.Xml.XmlReader.Create(filePath);
        return (IEnumerable<T>)serializer.ReadObject(reader);
    }
}

public class JsonSerializer : ISerializer
{
    public void Serialize<T>(IEnumerable<T> objects, string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(filePath, System.Text.Json.JsonSerializer.Serialize(objects, options));
    }

    public IEnumerable<T> Deserialize<T>(string filePath)
    {
        return System.Text.Json.JsonSerializer.Deserialize<List<T>>(File.ReadAllText(filePath));
    }
}

public class XmlSerializer : ISerializer
{
    public void Serialize<T>(IEnumerable<T> objects, string filePath)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
        using var writer = new StreamWriter(filePath);
        serializer.Serialize(writer, objects.ToList());
    }

    public IEnumerable<T> Deserialize<T>(string filePath)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<T>));
        using var reader = new StreamReader(filePath);
        return (IEnumerable<T>)serializer.Deserialize(reader);
    }
}

public static class SerializerFactory
{
    private static readonly Dictionary<string, ISerializer> _serializers = new()
    {
        ["binary"] = new BinarySerializer(),
        ["datacontract"] = new DataContractSerializer(),
        ["json"] = new JsonSerializer(),
        ["xml"] = new XmlSerializer()
    };

    public static ISerializer GetSerializer(string format)
    {
        if (_serializers.TryGetValue(format.ToLower(), out var serializer))
            return serializer;
        throw new ArgumentException($"Unsupported format: {format}");
    }

    public static IEnumerable<string> SupportedFormats => _serializers.Keys;
}

class Program
{
    static void Main()
    {
        // Создание коллекции объектов
        var employees = new List<Employee>
        {
            new Employee("John Doe", 30, "IT", new Address("123 Main St", "New York"), 50000),
            new Employee("Jane Smith", 25, "HR", new Address("456 Oak Ave", "Los Angeles"), 45000),
            new Employee("Bob Johnson", 35, "Finance", new Address("789 Pine Rd", "Chicago"), 60000)
        };

        Console.WriteLine("Original employees:");
        foreach (var emp in employees)
        {
            Console.WriteLine($"  {emp}");
        }
        Console.WriteLine();

        // Тестирование всех сериализаторов
        foreach (var format in SerializerFactory.SupportedFormats)
        {
            Console.WriteLine($"=== {format.ToUpper()} Serialization ===");

            string filePath = $"employees.{format}";
            var serializer = SerializerFactory.GetSerializer(format);

            // Сериализация
            serializer.Serialize(employees, filePath);
            Console.WriteLine($"Serialized to {filePath}");

            // Десериализация
            var deserializedEmployees = serializer.Deserialize<Employee>(filePath).ToList();

            Console.WriteLine("Deserialized employees (Salary should be 0):");
            foreach (var emp in deserializedEmployees)
            {
                Console.WriteLine($"  {emp}");
            }

            // Показать содержимое файла для текстовых форматов
            if (format == "json" || format == "xml" || format == "datacontract")
            {
                Console.WriteLine($"File content (first 500 chars):");
                var content = File.ReadAllText(filePath);
                Console.WriteLine(content.Substring(0, Math.Min(500, content.Length)) + "...");
            }
            Console.WriteLine();
        }

        // 6. XPath селекторы для XML
        Console.WriteLine("=== XPath Selectors ===");
        var xmlDoc = XDocument.Load("employees.xml");

        // Селектор 1: Все имена сотрудников
        var names = xmlDoc.XPathSelectElements("//Employee/Name").Select(n => n.Value);
        Console.WriteLine("All employee names: " + string.Join(", ", names));

        // Селектор 2: Сотрудники из Нью-Йорка
        var nyEmployees = xmlDoc.XPathSelectElements("//Employee[Address/City='New York']");
        Console.WriteLine("Employees from New York:");
        foreach (var emp in nyEmployees)
        {
            Console.WriteLine($"  {emp.Element("Name")?.Value}");
        }
        Console.WriteLine();

        // 7. LINQ to XML - создание нового документа и запросы
        Console.WriteLine("=== LINQ to XML ===");

        // Создание нового XML документа
        var newEmployees = new XElement("Employees",
            from e in employees
            select new XElement("Employee",
                new XElement("Name", e.Name),
                new XElement("Age", e.Age),
                new XElement("Department", e.Department),
                new XElement("Address",
                    new XElement("Street", e.Address.Street),
                    new XElement("City", e.Address.City)),
                new XElement("Salary", e.Salary) // Это поле будет в новом документе
            )
        );

        var newXmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), newEmployees);
        newXmlDoc.Save("new_employees.xml");
        Console.WriteLine("Created new_employees.xml with LINQ to XML");

        // Запросы к новому XML
        var linqNames = newXmlDoc.Descendants("Name").Select(n => n.Value);
        Console.WriteLine("Names from new XML: " + string.Join(", ", linqNames));

        var adults = newXmlDoc.Descendants("Employee")
            .Where(e => (int)e.Element("Age") > 28)
            .Select(e => e.Element("Name").Value);
        Console.WriteLine("Employees older than 28: " + string.Join(", ", adults));

        var byCity = newXmlDoc.Descendants("Employee")
            .GroupBy(e => e.Element("Address").Element("City").Value)
            .Select(g => new { City = g.Key, Count = g.Count() });

        Console.WriteLine("Employees by city:");
        foreach (var group in byCity)
        {
            Console.WriteLine($"  {group.City}: {group.Count}");
        }

        // LINQ to JSON пример
        Console.WriteLine("\n=== LINQ to JSON ===");
        var jsonString = File.ReadAllText("employees.json");
        using var jsonDoc = JsonDocument.Parse(jsonString);

        var jsonNames = jsonDoc.RootElement.EnumerateArray()
            .Select(e => e.GetProperty("Name").GetString());
        Console.WriteLine("Names from JSON: " + string.Join(", ", jsonNames));

        var highSalaryEmployees = employees
            .Where(e => e.Salary > 47000)
            .Select(e => e.Name);
        Console.WriteLine("Employees with salary > 47000: " + string.Join(", ", highSalaryEmployees));

        // Очистка
       /* try
        {
            foreach (var format in SerializerFactory.SupportedFormats)
            {
                File.Delete($"employees.{format}");
            }
            File.Delete("new_employees.xml");
            Console.WriteLine("\nTemporary files cleaned up.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cleaning files: {ex.Message}");
        }*/
    }
}