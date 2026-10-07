using System;
using System.Linq;

class Program
{
    static void Main()
    {
        
        int[] numbers = { 3, 7, 2, 9, 5 };
        string text = "Hello";

        
        (int max, int min, int sum, char firstChar) ProcessData(int[] array, string str)
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Массив не может быть пустым");
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("Строка не может быть пустой");

            int max = array.Max();
            int min = array.Min();
            int sum = array.Sum();
            char firstChar = str[0];

            return (max, min, sum, firstChar);
        }

        
        var result = ProcessData(numbers, text);

        
        Console.WriteLine($"Максимальный элемент: {result.max}");
        Console.WriteLine($"Минимальный элемент: {result.min}");
        Console.WriteLine($"Сумма элементов: {result.sum}");
        Console.WriteLine($"Первая буква строки: {result.firstChar}");
    }
}