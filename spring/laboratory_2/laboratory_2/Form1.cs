using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace StudentUniversityApp
{
    public partial class Form1 : Form
    {

        [Serializable]
        public class Address
        {
            public string City { get; set; }
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
            public int ExperienceYears { get; set; }
        }

        [Serializable]
        public class Student
        {
            public string FullName { get; set; }
            public int Age { get; set; }
            public string Speciality { get; set; }
            public DateTime BirthDate { get; set; }
            public int Course { get; set; }
            public string Group { get; set; }
            public double AvgScore { get; set; }
            public string Gender { get; set; } // "Male" или "Female"
            public Address Address { get; set; }
            public WorkPlace Work { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            // Установка максимальной даты рождения (сегодня)
            dtpBirthDate.MaxDate = DateTime.Today;
            // Выбор первого элемента в комбобоксе
            cbSpeciality.SelectedIndex = 0;
        }

        // Обработчик CheckBox "Работает"
        private void chkHasWork_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkHasWork.Checked;
            tbCompany.Enabled = enabled;
            tbPosition.Enabled = enabled;
            numExperience.Enabled = enabled;
        }

        // Обновление метки значения TrackBar
        private void trackBarStress_ValueChanged(object sender, EventArgs e)
        {
            lblStressValue.Text = trackBarStress.Value.ToString();
        }

        // Валидация данных
        private bool ValidateData()
        {
            bool isValid = true;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(tbFIO.Text))
            {
                errorProvider.SetError(tbFIO, "ФИО не может быть пустым");
                isValid = false;
            }

            if (numAge.Value < 16 || numAge.Value > 60)
            {
                errorProvider.SetError(numAge, "Возраст должен быть от 16 до 60");
                isValid = false;
            }

            if (dtpBirthDate.Value > DateTime.Today)
            {
                errorProvider.SetError(dtpBirthDate, "Дата рождения не может быть в будущем");
                isValid = false;
            }

            if (numAvgScore.Value < 2 || numAvgScore.Value > 10)
            {
                errorProvider.SetError(numAvgScore, "Средний балл должен быть от 2.0 до 10.0");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(tbGroup.Text))
            {
                errorProvider.SetError(tbGroup, "Укажите группу");
                isValid = false;
            }

            // Если чекбокс работы отмечен, поля должны быть заполнены
            if (chkHasWork.Checked)
            {
                if (string.IsNullOrWhiteSpace(tbCompany.Text))
                {
                    errorProvider.SetError(tbCompany, "Укажите компанию");
                    isValid = false;
                }
                if (string.IsNullOrWhiteSpace(tbPosition.Text))
                {
                    errorProvider.SetError(tbPosition, "Укажите должность");
                    isValid = false;
                }
                // стаж может быть 0
            }

            return isValid;
        }

        // Сбор данных с формы в объект Student
        private Student GetStudentFromForm()
        {
            Student student = new Student
            {
                FullName = tbFIO.Text,
                Age = (int)numAge.Value,
                Speciality = cbSpeciality.SelectedItem?.ToString(),
                BirthDate = dtpBirthDate.Value,
                Course = (int)numCourse.Value,
                Group = tbGroup.Text,
                AvgScore = (double)numAvgScore.Value,
                Gender = rbMale.Checked ? "Male" : "Female",
                Address = new Address
                {
                    City = tbCity.Text,
                    PostalCode = tbIndex.Text,
                    Street = tbStreet.Text,
                    House = tbHouse.Text,
                    Apartment = tbApartment.Text
                }
            };

            if (chkHasWork.Checked)
            {
                student.Work = new WorkPlace
                {
                    Company = tbCompany.Text,
                    Position = tbPosition.Text,
                    ExperienceYears = (int)numExperience.Value
                };
            }
            else
            {
                student.Work = null;
            }

            return student;
        }

        // Заполнение формы из объекта Student
        private void SetFormFromStudent(Student student)
        {
            tbFIO.Text = student.FullName;
            numAge.Value = student.Age;
            cbSpeciality.SelectedItem = student.Speciality;
            dtpBirthDate.Value = student.BirthDate;
            numCourse.Value = student.Course;
            tbGroup.Text = student.Group;
            numAvgScore.Value = (decimal)student.AvgScore;
            if (student.Gender == "Male")
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;

            // Адрес
            if (student.Address != null)
            {
                tbCity.Text = student.Address.City;
                tbIndex.Text = student.Address.PostalCode;
                tbStreet.Text = student.Address.Street;
                tbHouse.Text = student.Address.House;
                tbApartment.Text = student.Address.Apartment;
            }

            // Работа
            if (student.Work != null)
            {
                chkHasWork.Checked = true;
                tbCompany.Text = student.Work.Company;
                tbPosition.Text = student.Work.Position;
                numExperience.Value = student.Work.ExperienceYears;
            }
            else
            {
                chkHasWork.Checked = false;
                tbCompany.Clear();
                tbPosition.Clear();
                numExperience.Value = 0;
            }
        }

        // Расчет бюджета
        private void btnCalculateBudget_Click(object sender, EventArgs e)
        {
            if (!ValidateData()) return;

            // Доход = (курс * 300) + (средний балл * 150)
            // Расход = (возраст * 35) + (стаж работы * 10, если работает)
            double income = (double)numCourse.Value * 300 + (double)numAvgScore.Value * 150;
            double expense = (double)numAge.Value * 35;
            if (chkHasWork.Checked)
                expense += (double)numExperience.Value * 10;

            double budget = income - expense;
            lblBudgetResult.Text = $"Бюджет: {budget:N2} BYN";
        }

        // Сохранение в JSON
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateData()) return;

            Student student = GetStudentFromForm();

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json",
                FileName = "student.json"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = JsonSerializer.Serialize(student, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(sfd.FileName, json);
                    MessageBox.Show("Данные сохранены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Загрузка из JSON
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string json = File.ReadAllText(ofd.FileName);
                    Student student = JsonSerializer.Deserialize<Student>(json);
                    if (student != null)
                    {
                        SetFormFromStudent(student);
                        MessageBox.Show("Данные загружены", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}