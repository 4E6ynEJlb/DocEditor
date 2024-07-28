using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Awesomium.Windows.Forms;
namespace DocEditor.Services
{
    public class EditionService
    {
        public EditionService(int sectionsCount, bool tableUsed, EditionForm editionForm)
        {
            SectionsCount = sectionsCount;
            TableUsed = tableUsed;
            _EditionForm = editionForm;
            if (sectionsCount == 0 && !tableUsed)
                throw new Exception("Смысл создавать документ, если в нем ничего не будет?");
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
        int DelimiterSize = 10;
        int ButtonHeight = 23;
        int ButtonWidth = 85;
        int TableHeight = 200;
        int TableWidth = 450;
        int CheckBoxHeight = 19;
        int CheckBoxWidth = 35;
        int RemoveButtonSize = 20;
        int FilesAttached = 0;
        int AttachedFileCoordY = 0;
        bool TableUsed;
        bool WindowHeightMax = false;
        EditionForm _EditionForm;
        Size EditionFormStartSize;
        List<Control> Controls = new List<Control>();
        List<string> AttachedFiles = new List<string>();
        void ResizeWindow()
        {            
            WindowHeight = DelimiterSize * (3 + SectionsCount) + (TableUsed ? DelimiterSize + TableHeight : 0) + SectionsCount * SectionHeight + ButtonHeight * 2 + 40;
            MonitorHeight = Screen.PrimaryScreen.Bounds.Height;
            if (WindowHeight > MonitorHeight)
            {
                WindowHeight = MonitorHeight;
                _EditionForm.AutoScroll = true;
                WindowHeightMax = true;
            }
            _EditionForm.Size = new Size(WindowWidth, WindowHeight);
        }
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
        void AddAttachedFileControls(string filename)
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
            Guid fileGuid = Guid.NewGuid();
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
                Text = filename,
                AutoSize = true,
                Location = new Point(DelimiterSize + RemoveButtonSize, AttachedFileCoordY),
                Name = "fileLabel: " + fileGuid,
                Visible = true
            };
            AttachedFileCoordY += DelimiterSize + RemoveButtonSize;
            Controls.Add(label);
        }
        void removeFileButton_Click(object sender, EventArgs e)
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
        void backButton_Click(object sender, EventArgs e)
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
        void attachButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Documents|*.doc;*.docx";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog.FileName;
            AttachedFiles.Add(filename);
            AddAttachedFileControls(filename);
        }
        void saveButton_Click(object sender, EventArgs e)
        {
            ParsingService parsingService = new ParsingService(Controls, AttachedFiles);
            parsingService.Parse();
            _EditionForm.Close();
        }
        void dataGridViev_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (dataGridView.ColumnCount <= e.ColumnIndex + 1)
            {
                dataGridView.ColumnCount++;
            }
        }
    }
}
