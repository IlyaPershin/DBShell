using System;
using System.Windows.Forms;

namespace Forms
{
    public partial class AddNewRowForm : Form
    {
        public AddNewRowForm()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}