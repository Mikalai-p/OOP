using System;
using System.Collections.Generic;

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

    protected Document(string number, Date date, Organization organization)
    {
        Number = number;
        Date = date;
        Organization = organization;
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
        return $"Документ №{Number} от {Date}";
    }
}

public class Receipt : Document
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
        Console.WriteLine("=====================================");
    }

    

    public override string ToString()
    {
        return $"{GetDocumentInfo()}\nПлательщик: {Payer}\nОрганизация: {Organization}";
    }
}

public class Invoice : Document
{
    public List<string> Goods { get; set; }
    public string Shipper { get; set; }

    public Invoice(string number, Date date, Organization organization,
        List<string> goods, string shipper) : base(number, date, organization)
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
        return $"{GetDocumentInfo()}\nГрузоотправитель: {Shipper}\nТовары: {string.Join(", ", Goods)}";
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
}

public class Organization
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }

    public Organization(string name, string address, string phone)
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

class Program
{
    static void Main()
    {
        var org1 = new Organization("ООО 'Ромашка'", "ул. Ленина, 1", "+7-999-123-45-67");
        var org2 = new Organization("ИП Иванов", "пр. Мира, 10", "+7-912-345-67-89");
        var org3 = new Organization("ЗАО 'Вектор'", "ул. Центральная, 25", "+7-495-111-22-33");

        var date1 = new Date(15, 5, 2023);
        var date2 = new Date(20, 5, 2023);
        var date3 = new Date(10, 6, 2023);

        
        var receipt = new Receipt("001", date1, org1, 1500.50m, "Петров П.П.");

        var invoice = new Invoice("002", date2, org2,
            new List<string> { "Ноутбук", "Мышь", "Клавиатура" }, "ООО 'Поставщик'");
        var check = new Check("003", date1, org1, 2000.00m, "Сидорова А.И.");

        Document[] documents = { receipt, invoice, check };
        IDocument[] iDocuments = { receipt, invoice, check };

        foreach (var doc in documents)
        {
            if (doc is Receipt r)
                Console.WriteLine($"Это квитанция на сумму: {r.Amount}");
            else if (doc is Invoice inv)
                Console.WriteLine($"Это накладная с {inv.Goods.Count} позициями");

            var checkDoc = doc as Check;
            if (checkDoc != null)
                Console.WriteLine($"Это чек кассира: {checkDoc.Cashier}");
        }

        Console.WriteLine("\n" + new string('=', 50) + "\n");

        Printer printer = new Printer();

        foreach (var doc in iDocuments)
        {
            printer.IAmPrinting(doc);
        }

        
        Console.WriteLine("Демонстрация одноименных методов с одной квитанцией:");

        Console.WriteLine("\n1. Вызов receipt.Print() - полная информация:");
        receipt.Print();

        Console.WriteLine("\n2. Вызов (receipt as IDocument).Print() - минимальная информация:");
        (receipt as IDocument).Print();

            }
}