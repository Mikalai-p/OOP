using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Демонстрация TPL ===");

        try
        {
            // 1. Длительная задача - поиск простых чисел
            Console.WriteLine("\n1. Поиск простых чисел (Решето Эратосфена):");
            await RunPrimeNumberSearchAsync();

            // 2. Задача с токеном отмены
            Console.WriteLine("\n2. Задача с отменой:");
            await RunCancellableTaskAsync();

            // 3. Три задачи с возвратом результата
            Console.WriteLine("\n3. Три задачи с возвратом результата:");
            RunMultipleTasks();

            // 4. Задачи продолжения
            Console.WriteLine("\n4. Задачи продолжения:");
            await RunContinuationTasksAsync();

            // 5. Parallel.For/ForEach
            Console.WriteLine("\n5. Parallel.For/ForEach:");
            RunParallelLoops();

            // 6. Parallel.Invoke
            Console.WriteLine("\n6. Parallel.Invoke:");
            RunParallelInvoke();

            // 7. BlockingCollection
            Console.WriteLine("\n7. BlockingCollection (поставщики/покупатели):");
            await RunBlockingCollectionDemoAsync();

            // 8. Async/Await
            Console.WriteLine("\n8. Async/Await демонстрация:");
            await RunAsyncDemo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    // 1. Длительная задача - поиск простых чисел
    static async Task RunPrimeNumberSearchAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        Task<List<int>> primeTask = Task.Run(() => FindPrimes(100000));

        Console.WriteLine($"ID задачи: {primeTask.Id}");
        Console.WriteLine($"Статус: {primeTask.Status}");
        Console.WriteLine($"Завершена: {primeTask.IsCompleted}");

        var primes = await primeTask;
        stopwatch.Stop();

        Console.WriteLine($"Статус после завершения: {primeTask.Status}");
        Console.WriteLine($"Завершена: {primeTask.IsCompleted}");
        Console.WriteLine($"Найдено простых чисел: {primes.Count}");
        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");
    }

    static List<int> FindPrimes(int n)
    {
        if (n < 2) return new List<int>();

        bool[] isPrime = new bool[n + 1];
        for (int i = 2; i <= n; i++)
            isPrime[i] = true;

        for (int p = 2; p * p <= n; p++)
        {
            if (isPrime[p])
            {
                for (int i = p * p; i <= n; i += p)
                    isPrime[i] = false;
            }
        }

        List<int> primes = new List<int>();
        for (int i = 2; i <= n; i++)
        {
            if (isPrime[i])
                primes.Add(i);
        }
        return primes;
    }

    // 2. Задача с токеном отмены 
    static async Task RunCancellableTaskAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        try
        {
            Task cancelableTask = Task.Run(async () =>
            {
                Console.WriteLine("Задача с отменой начата");
                for (int i = 0; i < 100; i++)
                {

                    token.ThrowIfCancellationRequested();

                    await Task.Delay(100); // Используем асинхронную задержку
                    if (i % 10 == 0)
                        Console.WriteLine($"Выполнение: {i}%");
                }
                Console.WriteLine("Задача завершена успешно");
            }, token);

            await Task.Delay(500);
            cancellationTokenSource.CancelAfter(1000);

            await cancelableTask;
            Console.WriteLine("Задача завершилась успешно");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Задача была отменена через токен отмены");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
        finally
        {
            cancellationTokenSource?.Dispose();
        }
    }

    // 3. Три задачи с возвратом результата
    static void RunMultipleTasks()
    {
        try
        {
            Task<double> task1 = Task.Run(() => CalculateCircleArea(5));
            Task<double> task2 = Task.Run(() => CalculateRectangleArea(4, 6));
            Task<double> task3 = Task.Run(() => CalculateTriangleArea(3, 4, 5));

            Task.WaitAll(task1, task2, task3);

            Task<double> finalTask = Task.Run(() =>
            {
                double totalArea = task1.Result + task2.Result + task3.Result;
                return totalArea * 1.1;
            });

            Console.WriteLine($"Результаты: круг={task1.Result:F2}, прямоугольник={task2.Result:F2}, треугольник={task3.Result:F2}");
            Console.WriteLine($"Общая площадь +10%: {finalTask.Result:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в расчетах: {ex.Message}");
        }
    }

    static double CalculateCircleArea(double radius) => Math.PI * radius * radius;

    static double CalculateRectangleArea(double a, double b) => a * b;

    static double CalculateTriangleArea(double a, double b, double c)
    {
        double p = (a + b + c) / 2;
        return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
    }

    // 4. Задачи продолжения
    static async Task RunContinuationTasksAsync()
    {

        Console.WriteLine("Способ 1: ContinueWith");
        Task<int> firstTask = Task.Run(() => {
            Console.WriteLine("Первая задача выполняется");
            Thread.Sleep(500);
            return 42;
        });

        Task<string> continuationTask = firstTask.ContinueWith(previousTask =>
        {
            return $"Результат предыдущей задачи: {previousTask.Result}";
        });

        Console.WriteLine(continuationTask.Result);

        Console.WriteLine("Способ 2: GetAwaiter/GetResult");
        Task<int> task2 = Task.Run(() => {
            Thread.Sleep(500);
            return 100;
        });

        var awaiter = task2.GetAwaiter();
        awaiter.OnCompleted(() => {
            try
            {
                Console.WriteLine($"Результат через awaiter: {awaiter.GetResult()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в awaiter: {ex.Message}");
            }
        });

        await Task.Delay(600);
    }

    // 5. Parallel.For/ForEach
    static void RunParallelLoops()
    {
        try
        {
            const int size = 1000000;
            int[] numbers = new int[size];
            int[] sequentialResult = new int[size];
            int[] parallelResult = new int[size];

            // Инициализация массива
            Parallel.For(0, size, i => numbers[i] = i % 100);

            // Последовательная обработка
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < size; i++)
            {
                sequentialResult[i] = numbers[i] * numbers[i];
            }
            stopwatch.Stop();
            Console.WriteLine($"Последовательный цикл: {stopwatch.ElapsedMilliseconds} мс");

            // Параллельная обработка
            stopwatch.Restart();
            Parallel.For(0, size, i =>
            {
                parallelResult[i] = numbers[i] * numbers[i];
            });
            stopwatch.Stop();
            Console.WriteLine($"Parallel.For: {stopwatch.ElapsedMilliseconds} мс");

            // Проверка корректности
            bool isCorrect = true;
            for (int i = 0; i < size; i++)
            {
                if (sequentialResult[i] != parallelResult[i])
                {
                    isCorrect = false;
                    break;
                }
            }
            Console.WriteLine($"Результаты корректны: {isCorrect}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в параллельных циклах: {ex.Message}");
        }
    }

    // 6. Parallel.Invoke
    static void RunParallelInvoke()
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();

            Parallel.Invoke(
                () => ProcessData("Данные 1"),
                () => ProcessData("Данные 2"),
                () => ProcessData("Данные 3"),
                () => ProcessData("Данные 4"),
                () => ProcessData("Данные 5")
            );

            stopwatch.Stop();
            Console.WriteLine($"Parallel.Invoke завершен за {stopwatch.ElapsedMilliseconds} мс");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"Ошибка в Parallel.Invoke: {string.Join(", ", ex.InnerExceptions.Select(e => e.Message))}");
        }
    }

    static void ProcessData(string data)
    {
        Console.WriteLine($"Обработка {data} в потоке {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(300);
    }

    // 7. BlockingCollection
    static async Task RunBlockingCollectionDemoAsync()
    {
        var warehouse = new BlockingCollection<string>();
        var random = new Random();
        var tasks = new List<Task>();

        for (int i = 0; i < 5; i++)
        {
            int supplierId = i + 1;
            tasks.Add(Task.Run(() => Supplier(supplierId, warehouse, random)));
        }

        for (int i = 0; i < 10; i++)
        {
            int customerId = i + 1;
            tasks.Add(Task.Run(() => Customer(customerId, warehouse)));
        }

        await Task.WhenAll(tasks.Take(5));
        warehouse.CompleteAdding();

        await Task.WhenAll(tasks.Skip(5));

        Console.WriteLine("Все операции со складом завершены");
    }

    static void Supplier(int id, BlockingCollection<string> warehouse, Random random)
    {
        string[] products = { "Холодильник", "Телевизор", "Стиральная машина", "Пылесос", "Микроволновка" };

        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(random.Next(500, 2000));
            string product = $"{products[random.Next(products.Length)]} от поставщика {id}";
            warehouse.Add(product);
            Console.WriteLine($"Поставщик {id} доставил: {product}");
            PrintWarehouseState(warehouse);
        }
    }

    static void Customer(int id, BlockingCollection<string> warehouse)
    {
        while (!warehouse.IsCompleted)
        {
            try
            {
                if (warehouse.TryTake(out string product, 1000))
                {
                    Console.WriteLine($"Покупатель {id} купил: {product}");
                    PrintWarehouseState(warehouse);
                }
                else
                {
                    Console.WriteLine($"Покупатель {id} ушел без покупки");
                }
            }
            catch (InvalidOperationException)
            {
                break;
            }
        }
    }

    static void PrintWarehouseState(BlockingCollection<string> warehouse)
    {
        Console.WriteLine($"Товаров на складе: {warehouse.Count}");
    }

    // 8. Async/Await
    static async Task RunAsyncDemo()
    {
        Console.WriteLine("Начало асинхронной операции...");

        try
        {
            var result1 = await CalculateAsync(10);
            Console.WriteLine($"Результат 1: {result1}");

            var result2 = await CalculateAsync(20);
            Console.WriteLine($"Результат 2: {result2}");

            // Параллельное выполнение
            Console.WriteLine("Параллельное выполнение:");
            var task1 = CalculateAsync(15);
            var task2 = CalculateAsync(25);

            await Task.WhenAll(task1, task2);
            Console.WriteLine($"Оба результата: {task1.Result}, {task2.Result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в асинхронных операциях: {ex.Message}");
        }
    }

    static async Task<int> CalculateAsync(int value)
    {
        Console.WriteLine($"Начало расчета для {value} в потоке {Thread.CurrentThread.ManagedThreadId}");
        await Task.Delay(1000);
        return value * value + 42;
    }
}