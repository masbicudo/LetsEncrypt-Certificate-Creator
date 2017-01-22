using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public partial class frmText : Form
    {
        public string Text
        {
            get { return this.textBox1.Text; }
            set { this.textBox1.Text = value; }
        }

        public frmText()
        {
            InitializeComponent();
        }
    }
}
