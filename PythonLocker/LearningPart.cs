using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PythonLocker
{
    public partial class LearningPart : Form
    {
        public LearningPart()
        {
            InitializeComponent();
            this.Text = "Python_is_easy_program";
        }

        private void LearningPart_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Part1_learning part1_Learning = new Part1_learning(0);

            part1_Learning.FormClosed += (s, args) => this.Show();

            this.Hide();
            part1_Learning.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RulesOfUsage rulesOfUsage = new RulesOfUsage();
            rulesOfUsage.FormClosed += (s, args) => this.Show();
            this.Hide();
            rulesOfUsage.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Part1_learning part1_Learning = new Part1_learning(1);

            part1_Learning.FormClosed += (s, args) => this.Show();

            this.Hide();
            part1_Learning.Show();
        }
    }
}
