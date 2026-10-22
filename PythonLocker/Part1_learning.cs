using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace PythonLocker
{
    public partial class Part1_learning : Form
    {
        public int lesson_number;
        private Dictionary<string, string> texts;
        public Part1_learning(int number)
        {
            InitializeComponent();

            lesson_number = number;

            string json = File.ReadAllText(
                     @"C:\Users\Asus\source\repos\PythonLocker\PythonLocker\texts.json"
            );
            JsonDocument doc = JsonDocument.Parse(json);

            string lessonKey = "lesson_" + lesson_number;

            JsonElement lesson = doc.RootElement.GetProperty(lessonKey);

            texts = JsonSerializer.Deserialize<Dictionary<string, string>>(
                lesson.GetProperty("texts").GetRawText()
            ) ?? new Dictionary<string, string>();

            this.Text = lesson.GetProperty("title").GetString();

            label1.Text = texts["main_text_1"];
            label1.AutoSize = true;
        }
        private void Part1_learning_Shown(object sender, EventArgs e)
        {
            flowLayoutPanel1.AutoScrollPosition = new Point(0, 0);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(lesson_number);

            form1.FormClosed += (s, args) => this.Show();

            this.Hide();
            form1.Show();
        }
    }
}
