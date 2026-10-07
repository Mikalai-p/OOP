using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public enum DocumentStatus
{
    Draft, Submitted, Approved, Rejected, Archived
}

public enum LogLevel
{
    INFO, WARNING, ERROR
}

public class DocumentException : Exception
{
    public DocumentException(string message) : base(message) { }
    public DocumentException(string message, Exception inner) : base(message, inner) { }
}

public class InvalidDocumentDataException : DocumentException
{
    public InvalidDocumentDataException(string message) : base(message) { }
    public InvalidDocumentDataException(string message, Exception inner) : base(message, inner) { }
}

public class DocumentNotFoundException : DocumentException
{
    public DocumentNotFoundException(string message) : base(message) { }
    public DocumentNotFoundException(string message, Exception inner) : base(message, inner) { }
}

public class DocumentStorageException : IOException
{
    public DocumentStorageException(string message) : base(message) { }
    public DocumentStorageException(string message, Exception inner) : base(message, inner) { }
}

public class DocumentValidationException : Exception
{
    public DocumentValidationException(string message) : base(message) { }
    public DocumentValidationException(string message, Exception inner) : base(message, inner) { }
}

public interface ILogger
{
    void Log(LogLevel level, string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(LogLevel level, string message)
    {
        Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm}, {level}: {message}");
    }
}

public struct Address
{
    public string Street;
    public string City;
    public string PostalCode;

    public Address(string street, string city, string postalCode)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {PostalCode}";
    }
}

public interface IDocument
{
    string GetDocumentInfo();
    void Print();
}

public abstract class Document : IDocument
{
    public string Number { get; set; }
    public Date Date { get; set; }
    public Organization Organization { get; set; }
    public DocumentStatus Status { get; set; }

    protected Document(string number, Date date, Organization organization)
    {
        ValidateDocumentData(number, date);
        Number = number;
        Date = date;
        Organization = organization;
        Status = DocumentStatus.Draft;
    }

    private void ValidateDocumentData(string number, Date date)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new InvalidDocumentDataException("Номер документа не может быть пустым");

        if (date.Day < 1 || date.Day > 31)
            throw new InvalidDocumentDataException("Неверный день даты");

        if (date.Month < 1 || date.Month > 12)
            throw new InvalidDocumentDataException("Неверный месяц даты");
    }

    public abstract string GetDocumentInfo();

    public virtual void Print()
    {
        Console.WriteLine(ToString());
    }

    void IDocument.Print()
    {
        Console.WriteLine($"Документ: {GetType().Name} №{Number} от {Date}");
    }

    public override bool Equals(object obj)
    {
        if (obj is Document document)
            return Number == document.Number && Date.Equals(document.Date);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Number, Date);
    }

    public override string ToString()
    {
        return $"Документ №{Number} от {Date}, Статус: {Status}";
    }
}

public partial class Receipt : Document
{
    public decimal Amount { get; set; }
    public string Payer { get; set; }

    public Receipt(string number, Date date, Organization organization,
        decimal amount, string payer) : base(number, date, organization)
    {
        if (amount <= 0)
            throw new InvalidDocumentDataException("Сумма должна быть положительной");
        if (string.IsNullOrWhiteSpace(payer))
            throw new InvalidDocumentDataException("Плательщик не может быть пустым");

        Amount = amount;
        Payer = payer;
    }

    public override string GetDocumentInfo()
    {
        return $"Квитанция №{Number} на сумму {Amount} от {Date}";
    }
}

public partial class Receipt
{
    public override void Print()
    {
        Console.WriteLine("=== ПОЛНАЯ ИНФОРМАЦИЯ О КВИТАНЦИИ ===");
        Console.WriteLine($"Квитанция №{Number}");
        Console.WriteLine($"Дата: {Date}");
        Console.WriteLine($"Сумма: {Amount} руб.");
        Console.WriteLine($"Плательщик: {Payer}");
        Console.WriteLine($"Организация: {Organization.Name}");
        Console.WriteLine($"Адрес: {Organization.Address}");
        Console.WriteLine($"Телефон: {Organization.Phone}");
        Console.WriteLine($"Статус: {Status}");
        Console.WriteLine("=====================================");
    }

    public override string ToString()
    {
        return $"{GetDocumentInfo()}\nПлательщик: {Payer}\nОрганизация: {Organization}";
    }
}

public class Invoice : Document
{
    public List<ProductItem> Goods { get; set; }
    public string Shipper { get; set; }

    public Invoice(string number, Date date, Organization organization,
        List<ProductItem> goods, string shipper) : base(number, date, organization)
    {
        if (goods == null || goods.Count == 0)
            throw new InvalidDocumentDataException("Накладная должна содержать товары");
        if (string.IsNullOrWhiteSpace(shipper))
            throw new InvalidDocumentDataException("Грузоотправитель не может быть пустым");

        Goods = goods;
        Shipper = shipper;
    }

    public override string GetDocumentInfo()
    {
        return $"Накладная №{Number} от {Date} ({Goods.Count} позиций)";
    }

    public override string ToString()
    {
        return $"{GetDocumentInfo()}\nГрузоотправитель: {Shipper}\nТовары:\n{string.Join("\n", Goods)}";
    }
}

public sealed class Check : Document
{
    public decimal Total { get; set; }
    public string Cashier { get; set; }

    public Check(string number, Date date, Organization organization,
        decimal total, string cashier) : base(number, date, organization)
    {
        if (total <= 0)
            throw new InvalidDocumentDataException("Сумма чека должна быть положительной");
        if (string.IsNullOrWhiteSpace(cashier))
            throw new InvalidDocumentDataException("Кассир не может быть пустым");

        Total = total;
        Cashier = cashier;
    }

    public override string GetDocumentInfo()
    {
        return $"Чек №{Number} на сумму {Total} от {Date}";
    }

    public override string ToString()
    {
        return $"{GetDocumentInfo()}\nКассир: {Cashier}\nОрганизация: {Organization}";
    }
}

public class ProductItem
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public ProductItem(string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название товара не может быть пустым");
        if (price < 0)
            throw new ArgumentException("Цена не может быть отрицательной");
        if (quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");

        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public decimal TotalPrice => Price * Quantity;

    public override string ToString()
    {
        return $"{Name} - {Price} руб. x {Quantity} = {TotalPrice} руб.";
    }
}

public class Date
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }

    public Date(int day, int month, int year)
    {
        Day = day;
        Month = month;
        Year = year;
    }

    public override string ToString()
    {
        return $"{Day:00}.{Month:00}.{Year}";
    }

    public override bool Equals(object obj)
    {
        if (obj is Date date)
            return Day == date.Day && Month == date.Month && Year == date.Year;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Day, Month, Year);
    }

    public bool IsInRange(Date start, Date end)
    {
        int current = Year * 10000 + Month * 100 + Day;
        int startValue = start.Year * 10000 + start.Month * 100 + start.Day;
        int endValue = end.Year * 10000 + end.Month * 100 + end.Day;
        return current >= startValue && current <= endValue;
    }
}

public class Organization
{
    public string Name { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }

    public Organization(string name, Address address, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название организации не может быть пустым");
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Телефон не может быть пустым");

        Name = name;
        Address = address;
        Phone = phone;
    }

    public override string ToString()
    {
        return $"{Name} ({Address}, тел.: {Phone})";
    }
}

public class DocumentContainer
{
    private List<Document> documents = new List<Document>();
    private readonly ILogger _logger;

    public DocumentContainer(ILogger logger = null)
    {
        _logger = logger;
    }

    public void Add(Document document)
    {
        if (document == null)
        {
            _logger?.Log(LogLevel.ERROR, "Попытка добавить null-документ");
            throw new ArgumentNullException(nameof(document), "Документ не может быть null");
        }

        documents.Add(document);
        _logger?.Log(LogLevel.INFO, $"Добавлен документ №{document.Number}");
    }

    public bool Remove(Document document)
    {
        return documents.Remove(document);
    }

    public Document Get(int index)
    {
        if (index < 0 || index >= documents.Count)
        {
            _logger?.Log(LogLevel.ERROR, $"Попытка доступа к несуществующему индексу: {index}");
            throw new DocumentNotFoundException($"Документ с индексом {index} не найден");
        }
        return documents[index];
    }

    public void Set(int index, Document document)
    {
        if (index >= 0 && index < documents.Count)
            documents[index] = document;
    }

    public void PrintAll()
    {
        Console.WriteLine("=== ВСЕ ДОКУМЕНТЫ В КОНТЕЙНЕРЕ ===");
        for (int i = 0; i < documents.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {documents[i].GetDocumentInfo()}");
        }
        Console.WriteLine("===================================");
    }

    public List<Document> GetAllDocuments()
    {
        return new List<Document>(documents);
    }
}

public class AccountingController
{
    private DocumentContainer container;
    private readonly ILogger _logger;

    public AccountingController(DocumentContainer container, ILogger logger = null)
    {
        this.container = container;
        _logger = logger;
    }

    public decimal GetTotalCostForProduct(string productName)
    {
        Debug.Assert(container != null, "Контейнер документов не должен быть null");
        Debug.Assert(!string.IsNullOrEmpty(productName), "Название продукта не должно быть пустым");

        decimal total = 0;
        int productCount = 0;
        var documents = container.GetAllDocuments();

        foreach (var doc in documents)
        {
            if (doc is Invoice invoice)
            {
                foreach (var product in invoice.Goods)
                {
                    if (product.Name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                    {
                        total += product.TotalPrice;
                        productCount++;
                    }
                }
            }
        }

        
        try
        {
            int testDivision = 10 / productCount; 
            decimal average = total / productCount;
            _logger?.Log(LogLevel.INFO, $"Средняя стоимость '{productName}': {average}");
        }
        catch (DivideByZeroException ex)
        {
            _logger?.Log(LogLevel.WARNING, $"Деление на ноль при расчете для '{productName}'");
            throw new InvalidOperationException($"Невозможно вычислить среднюю стоимость: товар '{productName}' не найден", ex);
        }

        return total;
    }

    public int GetChecksCount()
    {
        int count = 0;
        var documents = container.GetAllDocuments();

        foreach (var doc in documents)
        {
            if (doc is Check)
            {
                count++;
            }
        }
        return count;
    }

    public void PrintDocumentsInPeriod(Date startDate, Date endDate)
    {
        Console.WriteLine($"=== ДОКУМЕНТЫ ЗА ПЕРИОД: {startDate} - {endDate} ===");
        int count = 0;
        var documents = container.GetAllDocuments();

        foreach (var doc in documents)
        {
            if (doc.Date.IsInRange(startDate, endDate))
            {
                doc.Print();
                Console.WriteLine();
                count++;
            }
        }

        if (count == 0)
        {
            Console.WriteLine("Документы за указанный период не найдены.");
        }
    }
}

class Program
{
    static void Main()
    {
        ILogger logger = new ConsoleLogger();

        try
        {
            logger.Log(LogLevel.INFO, "=== ЛАБОРАТОРНАЯ РАБОТА №6 - ОБРАБОТКА ИСКЛЮЧЕНИЙ ===");

            
            DemonstrateNormalWorkflow(logger);

            
            DemonstrateFiveExceptions(logger);

            
            DemonstrateMultipleHandlingAndRethrow(logger);

            
            DemonstrateAssert(logger);

            logger.Log(LogLevel.INFO, "Все демонстрации завершены успешно!");
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.ERROR, $"Критическая ошибка в Main: {ex.Message}");
            Console.WriteLine($"Тип исключения: {ex.GetType().Name}");
            Console.WriteLine($"Сообщение: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Внутреннее исключение: {ex.InnerException.Message}");
            }
            Console.WriteLine($"Стек вызовов: {ex.StackTrace}");
        }
        finally
        {
            logger.Log(LogLevel.INFO, "Программа завершена. Блок finally выполнен.");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }

    static void DemonstrateNormalWorkflow(ILogger logger)
    {
        logger.Log(LogLevel.INFO, "=== ДЕМОНСТРАЦИЯ НОРМАЛЬНОЙ РАБОТЫ ===");

        try
        {
            var container = new DocumentContainer(logger);

            
            var address = new Address("ул. Ленина, 1", "Москва", "101000");
            var org = new Organization("ООО 'Ромашка'", address, "+7-999-123-45-67");
            var date = new Date(15, 5, 2023);

            
            var receipt = new Receipt("001", date, org, 1500.50m, "Петров П.П.");
            var goods = new List<ProductItem>
            {
                new ProductItem("Ноутбук", 50000, 2),
                new ProductItem("Мышь", 1000, 5)
            };
            var invoice = new Invoice("002", date, org, goods, "ООО 'Поставщик'");
            var check = new Check("003", date, org, 2000.00m, "Сидорова А.И.");

            
            container.Add(receipt);
            container.Add(invoice);
            container.Add(check);

            
            var controller = new AccountingController(container, logger);
            container.PrintAll();

            decimal totalCost = controller.GetTotalCostForProduct("Мышь");
            Console.WriteLine($"Суммарная стоимость 'Мышь': {totalCost} руб.");

            
            Console.WriteLine("\n--- Демонстрация Print методов ---");
            receipt.Print();
            Console.WriteLine();
            (receipt as IDocument).Print();

            logger.Log(LogLevel.INFO, "Нормальная работа завершена успешно");
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.ERROR, $"Ошибка в нормальной работе: {ex.Message}");
        }
    }

    static void DemonstrateFiveExceptions(ILogger logger)
    {
        logger.Log(LogLevel.INFO, "=== ДЕМОНСТРАЦИЯ 5 ИСКЛЮЧИТЕЛЬНЫХ СИТУАЦИЙ ===");

        try
        {
            logger.Log(LogLevel.INFO, "1. Попытка добавить null-документ...");
            var container = new DocumentContainer(logger);
            container.Add(null);
        }
        catch (ArgumentNullException ex)
        {
            logger.Log(LogLevel.ERROR, $"ArgumentNullException: {ex.Message}");
        }

        try
        {
            logger.Log(LogLevel.INFO, "2. Попытка создать документ с пустым номером...");
            var org = new Organization("Test", new Address("a", "b", "c"), "123");
            var receipt = new Receipt("", new Date(1, 1, 2023), org, 100, "Payer");
        }
        catch (InvalidDocumentDataException ex)
        {
            logger.Log(LogLevel.ERROR, $"InvalidDocumentDataException: {ex.Message}");
        }

        try
        {
            logger.Log(LogLevel.INFO, "3. Попытка доступа к несуществующему документу...");
            var container = new DocumentContainer(logger);
            var doc = container.Get(10);
        }
        catch (DocumentNotFoundException ex)
        {
            logger.Log(LogLevel.ERROR, $"DocumentNotFoundException: {ex.Message}");
        }

        try
        {
            logger.Log(LogLevel.INFO, "4. Попытка чтения несуществующего файла...");
            File.ReadAllText("nonexistent_file.txt");
        }
        catch (FileNotFoundException ex)
        {
            logger.Log(LogLevel.ERROR, $"FileNotFoundException: {ex.FileName}");
        }

        try
        {
            logger.Log(LogLevel.INFO, "5. Деление на ноль при расчетах...");
            var container = new DocumentContainer(logger);
            var controller = new AccountingController(container, logger);
            controller.GetTotalCostForProduct("NonexistentProduct");
        }
        catch (InvalidOperationException ex)
        {
            logger.Log(LogLevel.ERROR, $"InvalidOperationException (деление на ноль): {ex.Message}");
        }

        logger.Log(LogLevel.INFO, "Демонстрация 5 исключений завершена");
    }

    static void DemonstrateMultipleHandlingAndRethrow(ILogger logger)
    {
        logger.Log(LogLevel.INFO, "=== ДЕМОНСТРАЦИЯ МНОГОКРАТНОЙ ОБРАБОТКИ И ПРОБРОСА ===");

        try
        {
          
            MethodThatThrowsException(logger);
        }
        catch (DocumentException ex)
        {
            logger.Log(LogLevel.ERROR, $"Исключение перехвачено после проброса: {ex.Message}");
        }

        
        try
        {
            ThrowSpecificException("validation_error");
        }
        catch (DocumentException ex) when (ex.Message.Contains("validation"))
        {
            logger.Log(LogLevel.INFO, "Обработано исключение с фильтром 'validation'");
        }
        catch (DocumentException ex) when (ex.Message.Contains("data"))
        {
            logger.Log(LogLevel.INFO, "Обработано исключение с фильтром 'data'");
        }
        catch (DocumentException ex)
        {
            logger.Log(LogLevel.INFO, "Обработано общее исключение DocumentException");
        }
    }

    static void MethodThatThrowsException(ILogger logger)
    {
        try
        {
            
            var container = new DocumentContainer(logger);
            container.Get(-1); // Неверный индекс
        }
        catch (DocumentNotFoundException ex)
        {
            logger.Log(LogLevel.WARNING, "Исключение перехвачено во внутреннем методе, пробрасываем выше...");
            
            throw;
        }
    }

    static void ThrowSpecificException(string type)
    {
        if (type == "validation_error")
            throw new DocumentValidationException("Ошибка валидации документа");
        else if (type == "data_error")
            throw new InvalidDocumentDataException("Ошибка данных документа");
        else
            throw new DocumentException("Общая ошибка документа");
    }

    static void DemonstrateAssert(ILogger logger)
    {
        logger.Log(LogLevel.INFO, "=== ДЕМОНСТРАЦИЯ ASSERT ===");

        try
        {
            var container = new DocumentContainer(logger);
            var controller = new AccountingController(container, logger);

           
            controller.GetTotalCostForProduct("TestProduct");

            // Тестирование Assert с null (должно сработать в Debug режиме)
             AccountingController nullController = null;
            
            logger.Log(LogLevel.INFO, "Assert демонстрация завершена");
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.ERROR, $"Ошибка при демонстрации Assert: {ex.Message}");
        }
    }
}