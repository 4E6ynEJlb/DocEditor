namespace DocEditor
{
    public partial class StartForm : Form//Начальная форма
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//Кнопка "Создание"
        {
            EditionForm editionForm = new EditionForm(this);
            editionForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)//Кнопка "Просмотр"
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog.FileName;
            BrowserForm browserForm = new BrowserForm(filename, this);
            browserForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)//Кнопка "Справка"
        {
            new Task(() => MessageBox.Show("В разработке", "Эта кнопка здесь для красоты", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)).Start();
        }
    }
}
