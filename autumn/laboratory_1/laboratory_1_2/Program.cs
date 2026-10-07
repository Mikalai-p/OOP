using System;
using System.Text;

class Program
{
    static void Main()
    {
        
        string str1 = "hello";
        string str2 = "world";
        string str3 = "hello";

        Console.WriteLine($"a. Сравнение строк:");
        Console.WriteLine($"str1 == str2: {str1 == str2}"); 
        Console.WriteLine($"str1 == str3: {str1 == str3}"); 
        Console.WriteLine($"string.ReferenceEquals(str1, str3): {string.ReferenceEquals(str1, str3)}"); 
        Console.WriteLine();

        
        string s1 = "C# ";
        string s2 = "is ";
        string s3 = "powerful";

        
        string concatenated = s1 + s2 + s3;
        Console.WriteLine($"b. Сцепление: {concatenated}");

        
        string copied = string.Copy(concatenated);
        Console.WriteLine($"Копирование: {copied}");

        
        string substring = concatenated.Substring(3, 5);
        Console.WriteLine($"Подстрока (индекс 3, длина 5): {substring}");

        
        string[] words = concatenated.Split(' ');
        Console.WriteLine("Разделение на слова:");
        foreach (var word in words)
            Console.WriteLine($"  {word}");

       
        string inserted = concatenated.Insert(3, "very ");
        Console.WriteLine($"Вставка 'very' на позицию 3: {inserted}");

       
        string removed = inserted.Remove(3, 5);
        Console.WriteLine($"Удаление подстроки (индекс 3, длина 5): {removed}");
        Console.WriteLine();

        
        int version = 9;
        string interpolated = $"C# {version} {s2} {s3}";
        Console.WriteLine($"Интерполирование: {interpolated}");
        Console.WriteLine();

       
        string emptyString = "";
        string nullString = null;

        Console.WriteLine("c. Проверка пустой и null строки:");
        Console.WriteLine($"string.IsNullOrEmpty(emptyString): {string.IsNullOrEmpty(emptyString)}");
        Console.WriteLine($"string.IsNullOrEmpty(nullString): {string.IsNullOrEmpty(nullString)}");

        
        Console.WriteLine($"emptyString.Length: {emptyString.Length}"); // 0
        
        Console.WriteLine($"emptyString.ToUpper(): {emptyString.ToUpper()}"); 
        Console.WriteLine();

        
        StringBuilder sb = new StringBuilder("Hello World!");
        Console.WriteLine($"d. Исходный StringBuilder: {sb}");

        
        sb.Remove(5, 7);
        Console.WriteLine($"После удаления (индекс 5, длина 7): {sb}");

        
        sb.Insert(0, "Start: ");
        sb.Append(" :End");
        Console.WriteLine($"После добавления в начало и конец: {sb}");
    }
}