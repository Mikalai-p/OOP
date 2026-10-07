using System;
using System.Collections.Generic;

public enum DocumentStatus
{
    Draft,       // Черновик - документ создан, но не отправлен
    Submitted,   // Подан - документ отправлен на рассмотрение
    Approved,    // Утвержден - документ прошел проверку и утвержден
    Rejected,    // Отклонен - документ не прошел проверку
    Archived     // В архиве - документ завершен и перемещен в архив

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
        Number = number;
        Date = date;
        Organization = organization;
        Status = DocumentStatus.Draft;
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
        Amount = amount;
        Payer = payer;
    }

    public override string GetDocumentInfo()
    {
        return $"Квитанция №{Number} на сумму {Amount} от {Date}";
    }
}

public class Invoice : Document
{
    public List<ProductItem> Goods { get; set; }
    public string Shipper { get; set; }

    public Invoice(string number, Date date, Organization organization,
        List<ProductItem> goods, string shipper) : base(number, date, organization)
    {
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
        Name = name;
        Address = address;
        Phone = phone;
    }

    public override string ToString()
    {
        return $"{Name} ({Address}, тел.: {Phone})";
    }
}

public class Printer
{
    public void IAmPrinting(IDocument document)
    {
        if (document is null) return;

        Console.WriteLine("Тип объекта: " + document.GetType().Name);
        Console.WriteLine(document.ToString());
        Console.WriteLine(new string('-', 40));
    }
}


public class DocumentContainer
{
    private List<Document> documents = new List<Document>();

    public void Add(Document document)
    {
        documents.Add(document);
    }

    public bool Remove(Document document)
    {
        return documents.Remove(document);
    }

    public Document Get(int index)
    {
        if (index >= 0 && index < documents.Count)
            return documents[index];
        return null;
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

    public AccountingController(DocumentContainer container)
    {
        this.container = container;
    }

    
    public decimal GetTotalCostForProduct(string productName)
    {
        decimal total = 0;
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
                    }
                }
            }
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

                if (count >= 2) break;
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
        
        var address1 = new Address("ул. Ленина, 1", "Москва", "101000");
        var address2 = new Address("пр. Мира, 10", "Санкт-Петербург", "190000");
        var address3 = new Address("ул. Центральная, 25", "Новосибирск", "630000");

        var org1 = new Organization("ООО 'Ромашка'", address1, "+7-999-123-45-67");
        var org2 = new Organization("ИП Иванов", address2, "+7-912-345-67-89");
        var org3 = new Organization("ЗАО 'Вектор'", address3, "+7-495-111-22-33");

        var date1 = new Date(15, 5, 2023);
        var date2 = new Date(20, 5, 2023);
        var date3 = new Date(10, 6, 2023);
        var date4 = new Date(25, 5, 2023);

        
        var goods1 = new List<ProductItem>
        {
            new ProductItem("Ноутбук", 50000, 2),
            new ProductItem("Мышь", 1000, 5),
            new ProductItem("Клавиатура", 2000, 3)
        };

        var goods2 = new List<ProductItem>
        {
            new ProductItem("Монитор", 15000, 4),
            new ProductItem("Мышь", 1000, 10),
            new ProductItem("Коврик для мыши", 500, 15)
        };

        var receipt = new Receipt("001", date1, org1, 1500.50m, "Петров П.П.");
        var invoice1 = new Invoice("002", date2, org2, goods1, "ООО 'Поставщик'");
        var invoice2 = new Invoice("004", date4, org3, goods2, "ИП Сидоров");
        var check = new Check("003", date1, org1, 2000.00m, "Сидорова А.И.");

        
        var container = new DocumentContainer();
        container.Add(receipt);
        container.Add(invoice1);
        container.Add(check);
        container.Add(invoice2);

        
        container.PrintAll();

        
        var controller = new AccountingController(container);

        
        string productName = "Мышь";
        decimal totalCost = controller.GetTotalCostForProduct(productName);
        Console.WriteLine($"Суммарная стоимость '{productName}': {totalCost} руб.");

        
        int checksCount = controller.GetChecksCount();
        Console.WriteLine($"Количество чеков: {checksCount}");

        Console.WriteLine("\n" + new string('=', 50) + "\n");

        
        var startPeriod = new Date(10, 5, 2023);
        var endPeriod = new Date(30, 5, 2023);
        controller.PrintDocumentsInPeriod(startPeriod, endPeriod);

        
        Console.WriteLine("Демонстрация работы с документами:");
        Console.WriteLine("\n1. Вызов receipt.Print() - полная информация:");
        receipt.Print();

        Console.WriteLine("\n2. Вызов (receipt as IDocument).Print() - минимальная информация:");
        (receipt as IDocument).Print();
    }
}