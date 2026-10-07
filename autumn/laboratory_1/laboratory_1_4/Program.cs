using System;

class Program
{
    static void Main()
    {
        
        var tuple = (42, "Hello", 'A', "World", 123456789UL);

        Console.WriteLine("a. Кортеж создан:");
        Console.WriteLine($"Полный кортеж: {tuple}");
        Console.WriteLine();

       
        Console.WriteLine("b. Вывод кортежа:");
        Console.WriteLine($"Целиком: {tuple}");
        Console.WriteLine($"Элемент 1: {tuple.Item1}");
        Console.WriteLine($"Элемент 3: {tuple.Item3}");
        Console.WriteLine($"Элемент 4: {tuple.Item4}");
        Console.WriteLine();

       
        Console.WriteLine("c. Распаковка кортежа:");

        
        (int num, string str1, char character, string str2, ulong bigNum) = tuple;
        Console.WriteLine($"Полная распаковка: {num}, {str1}, {character}, {str2}, {bigNum}");

        
        (int num2, _, char character2, _, ulong bigNum2) = tuple;
        Console.WriteLine($"Частичная распаковка: {num2}, {character2}, {bigNum2}");

        
        var (num3, str3, character3, str4, bigNum3) = tuple;
        Console.WriteLine($"Распаковка с var: {num3}, {str3}, {character3}, {str4}, {bigNum3}");
        Console.WriteLine();

        
        Console.WriteLine("d. Сравнение кортежей:");

        var tuple1 = (42, "Hello", 'A', "World", 123456789UL);
        var tuple2 = (42, "Hello", 'A', "World", 123456789UL);
        var tuple3 = (100, "Different", 'B', "Values", 999UL);

        Console.WriteLine($"tuple1 == tuple2: {tuple1.Equals(tuple2)}");
        Console.WriteLine($"tuple1 == tuple3: {tuple1.Equals(tuple3)}");

         }
}