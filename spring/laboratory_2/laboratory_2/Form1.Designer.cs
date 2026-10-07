namespace StudentUniversityApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tbFIO = new TextBox();
            numAge = new NumericUpDown();
            cbSpeciality = new ComboBox();
            dtpBirthDate = new DateTimePicker();
            numCourse = new NumericUpDown();
            tbGroup = new TextBox();
            numAvgScore = new NumericUpDown();
            gbGender = new GroupBox();
            rbMale = new RadioButton();
            rbFemale = new RadioButton();
            gbAddress = new GroupBox();
            lblCity = new Label();
            tbCity = new TextBox();
            lblIndex = new Label();
            tbIndex = new TextBox();
            lblStreet = new Label();
            tbStreet = new TextBox();
            lblHouse = new Label();
            tbHouse = new TextBox();
            lblApartment = new Label();
            tbApartment = new TextBox();
            gbWork = new GroupBox();
            chkHasWork = new CheckBox();
            lblCompany = new Label();
            tbCompany = new TextBox();
            lblPosition = new Label();
            tbPosition = new TextBox();
            lblExperience = new Label();
            numExperience = new NumericUpDown();
            trackBarStress = new TrackBar();
            lblStressValue = new Label();
            btnCalculateBudget = new Button();
            lblBudgetResult = new Label();
            btnSave = new Button();
            btnLoad = new Button();
            panel = new Panel();
            lblFIO = new Label();
            lblAge = new Label();
            lblSpeciality = new Label();
            lblBirth = new Label();
            lblCourse = new Label();
            lblGroup = new Label();
            lblAvgScore = new Label();
            lblStress = new Label();
            errorProvider = new ErrorProvider(components);
            ((System.ComponentModel.ISupportInitialize)numAge).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCourse).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAvgScore).BeginInit();
            gbGender.SuspendLayout();
            gbAddress.SuspendLayout();
            gbWork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numExperience).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarStress).BeginInit();
            panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).BeginInit();
            SuspendLayout();
            // 
            // tbFIO
            // 
            tbFIO.Location = new Point(160, 11);
            tbFIO.Margin = new Padding(4, 5, 4, 5);
            tbFIO.Name = "tbFIO";
            tbFIO.Size = new Size(265, 27);
            tbFIO.TabIndex = 1;
            // 
            // numAge
            // 
            numAge.Location = new Point(160, 58);
            numAge.Margin = new Padding(4, 5, 4, 5);
            numAge.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            numAge.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
            numAge.Name = "numAge";
            numAge.Size = new Size(267, 27);
            numAge.TabIndex = 3;
            numAge.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // cbSpeciality
            // 
            cbSpeciality.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSpeciality.Items.AddRange(new object[] { "Информатика", "Экономика", "Юриспруденция", "Менеджмент", "Дизайн" });
            cbSpeciality.Location = new Point(160, 103);
            cbSpeciality.Margin = new Padding(4, 5, 4, 5);
            cbSpeciality.Name = "cbSpeciality";
            cbSpeciality.Size = new Size(265, 28);
            cbSpeciality.TabIndex = 5;
            // 
            // dtpBirthDate
            // 
            dtpBirthDate.Location = new Point(160, 149);
            dtpBirthDate.Margin = new Padding(4, 5, 4, 5);
            dtpBirthDate.MaxDate = new DateTime(2026, 2, 17, 0, 0, 0, 0);
            dtpBirthDate.Name = "dtpBirthDate";
            dtpBirthDate.Size = new Size(265, 27);
            dtpBirthDate.TabIndex = 7;
            dtpBirthDate.Value = new DateTime(2026, 2, 17, 0, 0, 0, 0);
            // 
            // numCourse
            // 
            numCourse.Location = new Point(160, 197);
            numCourse.Margin = new Padding(4, 5, 4, 5);
            numCourse.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numCourse.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCourse.Name = "numCourse";
            numCourse.Size = new Size(267, 27);
            numCourse.TabIndex = 9;
            numCourse.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tbGroup
            // 
            tbGroup.Location = new Point(160, 242);
            tbGroup.Margin = new Padding(4, 5, 4, 5);
            tbGroup.Name = "tbGroup";
            tbGroup.Size = new Size(265, 27);
            tbGroup.TabIndex = 11;
            // 
            // numAvgScore
            // 
            numAvgScore.DecimalPlaces = 1;
            numAvgScore.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numAvgScore.Location = new Point(160, 289);
            numAvgScore.Margin = new Padding(4, 5, 4, 5);
            numAvgScore.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numAvgScore.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            numAvgScore.Name = "numAvgScore";
            numAvgScore.Size = new Size(267, 27);
            numAvgScore.TabIndex = 13;
            numAvgScore.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // gbGender
            // 
            gbGender.Controls.Add(rbMale);
            gbGender.Controls.Add(rbFemale);
            gbGender.Location = new Point(13, 338);
            gbGender.Margin = new Padding(4, 5, 4, 5);
            gbGender.Name = "gbGender";
            gbGender.Padding = new Padding(4, 5, 4, 5);
            gbGender.Size = new Size(427, 77);
            gbGender.TabIndex = 14;
            gbGender.TabStop = false;
            gbGender.Text = "Пол";
            // 
            // rbMale
            // 
            rbMale.AutoSize = true;
            rbMale.Checked = true;
            rbMale.Location = new Point(13, 31);
            rbMale.Margin = new Padding(4, 5, 4, 5);
            rbMale.Name = "rbMale";
            rbMale.Size = new Size(93, 24);
            rbMale.TabIndex = 0;
            rbMale.TabStop = true;
            rbMale.Text = "Мужской";
            rbMale.UseVisualStyleBackColor = true;
            // 
            // rbFemale
            // 
            rbFemale.AutoSize = true;
            rbFemale.Location = new Point(160, 31);
            rbFemale.Margin = new Padding(4, 5, 4, 5);
            rbFemale.Name = "rbFemale";
            rbFemale.Size = new Size(92, 24);
            rbFemale.TabIndex = 1;
            rbFemale.Text = "Женский";
            rbFemale.UseVisualStyleBackColor = true;
            // 
            // gbAddress
            // 
            gbAddress.Controls.Add(lblCity);
            gbAddress.Controls.Add(tbCity);
            gbAddress.Controls.Add(lblIndex);
            gbAddress.Controls.Add(tbIndex);
            gbAddress.Controls.Add(lblStreet);
            gbAddress.Controls.Add(tbStreet);
            gbAddress.Controls.Add(lblHouse);
            gbAddress.Controls.Add(tbHouse);
            gbAddress.Controls.Add(lblApartment);
            gbAddress.Controls.Add(tbApartment);
            gbAddress.Location = new Point(13, 431);
            gbAddress.Margin = new Padding(4, 5, 4, 5);
            gbAddress.Name = "gbAddress";
            gbAddress.Padding = new Padding(4, 5, 4, 5);
            gbAddress.Size = new Size(667, 200);
            gbAddress.TabIndex = 15;
            gbAddress.TabStop = false;
            gbAddress.Text = "Адрес";
            // 
            // lblCity
            // 
            lblCity.AutoSize = true;
            lblCity.Location = new Point(13, 38);
            lblCity.Margin = new Padding(4, 0, 4, 0);
            lblCity.Name = "lblCity";
            lblCity.Size = new Size(54, 20);
            lblCity.TabIndex = 0;
            lblCity.Text = "Город:";
            // 
            // tbCity
            // 
            tbCity.Location = new Point(133, 34);
            tbCity.Margin = new Padding(4, 5, 4, 5);
            tbCity.Name = "tbCity";
            tbCity.Size = new Size(199, 27);
            tbCity.TabIndex = 1;
            // 
            // lblIndex
            // 
            lblIndex.AutoSize = true;
            lblIndex.Location = new Point(347, 38);
            lblIndex.Margin = new Padding(4, 0, 4, 0);
            lblIndex.Name = "lblIndex";
            lblIndex.Size = new Size(62, 20);
            lblIndex.TabIndex = 2;
            lblIndex.Text = "Индекс:";
            // 
            // tbIndex
            // 
            tbIndex.Location = new Point(467, 34);
            tbIndex.Margin = new Padding(4, 5, 4, 5);
            tbIndex.Name = "tbIndex";
            tbIndex.Size = new Size(132, 27);
            tbIndex.TabIndex = 3;
            // 
            // lblStreet
            // 
            lblStreet.AutoSize = true;
            lblStreet.Location = new Point(13, 85);
            lblStreet.Margin = new Padding(4, 0, 4, 0);
            lblStreet.Name = "lblStreet";
            lblStreet.Size = new Size(55, 20);
            lblStreet.TabIndex = 4;
            lblStreet.Text = "Улица:";
            // 
            // tbStreet
            // 
            tbStreet.Location = new Point(133, 80);
            tbStreet.Margin = new Padding(4, 5, 4, 5);
            tbStreet.Name = "tbStreet";
            tbStreet.Size = new Size(199, 27);
            tbStreet.TabIndex = 5;
            // 
            // lblHouse
            // 
            lblHouse.AutoSize = true;
            lblHouse.Location = new Point(347, 85);
            lblHouse.Margin = new Padding(4, 0, 4, 0);
            lblHouse.Name = "lblHouse";
            lblHouse.Size = new Size(42, 20);
            lblHouse.TabIndex = 6;
            lblHouse.Text = "Дом:";
            // 
            // tbHouse
            // 
            tbHouse.Location = new Point(467, 80);
            tbHouse.Margin = new Padding(4, 5, 4, 5);
            tbHouse.Name = "tbHouse";
            tbHouse.Size = new Size(65, 27);
            tbHouse.TabIndex = 7;
            // 
            // lblApartment
            // 
            lblApartment.AutoSize = true;
            lblApartment.Location = new Point(13, 131);
            lblApartment.Margin = new Padding(4, 0, 4, 0);
            lblApartment.Name = "lblApartment";
            lblApartment.Size = new Size(78, 20);
            lblApartment.TabIndex = 8;
            lblApartment.Text = "Квартира:";
            // 
            // tbApartment
            // 
            tbApartment.Location = new Point(133, 126);
            tbApartment.Margin = new Padding(4, 5, 4, 5);
            tbApartment.Name = "tbApartment";
            tbApartment.Size = new Size(65, 27);
            tbApartment.TabIndex = 9;
            // 
            // gbWork
            // 
            gbWork.Controls.Add(chkHasWork);
            gbWork.Controls.Add(lblCompany);
            gbWork.Controls.Add(tbCompany);
            gbWork.Controls.Add(lblPosition);
            gbWork.Controls.Add(tbPosition);
            gbWork.Controls.Add(lblExperience);
            gbWork.Controls.Add(numExperience);
            gbWork.Location = new Point(13, 646);
            gbWork.Margin = new Padding(4, 5, 4, 5);
            gbWork.Name = "gbWork";
            gbWork.Padding = new Padding(4, 5, 4, 5);
            gbWork.Size = new Size(667, 185);
            gbWork.TabIndex = 16;
            gbWork.TabStop = false;
            gbWork.Text = "Место работы";
            // 
            // chkHasWork
            // 
            chkHasWork.AutoSize = true;
            chkHasWork.Location = new Point(13, 31);
            chkHasWork.Margin = new Padding(4, 5, 4, 5);
            chkHasWork.Name = "chkHasWork";
            chkHasWork.Size = new Size(93, 24);
            chkHasWork.TabIndex = 0;
            chkHasWork.Text = "Работает";
            chkHasWork.UseVisualStyleBackColor = true;
            chkHasWork.CheckedChanged += chkHasWork_CheckedChanged;
            // 
            // lblCompany
            // 
            lblCompany.AutoSize = true;
            lblCompany.Location = new Point(13, 77);
            lblCompany.Margin = new Padding(4, 0, 4, 0);
            lblCompany.Name = "lblCompany";
            lblCompany.Size = new Size(84, 20);
            lblCompany.TabIndex = 1;
            lblCompany.Text = "Компания:";
            // 
            // tbCompany
            // 
            tbCompany.Enabled = false;
            tbCompany.Location = new Point(133, 72);
            tbCompany.Margin = new Padding(4, 5, 4, 5);
            tbCompany.Name = "tbCompany";
            tbCompany.Size = new Size(199, 27);
            tbCompany.TabIndex = 2;
            // 
            // lblPosition
            // 
            lblPosition.AutoSize = true;
            lblPosition.Location = new Point(347, 77);
            lblPosition.Margin = new Padding(4, 0, 4, 0);
            lblPosition.Name = "lblPosition";
            lblPosition.Size = new Size(89, 20);
            lblPosition.TabIndex = 3;
            lblPosition.Text = "Должность:";
            // 
            // tbPosition
            // 
            tbPosition.Enabled = false;
            tbPosition.Location = new Point(467, 72);
            tbPosition.Margin = new Padding(4, 5, 4, 5);
            tbPosition.Name = "tbPosition";
            tbPosition.Size = new Size(132, 27);
            tbPosition.TabIndex = 4;
            // 
            // lblExperience
            // 
            lblExperience.AutoSize = true;
            lblExperience.Location = new Point(13, 123);
            lblExperience.Margin = new Padding(4, 0, 4, 0);
            lblExperience.Name = "lblExperience";
            lblExperience.Size = new Size(82, 20);
            lblExperience.TabIndex = 5;
            lblExperience.Text = "Стаж (лет):";
            // 
            // numExperience
            // 
            numExperience.Enabled = false;
            numExperience.Location = new Point(133, 120);
            numExperience.Margin = new Padding(4, 5, 4, 5);
            numExperience.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            numExperience.Name = "numExperience";
            numExperience.Size = new Size(107, 27);
            numExperience.TabIndex = 6;
            // 
            // trackBarStress
            // 
            trackBarStress.Location = new Point(160, 854);
            trackBarStress.Margin = new Padding(4, 5, 4, 5);
            trackBarStress.Name = "trackBarStress";
            trackBarStress.Size = new Size(200, 56);
            trackBarStress.TabIndex = 18;
            trackBarStress.Value = 5;
            trackBarStress.ValueChanged += trackBarStress_ValueChanged;
            // 
            // lblStressValue
            // 
            lblStressValue.AutoSize = true;
            lblStressValue.Location = new Point(373, 862);
            lblStressValue.Margin = new Padding(4, 0, 4, 0);
            lblStressValue.Name = "lblStressValue";
            lblStressValue.Size = new Size(17, 20);
            lblStressValue.TabIndex = 19;
            lblStressValue.Text = "5";
            // 
            // btnCalculateBudget
            // 
            btnCalculateBudget.Location = new Point(13, 938);
            btnCalculateBudget.Margin = new Padding(4, 5, 4, 5);
            btnCalculateBudget.Name = "btnCalculateBudget";
            btnCalculateBudget.Size = new Size(200, 35);
            btnCalculateBudget.TabIndex = 20;
            btnCalculateBudget.Text = "Рассчитать бюджет";
            btnCalculateBudget.UseVisualStyleBackColor = true;
            btnCalculateBudget.Click += btnCalculateBudget_Click;
            // 
            // lblBudgetResult
            // 
            lblBudgetResult.AutoSize = true;
            lblBudgetResult.Location = new Point(227, 946);
            lblBudgetResult.Margin = new Padding(4, 0, 4, 0);
            lblBudgetResult.Name = "lblBudgetResult";
            lblBudgetResult.Size = new Size(70, 20);
            lblBudgetResult.TabIndex = 21;
            lblBudgetResult.Text = "Бюджет: ";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(13, 1000);
            btnSave.Margin = new Padding(4, 5, 4, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(133, 35);
            btnSave.TabIndex = 22;
            btnSave.Text = "Сохранить";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(160, 1000);
            btnLoad.Margin = new Padding(4, 5, 4, 5);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(133, 35);
            btnLoad.TabIndex = 23;
            btnLoad.Text = "Загрузить";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // panel
            // 
            panel.AutoScroll = true;
            panel.Controls.Add(lblFIO);
            panel.Controls.Add(tbFIO);
            panel.Controls.Add(lblAge);
            panel.Controls.Add(numAge);
            panel.Controls.Add(lblSpeciality);
            panel.Controls.Add(cbSpeciality);
            panel.Controls.Add(lblBirth);
            panel.Controls.Add(dtpBirthDate);
            panel.Controls.Add(lblCourse);
            panel.Controls.Add(numCourse);
            panel.Controls.Add(lblGroup);
            panel.Controls.Add(tbGroup);
            panel.Controls.Add(lblAvgScore);
            panel.Controls.Add(numAvgScore);
            panel.Controls.Add(gbGender);
            panel.Controls.Add(gbAddress);
            panel.Controls.Add(gbWork);
            panel.Controls.Add(lblStress);
            panel.Controls.Add(trackBarStress);
            panel.Controls.Add(lblStressValue);
            panel.Controls.Add(btnCalculateBudget);
            panel.Controls.Add(lblBudgetResult);
            panel.Controls.Add(btnSave);
            panel.Controls.Add(btnLoad);
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(0, 0);
            panel.Margin = new Padding(4, 5, 4, 5);
            panel.Name = "panel";
            panel.Size = new Size(733, 1055);
            panel.TabIndex = 0;
            // 
            // lblFIO
            // 
            lblFIO.AutoSize = true;
            lblFIO.Location = new Point(13, 15);
            lblFIO.Margin = new Padding(4, 0, 4, 0);
            lblFIO.Name = "lblFIO";
            lblFIO.Size = new Size(45, 20);
            lblFIO.TabIndex = 0;
            lblFIO.Text = "ФИО:";
            // 
            // lblAge
            // 
            lblAge.AutoSize = true;
            lblAge.Location = new Point(13, 62);
            lblAge.Margin = new Padding(4, 0, 4, 0);
            lblAge.Name = "lblAge";
            lblAge.Size = new Size(67, 20);
            lblAge.TabIndex = 2;
            lblAge.Text = "Возраст:";
            // 
            // lblSpeciality
            // 
            lblSpeciality.AutoSize = true;
            lblSpeciality.Location = new Point(13, 108);
            lblSpeciality.Margin = new Padding(4, 0, 4, 0);
            lblSpeciality.Name = "lblSpeciality";
            lblSpeciality.Size = new Size(119, 20);
            lblSpeciality.TabIndex = 4;
            lblSpeciality.Text = "Специальность:";
            // 
            // lblBirth
            // 
            lblBirth.AutoSize = true;
            lblBirth.Location = new Point(13, 154);
            lblBirth.Margin = new Padding(4, 0, 4, 0);
            lblBirth.Name = "lblBirth";
            lblBirth.Size = new Size(119, 20);
            lblBirth.TabIndex = 6;
            lblBirth.Text = "Дата рождения:";
            // 
            // lblCourse
            // 
            lblCourse.AutoSize = true;
            lblCourse.Location = new Point(13, 200);
            lblCourse.Margin = new Padding(4, 0, 4, 0);
            lblCourse.Name = "lblCourse";
            lblCourse.Size = new Size(44, 20);
            lblCourse.TabIndex = 8;
            lblCourse.Text = "Курс:";
            // 
            // lblGroup
            // 
            lblGroup.AutoSize = true;
            lblGroup.Location = new Point(13, 246);
            lblGroup.Margin = new Padding(4, 0, 4, 0);
            lblGroup.Name = "lblGroup";
            lblGroup.Size = new Size(61, 20);
            lblGroup.TabIndex = 10;
            lblGroup.Text = "Группа:";
            // 
            // lblAvgScore
            // 
            lblAvgScore.AutoSize = true;
            lblAvgScore.Location = new Point(13, 292);
            lblAvgScore.Margin = new Padding(4, 0, 4, 0);
            lblAvgScore.Name = "lblAvgScore";
            lblAvgScore.Size = new Size(110, 20);
            lblAvgScore.TabIndex = 12;
            lblAvgScore.Text = "Средний балл:";
            // 
            // lblStress
            // 
            lblStress.AutoSize = true;
            lblStress.Location = new Point(13, 862);
            lblStress.Margin = new Padding(4, 0, 4, 0);
            lblStress.Name = "lblStress";
            lblStress.Size = new Size(128, 20);
            lblStress.TabIndex = 17;
            lblStress.Text = "Уровень стресса:";
            // 
            // errorProvider
            // 
            errorProvider.ContainerControl = this;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(733, 1055);
            Controls.Add(panel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Карточка студента";
            ((System.ComponentModel.ISupportInitialize)numAge).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCourse).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAvgScore).EndInit();
            gbGender.ResumeLayout(false);
            gbGender.PerformLayout();
            gbAddress.ResumeLayout(false);
            gbAddress.PerformLayout();
            gbWork.ResumeLayout(false);
            gbWork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numExperience).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarStress).EndInit();
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)errorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // Элементы управления
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label lblFIO;
        private System.Windows.Forms.TextBox tbFIO;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.NumericUpDown numAge;
        private System.Windows.Forms.Label lblSpeciality;
        private System.Windows.Forms.ComboBox cbSpeciality;
        private System.Windows.Forms.Label lblBirth;
        private System.Windows.Forms.DateTimePicker dtpBirthDate;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.NumericUpDown numCourse;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.TextBox tbGroup;
        private System.Windows.Forms.Label lblAvgScore;
        private System.Windows.Forms.NumericUpDown numAvgScore;
        private System.Windows.Forms.GroupBox gbGender;
        private System.Windows.Forms.RadioButton rbMale;
        private System.Windows.Forms.RadioButton rbFemale;
        private System.Windows.Forms.GroupBox gbAddress;
        private System.Windows.Forms.Label lblCity;
        private System.Windows.Forms.TextBox tbCity;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.TextBox tbIndex;
        private System.Windows.Forms.Label lblStreet;
        private System.Windows.Forms.TextBox tbStreet;
        private System.Windows.Forms.Label lblHouse;
        private System.Windows.Forms.TextBox tbHouse;
        private System.Windows.Forms.Label lblApartment;
        private System.Windows.Forms.TextBox tbApartment;
        private System.Windows.Forms.GroupBox gbWork;
        private System.Windows.Forms.CheckBox chkHasWork;
        private System.Windows.Forms.Label lblCompany;
        private System.Windows.Forms.TextBox tbCompany;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.TextBox tbPosition;
        private System.Windows.Forms.Label lblExperience;
        private System.Windows.Forms.NumericUpDown numExperience;
        private System.Windows.Forms.Label lblStress;
        private System.Windows.Forms.TrackBar trackBarStress;
        private System.Windows.Forms.Label lblStressValue;
        private System.Windows.Forms.Button btnCalculateBudget;
        private System.Windows.Forms.Label lblBudgetResult;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private ErrorProvider errorProvider;
    }
}