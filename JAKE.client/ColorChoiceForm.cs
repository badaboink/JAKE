using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JAKE.client
{
    public partial class ColorChoiceForm : Form
    {
        // Property to store the selected color
        public string SelectedColor { get; private set; }

        public ColorChoiceForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedColor = "green";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectedColor = "blue";
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectedColor = "red";
            this.Close();
        }

    }
}
