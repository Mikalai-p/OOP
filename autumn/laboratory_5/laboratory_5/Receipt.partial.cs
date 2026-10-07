using System;

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