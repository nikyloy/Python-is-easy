using System;
using System.Windows.Forms;

namespace PythonLocker
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LearningPart learningPart = new LearningPart();

            learningPart.FormClosed += (s, args) => this.Show();

            this.Hide();
            learningPart.Show();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
        }
    }
}
