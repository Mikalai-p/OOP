using System;

class Program
{
    static void Main()
    {
        
        int[,] matrix = {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9}
        };

        Console.WriteLine("a. Двумерный массив (матрица):");
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i, j]}\t");
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        
        string[] stringArray = { "apple", "banana", "cherry", "date" };

        Console.WriteLine("b. Одномерный массив строк:");
        Console.WriteLine($"Содержимое: {string.Join(", ", stringArray)}");
        Console.WriteLine($"Длина массива: {stringArray.Length}");

        
        Console.Write("Введите индекс для изменения (0-3): ");
        int index = int.Parse(Console.ReadLine());
        Console.Write("Введите новое значение: ");
        stringArray[index] = Console.ReadLine();

        Console.WriteLine($"Обновленный массив: {string.Join(", ", stringArray)}");
        Console.WriteLine();

        
        double[][] jaggedArray = new double[3][];
        jaggedArray[0] = new double[2];
        jaggedArray[1] = new double[3];
        jaggedArray[2] = new double[4];

        Console.WriteLine("c. Введите значения для ступенчатого массива:");
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                Console.Write($"Элемент [{i}][{j}]: ");
                jaggedArray[i][j] = double.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("Ступенчатый массив:");
        for (int i = 0; i < jaggedArray.Length; i++)
        {
            Console.Write($"Строка {i}: ");
            for (int j = 0; j < jaggedArray[i].Length; j++)
            {
                Console.Write($"{jaggedArray[i][j]}\t");
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        
        var implicitArray = new[] { 1, 2, 3, 4, 5 };
        var implicitString = "Неявно типизированная строка";

        Console.WriteLine("d. Неявно типизированные переменные:");
        Console.WriteLine($"Тип implicitArray: {implicitArray.GetType()}");
        Console.WriteLine($"Тип implicitString: {implicitString.GetType()}");
        Console.WriteLine($"Массив: {string.Join(", ", implicitArray)}");
        Console.WriteLine($"Строка: {implicitString}");
    }
}