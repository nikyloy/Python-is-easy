using System;
using System.Windows.Forms;

namespace PythonLocker
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            this.Name = "Settings";
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                MessageBox.Show("Настоящие программисты не используют светлой темы,\n\n потому что они работают много и глазам больно видеть \n\n светлый экран.");
                checkBox1.Checked = false;
            }
        }
    }
}
