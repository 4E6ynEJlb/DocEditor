using DocEditor.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocEditor
{
    public partial class EditionForm : Form
    {
        StartForm _StartForm;
        EditionService _EditionService;
        public EditionForm(StartForm startForm)
        {
            InitializeComponent();
            _StartForm = startForm;
            startForm.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
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
                new Task(() => MessageBox.Show(ex.Message, "смэрть", MessageBoxButtons.OK, MessageBoxIcon.Error)).Start();
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
