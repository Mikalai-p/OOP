using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab14
{
    // ====== 1. Работа с процессами ======
    public class ProcessManager
    {
        private readonly string _outputFilePath;

        public ProcessManager(string outputFilePath)
        {
            _outputFilePath = outputFilePath;
        }

        public void GetProcessesInfo()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_outputFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("=== ИНФОРМАЦИЯ О ЗАПУЩЕННЫХ ПРОЦЕССАХ ===");
                    writer.WriteLine($"Дата: {DateTime.Now}");
                    writer.WriteLine(new string('=', 100));

                    int count = 0;
                    foreach (var process in Process.GetProcesses().OrderBy(p => p.ProcessName))
                    {
                        try
                        {
                            writer.WriteLine($"ID процесса: {process.Id}");
                            writer.WriteLine($"Имя: {process.ProcessName}");
                            writer.WriteLine($"Приоритет: {process.BasePriority}");

                            try
                            {
                                writer.WriteLine($"Время запуска: {process.StartTime:yyyy-MM-dd HH:mm:ss}");
                            }
                            catch
                            {
                                writer.WriteLine($"Время запуска: Недоступно");
                            }

                            writer.WriteLine($"Состояние: {(process.Responding ? "Работает" : "Не отвечает")}");
                            writer.WriteLine($"Время CPU: {process.TotalProcessorTime}");
                            writer.WriteLine($"Память: {process.WorkingSet64 / 1024 / 1024} МБ");

                            writer.WriteLine(new string('-', 80));
                            count++;
                        }
                        catch (Exception ex)
                        {
                            writer.WriteLine($"Ошибка: {process.ProcessName} - {ex.Message}");
                        }
                    }

                    writer.WriteLine($"\nВсего процессов: {count}");
                }

                Console.WriteLine($"Информация о процессах записана в файл: {Path.GetFullPath(_outputFilePath)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении информации о процессах: {ex.Message}");
            }
        }
    }

    // ====== 2. Исследование текущего домена приложения (упрощенная версия для .NET Core) ======
    public class DomainExplorer
    {
        public void ExploreCurrentDomain()
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;

                Console.WriteLine("\n=== ИССЛЕДОВАНИЕ ТЕКУЩЕГО ДОМЕНА ===");
                Console.WriteLine($"Имя домена: {currentDomain.FriendlyName}");
                Console.WriteLine($"Базовый каталог: {currentDomain.BaseDirectory}");
                Console.WriteLine($"ID домена: {currentDomain.Id}");

                Console.WriteLine("\nЗагруженные сборки:");
                foreach (Assembly assembly in currentDomain.GetAssemblies().OrderBy(a => a.GetName().Name).Take(10))
                {
                    var name = assembly.GetName();
                    Console.WriteLine($"  • {name.Name} (v{name.Version})");
                }

                Console.WriteLine($"\nВсего загружено сборок: {currentDomain.GetAssemblies().Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при исследовании домена: {ex.Message}");
            }
        }
    }

    // ====== 3. Поток для расчета простых чисел ======
    public class PrimeNumbersTask : IDisposable
    {
        private Thread _workerThread;
        private CancellationTokenSource _cancellationTokenSource;
        private ManualResetEvent _pauseEvent;
        private readonly object _lockObject = new object();
        private readonly int _maxNumber;
        private readonly string _outputFilePath;
        private volatile bool _isRunning;

        public PrimeNumbersTask(int maxNumber, string outputFilePath)
        {
            _maxNumber = maxNumber;
            _outputFilePath = outputFilePath;
            _cancellationTokenSource = new CancellationTokenSource();
            _pauseEvent = new ManualResetEvent(true);
            _isRunning = false;

            _workerThread = new Thread(GeneratePrimes)
            {
                Name = "PrimeNumbersThread",
                Priority = ThreadPriority.Normal,
                IsBackground = true
            };
        }

        public void Start()
        {
            if (_isRunning) return;

            Console.WriteLine("Запуск потока для поиска простых чисел...");
            _isRunning = true;
            _workerThread.Start(_cancellationTokenSource.Token);
        }

        public void Pause()
        {
            Console.WriteLine("Приостановка потока...");
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            Console.WriteLine("Возобновление потока...");
            _pauseEvent.Set();
        }

        public void Stop()
        {
            Console.WriteLine("Остановка потока...");
            _isRunning = false;
            _cancellationTokenSource.Cancel();
            _pauseEvent.Set();

            if (_workerThread.IsAlive)
            {
                _workerThread.Join(2000);
            }
        }

        public void PrintThreadInfo()
        {
            if (_workerThread != null)
            {
                Console.WriteLine("\n=== ИНФОРМАЦИЯ О ПОТОКЕ ===");
                Console.WriteLine($"Имя: {_workerThread.Name}");
                Console.WriteLine($"ID: {_workerThread.ManagedThreadId}");
                Console.WriteLine($"Состояние: {_workerThread.ThreadState}");
                Console.WriteLine($"Приоритет: {_workerThread.Priority}");
                Console.WriteLine($"Фоновый: {_workerThread.IsBackground}");
                Console.WriteLine($"Живой: {_workerThread.IsAlive}");
            }
        }

        private void GeneratePrimes(object obj)
        {
            CancellationToken token = (CancellationToken)obj;

            try
            {
                using (StreamWriter writer = new StreamWriter(_outputFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine($"Простые числа от 1 до {_maxNumber}:");
                    Console.WriteLine($"Простые числа от 1 до {_maxNumber}:");

                    int primeCount = 0;
                    for (int i = 2; i <= _maxNumber; i++)
                    {
                        if (!_isRunning || token.IsCancellationRequested)
                        {
                            Console.WriteLine("Поток прерван по запросу пользователя");
                            return;
                        }

                        _pauseEvent.WaitOne();

                        if (IsPrime(i))
                        {
                            lock (_lockObject)
                            {
                                writer.WriteLine(i);
                                primeCount++;
                            }
                            Console.WriteLine($"Найдено простое число: {i}");

                            Thread.Sleep(50);
                        }
                    }

                    Console.WriteLine($"\nНайдено {primeCount} простых чисел");
                    writer.WriteLine($"\nВсего найдено: {primeCount} простых чисел");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в потоке: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }

        private bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            int limit = (int)Math.Sqrt(number);
            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;
        }

        public void Dispose()
        {
            Stop();
            _pauseEvent?.Dispose();
            _cancellationTokenSource?.Dispose();
        }
    }

    // ====== 4. Четные и нечетные числа с синхронизацией ======
    public class EvenOddNumbersTask
    {
        private readonly int _maxNumber;
        private readonly string _outputFilePath;
        private readonly object _syncLock = new object();
        private bool _evenTurn = true;
        private int _evenCount = 0;
        private int _oddCount = 0;

        public EvenOddNumbersTask(int maxNumber, string outputFilePath)
        {
            _maxNumber = maxNumber;
            _outputFilePath = outputFilePath;
        }

        public void StartSequential()
        {
            Console.WriteLine("\n=== РЕЖИМ: СНАЧАЛА ЧЕТНЫЕ, ПОТОМ НЕЧЕТНЫЕ ===");

            using (StreamWriter writer = new StreamWriter(_outputFilePath, false, Encoding.UTF8))
            {
                writer.WriteLine("Четные числа:");
                Console.WriteLine("Четные числа:");
                for (int i = 2; i <= _maxNumber; i += 2)
                {
                    writer.WriteLine(i);
                    Console.WriteLine($"  {i}");
                    Thread.Sleep(100);
                }

                writer.WriteLine("\nНечетные числа:");
                Console.WriteLine("\nНечетные числа:");
                for (int i = 1; i <= _maxNumber; i += 2)
                {
                    writer.WriteLine(i);
                    Console.WriteLine($"  {i}");
                    Thread.Sleep(150);
                }
            }
        }

        public void StartAlternating()
        {
            Console.WriteLine("\n=== РЕЖИМ: ПООЧЕРЕДНЫЙ ВЫВОД ===");

            Thread evenThread = new Thread(PrintEvenNumbers)
            {
                Name = "EvenThread",
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };

            Thread oddThread = new Thread(PrintOddNumbers)
            {
                Name = "OddThread",
                Priority = ThreadPriority.Normal,
                IsBackground = true
            };

            File.WriteAllText(_outputFilePath, "Поочередный вывод чисел:\n");

            evenThread.Start();
            oddThread.Start();

            evenThread.Join();
            oddThread.Join();

            Console.WriteLine($"\nЗавершено. Четных чисел: {_evenCount}, нечетных: {_oddCount}");
        }

        private void PrintEvenNumbers()
        {
            for (int i = 2; i <= _maxNumber; i += 2)
            {
                lock (_syncLock)
                {
                    while (!_evenTurn)
                    {
                        Monitor.Wait(_syncLock);
                    }

                    WriteNumber(i, "Четное");
                    _evenCount++;
                    _evenTurn = false;
                    Monitor.Pulse(_syncLock);
                }

                Thread.Sleep(100);
            }
        }

        private void PrintOddNumbers()
        {
            for (int i = 1; i <= _maxNumber; i += 2)
            {
                lock (_syncLock)
                {
                    while (_evenTurn)
                    {
                        Monitor.Wait(_syncLock);
                    }

                    WriteNumber(i, "Нечетное");
                    _oddCount++;
                    _evenTurn = true;
                    Monitor.Pulse(_syncLock);
                }

                Thread.Sleep(150);
            }
        }

        private void WriteNumber(int number, string type)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_outputFilePath, true, Encoding.UTF8))
                {
                    writer.WriteLine($"{type}: {number}");
                }
                Console.WriteLine($"{type}: {number}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи в файл: {ex.Message}");
            }
        }
    }

    // ====== 5. Повторяющаяся задача с Timer ======
    public class RepeatingTimerTask : IDisposable
    {
        private Timer _timer;
        private readonly string _logFilePath;
        private readonly object _fileLock = new object();
        private int _executionCount = 0;
        private readonly int _maxExecutions;
        private readonly int _intervalMs;

        public RepeatingTimerTask(string logFilePath, int intervalMs, int maxExecutions = 10)
        {
            _logFilePath = logFilePath;
            _maxExecutions = maxExecutions;
            _intervalMs = intervalMs;
            _timer = new Timer(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            Console.WriteLine($"Запуск таймера. Интервал: {_intervalMs} мс");
            _executionCount = 0;
            File.WriteAllText(_logFilePath, $"Лог таймера. Запуск: {DateTime.Now:HH:mm:ss}\n");
            _timer.Change(0, _intervalMs);
        }

        public void Stop()
        {
            Console.WriteLine("Остановка таймера...");
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerCallback(object state)
        {
            try
            {
                _executionCount++;
                string message = $"[{DateTime.Now:HH:mm:ss.fff}] Выполнение #{_executionCount}";

                lock (_fileLock)
                {
                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }

                Console.WriteLine(message);

                if (_executionCount >= _maxExecutions)
                {
                    Stop();
                    Console.WriteLine($"Таймер остановлен после {_maxExecutions} выполнений");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в таймере: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Stop();
            _timer?.Dispose();
        }
    }

    // ====== ДОПОЛНИТЕЛЬНО: Пул видеоканалов ======
    public class VideoChannelPool : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly List<bool> _channels;
        private readonly object _lock = new object();
        private readonly int _channelCount;
        private readonly TimeSpan _waitTimeout;

        public VideoChannelPool(int channelCount, TimeSpan waitTimeout)
        {
            _channelCount = channelCount;
            _waitTimeout = waitTimeout;
            _semaphore = new SemaphoreSlim(channelCount, channelCount);
            _channels = Enumerable.Repeat(false, channelCount).ToList();
        }

        public async Task<bool> TryUseChannelAsync(string clientName)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Клиент '{clientName}' пытается получить канал...");

            bool acquired = await _semaphore.WaitAsync(_waitTimeout);

            if (acquired)
            {
                int channelId = -1;
                lock (_lock)
                {
                    for (int i = 0; i < _channels.Count; i++)
                    {
                        if (!_channels[i])
                        {
                            _channels[i] = true;
                            channelId = i;
                            break;
                        }
                    }
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Клиент '{clientName}' получил канал #{channelId + 1}");
                return true;
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Клиент '{clientName}' не дождался канала и ушел");
                return false;
            }
        }

        public void ReleaseChannel(string clientName)
        {
            lock (_lock)
            {
                for (int i = 0; i < _channels.Count; i++)
                {
                    if (_channels[i])
                    {
                        _channels[i] = false;
                        _semaphore.Release();
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Клиент '{clientName}' освободил канал #{i + 1}");
                        break;
                    }
                }
            }
        }

        public void PrintStatus()
        {
            lock (_lock)
            {
                Console.WriteLine($"\nСтатус пула каналов:");
                Console.WriteLine($"Всего каналов: {_channelCount}");
                Console.WriteLine($"Свободно: {_semaphore.CurrentCount}");
                Console.WriteLine($"Занято: {_channelCount - _semaphore.CurrentCount}");
            }
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }
    }

    // ====== ОСНОВНАЯ ПРОГРАММА ======
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Лабораторная работа №14: Работа с потоками";

            Console.WriteLine("╔════════════════════════════════════════════════════╗");
            Console.WriteLine("║      ЛАБОРАТОРНАЯ РАБОТА №14: РАБОТА С ПОТОКАМИ    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                Console.WriteLine($"Рабочая директория: {basePath}\n");

                // Задание 1: Процессы
                Console.WriteLine("════════════════════════════════════════════════════");
                Console.WriteLine("ЗАДАНИЕ 1: ИНФОРМАЦИЯ О ПРОЦЕССАХ");
                Console.WriteLine("════════════════════════════════════════════════════");

                ProcessManager procManager = new ProcessManager(Path.Combine(basePath, "processes.txt"));
                procManager.GetProcessesInfo();

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();

                // Задание 2: Домены приложений (упрощенная версия)
                Console.WriteLine("\n════════════════════════════════════════════════════");
                Console.WriteLine("ЗАДАНИЕ 2: ИССЛЕДОВАНИЕ ТЕКУЩЕГО ДОМЕНА");
                Console.WriteLine("════════════════════════════════════════════════════");

                DomainExplorer domainExplorer = new DomainExplorer();
                domainExplorer.ExploreCurrentDomain();

                Console.WriteLine("\nПримечание: В .NET Core/5+ создание новых доменов ограничено.");
                Console.WriteLine("Вместо этого используется AssemblyLoadContext для изоляции сборок.");

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();

                // Задание 3: Простые числа
                Console.WriteLine("\n════════════════════════════════════════════════════");
                Console.WriteLine("ЗАДАНИЕ 3: ПОИСК ПРОСТЫХ ЧИСЕЛ В ПОТОКЕ");
                Console.WriteLine("════════════════════════════════════════════════════");

                Console.Write("Введите верхнюю границу для поиска простых чисел (n): ");
                if (int.TryParse(Console.ReadLine(), out int n) && n > 0)
                {
                    using (var primeTask = new PrimeNumbersTask(n, Path.Combine(basePath, "primes.txt")))
                    {
                        primeTask.Start();
                        Thread.Sleep(1000);

                        primeTask.PrintThreadInfo();

                        primeTask.Pause();
                        Console.WriteLine("Поток на паузе 2 секунды...");
                        Thread.Sleep(2000);

                        primeTask.Resume();
                        Thread.Sleep(1000);

                        primeTask.Stop();
                        primeTask.PrintThreadInfo();
                    }
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Используем значение по умолчанию: 50");
                    using (var primeTask = new PrimeNumbersTask(50, Path.Combine(basePath, "primes.txt")))
                    {
                        primeTask.Start();
                        Thread.Sleep(1000);
                        primeTask.Stop();
                    }
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();

                // Задание 4: Четные и нечетные числа
                Console.WriteLine("\n════════════════════════════════════════════════════");
                Console.WriteLine("ЗАДАНИЕ 4: ЧЕТНЫЕ И НЕЧЕТНЫЕ ЧИСЛА");
                Console.WriteLine("════════════════════════════════════════════════════");

                Console.Write("Введите верхнюю границу для вывода чисел: ");
                if (int.TryParse(Console.ReadLine(), out int maxNum) && maxNum > 0)
                {
                    var evenOddTask = new EvenOddNumbersTask(maxNum, Path.Combine(basePath, "numbers.txt"));

                    Console.WriteLine("\n--- Режим 1: Сначала четные, потом нечетные ---");
                    evenOddTask.StartSequential();

                    Console.WriteLine("\nНажмите Enter для перехода к поочередному выводу...");
                    Console.ReadLine();

                    Console.WriteLine("\n--- Режим 2: Поочередный вывод ---");
                    evenOddTask.StartAlternating();
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Используем значение по умолчанию: 20");
                    var evenOddTask = new EvenOddNumbersTask(20, Path.Combine(basePath, "numbers.txt"));
                    evenOddTask.StartAlternating();
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();

                // Задание 5: Таймер
                Console.WriteLine("\n════════════════════════════════════════════════════");
                Console.WriteLine("ЗАДАНИЕ 5: ПОВТОРЯЮЩАЯСЯ ЗАДАЧА С TIMER");
                Console.WriteLine("════════════════════════════════════════════════════");

                using (var timerTask = new RepeatingTimerTask(
                    Path.Combine(basePath, "timer_log.txt"),
                    1000,
                    5))
                {
                    timerTask.Start();

                    Console.WriteLine("Таймер запущен на 5 секунд (5 выполнений)...");
                    Thread.Sleep(6000);

                    timerTask.Stop();
                }

                // Дополнительное задание: Пул видеоканалов
                Console.WriteLine("\n════════════════════════════════════════════════════");
                Console.WriteLine("ДОПОЛНИТЕЛЬНО: ПУЛ ВИДЕОКАНАЛОВ (Семафоры)");
                Console.WriteLine("════════════════════════════════════════════════════");

                using (var channelPool = new VideoChannelPool(3, TimeSpan.FromSeconds(2)))
                {
                    channelPool.PrintStatus();

                    var clients = new List<string> { "Клиент1", "Клиент2", "Клиент3", "Клиент4", "Клиент5" };
                    var tasks = new List<Task>();
                    var random = new Random();

                    Console.WriteLine("\nКлиенты пытаются получить доступ к каналам:");

                    foreach (var client in clients)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            if (await channelPool.TryUseChannelAsync(client))
                            {
                                // Имитация работы с каналом
                                int workTime = random.Next(1000, 3000);
                                await Task.Delay(workTime);
                                channelPool.ReleaseChannel(client);
                            }
                        }));
                    }

                    await Task.WhenAll(tasks);
                    Console.WriteLine("\nВсе клиенты обработаны.");
                    channelPool.PrintStatus();
                }

                Console.WriteLine("\n════════════════════════════════════════════════════");
                Console.WriteLine("ПРОГРАММА УСПЕШНО ЗАВЕРШЕНА!");
                Console.WriteLine("════════════════════════════════════════════════════");

                Console.WriteLine("\nСозданные файлы:");
                Console.WriteLine($"  • processes.txt - информация о процессах");
                Console.WriteLine($"  • primes.txt - простые числа");
                Console.WriteLine($"  • numbers.txt - четные/нечетные числа");
                Console.WriteLine($"  • timer_log.txt - лог таймера");
                Console.WriteLine($"\nВсе файлы сохранены в директории: {basePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n╔════════════════════════════════════════════════════╗");
                Console.WriteLine($"║               КРИТИЧЕСКАЯ ОШИБКА                    ║");
                Console.WriteLine($"╚════════════════════════════════════════════════════╝");
                Console.WriteLine($"\nСообщение: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}