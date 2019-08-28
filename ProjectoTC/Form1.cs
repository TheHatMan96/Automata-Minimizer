using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectoTC
{
    public partial class Form1 : Form
    {
        List<Node> states= new List<Node>();
        List<AllTransition> transition = new List<AllTransition>();

        public Form1()
        {
            InitializeComponent();
        }       

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "jff",
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName;                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileLogicAutomata fm = new FileLogicAutomata(textBox1.Text,textBox2.Text);
            fm.LoadFile();
            fm.RemoveLongState();
            fm.minimizerUpToStep();
        }
    }
}
