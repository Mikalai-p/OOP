using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections; 

public class CustomList : IEnumerable<int> 
{
    private List<int> items;

    
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

    
    public CustomList()
    {
        items = new List<int>();
        InitializeNestedObjects();
    }

    public CustomList(IEnumerable<int> collection)
    {
        items = new List<int>(collection);
        InitializeNestedObjects();
    }

    private void InitializeNestedObjects()
    {
        Production = new Production { Id = 1, OrganizationName = "IT Solutions Inc." };
        Dev = new Developer(101, "Pinchuk Nikolai", "Software Development");
    }

    
    public int this[int index]
    {
        get => items[index];
        set => items[index] = value;
    }

    
    public void Add(int item) => items.Add(item);
    public void Remove(int item) => items.Remove(item);
    public int Count => items.Count;
    public bool Contains(int item) => items.Contains(item);

    
    public IEnumerator<int> GetEnumerator() => items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    
    public static CustomList operator +(int item, CustomList list)
    {
        CustomList result = new CustomList(list.items);
        result.items.Insert(0, item);
        return result;
    }

    public static CustomList operator --(CustomList list)
    {
        if (list.items.Count > 0)
        {
            CustomList result = new CustomList(list.items);
            result.items.RemoveAt(0);
            return result;
        }
        return new CustomList(list.items);
    }

    public static bool operator !=(CustomList list1, CustomList list2)
    {
        return !(list1 == list2);
    }

    public static bool operator ==(CustomList list1, CustomList list2)
    {
        if (ReferenceEquals(list1, list2)) return true;
        if ((object)list1 == null || (object)list2 == null) return false;
        return list1.items.SequenceEqual(list2.items);
    }

    public static CustomList operator *(CustomList list1, CustomList list2)
    {
        CustomList result = new CustomList(list1.items);
        result.items.AddRange(list2.items);
        return result;
    }

    public override bool Equals(object obj)
    {
        return obj is CustomList list && this == list;
    }

    public override int GetHashCode()
    {
        return items.GetHashCode();
    }

    public override string ToString()
    {
        return $"CustomList: [{string.Join(", ", items)}]";
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

public static class StatisticOperation
{
        public static int Sum(CustomList list)
    {
        return list.Sum();
    }

    
    public static int DifferenceMaxMin(CustomList list)
    {
        if (!list.Any()) return 0;
        return list.Max() - list.Min();
    }

    
    public static int CountElements(CustomList list)
    {
        return list.Count;
    }

    
    public static int CountCapitalizedWords(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return 0;

        return str.Split(new char[] { ' ', '.', ',', '!', '?' },
                        StringSplitOptions.RemoveEmptyEntries)
                 .Count(word => word.Length > 0 && char.IsUpper(word[0]));
    }

    
    public static bool HasDuplicates(this CustomList list)
    {
        return list.Count != list.Distinct().Count();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== ТЕСТИРОВАНИЕ КЛАССА CUSTOMLIST ===\n");

        
        CustomList list1 = new CustomList(new int[] { 1, 2, 3 });
        CustomList list2 = new CustomList(new int[] { 4, 5, 6 });

        Console.WriteLine($"list1: {list1}");
        Console.WriteLine($"list2: {list2}");

        
        Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ПЕРЕГРУЖЕННЫХ ОПЕРАЦИЙ ===");

        CustomList list3 = 0 + list1;
        Console.WriteLine($"0 + list1: {list3}");

        CustomList list4 = --list1;
        Console.WriteLine($"--list1: {list4}");

        Console.WriteLine($"list1 != list2: {list1 != list2}");
        Console.WriteLine($"list1 != list1: {list1 != list1}");

        CustomList list5 = list1 * list2;
        Console.WriteLine($"list1 * list2: {list5}");

        
        Console.WriteLine("\n=== ВЛОЖЕННЫЕ ОБЪЕКТЫ ===");
        Console.WriteLine(list1.Production);
        Console.WriteLine(list1.Dev);

        
        Console.WriteLine("\n=== STATISTICOPERATION ===");
        Console.WriteLine($"Сумма list1: {StatisticOperation.Sum(list1)}");
        Console.WriteLine($"Разница макс-мин list5: {StatisticOperation.DifferenceMaxMin(list5)}");
        Console.WriteLine($"Количество элементов list5: {StatisticOperation.CountElements(list5)}");

        
        Console.WriteLine("\n=== МЕТОДЫ РАСШИРЕНИЯ ===");

        string testString = "Hello World This Is a Test String";
        Console.WriteLine($"Строка: '{testString}'");
        Console.WriteLine($"Слов с заглавной буквы: {testString.CountCapitalizedWords()}");

        CustomList duplicateList = new CustomList(new int[] { 1, 2, 3, 2, 5 });
        Console.WriteLine($"Список с дубликатами: {duplicateList}");
        Console.WriteLine($"Есть дубликаты: {duplicateList.HasDuplicates()}");
        Console.WriteLine($"list1 имеет дубликаты: {list1.HasDuplicates()}");

        
        Console.WriteLine("\n=== ДОПОЛНИТЕЛЬНОЕ ТЕСТИРОВАНИЕ ===");

        Console.WriteLine($"list1[0]: {list1[0]}");
        list1[0] = 10;
        Console.WriteLine($"После list1[0] = 10: {list1}");

        list1.Add(7);
        Console.WriteLine($"После Add(7): {list1}");

        list1.Remove(10);
        Console.WriteLine($"После Remove(10): {list1}");

        Console.WriteLine($"Contains(3): {list1.Contains(3)}");
        Console.WriteLine($"Count: {list1.Count}");
    }
}