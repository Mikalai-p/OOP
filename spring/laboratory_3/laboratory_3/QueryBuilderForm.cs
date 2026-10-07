using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace UniversityLab3
{
    public partial class QueryBuilderForm : Form
    {
        private ComboBox cmbField;
        private ComboBox cmbOperator;
        private TextBox txtValue;
        private Button btnOk;
        private Button btnCancel;
        private ListBox lstCriteria;
        private Button btnAddCriterion;
        private List<Criterion> criteria = new List<Criterion>();

        public QueryBuilderForm()
        {

            SetupUI();
        }


        private void SetupUI()
        {
            this.Text = "Конструктор запросов";
            this.Size = new System.Drawing.Size(400, 350);

            Label lblField = new Label { Text = "Поле:", Location = new System.Drawing.Point(12, 15), Size = new System.Drawing.Size(60, 25) };
            cmbField = new ComboBox { Location = new System.Drawing.Point(80, 12), Size = new System.Drawing.Size(120, 25) };
            cmbField.Items.AddRange(new[] { "ФИО", "Специальность", "Группа", "Компания" });
            cmbField.SelectedIndex = 0;

            Label lblOperator = new Label { Text = "Оператор:", Location = new System.Drawing.Point(12, 45), Size = new System.Drawing.Size(60, 25) };
            cmbOperator = new ComboBox { Location = new System.Drawing.Point(80, 42), Size = new System.Drawing.Size(120, 25) };
            cmbOperator.Items.AddRange(new[] { "Содержит", "Равно", "Регулярное выражение" });
            cmbOperator.SelectedIndex = 0;

            Label lblValue = new Label { Text = "Значение:", Location = new System.Drawing.Point(12, 75), Size = new System.Drawing.Size(60, 25) };
            txtValue = new TextBox { Location = new System.Drawing.Point(80, 72), Size = new System.Drawing.Size(200, 25) };

            btnAddCriterion = new Button { Text = "Добавить", Location = new System.Drawing.Point(300, 70), Size = new System.Drawing.Size(75, 30) };
            btnAddCriterion.Click += BtnAddCriterion_Click;

            lstCriteria = new ListBox { Location = new System.Drawing.Point(12, 110), Size = new System.Drawing.Size(360, 150) };

            btnOk = new Button { Text = "Выполнить", Location = new System.Drawing.Point(200, 270), Size = new System.Drawing.Size(80, 30), DialogResult = DialogResult.OK };
            btnCancel = new Button { Text = "Отмена", Location = new System.Drawing.Point(290, 270), Size = new System.Drawing.Size(80, 30), DialogResult = DialogResult.Cancel };

            this.Controls.AddRange(new Control[] { lblField, cmbField, lblOperator, cmbOperator, lblValue, txtValue, btnAddCriterion, lstCriteria, btnOk, btnCancel });
        }

        private void BtnAddCriterion_Click(object sender, EventArgs e)
        {
            string field = cmbField.SelectedItem.ToString();
            string op = cmbOperator.SelectedItem.ToString();
            string value = txtValue.Text.Trim();
            if (string.IsNullOrEmpty(value))
            {
                MessageBox.Show("Введите значение");
                return;
            }
            criteria.Add(new Criterion { Field = field, Operator = op, Value = value });
            lstCriteria.Items.Add($"{field} {op} \"{value}\"");
            txtValue.Clear();
        }

        public List<Student> ExecuteQuery(IEnumerable<Student> source)
        {
            IEnumerable<Student> query = source;
            foreach (var crit in criteria)
            {
                if (crit.Operator == "Содержит")
                {
                    query = query.Where(s => GetFieldValue(s, crit.Field).IndexOf(crit.Value, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else if (crit.Operator == "Равно")
                {
                    query = query.Where(s => GetFieldValue(s, crit.Field).Equals(crit.Value, StringComparison.OrdinalIgnoreCase));
                }
                else if (crit.Operator == "Регулярное выражение")
                {
                    try
                    {
                        var regex = new Regex(crit.Value, RegexOptions.IgnoreCase);
                        query = query.Where(s => regex.IsMatch(GetFieldValue(s, crit.Field)));
                    }
                    catch { }
                }
            }
            return query.ToList();
        }

        private string GetFieldValue(Student s, string field)
        {
            return field switch
            {
                "ФИО" => s.FullName,
                "Специальность" => s.Specialty,
                "Группа" => s.Group,
                "Компания" => s.WorkPlace?.Company ?? "",
                _ => ""
            };
        }

        private class Criterion
        {
            public string Field { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }
    }
}