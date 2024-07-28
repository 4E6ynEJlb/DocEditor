using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DocEditor.Services
{
    public class ParsingService
    {
        public ParsingService(List<Control> controls, List<string> files) 
        {
            Files = files;
            Sections = controls.Where(c => c.Name.Contains("richTextBox")).OrderBy(s => s.Name).ToList();
            CheckBoxes = controls.Where(c => c.Name.Contains("checkBox")).ToList();
            Table = (DataGridView)controls.FirstOrDefault(c => c.Name.Equals("dataGridView"));
        }
        List<string> Files;
        List<Control> Sections;
        List<Control> CheckBoxes;
        DataGridView Table;
        XNamespace NameSpace = "http://www.w3.org/1999/xhtml";
        [XmlAttribute("xmlns", DataType = "language")]
        public string attrLang = "ru";
        public void Parse()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files(.xml)|*.xml|all Files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;            
            string filename = saveFileDialog.FileName;
            XDocument xDocument = new XDocument( 
                new XElement(NameSpace + "htmlx", new XAttribute("xmlns", NameSpace.NamespaceName), new XAttribute("lang", "ru"),
                    new XElement(NameSpace + "body2",
                        new XElement(NameSpace + "container", new XAttribute("id", "electronic-document"), new XAttribute("style", "display: none;"),
                            new XElement(NameSpace + "e-doc", "Электронный документ"),
                            ParseContent(),
                            ParseApplications(),
                            ParseStyle()
                        )
                    )
                )
            );
            xDocument.Save(filename);
            new Task(() => MessageBox.Show("Готово")).Start();
        }
        private XElement ParseContent()//content
        {
            var content = new List<XElement>();
            foreach ( var section in Sections ) 
                content.Add(ParseSection((RichTextBox)section));
            if (Table != null)
                content.Add(ParseTable());
            XElement xElement = new XElement(NameSpace + "content", content.ToArray());
            return xElement;
        }
        private XElement ParseSection(RichTextBox richTextBox)
        {
            int sectionIndex = int.Parse(richTextBox.Name.Substring(11));
            List<Control> checkBoxes = CheckBoxes.Where(c => c.Name.Contains(sectionIndex.ToString())).ToList();
            XElement xElement = new XElement(NameSpace + "p", new XAttribute("align", "center"), richTextBox.Text);
            foreach (Control c in checkBoxes)
            {
                if (((CheckBox)c).Checked)
                {
                    string tag = c.Name.Substring(9);
                    switch (tag) 
                    {
                        case "J":
                            xElement = new XElement(NameSpace + "b", xElement);
                            break;
                        case "K":
                            xElement = new XElement(NameSpace + "i", xElement);
                            break;
                        case "Ch":
                            xElement = new XElement(NameSpace + "u", xElement);
                            break;
                    }                    
                }
            }
            return xElement;
        }
        private XElement ParseTable()
        {
            List<XElement> rows = new List<XElement>();
            for (int rowIndex = 0; rowIndex < Table.RowCount; rowIndex++)
            {
                rows.Add(ParseRow(rowIndex));
            }
            return new XElement(NameSpace + "table", new XAttribute("border", 1), new XElement(NameSpace + "tbody", rows.ToArray()));
        }
        private XElement ParseRow(int rowIndex)
        {
            List<XElement> cells = new List<XElement>();
            for (int cell = 0; cell < Table.ColumnCount; cell++)
            {
                object value = Table[cell, rowIndex].Value;
                cells.Add(new XElement(NameSpace + "td", value == null ? "" : value.ToString()));
            }
            return new XElement(NameSpace + "tr", cells.ToArray());
        }
        private XElement ParseApplications()//applications
        {
            List<XElement> applications = new List<XElement>();
            foreach (string filename in Files)
            {
                applications.Add(ParseApplication(filename));
            }
            return new XElement(NameSpace + "applications", applications.ToArray());
        }
        private XElement ParseApplication(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            string fileBase64 = Convert.ToBase64String(bytes);
            int slashIndex = path.LastIndexOf("\\");
            string filename = path.Substring(slashIndex + 1);            
            return 
                new XElement(NameSpace + "application", 
                    new XElement(NameSpace + "a", new XAttribute("download", filename), new XAttribute("href", "data:application/msword;base64," + fileBase64), 
                        new XElement(NameSpace + "name", filename)));
        }
        private XElement ParseStyle()
        {
            string Workdir = Directory.GetCurrentDirectory();
            string style = File.ReadAllText(Workdir + "\\Style.txt");
            return new XElement(NameSpace + "style", style);
        }
    }
}
