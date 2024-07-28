namespace DocEditor.Services
{
    public class EditionService
    {
        /// <summary>
        /// Сервис для модификации формы изменения, инициализации парсера и сохранения созданного документа
        /// </summary>
        /// <param name="sectionsCount">Кол-во секций текста</param>
        /// <param name="tableUsed">Наличие таблицы</param>
        /// <param name="editionForm"></param>
        /// <exception cref="ArgumentException">Выбрасывается при создании документа без секций и таблицы, т.е. заведомо пустого документа</exception>
        public EditionService(int sectionsCount, bool tableUsed, EditionForm editionForm)
        {
            SectionsCount = sectionsCount;
            TableUsed = tableUsed;
            _EditionForm = editionForm;
            if (sectionsCount == 0 && !tableUsed)
                throw new ArgumentException("Смысл создавать документ, если в нем ничего не будет?");
            EditionFormStartSize = _EditionForm.Size;
            ResizeWindow();
            AddControls();  
        }
        int SectionsCount;
        int MonitorHeight = 0;
        int WindowHeight = 0;
        int WindowWidth = 500;
        int SectionHeight = 100;
        int SectionWidth = 400;
        int DelimiterSize = 10;//Пробелы между элементами управления
        int ButtonHeight = 23;
        int ButtonWidth = 85;
        int TableHeight = 200;
        int TableWidth = 450;
        int CheckBoxHeight = 19;
        int CheckBoxWidth = 35;
        int RemoveButtonSize = 20;//Размер квадратной кнопки с крестиком
        int AttachedFileCoordY = 0;//Координата У для размещения информации о прикрепленном файле
        bool TableUsed;
        EditionForm _EditionForm;
        Size EditionFormStartSize;//Размеры формы редактирования перед модификацией
        List<Control> Controls = new List<Control>();//Создаваемые в сервисе элементы управления
        List<string> AttachedFiles = new List<string>();
        /// <summary>
        /// Изменяет размер формы во время модификации
        /// </summary>
        void ResizeWindow()
        {            
            WindowHeight = DelimiterSize * (3 + SectionsCount) + (TableUsed ? DelimiterSize + TableHeight : 0) + SectionsCount * SectionHeight + ButtonHeight * 2 + 40;
            MonitorHeight = Screen.PrimaryScreen.Bounds.Height;
            if (WindowHeight > MonitorHeight)
            {
                WindowHeight = MonitorHeight;
                _EditionForm.AutoScroll = true;
            }
            _EditionForm.Size = new Size(WindowWidth, WindowHeight);
        }
        /// <summary>
        /// Добавляет элементы управления во время модификации
        /// </summary>
        void AddControls()
        {
            int yCoord = DelimiterSize;
            for (int sectionIndex = 0; sectionIndex < SectionsCount; sectionIndex++, yCoord += DelimiterSize + SectionHeight)
            {
                RichTextBox richTextBox = new RichTextBox()
                {
                    Location = new Point(DelimiterSize, yCoord),
                    Size = new Size(SectionWidth, SectionHeight),
                    Parent = _EditionForm,
                    Cursor = Cursors.IBeam,
                    Visible = true,       
                    Name = "richTextBox" + sectionIndex.ToString()
                };
                Controls.Add(richTextBox);
                AddCheckBoxes(yCoord, sectionIndex);
            }
            if (TableUsed)
            {
                DataGridView dataGridView = new DataGridView()
                {
                    Location = new Point(DelimiterSize, yCoord),
                    Size = new Size(TableWidth, TableHeight),
                    Parent = _EditionForm,
                    Cursor = Cursors.Arrow,
                    Visible = true,
                    ColumnCount = 1,
                    ColumnHeadersVisible = false,
                    RowHeadersVisible = false, 
                    Name = "dataGridView"
                };
                dataGridView.CellValueChanged += dataGridViev_CellValueChanged;
                yCoord += DelimiterSize + TableHeight;
                Controls.Add(dataGridView);

            }
            Button backButton = new Button()
            {
                Location = new Point(DelimiterSize, yCoord),
                Size = new Size(ButtonWidth, ButtonHeight),
                Parent = _EditionForm,
                Cursor = Cursors.Hand,
                Visible = true,
                Text = "Назад"
            };
            backButton.Click += backButton_Click;
            Controls.Add(backButton);
            Button saveButton = new Button()
            {
                Location = new Point(DelimiterSize * 2 + ButtonWidth, yCoord),
                Size = new Size(ButtonWidth, ButtonHeight),
                Parent = _EditionForm,
                Cursor = Cursors.Hand,
                Visible = true,
                Text = "Сохранить"
            };
            saveButton.Click += saveButton_Click;
            Controls.Add(saveButton);
            yCoord += DelimiterSize + ButtonHeight;
            Button attachButton = new Button()
            {
                Location = new Point(DelimiterSize, yCoord),
                Size = new Size(ButtonWidth*2 + DelimiterSize, ButtonHeight),
                Parent = _EditionForm,
                Cursor = Cursors.Hand,
                Visible = true,
                Text = "Прикрепить файл"
            };
            attachButton.Click += attachButton_Click;
            AttachedFileCoordY = yCoord + ButtonHeight + DelimiterSize;
            Controls.Add(attachButton);
        }
        /// <summary>
        /// Добавляет чекбоксы "Ж", "К", "Ч" рядом с секцией
        /// </summary>
        /// <param name="yCoord">Координата У секции</param>
        /// <param name="sectionIndex">Индекс секции</param>
        void AddCheckBoxes(int yCoord, int sectionIndex)
        {
            CheckBox checkBoxJ = new CheckBox()
            {
                Location = new Point(2 * DelimiterSize + SectionWidth, yCoord),
                Size = new Size(CheckBoxWidth, CheckBoxHeight),
                Parent = _EditionForm,
                Cursor = Cursors.Hand,
                Visible = true,
                Font = new Font("Seggoe UI", 9F, FontStyle.Bold),
                Text = "Ж",
                Name = "checkBox" + sectionIndex.ToString() + "J"
            };
            Controls.Add(checkBoxJ);
            CheckBox checkBoxK = new CheckBox()
            {
                Location = new Point(2 * DelimiterSize + SectionWidth, yCoord + DelimiterSize + CheckBoxHeight),
                Size = new Size(CheckBoxWidth, CheckBoxHeight),
                Parent = _EditionForm,
                Cursor = Cursors.Hand,
                Visible = true,
                Font = new Font("Seggoe UI", 9F, FontStyle.Italic),
                Text = "К",
                Name = "checkBox" + sectionIndex.ToString() + "K"
            };
            Controls.Add(checkBoxK);
            CheckBox checkBoxCh = new CheckBox()
            {
                Location = new Point(2 * DelimiterSize + SectionWidth, yCoord + 2 * (DelimiterSize + CheckBoxHeight)),
                Size = new Size(CheckBoxWidth, CheckBoxHeight),
                Parent = _EditionForm,
                Cursor = Cursors.Hand,
                Visible = true,
                Font = new Font("Seggoe UI", 9F, FontStyle.Underline),
                Text = "Ч",
                Name = "checkBox" + sectionIndex.ToString() + "Ch"
            };
            Controls.Add(checkBoxCh);
        }
        /// <summary>
        /// Добавляет кнопку удаления и путь к файлу на форму
        /// </summary>
        /// <param name="filepath">Путь к файлу</param>
        void AddAttachedFileControls(string filepath)
        {
            if (!_EditionForm.AutoScroll)
            {
                if (WindowHeight + RemoveButtonSize + DelimiterSize >= MonitorHeight)
                {
                    _EditionForm.Size = new Size(WindowWidth, MonitorHeight);
                    _EditionForm.AutoScroll = true;
                }
                else
                {
                    WindowHeight += RemoveButtonSize + DelimiterSize;
                    _EditionForm.Size = new Size(WindowWidth, WindowHeight);
                }
            }
            Guid fileGuid = Guid.NewGuid();//Guid, связывающий кнопку с меткой в имени 
            Button button = new Button()
            {
                Parent = _EditionForm,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleRight,
                Text = "×",
                Size = new Size(RemoveButtonSize, RemoveButtonSize),
                Location = new Point( DelimiterSize, AttachedFileCoordY),
                Name = "removeFileButton: " + fileGuid,
                Visible = true,
            };
            button.FlatAppearance.MouseOverBackColor = Color.Red;
            button.FlatAppearance.MouseDownBackColor = Color.Maroon;
            button.FlatAppearance.BorderColor = SystemColors.Control;
            button.FlatAppearance.BorderSize = 0;
            button.Click += removeFileButton_Click;
            Controls.Add(button);
            Label label = new Label()
            {
                Parent = _EditionForm,
                Font = new Font("Segoe UI", 7F),
                Text = filepath,
                AutoSize = true,
                Location = new Point(DelimiterSize + RemoveButtonSize, AttachedFileCoordY),
                Name = "fileLabel: " + fileGuid,
                Visible = true
            };
            AttachedFileCoordY += DelimiterSize + RemoveButtonSize;
            Controls.Add(label);
        }
        void removeFileButton_Click(object sender, EventArgs e)//Клик по кнопке для открепления файла
        {
            Button button = (Button)sender;
            Guid fileGuid = Guid.Parse(button.Name.Substring(18));
            int coordY = (int)button.Location.Y;
            Label label = (Label)Controls.FirstOrDefault(c => c.Name.Contains(fileGuid.ToString()) && c.Name.Contains("fileLabel"));
            string filename = label.Text;
            List<Control> movingControls = Controls.Where(c => c.Location.Y>coordY).ToList();
            for (int i = 0; i < movingControls.Count; i++)
            {
                Point location = movingControls[i].Location;
                location.Y-= DelimiterSize + RemoveButtonSize;
                movingControls[i].Location = location;
            }
            label.Visible = false;
            button.Visible = false;
            for (int i = 0; i < AttachedFiles.Count; i++)
            {
                if (AttachedFiles[i].Equals(filename))
                {
                    AttachedFiles.RemoveAt(i);
                    break;
                }
            }
            if (!_EditionForm.AutoScroll)
            {
                Size size = _EditionForm.Size;
                size.Height -= DelimiterSize + RemoveButtonSize;
                _EditionForm.Size = size;
            }
        }
        void backButton_Click(object sender, EventArgs e)//Кнопка "Назад"
        {
            _EditionForm.label1.Show();
            _EditionForm.label2.Show();
            _EditionForm.trackBar1.Show();
            _EditionForm.checkBox1.Show();
            _EditionForm.button1.Show();
            _EditionForm.Size = EditionFormStartSize;
            _EditionForm.AutoScroll = false;
            foreach (Control control in Controls)
            {
                control.Hide();
            }
        }
        void attachButton_Click(object sender, EventArgs e)//Кнопка "Прикрепить файл"
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Documents|*.doc;*.docx";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog.FileName;
            AttachedFiles.Add(filename);
            AddAttachedFileControls(filename);
        }
        void saveButton_Click(object sender, EventArgs e)//Кнопка "Сохранить"
        {
            ParsingService parsingService = new ParsingService(Controls, AttachedFiles);
            parsingService.Parse();
            _EditionForm.Close();
        }
        void dataGridViev_CellValueChanged(object sender, DataGridViewCellEventArgs e)//Обработчик события изменения ячейки таблицы. По мере необходимости увеличивает число столбцов
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (dataGridView.ColumnCount <= e.ColumnIndex + 1)
            {
                dataGridView.ColumnCount++;
            }
        }
    }
}
