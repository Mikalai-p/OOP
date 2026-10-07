using Timer = System.Windows.Forms.Timer;

namespace UniversityLab3
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem загрузитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поискToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem пофиоToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem пофиорегулярноеВыражениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem поспециальностиРегулярноеВыражениеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem конструкторЗапросовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сортировкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem пофиоToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem повозрастуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem покурсуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem посреднемуБаллуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьРезультатыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem опрограммеToolStripMenuItem;
        // Новые пункты меню для вида
        private System.Windows.Forms.ToolStripMenuItem видToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem панельИнструментовToolStripMenuItem;

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripButton tsbSort;
        private System.Windows.Forms.ToolStripButton tsbClear;
        private System.Windows.Forms.ToolStripButton tsbDelete;
        private System.Windows.Forms.ToolStripButton tsbPrev;
        private System.Windows.Forms.ToolStripButton tsbNext;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabelCount;
        private System.Windows.Forms.ToolStripLabel toolStripLabelLastAction;
        private System.Windows.Forms.ToolStripLabel toolStripLabelDateTime;
        private System.Windows.Forms.GroupBox groupBoxStudent;
        private System.Windows.Forms.Label labelFullName;
        private System.Windows.Forms.TextBox textBoxFullName;
        private System.Windows.Forms.Label labelAge;
        private System.Windows.Forms.NumericUpDown numericAge;
        private System.Windows.Forms.Label labelSpecialty;
        private System.Windows.Forms.TextBox textBoxSpecialty;
        private System.Windows.Forms.Label labelBirthDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerBirth;
        private System.Windows.Forms.Label labelCourse;
        private System.Windows.Forms.NumericUpDown numericCourse;
        private System.Windows.Forms.Label labelGroup;
        private System.Windows.Forms.TextBox textBoxGroup;
        private System.Windows.Forms.Label labelAvgGrade;
        private System.Windows.Forms.TextBox textBoxAvgGrade;
        private System.Windows.Forms.Label labelGender;
        private System.Windows.Forms.ComboBox comboBoxGender;
        private System.Windows.Forms.GroupBox groupBoxAddress;
        private System.Windows.Forms.Label labelCity;
        private System.Windows.Forms.TextBox textBoxCity;
        private System.Windows.Forms.Label labelPostalCode;
        private System.Windows.Forms.TextBox textBoxPostalCode;
        private System.Windows.Forms.Label labelStreet;
        private System.Windows.Forms.TextBox textBoxStreet;
        private System.Windows.Forms.Label labelHouse;
        private System.Windows.Forms.TextBox textBoxHouse;
        private System.Windows.Forms.Label labelApartment;
        private System.Windows.Forms.TextBox textBoxApartment;
        private System.Windows.Forms.GroupBox groupBoxWork;
        private System.Windows.Forms.Label labelCompany;
        private System.Windows.Forms.TextBox textBoxCompany;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.TextBox textBoxPosition;
        private System.Windows.Forms.Label labelExperience;
        private System.Windows.Forms.NumericUpDown numericExperience;
        private System.Windows.Forms.Button buttonAddStudent;
        private System.Windows.Forms.Button buttonCalculateBudget;
        private System.Windows.Forms.DataGridView dataGridViewStudents;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Timer timerDateTime;
        private System.Windows.Forms.BindingSource studentsBindingSource;

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
            components = new System.ComponentModel.Container();
            menuStrip = new MenuStrip();
            файлToolStripMenuItem = new ToolStripMenuItem();
            сохранитьToolStripMenuItem = new ToolStripMenuItem();
            загрузитьToolStripMenuItem = new ToolStripMenuItem();
            выходToolStripMenuItem = new ToolStripMenuItem();
            поискToolStripMenuItem = new ToolStripMenuItem();
            пофиоToolStripMenuItem = new ToolStripMenuItem();
            пофиорегулярноеВыражениеToolStripMenuItem = new ToolStripMenuItem();
            поспециальностиРегулярноеВыражениеToolStripMenuItem = new ToolStripMenuItem();
            конструкторЗапросовToolStripMenuItem = new ToolStripMenuItem();
            сортировкаToolStripMenuItem = new ToolStripMenuItem();
            пофиоToolStripMenuItem1 = new ToolStripMenuItem();
            повозрастуToolStripMenuItem = new ToolStripMenuItem();
            покурсуToolStripMenuItem = new ToolStripMenuItem();
            посреднемуБаллуToolStripMenuItem = new ToolStripMenuItem();
            сохранитьРезультатыToolStripMenuItem = new ToolStripMenuItem();
            вXMLToolStripMenuItem = new ToolStripMenuItem();
            вJSONToolStripMenuItem = new ToolStripMenuItem();
            // Новые пункты меню
            видToolStripMenuItem = new ToolStripMenuItem();
            панельИнструментовToolStripMenuItem = new ToolStripMenuItem();
            справкаToolStripMenuItem = new ToolStripMenuItem();
            опрограммеToolStripMenuItem = new ToolStripMenuItem();
            toolStrip = new ToolStrip();
            tsbSearch = new ToolStripButton();
            tsbSort = new ToolStripButton();
            tsbClear = new ToolStripButton();
            tsbDelete = new ToolStripButton();
            tsbPrev = new ToolStripButton();
            tsbNext = new ToolStripButton();
            statusStrip = new StatusStrip();
            toolStripLabelCount = new ToolStripLabel();
            toolStripLabelLastAction = new ToolStripLabel();
            toolStripLabelDateTime = new ToolStripLabel();
            groupBoxStudent = new GroupBox();
            comboBoxGender = new ComboBox();
            labelGender = new Label();
            textBoxAvgGrade = new TextBox();
            labelAvgGrade = new Label();
            textBoxGroup = new TextBox();
            labelGroup = new Label();
            numericCourse = new NumericUpDown();
            labelCourse = new Label();
            dateTimePickerBirth = new DateTimePicker();
            labelBirthDate = new Label();
            textBoxSpecialty = new TextBox();
            labelSpecialty = new Label();
            numericAge = new NumericUpDown();
            labelAge = new Label();
            textBoxFullName = new TextBox();
            labelFullName = new Label();
            groupBoxAddress = new GroupBox();
            textBoxApartment = new TextBox();
            labelApartment = new Label();
            textBoxHouse = new TextBox();
            labelHouse = new Label();
            textBoxStreet = new TextBox();
            labelStreet = new Label();
            textBoxPostalCode = new TextBox();
            labelPostalCode = new Label();
            textBoxCity = new TextBox();
            labelCity = new Label();
            groupBoxWork = new GroupBox();
            numericExperience = new NumericUpDown();
            labelExperience = new Label();
            textBoxPosition = new TextBox();
            labelPosition = new Label();
            textBoxCompany = new TextBox();
            labelCompany = new Label();
            buttonAddStudent = new Button();
            buttonCalculateBudget = new Button();
            dataGridViewStudents = new DataGridView();
            errorProvider = new ErrorProvider(components);
            timerDateTime = new Timer(components);
            studentsBindingSource = new BindingSource(components);
            menuStrip.SuspendLayout();
            toolStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            groupBoxStudent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericCourse).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericAge).BeginInit();
            groupBoxAddress.SuspendLayout();
            groupBoxWork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericExperience).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)studentsBindingSource).BeginInit();
            SuspendLayout();

            // menuStrip
            menuStrip.Items.AddRange(new ToolStripItem[] {
                файлToolStripMenuItem,
                поискToolStripMenuItem,
                сортировкаToolStripMenuItem,
                сохранитьРезультатыToolStripMenuItem,
                видToolStripMenuItem,        // добавлен пункт Вид
                справкаToolStripMenuItem
            });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(984, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip";

            // файлToolStripMenuItem
            файлToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { сохранитьToolStripMenuItem, загрузитьToolStripMenuItem, выходToolStripMenuItem });
            файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            файлToolStripMenuItem.Text = "Файл";

            сохранитьToolStripMenuItem.Name = "сохранитьToolStripMenuItem";
            сохранитьToolStripMenuItem.Text = "Сохранить";
            сохранитьToolStripMenuItem.Click += СохранитьToolStripMenuItem_Click;

            загрузитьToolStripMenuItem.Name = "загрузитьToolStripMenuItem";
            загрузитьToolStripMenuItem.Text = "Загрузить";
            загрузитьToolStripMenuItem.Click += ЗагрузитьToolStripMenuItem_Click;

            выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            выходToolStripMenuItem.Text = "Выход";
            выходToolStripMenuItem.Click += ВыходToolStripMenuItem_Click;

            // поискToolStripMenuItem
            поискToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { пофиоToolStripMenuItem, пофиорегулярноеВыражениеToolStripMenuItem, поспециальностиРегулярноеВыражениеToolStripMenuItem, конструкторЗапросовToolStripMenuItem });
            поискToolStripMenuItem.Name = "поискToolStripMenuItem";
            поискToolStripMenuItem.Text = "Поиск";

            пофиоToolStripMenuItem.Name = "пофиоToolStripMenuItem";
            пофиоToolStripMenuItem.Text = "По ФИО (простой)";
            пофиоToolStripMenuItem.Click += ПофиоToolStripMenuItem_Click;

            пофиорегулярноеВыражениеToolStripMenuItem.Name = "пофиорегулярноеВыражениеToolStripMenuItem";
            пофиорегулярноеВыражениеToolStripMenuItem.Text = "По ФИО (регулярное выражение)";
            пофиорегулярноеВыражениеToolStripMenuItem.Click += ПофиорегулярноеВыражениеToolStripMenuItem_Click;

            поспециальностиРегулярноеВыражениеToolStripMenuItem.Name = "поспециальностиРегулярноеВыражениеToolStripMenuItem";
            поспециальностиРегулярноеВыражениеToolStripMenuItem.Text = "По специальности (регулярное выражение)";
            поспециальностиРегулярноеВыражениеToolStripMenuItem.Click += ПоспециальностиРегулярноеВыражениеToolStripMenuItem_Click;

            конструкторЗапросовToolStripMenuItem.Name = "конструкторЗапросовToolStripMenuItem";
            конструкторЗапросовToolStripMenuItem.Text = "Конструктор запросов...";
            конструкторЗапросовToolStripMenuItem.Click += КонструкторЗапросовToolStripMenuItem_Click;

            // сортировкаToolStripMenuItem
            сортировкаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { пофиоToolStripMenuItem1, повозрастуToolStripMenuItem, покурсуToolStripMenuItem, посреднемуБаллуToolStripMenuItem });
            сортировкаToolStripMenuItem.Name = "сортировкаToolStripMenuItem";
            сортировкаToolStripMenuItem.Text = "Сортировка по";

            пофиоToolStripMenuItem1.Name = "пофиоToolStripMenuItem1";
            пофиоToolStripMenuItem1.Text = "ФИО";
            пофиоToolStripMenuItem1.Click += ПофиоToolStripMenuItem1_Click;

            повозрастуToolStripMenuItem.Name = "повозрастуToolStripMenuItem";
            повозрастуToolStripMenuItem.Text = "Возрасту";
            повозрастуToolStripMenuItem.Click += ПовозрастуToolStripMenuItem_Click;

            покурсуToolStripMenuItem.Name = "покурсуToolStripMenuItem";
            покурсуToolStripMenuItem.Text = "Курсу";
            покурсуToolStripMenuItem.Click += ПокурсуToolStripMenuItem_Click;

            посреднемуБаллуToolStripMenuItem.Name = "посреднемуБаллуToolStripMenuItem";
            посреднемуБаллуToolStripMenuItem.Text = "Среднему баллу";
            посреднемуБаллуToolStripMenuItem.Click += ПосреднемуБаллуToolStripMenuItem_Click;

            // сохранитьРезультатыToolStripMenuItem
            сохранитьРезультатыToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { вXMLToolStripMenuItem, вJSONToolStripMenuItem });
            сохранитьРезультатыToolStripMenuItem.Name = "сохранитьРезультатыToolStripMenuItem";
            сохранитьРезультатыToolStripMenuItem.Text = "Сохранить результаты";

            вXMLToolStripMenuItem.Name = "вXMLToolStripMenuItem";
            вXMLToolStripMenuItem.Text = "в XML";
            вXMLToolStripMenuItem.Click += ВXMLToolStripMenuItem_Click;

            вJSONToolStripMenuItem.Name = "вJSONToolStripMenuItem";
            вJSONToolStripMenuItem.Text = "в JSON";
            вJSONToolStripMenuItem.Click += ВJSONToolStripMenuItem_Click;

            // видToolStripMenuItem
            видToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { панельИнструментовToolStripMenuItem });
            видToolStripMenuItem.Name = "видToolStripMenuItem";
            видToolStripMenuItem.Text = "Вид";

            // панельИнструментовToolStripMenuItem
            панельИнструментовToolStripMenuItem.Name = "панельИнструментовToolStripMenuItem";
            панельИнструментовToolStripMenuItem.Text = "Панель инструментов";
            панельИнструментовToolStripMenuItem.Checked = true;
            панельИнструментовToolStripMenuItem.CheckOnClick = true;
            панельИнструментовToolStripMenuItem.Click += ПанельИнструментовToolStripMenuItem_Click;

            // справкаToolStripMenuItem
            справкаToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { опрограммеToolStripMenuItem });
            справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            справкаToolStripMenuItem.Text = "Справка";

            опрограммеToolStripMenuItem.Name = "опрограммеToolStripMenuItem";
            опрограммеToolStripMenuItem.Text = "О программе";
            опрограммеToolStripMenuItem.Click += ОпрограммеToolStripMenuItem_Click;

            // toolStrip
            toolStrip.Items.AddRange(new ToolStripItem[] { tsbSearch, tsbSort, tsbClear, tsbDelete, tsbPrev, tsbNext });
            toolStrip.Location = new Point(0, 24);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(984, 25);
            toolStrip.TabIndex = 1;
            toolStrip.Text = "toolStrip";
            toolStrip.AllowItemReorder = true;
            toolStrip.AllowDrop = false;
            toolStrip.GripStyle = ToolStripGripStyle.Visible;
            toolStrip.Stretch = true;

            // tsbSearch
            tsbSearch.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbSearch.Text = "Поиск";
            tsbSearch.ToolTipText = "Поиск (простой по ФИО)";
            tsbSearch.Click += TsbSearch_Click;

            // tsbSort
            tsbSort.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbSort.Text = "Сорт.";
            tsbSort.ToolTipText = "Сортировка по ФИО";
            tsbSort.Click += TsbSort_Click;

            // tsbClear
            tsbClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbClear.Text = "Очистить";
            tsbClear.ToolTipText = "Очистить поля ввода";
            tsbClear.Click += TsbClear_Click;

            // tsbDelete
            tsbDelete.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbDelete.Text = "Удалить";
            tsbDelete.ToolTipText = "Удалить выбранного студента";
            tsbDelete.Click += TsbDelete_Click;

            // tsbPrev
            tsbPrev.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbPrev.Text = "<";
            tsbPrev.ToolTipText = "Предыдущий студент";
            tsbPrev.Click += TsbPrev_Click;

            // tsbNext
            tsbNext.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbNext.Text = ">";
            tsbNext.ToolTipText = "Следующий студент";
            tsbNext.Click += TsbNext_Click;

            // statusStrip
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripLabelCount, toolStripLabelLastAction, toolStripLabelDateTime });
            statusStrip.Location = new Point(0, 539);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(984, 22);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip";

            toolStripLabelCount.Name = "toolStripLabelCount";
            toolStripLabelCount.Text = "Количество: 0";

            toolStripLabelLastAction.Name = "toolStripLabelLastAction";
            toolStripLabelLastAction.Text = "Последнее действие: ---";

            toolStripLabelDateTime.Name = "toolStripLabelDateTime";
            toolStripLabelDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            // groupBoxStudent
            groupBoxStudent.Controls.Add(comboBoxGender);
            groupBoxStudent.Controls.Add(labelGender);
            groupBoxStudent.Controls.Add(textBoxAvgGrade);
            groupBoxStudent.Controls.Add(labelAvgGrade);
            groupBoxStudent.Controls.Add(textBoxGroup);
            groupBoxStudent.Controls.Add(labelGroup);
            groupBoxStudent.Controls.Add(numericCourse);
            groupBoxStudent.Controls.Add(labelCourse);
            groupBoxStudent.Controls.Add(dateTimePickerBirth);
            groupBoxStudent.Controls.Add(labelBirthDate);
            groupBoxStudent.Controls.Add(textBoxSpecialty);
            groupBoxStudent.Controls.Add(labelSpecialty);
            groupBoxStudent.Controls.Add(numericAge);
            groupBoxStudent.Controls.Add(labelAge);
            groupBoxStudent.Controls.Add(textBoxFullName);
            groupBoxStudent.Controls.Add(labelFullName);
            groupBoxStudent.Location = new Point(12, 52);
            groupBoxStudent.Name = "groupBoxStudent";
            groupBoxStudent.Size = new Size(450, 250);
            groupBoxStudent.TabIndex = 3;
            groupBoxStudent.TabStop = false;
            groupBoxStudent.Text = "Студент";

            // labelFullName
            labelFullName.Text = "ФИО:";
            labelFullName.Location = new Point(6, 22);
            labelFullName.Size = new Size(90, 23);

            textBoxFullName.Location = new Point(120, 19);
            textBoxFullName.Size = new Size(300, 23);
            textBoxFullName.Validating += textBoxFullName_Validating;

            // labelAge
            labelAge.Text = "Возраст:";
            labelAge.Location = new Point(6, 51);
            labelAge.Size = new Size(90, 23);

            numericAge.Location = new Point(120, 49);
            numericAge.Minimum = 16;
            numericAge.Maximum = 100;
            numericAge.Value = 18;

            // labelSpecialty
            labelSpecialty.Text = "Специальность:";
            labelSpecialty.Location = new Point(6, 80);
            labelSpecialty.Size = new Size(90, 23);

            textBoxSpecialty.Location = new Point(120, 77);
            textBoxSpecialty.Size = new Size(300, 23);

            // labelBirthDate
            labelBirthDate.Text = "Дата рождения:";
            labelBirthDate.Location = new Point(6, 109);
            labelBirthDate.Size = new Size(90, 23);

            dateTimePickerBirth.Location = new Point(120, 106);
            dateTimePickerBirth.Size = new Size(200, 23);
            dateTimePickerBirth.Value = DateTime.Now.AddYears(-18);

            // labelCourse
            labelCourse.Text = "Курс:";
            labelCourse.Location = new Point(6, 138);
            labelCourse.Size = new Size(90, 23);

            numericCourse.Location = new Point(120, 135);
            numericCourse.Minimum = 1;
            numericCourse.Maximum = 6;
            numericCourse.Value = 1;

            // labelGroup
            labelGroup.Text = "Группа:";
            labelGroup.Location = new Point(6, 167);
            labelGroup.Size = new Size(90, 23);

            textBoxGroup.Location = new Point(120, 164);
            textBoxGroup.Size = new Size(150, 23);

            // labelAvgGrade
            labelAvgGrade.Text = "Средний балл:";
            labelAvgGrade.Location = new Point(6, 196);
            labelAvgGrade.Size = new Size(90, 23);

            textBoxAvgGrade.Location = new Point(120, 193);
            textBoxAvgGrade.Size = new Size(100, 23);
            textBoxAvgGrade.Validating += textBoxAvgGrade_Validating;

            // labelGender
            labelGender.Text = "Пол:";
            labelGender.Location = new Point(6, 225);
            labelGender.Size = new Size(90, 23);

            comboBoxGender.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxGender.Location = new Point(120, 222);
            comboBoxGender.Size = new Size(100, 23);
            comboBoxGender.DataSource = Enum.GetValues(typeof(Gender));

            // groupBoxAddress
            groupBoxAddress.Controls.Add(textBoxApartment);
            groupBoxAddress.Controls.Add(labelApartment);
            groupBoxAddress.Controls.Add(textBoxHouse);
            groupBoxAddress.Controls.Add(labelHouse);
            groupBoxAddress.Controls.Add(textBoxStreet);
            groupBoxAddress.Controls.Add(labelStreet);
            groupBoxAddress.Controls.Add(textBoxPostalCode);
            groupBoxAddress.Controls.Add(labelPostalCode);
            groupBoxAddress.Controls.Add(textBoxCity);
            groupBoxAddress.Controls.Add(labelCity);
            groupBoxAddress.Location = new Point(468, 52);
            groupBoxAddress.Name = "groupBoxAddress";
            groupBoxAddress.Size = new Size(250, 200);
            groupBoxAddress.TabIndex = 4;
            groupBoxAddress.TabStop = false;
            groupBoxAddress.Text = "Адрес";

            // labelCity
            labelCity.Text = "Город:";
            labelCity.Location = new Point(6, 22);
            labelCity.Size = new Size(70, 23);

            textBoxCity.Location = new Point(90, 19);
            textBoxCity.Size = new Size(150, 23);

            // labelPostalCode
            labelPostalCode.Text = "Индекс:";
            labelPostalCode.Location = new Point(6, 51);
            labelPostalCode.Size = new Size(70, 23);

            textBoxPostalCode.Location = new Point(90, 48);
            textBoxPostalCode.Size = new Size(100, 23);
            textBoxPostalCode.Validating += textBoxPostalCode_Validating;

            // labelStreet
            labelStreet.Text = "Улица:";
            labelStreet.Location = new Point(6, 80);
            labelStreet.Size = new Size(70, 23);

            textBoxStreet.Location = new Point(90, 77);
            textBoxStreet.Size = new Size(150, 23);

            // labelHouse
            labelHouse.Text = "Дом:";
            labelHouse.Location = new Point(6, 109);
            labelHouse.Size = new Size(70, 23);

            textBoxHouse.Location = new Point(90, 106);
            textBoxHouse.Size = new Size(60, 23);

            // labelApartment
            labelApartment.Text = "Кв.:";
            labelApartment.Location = new Point(6, 138);
            labelApartment.Size = new Size(70, 23);

            textBoxApartment.Location = new Point(90, 135);
            textBoxApartment.Size = new Size(60, 23);

            // groupBoxWork
            groupBoxWork.Controls.Add(numericExperience);
            groupBoxWork.Controls.Add(labelExperience);
            groupBoxWork.Controls.Add(textBoxPosition);
            groupBoxWork.Controls.Add(labelPosition);
            groupBoxWork.Controls.Add(textBoxCompany);
            groupBoxWork.Controls.Add(labelCompany);
            groupBoxWork.Location = new Point(468, 258);
            groupBoxWork.Name = "groupBoxWork";
            groupBoxWork.Size = new Size(250, 130);
            groupBoxWork.TabIndex = 5;
            groupBoxWork.TabStop = false;
            groupBoxWork.Text = "Место работы";

            // labelCompany
            labelCompany.Text = "Компания:";
            labelCompany.Location = new Point(6, 22);
            labelCompany.Size = new Size(70, 23);

            textBoxCompany.Location = new Point(90, 19);
            textBoxCompany.Size = new Size(150, 23);

            // labelPosition
            labelPosition.Text = "Должность:";
            labelPosition.Location = new Point(6, 51);
            labelPosition.Size = new Size(70, 23);

            textBoxPosition.Location = new Point(90, 48);
            textBoxPosition.Size = new Size(150, 23);

            // labelExperience
            labelExperience.Text = "Стаж (лет):";
            labelExperience.Location = new Point(6, 80);
            labelExperience.Size = new Size(70, 23);

            numericExperience.Location = new Point(90, 77);
            numericExperience.Minimum = 0;
            numericExperience.Maximum = 70;
            numericExperience.Value = 0;

            // buttonAddStudent
            buttonAddStudent.Location = new Point(468, 394);
            buttonAddStudent.Size = new Size(120, 30);
            buttonAddStudent.Text = "Добавить студента";
            buttonAddStudent.Click += ButtonAddStudent_Click;

            // buttonCalculateBudget
            buttonCalculateBudget.Location = new Point(598, 394);
            buttonCalculateBudget.Size = new Size(120, 30);
            buttonCalculateBudget.Text = "Рассчитать бюджет";
            buttonCalculateBudget.Click += ButtonCalculateBudget_Click;

            // dataGridViewStudents
            dataGridViewStudents.Location = new Point(12, 430);   // изменено, чтобы кнопки были видны
            dataGridViewStudents.Size = new Size(960, 128);       // изменено
            dataGridViewStudents.TabIndex = 6;
            dataGridViewStudents.AutoGenerateColumns = true;
            dataGridViewStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewStudents.MultiSelect = false;
            dataGridViewStudents.DataSource = studentsBindingSource;
            dataGridViewStudents.SelectionChanged += dataGridViewStudents_SelectionChanged;

            // errorProvider
            errorProvider.ContainerControl = this;

            // timerDateTime
            timerDateTime.Enabled = true;
            timerDateTime.Interval = 1000;
            timerDateTime.Tick += TimerDateTime_Tick;

            // studentsBindingSource
            studentsBindingSource.DataSource = typeof(Student);

            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(dataGridViewStudents);
            Controls.Add(buttonCalculateBudget);
            Controls.Add(buttonAddStudent);
            Controls.Add(groupBoxWork);
            Controls.Add(groupBoxAddress);
            Controls.Add(groupBoxStudent);
            Controls.Add(statusStrip);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "Form1";
            Text = "Университет (Лабораторная работа №3)";
            Load += Form1_Load;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            groupBoxStudent.ResumeLayout(false);
            groupBoxStudent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericCourse).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericAge).EndInit();
            groupBoxAddress.ResumeLayout(false);
            groupBoxAddress.PerformLayout();
            groupBoxWork.ResumeLayout(false);
            groupBoxWork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericExperience).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewStudents).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ((System.ComponentModel.ISupportInitialize)studentsBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}