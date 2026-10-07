using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void MutationHandler(object sender, MutationEventArgs e);
public delegate void DeletionHandler(object sender);
public delegate string StringProcessor(string input);
public delegate bool StringPredicate(string str);

public class MutationEventArgs : EventArgs
{
    public int Seed { get; }
    public MutationEventArgs(int seed) => Seed = seed;
}

public class Programmer
{
    
    public event EventHandler StandardDelete;
    public event EventHandler<MutationEventArgs> StandardMutate;

    public event DeletionHandler CustomDelete;                  
    public event MutationHandler CustomMutate;                  
    public void OnStandardDelete() => StandardDelete?.Invoke(this, EventArgs.Empty);
    public void OnStandardMutate(int seed) => StandardMutate?.Invoke(this, new MutationEventArgs(seed));
    public void OnCustomDelete() => CustomDelete?.Invoke(this);
    public void OnCustomMutate(int seed) => CustomMutate?.Invoke(this, new MutationEventArgs(seed));
}

public class StringList
{
    public List<string> List { get; } = new List<string>();
    private readonly Random random = new Random();
    public string Name { get; }

    public StringList(string name)
    {
        Name = name;
    }

    public void DeleteFirstStandard(object sender, EventArgs e)
    {
        if (List.Count > 0)
        {
            Console.WriteLine($"{Name} (стандартный): Удаляю первый элемент '{List[0]}'");
            List.RemoveAt(0);
        }
    }

    public void ShuffleStandard(object sender, MutationEventArgs e)
    {
        Console.WriteLine($"{Name} (стандартный): Перемешиваю список");
        var rng = new Random(e.Seed);
        for (int i = List.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (List[i], List[j]) = (List[j], List[i]);
        }
    }

    public void DeleteFirstCustom(object sender)
    {
        if (List.Count > 0)
        {
            Console.WriteLine($"{Name} (пользовательский): Удаляю первый элемент '{List[0]}'");
            List.RemoveAt(0);
        }
    }

    public void DeleteLastCustom(object sender)
    {
        if (List.Count > 0)
        {
            var last = List[List.Count - 1];
            Console.WriteLine($"{Name} (пользовательский): Удаляю последний элемент '{last}'");
            List.RemoveAt(List.Count - 1);
        }
    }

    public void DeleteRandomCustom(object sender)
    {
        if (List.Count > 0)
        {
            int index = random.Next(List.Count);
            var item = List[index];
            Console.WriteLine($"{Name} (пользовательский): Удаляю случайный элемент '{item}'");
            List.RemoveAt(index);
        }
    }

    public void ShuffleCustom(object sender, MutationEventArgs e)
    {
        Console.WriteLine($"{Name} (пользовательский): Перемешиваю список");
        var rng = new Random(e.Seed);
        for (int i = List.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (List[i], List[j]) = (List[j], List[i]);
        }
    }

    public void ReverseListCustom(object sender, MutationEventArgs e)
    {
        Console.WriteLine($"{Name} (пользовательский): Реверсирую список");
        List.Reverse();
    }

    public void DuplicateFirstCustom(object sender, MutationEventArgs e)
    {
        if (List.Count > 0)
        {
            Console.WriteLine($"{Name} (пользовательский): Дублирую первый элемент");
            List.Insert(0, List[0]);
        }
    }

    public override string ToString() => $"{Name}: [{string.Join(", ", List)}]";
}

class Program
{
    static void Main()
    {
        var programmer = new Programmer();

        var lists = new[] {
            new StringList("List1"),
            new StringList("List2"),
            new StringList("List3"),
            new StringList("List4"),
            new StringList("List5")  
        };

        string[][] initialData = {
            new[] { "One", "Two", "Three", "Four", "Five" },
            new[] { "Alpha", "Beta", "Gamma", "Delta" },
            new[] { "Red", "Green", "Blue" },
            new[] { "Apple", "Banana", "Cherry", "Date" },
            new[] { "X", "Y", "Z" }
        };

        for (int i = 0; i < lists.Length; i++)
        {
            lists[i].List.AddRange(initialData[i]);
        }

        programmer.StandardDelete += lists[0].DeleteFirstStandard;
        programmer.StandardMutate += lists[1].ShuffleStandard;

        programmer.CustomDelete += lists[2].DeleteFirstCustom;
        programmer.CustomDelete += lists[3].DeleteLastCustom;
        programmer.CustomDelete += lists[4].DeleteRandomCustom;

        programmer.CustomMutate += lists[2].ShuffleCustom;
        programmer.CustomMutate += lists[3].ReverseListCustom;
        programmer.CustomMutate += lists[4].DuplicateFirstCustom;

        Console.WriteLine("До событий:");
        foreach (var list in lists)
            Console.WriteLine($"  {list}");

        Console.WriteLine("\n=== Стандартные делегаты ===");
        Console.WriteLine("Первое стандартное удаление:");
        programmer.OnStandardDelete();

        Console.WriteLine("\nПервая стандартная мутация:");
        programmer.OnStandardMutate(DateTime.Now.Millisecond);

        Console.WriteLine("\n=== Пользовательские делегаты ===");
        Console.WriteLine("Первое пользовательское удаление:");
        programmer.OnCustomDelete();

        Console.WriteLine("\nПервая пользовательская мутация:");
        programmer.OnCustomMutate(DateTime.Now.Millisecond + 100);

        Console.WriteLine("\nПосле первого раунда событий:");
        foreach (var list in lists)
            Console.WriteLine($"  {list}");

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("ДЕМОНСТРАЦИЯ РАЗЛИЧНЫХ ТИПОВ ДЕЛЕГАТОВ:");

        string testString = "  Hello,   World! This is a test...  ";

        Console.WriteLine("\n--- СТАНДАРТНЫЕ ДЕЛЕГАТЫ ---");

        Func<string, bool> funcHasLetters = s => s.Any(char.IsLetter);
        Func<string, bool> funcIsLongEnough = s => s.Length > 5;

        Action<string> actionPrint = s => Console.WriteLine($"  Результат: '{s}'");

        Func<string, string> funcRemovePunctuation = s =>
            new string(s.Where(c => !char.IsPunctuation(c)).ToArray());

        Func<string, string> funcToUpper = s => s.ToUpper();

        Console.WriteLine("\n--- ПОЛЬЗОВАТЕЛЬСКИЕ ДЕЛЕГАТЫ ---");

        StringPredicate predicateHasLetters = s => s.Any(char.IsLetter);
        StringPredicate predicateIsLongEnough = s => s.Length > 5;

        StringProcessor processorRemovePunctuation = s =>
            new string(s.Where(c => !char.IsPunctuation(c)).ToArray());

        StringProcessor processorToUpper = s => s.ToUpper();
        StringProcessor processorAddSymbols = s => $"[[{s}]]";

        StringProcessor processorRemoveExtraSpaces = s =>
            string.Join(" ", s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

        StringProcessor processorReverseWords = s =>
            string.Join(" ", s.Split(' ').Reverse());

        Console.WriteLine("\n--- Predicate<T> ДЕЛЕГАТ ---");
        Predicate<string> predicateHasDigits = s => s.Any(char.IsDigit);
        Predicate<string> predicateIsNotEmpty = s => !string.IsNullOrEmpty(s);

        Console.WriteLine($"\nИсходная строка: '{testString}'");

        Console.WriteLine("\nПроверки со стандартными делегатами:");
        Console.WriteLine($"  Содержит буквы (Func): {funcHasLetters(testString)}");
        Console.WriteLine($"  Достаточно длинная (Func): {funcIsLongEnough(testString)}");
        Console.WriteLine($"  Содержит цифры (Predicate): {predicateHasDigits(testString)}");
        Console.WriteLine($"  Не пустая (Predicate): {predicateIsNotEmpty(testString)}");

        Console.WriteLine("\nПроверки с пользовательскими делегатами:");
        Console.WriteLine($"  Содержит буквы (StringPredicate): {predicateHasLetters(testString)}");
        Console.WriteLine($"  Достаточно длинная (StringPredicate): {predicateIsLongEnough(testString)}");

        Console.WriteLine("\n--- КОМПОЗИЦИЯ ДЕЛЕГАТОВ ---");

        Func<string, string> standardPipeline = funcRemovePunctuation;
        standardPipeline = standardPipeline.ComposeStandard(funcToUpper);
        string standardResult = standardPipeline(testString);
        Console.WriteLine($"Стандартный конвейер: '{standardResult}'");

        StringProcessor customPipeline = processorRemovePunctuation;
        customPipeline = customPipeline.ComposeCustom(processorRemoveExtraSpaces);
        customPipeline = customPipeline.ComposeCustom(processorToUpper);
        customPipeline = customPipeline.ComposeCustom(processorReverseWords);
        customPipeline = customPipeline.ComposeCustom(processorAddSymbols);
        string customResult = customPipeline(testString);
        Console.WriteLine($"Пользовательский конвейер: '{customResult}'");

           }
}

public static class FunctionExtensions
{

    public static StringProcessor ComposeCustom(this StringProcessor f1, StringProcessor f2) =>
        x => f2(f1(x));

    public static Func<T, T> ComposeStandard<T>(this Func<T, T> f1, Func<T, T> f2) =>
        x => f2(f1(x));
}