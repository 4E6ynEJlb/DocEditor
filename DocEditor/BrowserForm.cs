using Microsoft.Web.WebView2.WinForms;
using System.Xml.Linq;
using System.Drawing.Printing;
using Spire.Doc;
namespace DocEditor
{
    public partial class BrowserForm : Form //Форма отображения документа
    {
        public BrowserForm(string filepath, StartForm startForm)
        {
            InitializeComponent();
            Filepath = filepath;
            Size = Screen.PrimaryScreen.Bounds.Size;
            Browser = new WebView2()
            {
                Parent = this,
                Location = new Point(0, 0),
                Size = new Size(Size.Width, Size.Height - 40),
                Visible = true,
                Source = new Uri(Filepath),
            };
            _StartForm = startForm;
            startForm.Hide();
            CreateButtons();
            int slashIndex = filepath.LastIndexOf("\\");
            _Label = new Label()
            {
                Parent = Browser,
                Location = new Point(0, 0),
                AutoSize = true,
                Visible = true,
                Text = filepath.Substring(slashIndex + 1)
            };
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }
        string Filepath;
        WebView2 Browser;
        Label _Label;
        StartForm _StartForm;
        Button BackButton;
        Button PrintButton;
        XNamespace _XNameSpace = "http://www.w3.org/1999/xhtml";
        /// <summary>
        /// Создает кнопки на форме
        /// </summary>
        void CreateButtons()
        {
            BackButton = new Button()
            {
                Location = new Point(10, Size.Height - 30),
                Size = new Size(85, 20),
                Parent = this,
                Cursor = Cursors.Hand,
                Visible = true,
                Text = "Назад"
            };
            BackButton.Click += BackButton_Click;
            PrintButton = new Button()
            {
                Location = new Point(105, Size.Height - 30),
                Size = new Size(85, 20),
                Parent = this,
                Cursor = Cursors.Hand,
                Visible = true,
                Text = "Печать"
            };
            PrintButton.Click += PrintButton_Click;
        }
        private void BackButton_Click(object sender, EventArgs e)//Кнопка "Назад"
        {
            Close();
        }
        private void PrintButton_Click(object sender, EventArgs e)//Кнопка "Печать"
        {
            Browser.CoreWebView2.ShowPrintUI();
            PrintApplications();
        }
        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _StartForm.Show();
        }
        void PrintApplications()//Печатает приложения
        {
            XDocument xDocument = XDocument.Load(Filepath);
            if (xDocument.Element(_XNameSpace + "htmlx").Element(_XNameSpace + "body2").Element(_XNameSpace + "container").Element(_XNameSpace + "applications").Elements(_XNameSpace + "application").Count() == 0)
                return;
            var dialogResult = MessageBox.Show(this, "Печать приложений необходима?", "Печать приложений", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;
            foreach (XElement element in xDocument.Element(_XNameSpace + "htmlx").Element(_XNameSpace + "body2").Element(_XNameSpace + "container").Element(_XNameSpace + "applications").Elements(_XNameSpace + "application"))
            {
                XElement application = element.Element(_XNameSpace + "a");
                XAttribute href = application.Attribute("href");
                string hrefValue = href.Value;
                string base64String = hrefValue.Replace("data:application/msword;base64,", "");
                byte[] bytes = Convert.FromBase64String(base64String);
                MemoryStream memoryStream = new MemoryStream(bytes);
                Document document = new Document(memoryStream);
                PrintDialog dialog = new PrintDialog();
                dialog.AllowPrintToFile = true;
                dialog.AllowCurrentPage = true;
                dialog.AllowSomePages = true;
                dialog.UseEXDialog = true;
                dialog.Document = document.PrintDocument;
                PrintDocument printDoc = document.PrintDocument;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        printDoc.Print();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }
    }
}
