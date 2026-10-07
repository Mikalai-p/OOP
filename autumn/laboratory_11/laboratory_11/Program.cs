using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

public static class Reflector
{
    private static readonly Dictionary<Type, Func<object>> _generators = new()
    {
        { typeof(int), () => new Random().Next(1, 100) },
        { typeof(string), () => Guid.NewGuid().ToString().Substring(0, 8) },
        { typeof(bool), () => new Random().Next(0, 2) == 1 },
        { typeof(decimal), () => (decimal)new Random().NextDouble() * 100m },
        { typeof(double), () => new Random().NextDouble() * 100 },
        { typeof(DateTime), () => DateTime.Now.AddDays(new Random().Next(-365, 365)) }
    };

    public static void SaveToFile(string content, string fileName = "reflector_output.txt")
    {
        File.AppendAllText(fileName, content + Environment.NewLine);
        Console.WriteLine(content); 
    }

    public static void GetAssemblyName(string className, string fileName = "reflector_output.txt")
    {
        Type type = GetTypeByName(className);
        SaveToFile($"Class: {className}, Assembly: {type.Assembly.GetName().Name}", fileName);
    }

    public static void HasPublicConstructors(string className, string fileName = "reflector_output.txt")
    {
        Type type = GetTypeByName(className);
        bool hasPublicConstructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any();
        SaveToFile($"Class: {className}, Has public constructors: {hasPublicConstructors}", fileName);
    }

    public static IEnumerable<string> GetPublicMethods(string className)
    {
        Type type = GetTypeByName(className);
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                   .Where(m => !m.IsSpecialName) // Исключаем методы доступа к свойствам
                   .Select(m => $"{m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})")
                   .Distinct();
    }

    public static void WritePublicMethods(string className, string fileName = "reflector_output.txt")
    {
        var methods = GetPublicMethods(className);
        SaveToFile($"Class: {className}, Public methods: {string.Join("; ", methods)}", fileName);
    }

    public static IEnumerable<string> GetFieldsAndProperties(string className)
    {
        Type type = GetTypeByName(className);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Select(f => $"Field: {f.Name} ({f.FieldType.Name})");
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Select(p => $"Property: {p.Name} ({p.PropertyType.Name})");
        return fields.Concat(properties);
    }

    public static void WriteFieldsAndProperties(string className, string fileName = "reflector_output.txt")
    {
        SaveToFile($"Class: {className}, Fields and properties: {string.Join("; ", GetFieldsAndProperties(className))}", fileName);
    }

    public static IEnumerable<string> GetInterfaces(string className)
    {
        Type type = GetTypeByName(className);
        return type.GetInterfaces().Select(i => i.Name);
    }

    public static void WriteInterfaces(string className, string fileName = "reflector_output.txt")
    {
        SaveToFile($"Class: {className}, Interfaces: {string.Join(", ", GetInterfaces(className))}", fileName);
    }

    public static IEnumerable<string> GetMethodsByParameterType(string className, Type parameterType)
    {
        Type type = GetTypeByName(className);
        return type.GetMethods()
            .Where(m => m.GetParameters()
                .Any(p => p.ParameterType == parameterType))
            .Select(m => m.Name);
    }

    public static void WriteMethodsByParameterType(string className, Type parameterType, string fileName = "reflector_output.txt")
    {
        SaveToFile($"Class: {className}, Methods with {parameterType.Name} parameter: {string.Join(", ", GetMethodsByParameterType(className, parameterType))}", fileName);
    }

    public static object Invoke(object obj, string methodName, object[] parameters)
    {
        Type type = obj.GetType();
        MethodInfo method = type.GetMethod(methodName)
            ?? throw new ArgumentException($"Method {methodName} not found in type {type.Name}");

        return method.Invoke(obj, parameters);
    }

    public static object InvokeFromFile(string className, string methodName, string fileName = "parameters.txt")
    {
        Type type = GetTypeByName(className);

        object instance = Create(type);

        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Parameters file {fileName} not found");
        }

        string[] paramLines = File.ReadAllLines(fileName);
        MethodInfo method = type.GetMethod(methodName)
            ?? throw new ArgumentException($"Method {methodName} not found");

        ParameterInfo[] methodParams = method.GetParameters();
        object[] parameters = new object[methodParams.Length];

        for (int i = 0; i < methodParams.Length && i < paramLines.Length; i++)
        {
            parameters[i] = Convert.ChangeType(paramLines[i].Trim(), methodParams[i].ParameterType);
        }

        return Invoke(instance, methodName, parameters);
    }

    public static object InvokeWithGeneratedParameters(string className, string methodName)
    {
        Type type = GetTypeByName(className);
        object instance = Create(type);

        MethodInfo method = type.GetMethod(methodName)
            ?? throw new ArgumentException($"Method {methodName} not found");

        ParameterInfo[] methodParams = method.GetParameters();
        object[] parameters = new object[methodParams.Length];

        for (int i = 0; i < methodParams.Length; i++)
        {
            Type paramType = methodParams[i].ParameterType;
            if (_generators.ContainsKey(paramType))
            {
                parameters[i] = _generators[paramType]();
            }
            else
            {
                parameters[i] = null;
            }
        }

        Console.WriteLine($"Invoking {methodName} with generated parameters: {string.Join(", ", parameters)}");
        return Invoke(instance, methodName, parameters);
    }

    public static T Create<T>() where T : class
    {
        Type type = typeof(T);
        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException($"No parameterless constructor found for {type.Name}");
        return (T)constructor.Invoke(null);
    }

    public static object Create(Type type)
    {
        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException($"No parameterless constructor found for {type.Name}");
        return constructor.Invoke(null);
    }

    private static Type GetTypeByName(string className)
    {
        Type type = Type.GetType(className)
                    ?? Type.GetType($"{className}, {Assembly.GetExecutingAssembly().GetName().Name}");

        if (type == null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(className);
                if (type != null) break;
            }
        }

        return type ?? throw new ArgumentException($"Type {className} not found");
    }
}

public class Person
{
    public string Name { get; set; } = "Unknown";
    public int Age { get; set; }

    public Person() { }
    public Person(string name) => Name = name;
    public Person(string name, int age) { Name = name; Age = age; }

    public void Display() => Console.WriteLine($"Person: Name: {Name}, Age: {Age}");
    public void UpdateAge(int newAge)
    {
        Age = newAge;
        Console.WriteLine($"Age updated to: {Age}");
    }

    public string GetInfo() => $"Person: {Name}, {Age} years old";
}

public class Calculator
{
    public Calculator() { } 

    public double Add(double a, double b)
    {
        double result = a + b;
        Console.WriteLine($"Calculator.Add({a}, {b}) = {result}");
        return result;
    }

    public bool IsPositive(int number)
    {
        bool result = number > 0;
        Console.WriteLine($"Calculator.IsPositive({number}) = {result}");
        return result;
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine($"Calculator message: {message}");
    }
}

public class StringAnalyzer
{
    public int GetLength(string text) => text.Length;
    public bool ContainsSubstring(string text, string substring) => text.Contains(substring);
}

class Program
{
    static void Main()
    {
        if (File.Exists("reflector_output.txt"))
            File.Delete("reflector_output.txt");

        Console.WriteLine("=== Researching Person class ===");
        ResearchPerson();

        Console.WriteLine("\n=== Researching Calculator class ===");
        ResearchCalculator();

        Console.WriteLine("\n=== Researching .NET String class ===");
        ResearchStringClass();

        Console.WriteLine("\n=== Demonstration completed ===");
    }

    static void ResearchPerson()
    {
        Reflector.GetAssemblyName("Person");
        Reflector.HasPublicConstructors("Person");
        Reflector.WritePublicMethods("Person");
        Reflector.WriteFieldsAndProperties("Person");
        Reflector.WriteInterfaces("Person");
        Reflector.WriteMethodsByParameterType("Person", typeof(int));

        var person = Reflector.Create<Person>();
        person.Display();

        Reflector.InvokeWithGeneratedParameters("Person", "UpdateAge");

        File.WriteAllText("parameters.txt", "25");
        Reflector.InvokeFromFile("Person", "UpdateAge");
    }

    static void ResearchCalculator()
    {
        Reflector.GetAssemblyName("Calculator");
        Reflector.HasPublicConstructors("Calculator");
        Reflector.WritePublicMethods("Calculator");
        Reflector.WriteFieldsAndProperties("Calculator");
        Reflector.WriteInterfaces("Calculator");

        Reflector.InvokeWithGeneratedParameters("Calculator", "Add");
        Reflector.InvokeWithGeneratedParameters("Calculator", "IsPositive");

        var calculator = Reflector.Create<Calculator>();
        Reflector.Invoke(calculator, "DisplayMessage", new object[] { "Hello from Reflector!" });
    }

    static void ResearchStringClass()
    {
        Reflector.GetAssemblyName("System.String");
        Reflector.HasPublicConstructors("System.String");
        Reflector.WritePublicMethods("System.String");

        Reflector.WriteMethodsByParameterType("System.String", typeof(string));
    }
}