using System;

class Program
{
    static void Main()
    {
        
        Console.WriteLine("=== Задание 1a ===");
        bool boolVar = true;
        byte byteVar = 255;
        sbyte sbyteVar = -128;
        char charVar = 'A';
        decimal decimalVar = 123.456m;
        double doubleVar = 123.456;
        float floatVar = 123.456f;
        int intVar = 2147483647;
        uint uintVar = 4294967295;
        long longVar = 9223372036854775807;
        ulong ulongVar = 18446744073709551615;
        short shortVar = 32767;
        ushort ushortVar = 65535;

        Console.WriteLine($"bool: {boolVar}");
        Console.WriteLine($"byte: {byteVar}");
        Console.WriteLine($"sbyte: {sbyteVar}");
        Console.WriteLine($"char: {charVar}");
        Console.WriteLine($"decimal: {decimalVar}");
        Console.WriteLine($"double: {doubleVar}");
        Console.WriteLine($"float: {floatVar}");
        Console.WriteLine($"int: {intVar}");
        Console.WriteLine($"uint: {uintVar}");
        Console.WriteLine($"long: {longVar}");
        Console.WriteLine($"ulong: {ulongVar}");
        Console.WriteLine($"short: {shortVar}");
        Console.WriteLine($"ushort: {ushortVar}");

        
        Console.WriteLine("\n=== Задание 1b ===");
        
        int intValue = 100;
        double doubleValue = intValue;
        Console.WriteLine($"Неявное: int {intValue} -> double {doubleValue}");

       
        doubleValue = 123.456;
        intValue = (int)doubleValue;
        Console.WriteLine($"Явное: double {doubleValue} -> int {intValue}");

        
        string strValue = "123";
        intValue = Convert.ToInt32(strValue);
        Console.WriteLine($"Convert: string '{strValue}' -> int {intValue}");

        
        Console.WriteLine("\n=== Задание 1c ===");
        int originalValue = 42;
        object boxedValue = originalValue; 
        int unboxedValue = (int)boxedValue; 
        Console.WriteLine($"Упаковка/распаковка: {originalValue} -> {boxedValue} -> {unboxedValue}");

        
        Console.WriteLine("\n=== Задание 1d ===");
        var implicitVar = "Hello World";
        Console.WriteLine($"Неявно типизированная переменная: {implicitVar} (тип: {implicitVar.GetType()})");

        
        Console.WriteLine("\n=== Задание 1e ===");
        int? nullableInt = null;
        Console.WriteLine($"Nullable переменная: {nullableInt}");
        nullableInt = 42;
        Console.WriteLine($"Nullable переменная после присваивания: {nullableInt}");

        
        Console.WriteLine("\n=== Задание 1f ===");
        var varVariable = "Hello World";
        Console.WriteLine($"Переменная var: {varVariable} (тип: {varVariable.GetType()})");

        Console.WriteLine("Ошибка при попытке присвоить значение другого типа: CS0029");
    }
}