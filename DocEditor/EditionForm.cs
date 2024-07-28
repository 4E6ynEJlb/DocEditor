using DocEditor.Services;

namespace DocEditor
{
    public partial class EditionForm : Form //Форма редактирования документа
    {
        StartForm _StartForm;
        EditionService _EditionService;
        public EditionForm(StartForm startForm)
        {
            InitializeComponent();
            _StartForm = startForm;
            startForm.Hide();
        }

        private void button1_Click(object sender, EventArgs e) //Кнопка "Создать"
        {
            try
            {
                label1.Hide();
                label2.Hide();
                trackBar1.Hide();
                checkBox1.Hide();
                button1.Hide();
                _EditionService = new EditionService(trackBar1.Value, checkBox1.Checked, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label1.Show();
                label2.Show();
                trackBar1.Show();
                checkBox1.Show();
                button1.Show();
            }
        }

        private void CreationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _StartForm.Show();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label2.Text = trackBar1.Value.ToString();
        }
    }
}
