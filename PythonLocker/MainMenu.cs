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
        private void button2_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.FormClosed += (s, args) => this.Show();
            this.Hide();
            settings.Show();
        }
        private void leave_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
