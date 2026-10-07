using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Linq;

namespace UniversityLab3
{
    public partial class SearchResultForm : Form
    {
        private DataGridView dataGridView;
        private Button btnSave;
        private List<Student> _results;

        public SearchResultForm(List<Student> results, string title)
        {
            InitializeComponent();
            this.Text = title;
            this.Size = new System.Drawing.Size(800, 400);
            _results = results;

            // DataGridView
            dataGridView = new DataGridView
            {
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(760, 300),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                DataSource = results.ToList() // копируем список для отображения
            };

            // Кнопка сохранения
            btnSave = new Button
            {
                Text = "Сохранить в файл",
                Location = new System.Drawing.Point(12, 320),
                Size = new System.Drawing.Size(150, 30)
            };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(dataGridView);
            this.Controls.Add(btnSave);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Получаем актуальные данные из DataGridView (если они могли измениться)
            List<Student> studentsToSave = dataGridView.DataSource as List<Student>;
            if (studentsToSave == null || studentsToSave.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML файлы (*.xml)|*.xml|JSON файлы (*.json)|*.json";
                sfd.FilterIndex = 1;
                sfd.DefaultExt = "xml";
                sfd.FileName = "search_results";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string extension = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                        if (extension == ".xml")
                        {
                            // Сериализация в XML
                            XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
                            using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                serializer.Serialize(fs, studentsToSave);
                            }
                        }
                        else if (extension == ".json")
                        {
                            // Сериализация в JSON
                            var options = new JsonSerializerOptions { WriteIndented = true };
                            string json = JsonSerializer.Serialize(studentsToSave, options);
                            File.WriteAllText(sfd.FileName, json);
                        }
                        else
                        {
                            MessageBox.Show("Неподдерживаемый формат файла.");
                            return;
                        }

                        MessageBox.Show($"Результаты успешно сохранены в файл:\n{sfd.FileName}", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Обязательный метод инициализации компонентов (если не используется дизайнер)
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, так как мы создаём контролы вручную в конструкторе.
            // Однако для совместимости с дизайнером оставляем его.
        }
    }
}