using System;

class Program
{
    static void Main()
    {
        void CheckedFunction()
        {
            checked
            {
                int value = int.MaxValue;
                value++; 
                Console.WriteLine($"Checked: {value}");
            }
        }

        void UncheckedFunction()
        {
            unchecked
            {
                int value = int.MaxValue;
                value++; 
                Console.WriteLine($"Unchecked: {value}");
            }
        }

        try
        {
            CheckedFunction();
        }
        catch (OverflowException)
        {
            Console.WriteLine("Checked: OverflowException произошло!");
        }

        UncheckedFunction();
    }
}