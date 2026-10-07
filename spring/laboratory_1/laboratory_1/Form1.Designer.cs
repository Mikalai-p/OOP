namespace BinaryCalculator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxInput1;
        private System.Windows.Forms.TextBox textBoxInput2;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonAnd;
        private System.Windows.Forms.Button buttonOr;
        private System.Windows.Forms.Button buttonXor;
        private System.Windows.Forms.Button buttonNot;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.RadioButton radioBinary;
        private System.Windows.Forms.RadioButton radioOctal;
        private System.Windows.Forms.RadioButton radioDecimal;
        private System.Windows.Forms.RadioButton radioHexadecimal;
        private System.Windows.Forms.GroupBox groupBoxOperations;
        private System.Windows.Forms.GroupBox groupBoxNumberSystems;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label label4;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBoxInput1 = new TextBox();
            textBoxInput2 = new TextBox();
            labelResult = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            buttonAnd = new Button();
            buttonOr = new Button();
            buttonXor = new Button();
            buttonNot = new Button();
            buttonClear = new Button();
            radioBinary = new RadioButton();
            radioOctal = new RadioButton();
            radioDecimal = new RadioButton();
            radioHexadecimal = new RadioButton();
            groupBoxOperations = new GroupBox();
            groupBoxNumberSystems = new GroupBox();
            labelError = new Label();
            label4 = new Label();
            groupBoxOperations.SuspendLayout();
            groupBoxNumberSystems.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxInput1
            // 
            textBoxInput1.Location = new Point(160, 46);
            textBoxInput1.Margin = new Padding(4, 5, 4, 5);
            textBoxInput1.Name = "textBoxInput1";
            textBoxInput1.Size = new Size(265, 27);
            textBoxInput1.TabIndex = 0;
            textBoxInput1.Text = "1010";
            // 
            // textBoxInput2
            // 
            textBoxInput2.Location = new Point(160, 108);
            textBoxInput2.Margin = new Padding(4, 5, 4, 5);
            textBoxInput2.Name = "textBoxInput2";
            textBoxInput2.Size = new Size(265, 27);
            textBoxInput2.TabIndex = 1;
            textBoxInput2.Text = "1100";
            // 
            // labelResult
            // 
            labelResult.BackColor = SystemColors.Info;
            labelResult.BorderStyle = BorderStyle.FixedSingle;
            labelResult.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            labelResult.Location = new Point(161, 258);
            labelResult.Margin = new Padding(4, 0, 4, 0);
            labelResult.Name = "labelResult";
            labelResult.Size = new Size(266, 45);
            labelResult.TabIndex = 2;
            labelResult.Text = "0";
            labelResult.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(40, 51);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(110, 20);
            label1.TabIndex = 3;
            label1.Text = "Первое число:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(40, 112);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(107, 20);
            label2.TabIndex = 4;
            label2.Text = "Второе число:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(40, 258);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(78, 20);
            label3.TabIndex = 5;
            label3.Text = "Результат:";
            // 
            // buttonAnd
            // 
            buttonAnd.Location = new Point(27, 38);
            buttonAnd.Margin = new Padding(4, 5, 4, 5);
            buttonAnd.Name = "buttonAnd";
            buttonAnd.Size = new Size(107, 46);
            buttonAnd.TabIndex = 6;
            buttonAnd.Text = "AND (&)";
            buttonAnd.UseVisualStyleBackColor = true;
            // 
            // buttonOr
            // 
            buttonOr.Location = new Point(147, 38);
            buttonOr.Margin = new Padding(4, 5, 4, 5);
            buttonOr.Name = "buttonOr";
            buttonOr.Size = new Size(107, 46);
            buttonOr.TabIndex = 7;
            buttonOr.Text = "OR (|)";
            buttonOr.UseVisualStyleBackColor = true;
            // 
            // buttonXor
            // 
            buttonXor.Location = new Point(267, 38);
            buttonXor.Margin = new Padding(4, 5, 4, 5);
            buttonXor.Name = "buttonXor";
            buttonXor.Size = new Size(107, 46);
            buttonXor.TabIndex = 8;
            buttonXor.Text = "XOR (^)";
            buttonXor.UseVisualStyleBackColor = true;
            // 
            // buttonNot
            // 
            buttonNot.Location = new Point(387, 38);
            buttonNot.Margin = new Padding(4, 5, 4, 5);
            buttonNot.Name = "buttonNot";
            buttonNot.Size = new Size(107, 46);
            buttonNot.TabIndex = 9;
            buttonNot.Text = "NOT (~)";
            buttonNot.UseVisualStyleBackColor = true;
            // 
            // buttonClear
            // 
            buttonClear.Location = new Point(235, 502);
            buttonClear.Margin = new Padding(4, 5, 4, 5);
            buttonClear.Name = "buttonClear";
            buttonClear.Size = new Size(133, 54);
            buttonClear.TabIndex = 10;
            buttonClear.Text = "Очистка";
            buttonClear.UseVisualStyleBackColor = true;
            // 
            // radioBinary
            // 
            radioBinary.AutoSize = true;
            radioBinary.Checked = true;
            radioBinary.Location = new Point(27, 38);
            radioBinary.Margin = new Padding(4, 5, 4, 5);
            radioBinary.Name = "radioBinary";
            radioBinary.Size = new Size(77, 24);
            radioBinary.TabIndex = 11;
            radioBinary.TabStop = true;
            radioBinary.Text = "Двоич.";
            radioBinary.UseVisualStyleBackColor = true;
            // 
            // radioOctal
            // 
            radioOctal.AutoSize = true;
            radioOctal.Location = new Point(120, 38);
            radioOctal.Margin = new Padding(4, 5, 4, 5);
            radioOctal.Name = "radioOctal";
            radioOctal.Size = new Size(77, 24);
            radioOctal.TabIndex = 12;
            radioOctal.Text = "Восьм.";
            radioOctal.UseVisualStyleBackColor = true;
            // 
            // radioDecimal
            // 
            radioDecimal.AutoSize = true;
            radioDecimal.Location = new Point(213, 38);
            radioDecimal.Margin = new Padding(4, 5, 4, 5);
            radioDecimal.Name = "radioDecimal";
            radioDecimal.Size = new Size(72, 24);
            radioDecimal.TabIndex = 13;
            radioDecimal.Text = "Десят.";
            radioDecimal.UseVisualStyleBackColor = true;
            // 
            // radioHexadecimal
            // 
            radioHexadecimal.AutoSize = true;
            radioHexadecimal.Location = new Point(307, 38);
            radioHexadecimal.Margin = new Padding(4, 5, 4, 5);
            radioHexadecimal.Name = "radioHexadecimal";
            radioHexadecimal.Size = new Size(94, 24);
            radioHexadecimal.TabIndex = 14;
            radioHexadecimal.Text = "Шестндц.";
            radioHexadecimal.UseVisualStyleBackColor = true;
            // 
            // groupBoxOperations
            // 
            groupBoxOperations.Controls.Add(buttonAnd);
            groupBoxOperations.Controls.Add(buttonOr);
            groupBoxOperations.Controls.Add(buttonXor);
            groupBoxOperations.Controls.Add(buttonNot);
            groupBoxOperations.Location = new Point(40, 154);
            groupBoxOperations.Margin = new Padding(4, 5, 4, 5);
            groupBoxOperations.Name = "groupBoxOperations";
            groupBoxOperations.Padding = new Padding(4, 5, 4, 5);
            groupBoxOperations.Size = new Size(520, 108);
            groupBoxOperations.TabIndex = 15;
            groupBoxOperations.TabStop = false;
            groupBoxOperations.Text = "Операции";
            // 
            // groupBoxNumberSystems
            // 
            groupBoxNumberSystems.Controls.Add(radioBinary);
            groupBoxNumberSystems.Controls.Add(radioOctal);
            groupBoxNumberSystems.Controls.Add(radioDecimal);
            groupBoxNumberSystems.Controls.Add(radioHexadecimal);
            groupBoxNumberSystems.Location = new Point(40, 303);
            groupBoxNumberSystems.Margin = new Padding(4, 5, 4, 5);
            groupBoxNumberSystems.Name = "groupBoxNumberSystems";
            groupBoxNumberSystems.Padding = new Padding(4, 5, 4, 5);
            groupBoxNumberSystems.Size = new Size(427, 92);
            groupBoxNumberSystems.TabIndex = 16;
            groupBoxNumberSystems.TabStop = false;
            groupBoxNumberSystems.Text = "Система счисления для результата";
            // 
            // labelError
            // 
            labelError.ForeColor = Color.Red;
            labelError.Location = new Point(160, 400);
            labelError.Margin = new Padding(4, 0, 4, 0);
            labelError.Name = "labelError";
            labelError.Size = new Size(267, 46);
            labelError.TabIndex = 17;
            labelError.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(40, 408);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(94, 20);
            label4.TabIndex = 18;
            label4.Text = "Сообщения:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(646, 563);
            Controls.Add(label4);
            Controls.Add(labelError);
            Controls.Add(groupBoxNumberSystems);
            Controls.Add(groupBoxOperations);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(labelResult);
            Controls.Add(textBoxInput2);
            Controls.Add(textBoxInput1);
            Controls.Add(buttonClear);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Бинарный калькулятор";
            groupBoxOperations.ResumeLayout(false);
            groupBoxNumberSystems.ResumeLayout(false);
            groupBoxNumberSystems.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}