using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PythonLocker
{
    public partial class Form1 : Form
    {
        public int lesson_number = 0;

        private string expectedOutput = "";
        public Form1(int number)
        {
            lesson_number = number;

            InitializeComponent();

            string json = File.ReadAllText(
                @"C:\Users\Asus\source\repos\PythonLocker\PythonLocker\texts.json"
            );

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                string lessonKey = "lesson_" + lesson_number;

                JsonElement lesson = doc.RootElement
                    .GetProperty(lessonKey);

                Dictionary<string, string> texts =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(
                        lesson.GetProperty("texts").GetRawText()
                    ) ?? new Dictionary<string, string>();

                this.Text = lesson.GetProperty("title").GetString()
                    + " | Задания";

                label1.Text = texts["test_part_1"];
                this.expectedOutput = texts["test_part_4"];
                label3.Text = texts["test_part_2"];

                label4.Text = texts["test_part_3"];
                label4.Visible = false;
            }
        }
        private string NormalizeOutput(string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .TrimEnd('\n', ' ', '\t');
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            string folder = Path.Combine(
                Path.GetTempPath(),
                "PythonLocker"
            );

            Directory.CreateDirectory(folder);

            string filePath = Path.Combine(
                folder,
                Guid.NewGuid().ToString() + ".py"
            );

            try
            {
                File.WriteAllText(filePath, richTextBox1.Text);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "\"" + filePath + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    Task<string> outputTask =
                        process.StandardOutput.ReadToEndAsync();

                    Task<string> errorTask =
                        process.StandardError.ReadToEndAsync();

                    Task exitTask = Task.Run(() => process.WaitForExit());

                    Task completed = await Task.WhenAny(
                        exitTask,
                        Task.Delay(5000)
                    );

                    if (completed != exitTask)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                            // Процесс уже мог завершиться
                        }

                        label2.Text =
                            "Превышено время выполнения (5 секунд).";
                        return;
                    }

                    string output = await outputTask;
                    string error = await errorTask;

                    if (process.ExitCode == 0)
                    {
                        string actual = NormalizeOutput(output);
                        string expected = NormalizeOutput(expectedOutput);

                        if (actual == expected)
                        {
                            label2.Text =
                                "ЗАДАНИЕ ВЫПОЛНЕНО!\r\n\r\n" + output;
                            label2.ForeColor = Color.Green;
                        }
                        else
                        {
                            label2.Text =
                                "ПОКА НЕПРАВИЛЬНО.\r\n" +
                                "Проверь вывод программы и попробуй ещё раз.\r\n\r\n" +
                                "Твой результат:\r\n" + output;

                            label2.ForeColor = Color.DarkOrange;
                        }
                    }
                    else
                    {
                        label2.Text =
                            "В ПРОГРАММЕ ОШИБКА:\r\n" + error;

                        label2.ForeColor = Color.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                label2.Text =
                    "Не удалось запустить Python:\r\n" + ex.Message;
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        

        private void label2_Click(object sender, EventArgs e)
        {
        }
        int a = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            if(a == 0)
            {
                label4.Visible = true;
                a++;
            }
            else
            {

                label4.Visible = false;
                a--;
            }
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}