using System;
using System.Windows.Forms;

namespace BinaryCalculator
{
    public partial class Form1 : Form
    {
        private Calculator calculator;

        public Form1()
        {
            InitializeComponent();
            InitializeCalculator();
        }

        private void InitializeCalculator()
        {
            calculator = new Calculator();

            // Подписываемся на события
            calculator.OperationPerformed += Calculator_OperationPerformed;

            // Подписываемся на события кнопок
            buttonAnd.Click += ButtonAnd_Click;
            buttonOr.Click += ButtonOr_Click;
            buttonXor.Click += ButtonXor_Click;
            buttonNot.Click += ButtonNot_Click;
            buttonClear.Click += ButtonClear_Click;

            // Подписываемся на события переключения систем счисления
            radioBinary.CheckedChanged += RadioButton_CheckedChanged;
            radioOctal.CheckedChanged += RadioButton_CheckedChanged;
            radioDecimal.CheckedChanged += RadioButton_CheckedChanged;
            radioHexadecimal.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void Calculator_OperationPerformed(string operation, string result)
        {
            if (operation == "CLEAR")
            {
                labelResult.Text = "0";
                labelError.Text = "";
                return;
            }

            UpdateResultDisplay(result);
            labelError.Text = $"Выполнено: {operation}";
        }

        private void UpdateResultDisplay(string binaryResult)
        {
            try
            {
                long decimalValue = calculator.BinaryToDecimal(binaryResult);

                if (radioBinary.Checked)
                {
                    labelResult.Text = binaryResult;
                }
                else if (radioOctal.Checked)
                {
                    labelResult.Text = calculator.DecimalToOctal(decimalValue);
                }
                else if (radioDecimal.Checked)
                {
                    labelResult.Text = decimalValue.ToString();
                }
                else if (radioHexadecimal.Checked)
                {
                    labelResult.Text = calculator.DecimalToHexadecimal(decimalValue);
                }
            }
            catch (FormatException ex)
            {
                labelError.Text = $"Ошибка: {ex.Message}";
                labelResult.Text = "0";
            }
        }

        private void ButtonAnd_Click(object sender, EventArgs e)
        {
            PerformOperation((calc, x, y) => calc.PerformAnd(x, y));
        }

        private void ButtonOr_Click(object sender, EventArgs e)
        {
            PerformOperation((calc, x, y) => calc.PerformOr(x, y));
        }

        private void ButtonXor_Click(object sender, EventArgs e)
        {
            PerformOperation((calc, x, y) => calc.PerformXor(x, y));
        }

        private void ButtonNot_Click(object sender, EventArgs e)
        {
            try
            {
                if (!calculator.ValidateBinaryInput(textBoxInput1.Text))
                {
                    labelError.Text = calculator.LastError;
                    return;
                }

                string result = calculator.PerformNot(textBoxInput1.Text);
                UpdateResultDisplay(result);
                labelError.Text = "Операция NOT выполнена успешно";
            }
            catch (Exception ex)
            {
                labelError.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            calculator.Clear();
            textBoxInput1.Text = "0";
            textBoxInput2.Text = "0";
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(labelResult.Text) && labelResult.Text != "0")
            {
                // Получаем текущее двоичное значение из результата
                // Для этого нужно преобразовать текущий результат обратно в двоичный вид
                try
                {
                    long decimalValue;

                    if (radioBinary.Checked)
                    {
                        // Уже отображается в двоичном виде
                        return;
                    }
                    else if (radioOctal.Checked)
                    {
                        // Преобразуем восьмеричное в десятичное
                        decimalValue = Convert.ToInt64(labelResult.Text, 8);
                    }
                    else if (radioDecimal.Checked)
                    {
                        // Уже десятичное
                        decimalValue = Convert.ToInt64(labelResult.Text);
                    }
                    else if (radioHexadecimal.Checked)
                    {
                        // Преобразуем шестнадцатеричное в десятичное
                        decimalValue = Convert.ToInt64(labelResult.Text, 16);
                    }
                    else
                    {
                        return;
                    }

                    UpdateResultDisplay(calculator.DecimalToBinary(decimalValue));
                }
                catch
                {
                    labelError.Text = "Ошибка преобразования систем счисления";
                }
            }
        }

        private delegate string BinaryOperation(Calculator calc, string x, string y);

        private void PerformOperation(BinaryOperation operation)
        {
            try
            {
                if (!calculator.ValidateBinaryInput(textBoxInput1.Text) ||
                    !calculator.ValidateBinaryInput(textBoxInput2.Text))
                {
                    labelError.Text = calculator.LastError;
                    return;
                }

                string result = operation(calculator, textBoxInput1.Text, textBoxInput2.Text);
                UpdateResultDisplay(result);
                labelError.Text = "Операция выполнена успешно";
            }
            catch (Exception ex)
            {
                labelError.Text = $"Ошибка: {ex.Message}";
            }
        }

        // Валидация ввода в реальном времени
        private void TextBoxInput1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidateBinaryInput(e);
        }

        private void TextBoxInput2_KeyPress(object sender, KeyPressEventArgs e)
        {
            ValidateBinaryInput(e);
        }

        private void ValidateBinaryInput(KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && e.KeyChar != '0' && e.KeyChar != '1')
            {
                e.Handled = true;
                labelError.Text = "Вводите только 0 и 1";
            }
            else
            {
                labelError.Text = "";
            }
        }
    }
}