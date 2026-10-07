using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Text.Json;
using Microsoft.VisualBasic;

namespace UniversityLab3
{
    public partial class Form1 : Form
    {
        private BindingList<Student> students = new BindingList<Student>();
        private int currentSelectedIndex = -1;

        public Form1()
        {
            InitializeComponent();
            studentsBindingSource.DataSource = students;
            comboBoxGender.DataSource = Enum.GetValues(typeof(Gender));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateStatusCount();
        }

        // ---------- Валидация и добавление студента ----------
        private void ButtonAddStudent_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren()) return;

            try
            {
                var student = new Student
                {
                    FullName = textBoxFullName.Text.Trim(),
                    Age = (int)numericAge.Value,
                    Specialty = textBoxSpecialty.Text.Trim(),
                    BirthDate = dateTimePickerBirth.Value,
                    Course = (int)numericCourse.Value,
                    Group = textBoxGroup.Text.Trim(),
                    AverageGrade = double.Parse(textBoxAvgGrade.Text.Trim()),
                    Gender = (Gender)comboBoxGender.SelectedItem,
                    Address = new Address
                    {
                        City = textBoxCity.Text.Trim(),
                        PostalCode = textBoxPostalCode.Text.Trim(),
                        Street = textBoxStreet.Text.Trim(),
                        House = textBoxHouse.Text.Trim(),
                        Apartment = textBoxApartment.Text.Trim()
                    },
                    WorkPlace = new WorkPlace
                    {
                        Company = textBoxCompany.Text.Trim(),
                        Position = textBoxPosition.Text.Trim(),
                        Experience = (int)numericExperience.Value
                    }
                };

                // Дополнительная валидация через DataAnnotations
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(student);
                bool isValid = Validator.TryValidateObject(student, context, validationResults, true);

                // Рекурсивная валидация Address
                if (student.Address != null)
                {
                    var addrContext = new ValidationContext(student.Address);
                    isValid = Validator.TryValidateObject(student.Address, addrContext, validationResults, true) && isValid;
                }

                // Рекурсивная валидация WorkPlace
                if (student.WorkPlace != null)
                {
                    var workContext = new ValidationContext(student.WorkPlace);
                    isValid = Validator.TryValidateObject(student.WorkPlace, workContext, validationResults, true) && isValid;
                }

                if (!isValid)
                {
                    string errors = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                    MessageBox.Show($"Ошибки валидации:\n{errors}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                students.Add(student);
                SetLastAction($"Добавлен студент: {student.FullName}");
                UpdateStatusCount();
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ClearInputFields()
        {
            textBoxFullName.Clear();
            numericAge.Value = 18;
            textBoxSpecialty.Clear();
            dateTimePickerBirth.Value = DateTime.Now.AddYears(-18);
            numericCourse.Value = 1;
            textBoxGroup.Clear();
            textBoxAvgGrade.Clear();
            comboBoxGender.SelectedIndex = 0;
            textBoxCity.Clear();
            textBoxPostalCode.Clear();
            textBoxStreet.Clear();
            textBoxHouse.Clear();
            textBoxApartment.Clear();
            textBoxCompany.Clear();
            textBoxPosition.Clear();
            numericExperience.Value = 0;
        }

        // ---------- Валидация отдельных полей (события Validating) ----------
        private void textBoxFullName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxFullName.Text))
            {
                errorProvider.SetError(textBoxFullName, "ФИО обязательно");
                e.Cancel = true;
            }
            else if (!Regex.IsMatch(textBoxFullName.Text, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$"))
            {
                errorProvider.SetError(textBoxFullName, "ФИО может содержать только буквы, пробелы и дефис");
                e.Cancel = true;
            }
            else
                errorProvider.SetError(textBoxFullName, "");
        }

        private void textBoxAvgGrade_Validating(object sender, CancelEventArgs e)
        {
            if (!double.TryParse(textBoxAvgGrade.Text, out double grade) || grade < 0 || grade > 10)
            {
                errorProvider.SetError(textBoxAvgGrade, "Средний балл должен быть числом от 0 до 10");
                e.Cancel = true;
            }
            else
                errorProvider.SetError(textBoxAvgGrade, "");
        }

        private void textBoxPostalCode_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxPostalCode.Text) && !Regex.IsMatch(textBoxPostalCode.Text, @"^\d{6}$"))
            {
                errorProvider.SetError(textBoxPostalCode, "Индекс должен состоять из 6 цифр");
                e.Cancel = true;
            }
            else
                errorProvider.SetError(textBoxPostalCode, "");
        }

        // ---------- Меню и панель инструментов ----------
        private void TsbSearch_Click(object sender, EventArgs e) => ПофиоToolStripMenuItem_Click(null, null);
        private void TsbSort_Click(object sender, EventArgs e) => ПофиоToolStripMenuItem1_Click(null, null);
        private void TsbClear_Click(object sender, EventArgs e) => ClearInputFields();
        private void TsbDelete_Click(object sender, EventArgs e) => DeleteSelectedStudent();
        private void TsbPrev_Click(object sender, EventArgs e) => SelectPreviousStudent();
        private void TsbNext_Click(object sender, EventArgs e) => SelectNextStudent();

        private void DeleteSelectedStudent()
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                var student = (Student)dataGridViewStudents.SelectedRows[0].DataBoundItem;
                students.Remove(student);
                SetLastAction($"Удален студент: {student.FullName}");
                UpdateStatusCount();
            }
        }

        private void SelectPreviousStudent()
        {
            if (dataGridViewStudents.Rows.Count == 0) return;
            int newIndex = currentSelectedIndex - 1;
            if (newIndex < 0) newIndex = dataGridViewStudents.Rows.Count - 1;
            if (newIndex >= 0 && newIndex < dataGridViewStudents.Rows.Count)
            {
                dataGridViewStudents.ClearSelection();
                dataGridViewStudents.Rows[newIndex].Selected = true;
                currentSelectedIndex = newIndex;
            }
        }

        private void SelectNextStudent()
        {
            if (dataGridViewStudents.Rows.Count == 0) return;
            int newIndex = currentSelectedIndex + 1;
            if (newIndex >= dataGridViewStudents.Rows.Count) newIndex = 0;
            if (newIndex >= 0 && newIndex < dataGridViewStudents.Rows.Count)
            {
                dataGridViewStudents.ClearSelection();
                dataGridViewStudents.Rows[newIndex].Selected = true;
                currentSelectedIndex = newIndex;
            }
        }

        // ---------- Поиск ----------
        private void ПофиоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string searchText = Microsoft.VisualBasic.Interaction.InputBox("Введите ФИО для поиска:", "Поиск", "");
            if (string.IsNullOrWhiteSpace(searchText)) return;

            var results = students
                .Where(s => s.FullName != null && s.FullName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            ShowSearchResults(results, $"Результаты поиска по ФИО (простой): {searchText}");
        }

        private void ПофиорегулярноеВыражениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pattern = Microsoft.VisualBasic.Interaction.InputBox("Введите регулярное выражение для ФИО:", "Regex поиск", "");
            if (string.IsNullOrWhiteSpace(pattern)) return;

            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                var results = students
                    .Where(s => s.FullName != null && regex.IsMatch(s.FullName))
                    .ToList();
                ShowSearchResults(results, $"Результаты поиска по ФИО (regex): {pattern}");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка в регулярном выражении: {ex.Message}");
            }
        }

        private void ПоспециальностиРегулярноеВыражениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pattern = Microsoft.VisualBasic.Interaction.InputBox("Введите регулярное выражение для специальности:", "Regex поиск", "");
            if (string.IsNullOrWhiteSpace(pattern)) return;

            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                var results = students
                    .Where(s => s.Specialty != null && regex.IsMatch(s.Specialty))
                    .ToList();
                ShowSearchResults(results, $"Результаты поиска по специальности (regex): {pattern}");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка в регулярном выражении: {ex.Message}");
            }
        }

        private void КонструкторЗапросовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var builder = new QueryBuilderForm())
            {
                if (builder.ShowDialog() == DialogResult.OK)
                {
                    var results = builder.ExecuteQuery(students);
                    ShowSearchResults(results, "Результаты сложного поиска");
                }
            }
        }

        private void ShowSearchResults(List<Student> results, string title)
        {
            if (results.Count == 0)
            {
                MessageBox.Show("Ничего не найдено.");
                return;
            }
            var resultForm = new SearchResultForm(results, title);
            resultForm.ShowDialog();
        }

        // ---------- Сортировка ----------
        private void ПофиоToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var sorted = students.OrderBy(s => s.FullName).ToList();
            ReplaceList(sorted);
            SetLastAction("Сортировка по ФИО");
        }

        private void ПовозрастуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = students.OrderBy(s => s.Age).ToList();
            ReplaceList(sorted);
            SetLastAction("Сортировка по возрасту");
        }

        private void ПокурсуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = students.OrderBy(s => s.Course).ThenBy(s => s.FullName).ToList();
            ReplaceList(sorted);
            SetLastAction("Сортировка по курсу");
        }

        private void ПосреднемуБаллуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = students.OrderByDescending(s => s.AverageGrade).ToList();
            ReplaceList(sorted);
            SetLastAction("Сортировка по среднему баллу (убывание)");
        }

        private void ReplaceList(List<Student> sorted)
        {
            students.Clear();
            foreach (var s in sorted)
                students.Add(s);
        }

        // ---------- Сериализация ----------
        private void ВXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "XML files (*.xml)|*.xml", FileName = "students.xml" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var listToSave = students.ToList();
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
                    using (var fs = new FileStream(sfd.FileName, FileMode.Create))
                    {
                        serializer.Serialize(fs, listToSave);
                    }
                    SetLastAction($"Результаты сохранены в XML: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private void ВJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON files (*.json)|*.json", FileName = "students.json" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var listToSave = students.ToList();
                    string json = JsonSerializer.Serialize(listToSave, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(sfd.FileName, json);
                    SetLastAction($"Результаты сохранены в JSON: {sfd.FileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}");
                }
            }
        }

        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Сохранить весь список студентов (аналогично XML)
            ВXMLToolStripMenuItem_Click(sender, e);
        }

        private void ЗагрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "XML files (*.xml)|*.xml|JSON files (*.json)|*.json" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (ofd.FileName.EndsWith(".xml"))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
                        using (var fs = new FileStream(ofd.FileName, FileMode.Open))
                        {
                            var loaded = (List<Student>)serializer.Deserialize(fs);
                            students.Clear();
                            foreach (var s in loaded)
                                students.Add(s);
                        }
                    }
                    else if (ofd.FileName.EndsWith(".json"))
                    {
                        string json = File.ReadAllText(ofd.FileName);
                        var loaded = JsonSerializer.Deserialize<List<Student>>(json);
                        students.Clear();
                        foreach (var s in loaded)
                            students.Add(s);
                    }
                    SetLastAction($"Загружено из файла: {ofd.FileName}");
                    UpdateStatusCount();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }

        private void ВыходToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        // ---------- О программе ----------
        private void ОпрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Лабораторная работа №3\nВыполнил: Пинчук Николай Александрович\nВерсия 1.0", "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ---------- Бюджет университета ----------
        private void ButtonCalculateBudget_Click(object sender, EventArgs e)
        {
            double budget = students.Sum(s => CalculateStudentContribution(s));
            MessageBox.Show($"Общий бюджет университета: {budget:C2}", "Бюджет");
            SetLastAction("Рассчитан бюджет");
        }

        private double CalculateStudentContribution(Student s)
        {
            // Условная формула: базовая стоимость 1000 * курс * коэффициент специальности
            double baseAmount = 1000;
            double specialtyFactor = s.Specialty.ToLower().Contains("информатика") ? 1.5 : 1.0;
            return baseAmount * s.Course * specialtyFactor;
        }

        // ---------- Строка состояния ----------
        private void UpdateStatusCount()
        {
            toolStripLabelCount.Text = $"Количество студентов: {students.Count}";
        }

        private void SetLastAction(string action)
        {
            toolStripLabelLastAction.Text = $"Последнее действие: {action}";
        }

        private void TimerDateTime_Tick(object sender, EventArgs e)
        {
            toolStripLabelDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        // Обработка навигации при смене выделения
        private void dataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
                currentSelectedIndex = dataGridViewStudents.SelectedRows[0].Index;
        }

        // ---------- Новый обработчик для скрытия/показа панели инструментов ----------
        private void ПанельИнструментовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = панельИнструментовToolStripMenuItem.Checked;
        }
    }

    // ---------- Модели данных и атрибуты валидации ----------
    public enum Gender { Мужской, Женский }

    // Собственный атрибут валидации для почтового индекса
    public class ValidatePostalCodeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return true; // необязательное поле
            return Regex.IsMatch(value.ToString(), @"^\d{6}$");
        }
    }

    [Serializable]
    public class Address
    {
        [Required(ErrorMessage = "Город обязателен")]
        public string City { get; set; }

        [ValidatePostalCode(ErrorMessage = "Индекс должен состоять из 6 цифр")]
        public string PostalCode { get; set; }

        public string Street { get; set; }
        public string House { get; set; }
        public string Apartment { get; set; }
    }

    [Serializable]
    public class WorkPlace
    {
        public string Company { get; set; }
        public string Position { get; set; }
        [Range(0, 70, ErrorMessage = "Стаж должен быть от 0 до 70 лет")]
        public int Experience { get; set; }
    }

    [Serializable]
    public class Student
    {
        [Required(ErrorMessage = "ФИО обязательно")]
        [RegularExpression(@"^[а-яА-ЯёЁa-zA-Z\s\-]+$", ErrorMessage = "ФИО может содержать только буквы, пробелы и дефис")]
        public string FullName { get; set; }

        [Range(16, 100, ErrorMessage = "Возраст должен быть от 16 до 100")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Специальность обязательна")]
        public string Specialty { get; set; }

        [DataType(DataType.Date)]
        [CustomValidation(typeof(Student), nameof(ValidateBirthDate))]
        public DateTime BirthDate { get; set; }

        [Range(1, 6, ErrorMessage = "Курс от 1 до 6")]
        public int Course { get; set; }

        [Required(ErrorMessage = "Группа обязательна")]
        [RegularExpression(@"^[А-Яа-я0-9\-]+$", ErrorMessage = "Группа может содержать буквы, цифры и дефис")]
        public string Group { get; set; }

        [Range(0, 10, ErrorMessage = "Средний балл от 0 до 10")]
        public double AverageGrade { get; set; }

        public Gender Gender { get; set; }

        public Address Address { get; set; }
        public WorkPlace WorkPlace { get; set; }

        // Кастомная валидация даты рождения (не в будущем)
        public static ValidationResult ValidateBirthDate(DateTime birthDate, ValidationContext context)
        {
            if (birthDate > DateTime.Now)
                return new ValidationResult("Дата рождения не может быть в будущем");
            return ValidationResult.Success;
        }
    }
}