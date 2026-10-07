using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

public class AVFLog
{
    private readonly string logFile = "xxxlogfile.txt";

    public void WriteLog(string action, string details)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {details}";
                sw.WriteLine(logEntry);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
        }
    }

    public string ReadLog()
    {
        try
        {
            if (!File.Exists(logFile)) return "Файл лога не существует";
            using (StreamReader sr = new StreamReader(logFile, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            return $"Ошибка чтения лога: {ex.Message}";
        }
    }

    public string SearchLog(DateTime date)
    {
        return SearchLog(date, date.AddDays(1));
    }

    public string SearchLog(DateTime start, DateTime end)
    {
        try
        {
            if (!File.Exists(logFile)) return "Файл лога не существует";
            var lines = File.ReadLines(logFile, Encoding.UTF8)
                .Where(line =>
                {
                    var parts = line.Split('|');
                    if (parts.Length < 1) return false;
                    return DateTime.TryParse(parts[0].Trim(), out DateTime logTime)
                        && logTime >= start
                        && logTime < end;
                });
            return string.Join(Environment.NewLine, lines);
        }
        catch (Exception ex)
        {
            return $"Ошибка поиска по диапазону: {ex.Message}";
        }
    }

    public string SearchLog(string keyword)
    {
        try
        {
            if (!File.Exists(logFile)) return "Файл лога не существует";
            var lines = File.ReadLines(logFile, Encoding.UTF8)
                .Where(line => line.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            return string.Join(Environment.NewLine, lines);
        }
        catch (Exception ex)
        {
            return $"Ошибка поиска по ключевому слову: {ex.Message}";
        }
    }

    public int GetEntriesCount()
    {
        try
        {
            if (!File.Exists(logFile)) return 0;
            return File.ReadAllLines(logFile).Length;
        }
        catch
        {
            return 0;
        }
    }

    public void KeepCurrentHourEntries()
    {
        try
        {
            if (!File.Exists(logFile)) return;
            var currentHour = DateTime.Now.Date.AddHours(DateTime.Now.Hour);
            var nextHour = currentHour.AddHours(1);
            var filteredLines = File.ReadLines(logFile, Encoding.UTF8)
                .Where(line =>
                {
                    var parts = line.Split('|');
                    if (parts.Length < 1) return false;
                    return DateTime.TryParse(parts[0].Trim(), out DateTime logTime)
                        && logTime >= currentHour
                        && logTime < nextHour;
                }).ToArray();
            File.WriteAllLines(logFile, filteredLines, Encoding.UTF8);
            WriteLog("LOG_CLEANUP", $"Оставлены записи за {currentHour:HH:00}-{nextHour:HH:00}");
        }
        catch (Exception ex)
        {
            WriteLog("ERROR", $"KeepCurrentHourEntries: {ex.Message}");
        }
    }
}

public class AVFDiskInfo
{
    private readonly AVFLog logger = new AVFLog();

    public void PrintDiskInfo()
    {
        try
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    Console.WriteLine($"Имя: {drive.Name}");
                    Console.WriteLine($"Тип: {drive.DriveType}");
                    Console.WriteLine($"Файловая система: {drive.DriveFormat}");
                    Console.WriteLine($"Объем: {drive.TotalSize / (1024.0 * 1024 * 1024):F2} ГБ");
                    Console.WriteLine($"Свободно: {drive.TotalFreeSpace / (1024.0 * 1024 * 1024):F2} ГБ");
                    Console.WriteLine($"Доступно: {drive.AvailableFreeSpace / (1024.0 * 1024 * 1024):F2} ГБ");
                    Console.WriteLine($"Метка тома: {drive.VolumeLabel}");
                    Console.WriteLine("-----------------------------------");

                    logger.WriteLog("DISK_INFO",
                        $"Drive: {drive.Name} | " +
                        $"Format: {drive.DriveFormat} | " +
                        $"Total: {drive.TotalSize} | " +
                        $"Free: {drive.TotalFreeSpace}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.WriteLog("ERROR", $"DiskInfo: {ex.Message}");
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}

public class AVFFileInfo
{
    private readonly AVFLog logger = new AVFLog();

    public void PrintFileInfo(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не существует");
                logger.WriteLog("ERROR", $"FileInfo: файл {filePath} не найден");
                return;
            }

            FileInfo file = new FileInfo(filePath);
            Console.WriteLine($"Полный путь: {file.FullName}");
            Console.WriteLine($"Имя: {file.Name}");
            Console.WriteLine($"Размер: {file.Length} байт");
            Console.WriteLine($"Расширение: {file.Extension}");
            Console.WriteLine($"Дата создания: {file.CreationTime}");
            Console.WriteLine($"Дата изменения: {file.LastWriteTime}");

            logger.WriteLog("FILE_INFO",
                $"Path: {file.FullName} | " +
                $"Size: {file.Length} | " +
                $"Created: {file.CreationTime}");
        }
        catch (Exception ex)
        {
            logger.WriteLog("ERROR", $"FileInfo: {ex.Message}");
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}

public class AVFDirInfo
{
    private readonly AVFLog logger = new AVFLog();

    public void PrintDirInfo(string dirPath)
    {
        try
        {
            if (!Directory.Exists(dirPath))
            {
                Console.WriteLine("Директория не существует");
                logger.WriteLog("ERROR", $"DirInfo: директория {dirPath} не найдена");
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(dirPath);
            Console.WriteLine($"Количество файлов: {dir.GetFiles().Length}");
            Console.WriteLine($"Время создания: {dir.CreationTime}");
            Console.WriteLine($"Количество поддиректорий: {dir.GetDirectories().Length}");

            var parent = dir.Parent;
            Console.WriteLine($"Родительские директории: {GetParentDirectories(dir)}");

            logger.WriteLog("DIR_INFO",
                $"Path: {dir.FullName} | " +
                $"Files: {dir.GetFiles().Length} | " +
                $"Subdirs: {dir.GetDirectories().Length}");
        }
        catch (Exception ex)
        {
            logger.WriteLog("ERROR", $"DirInfo: {ex.Message}");
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private string GetParentDirectories(DirectoryInfo dir)
    {
        var parents = new System.Collections.Generic.List<string>();
        var current = dir.Parent;
        while (current != null)
        {
            parents.Add(current.FullName);
            current = current.Parent;
        }
        return string.Join(" -> ", parents);
    }
}

public class AVFFileManager
{
    private readonly AVFLog logger = new AVFLog();

    public void PerformActions(string drivePath, string sourceDir, string extension)
    {
        try
        {
            // Часть A
            Console.WriteLine("\n=== Часть A ===");
            if (!Directory.Exists("AVFInspect"))
            {
                Directory.CreateDirectory("AVFInspect");
                logger.WriteLog("DIR_CREATED", "AVFInspect");
            }

            // Получаем список файлов и папок заданного диска
            var driveInfo = new DriveInfo(drivePath);
            if (!driveInfo.IsReady)
            {
                throw new Exception($"Диск {drivePath} не готов");
            }

            string dirInfoPath = Path.Combine("AVFInspect", "xxxdirinfo.txt");
            using (StreamWriter sw = new StreamWriter(dirInfoPath, false, Encoding.UTF8))
            {
                sw.WriteLine($"Информация о диске {drivePath}:");
                sw.WriteLine($"Свободное место: {driveInfo.TotalFreeSpace / (1024.0 * 1024 * 1024):F2} ГБ");
                sw.WriteLine($"Файловая система: {driveInfo.DriveFormat}");
                sw.WriteLine("\nСодержимое корня диска:");

                foreach (var dir in Directory.GetDirectories(drivePath))
                    sw.WriteLine($"DIR: {dir}");

                foreach (var file in Directory.GetFiles(drivePath))
                    sw.WriteLine($"FILE: {file}");
            }
            logger.WriteLog("FILE_CREATED", dirInfoPath);

            string copyPath = Path.Combine("AVFInspect", "xxxdirinfo_copy.txt");
            File.Copy(dirInfoPath, copyPath, true);
            logger.WriteLog("FILE_COPIED", $"{dirInfoPath} -> {copyPath}");

            File.Delete(dirInfoPath);
            logger.WriteLog("FILE_DELETED", dirInfoPath);

            // Часть B
            Console.WriteLine("\n=== Часть B ===");
            if (!Directory.Exists("AVFFiles"))
            {
                Directory.CreateDirectory("AVFFiles");
                logger.WriteLog("DIR_CREATED", "AVFFiles");
            }

            if (!Directory.Exists(sourceDir))
            {
                throw new Exception($"Исходная директория {sourceDir} не существует");
            }

            foreach (string file in Directory.GetFiles(sourceDir, $"*.{extension}"))
            {
                string destFile = Path.Combine("AVFFiles", Path.GetFileName(file));
                File.Copy(file, destFile, true);
                logger.WriteLog("FILE_COPIED", $"{file} -> {destFile}");
            }

            string targetDir = Path.Combine("AVFInspect", "AVFFiles");
            if (Directory.Exists(targetDir))
            {
                Directory.Delete(targetDir, true);
            }
            Directory.Move("AVFFiles", targetDir);
            logger.WriteLog("DIR_MOVED", "AVFFiles -> AVFInspect/AVFFiles");

            // Часть C
            Console.WriteLine("\n=== Часть C ===");
            string zipPath = "AVFFiles.zip";
            if (File.Exists(zipPath))
                File.Delete(zipPath);

            ZipFile.CreateFromDirectory(targetDir, zipPath);
            logger.WriteLog("ZIP_CREATED", $"{targetDir} -> {zipPath}");

            string extractPath = "AVFUnzipped";
            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            ZipFile.ExtractToDirectory(zipPath, extractPath);
            logger.WriteLog("ZIP_EXTRACTED", $"{zipPath} -> {extractPath}");

            Console.WriteLine("Операции выполнены успешно");
        }
        catch (Exception ex)
        {
            logger.WriteLog("ERROR", $"FileManager: {ex.Message}");
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}

class Program
{
    static void Main()
    {
        AVFLog log = new AVFLog();

        Console.WriteLine("=== Демонстрация AVFDiskInfo ===");
        AVFDiskInfo diskInfo = new AVFDiskInfo();
        diskInfo.PrintDiskInfo();

        Console.WriteLine("\n=== Демонстрация AVFFileInfo ===");
        AVFFileInfo fileInfo = new AVFFileInfo();
        fileInfo.PrintFileInfo("test.txt");

        Console.WriteLine("\n=== Демонстрация AVFDirInfo ===");
        AVFDirInfo dirInfo = new AVFDirInfo();
        dirInfo.PrintDirInfo(".");

        Console.WriteLine("\n=== Демонстрация AVFFileManager ===");
        AVFFileManager manager = new AVFFileManager();
        manager.PerformActions("C:\\", "C:\\Test", "txt");

        Console.WriteLine("\n=== Работа с логом ===");
        Console.WriteLine("Поиск по ключевому слову 'FILE_CREATED':");
        Console.WriteLine(log.SearchLog("FILE_CREATED"));

        Console.WriteLine($"\nВсего записей в логе: {log.GetEntriesCount()}");

        Console.WriteLine("\nЗаписи за сегодня:");
        Console.WriteLine(log.SearchLog(DateTime.Today));

        log.KeepCurrentHourEntries();
        Console.WriteLine("\nПосле очистки остались записи за текущий час");
        Console.WriteLine($"Записей осталось: {log.GetEntriesCount()}");

        Console.ReadKey();
    }
}