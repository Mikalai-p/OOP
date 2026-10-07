using System;

namespace BinaryCalculator
{
    public class Calculator
    {
        public delegate void OperationPerformedEventHandler(string operation, string result);
        public event OperationPerformedEventHandler OperationPerformed;

        private string _lastError;

        public string LastError => _lastError;

        public long BinaryToDecimal(string binary)
        {
            try
            {
                if (string.IsNullOrEmpty(binary))
                    return 0;

                return Convert.ToInt64(binary, 2);
            }
            catch (FormatException)
            {
                _lastError = "Некорректный двоичный формат";
                throw;
            }
        }

        public string DecimalToBinary(long number)
        {
            return Convert.ToString(number, 2);
        }

        public string DecimalToOctal(long number)
        {
            return Convert.ToString(number, 8);
        }

        public string DecimalToHexadecimal(long number)
        {
            return Convert.ToString(number, 16).ToUpper();
        }

        public string PerformAnd(string binary1, string binary2)
        {
            try
            {
                long num1 = BinaryToDecimal(binary1);
                long num2 = BinaryToDecimal(binary2);
                long result = num1 & num2;

                OnOperationPerformed("AND", DecimalToBinary(result));
                return DecimalToBinary(result);
            }
            catch
            {
                throw;
            }
        }

        public string PerformOr(string binary1, string binary2)
        {
            try
            {
                long num1 = BinaryToDecimal(binary1);
                long num2 = BinaryToDecimal(binary2);
                long result = num1 | num2;

                OnOperationPerformed("OR", DecimalToBinary(result));
                return DecimalToBinary(result);
            }
            catch
            {
                throw;
            }
        }

        public string PerformXor(string binary1, string binary2)
        {
            try
            {
                long num1 = BinaryToDecimal(binary1);
                long num2 = BinaryToDecimal(binary2);
                long result = num1 ^ num2;

                OnOperationPerformed("XOR", DecimalToBinary(result));
                return DecimalToBinary(result);
            }
            catch
            {
                throw;
            }
        }

        public string PerformNot(string binary)
        {
            try
            {
                if (string.IsNullOrEmpty(binary))
                    return "0";

                long num = BinaryToDecimal(binary);
                long result = ~num;

                // Ограничиваем результат для отображения в двоичном виде
                // Берем столько бит, сколько было во входном числе
                int bits = binary.Length;
                long mask = (1L << bits) - 1;
                result &= mask;

                string binaryResult = DecimalToBinary(result);
                // Дополняем нулями слева до исходной длины
                binaryResult = binaryResult.PadLeft(bits, '0');

                OnOperationPerformed("NOT", binaryResult);
                return binaryResult;
            }
            catch
            {
                throw;
            }
        }

        protected virtual void OnOperationPerformed(string operation, string result)
        {
            OperationPerformed?.Invoke(operation, result);
        }

        public void Clear()
        {
            _lastError = null;
            OnOperationPerformed("CLEAR", "0");
        }

        public bool ValidateBinaryInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            foreach (char c in input)
            {
                if (c != '0' && c != '1')
                {
                    _lastError = "Допустимы только символы 0 и 1";
                    return false;
                }
            }

            return true;
        }
    }
}