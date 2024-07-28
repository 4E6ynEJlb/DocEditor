using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Drawing.Printing;
using Spire.Doc;
using Spire.Doc.License;
namespace DocEditor
{
    public partial class BrowserForm : Form
    {
        public BrowserForm(string filename, StartForm startForm)
        {
            InitializeComponent();
            Filename = filename;
            Size = Screen.PrimaryScreen.Bounds.Size;
            Browser = new WebView2()
            {
                Parent = this,
                Location = new Point(0, 0),
                Size = new Size(Size.Width, Size.Height - 40),
                Visible = true,
                Source = new Uri(Filename),
            };
            _StartForm = startForm;
            startForm.Hide();
            CreateButtons();
            int slashIndex = filename.LastIndexOf("\\");
            _Label = new Label()
            {
                Parent = Browser,
                Location = new Point(0, 0),
                AutoSize = true,
                Visible = true,
                Text = filename.Substring(slashIndex + 1)
            };
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }
        string Filename;
        WebView2 Browser;
        Label _Label;
        StartForm _StartForm;
        Button BackButton;
        Button PrintButton;
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
        private void BackButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void PrintButton_Click(object sender, EventArgs e)
        {
            Browser.CoreWebView2.ShowPrintUI();
            PrintApplications();
        }
        private void BrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _StartForm.Show();
        }
        XNamespace NameSpace = "http://www.w3.org/1999/xhtml";
        void PrintApplications()
        {
            XDocument xDocument = XDocument.Load(Filename);
            if (xDocument.Element(NameSpace + "htmlx").Element(NameSpace + "body2").Element(NameSpace + "container").Element(NameSpace + "applications").Elements(NameSpace + "application").Count() == 0)
                return;
            var dialogResult = MessageBox.Show(this, "Печать приложения необходима?", "Печать приложения", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No)
                return;
            foreach (XElement element in xDocument.Element(NameSpace + "htmlx").Element(NameSpace + "body2").Element(NameSpace + "container").Element(NameSpace + "applications").Elements(NameSpace + "application"))
            {
                XElement application = element.Element(NameSpace + "a");
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
