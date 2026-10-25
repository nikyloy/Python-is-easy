
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PythonLocker
{
    public partial class Form1 : Form
    {
        public int lesson_number = 0;

        private string expectedOutput = "";
        private const int ExecutionTimeoutMs = 5000;

        public Form1(int number)
        {
            InitializeComponent();

            lesson_number = number;

            LoadLesson();
        }

        private void LoadLesson()
        {
            string jsonPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Properties",
                "texts.json"
            );

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException(
                    "Файл texts.json не найден.",
                    jsonPath
                );
            }

            string json = File.ReadAllText(jsonPath);

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                string lessonKey = "lesson_" + lesson_number;

                JsonElement lesson = doc.RootElement
                    .GetProperty(lessonKey);

                this.Text =
                    lesson.GetProperty("title").GetString()
                    + " | Задания";

                Dictionary<string, string> texts =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(
                        lesson.GetProperty("texts").GetRawText()
                    ) ?? new Dictionary<string, string>();

                label1.Text = GetText(texts, "test_part_1");
                label3.Text = GetText(texts, "test_part_2");
                label4.Text = GetText(texts, "test_part_3");

                expectedOutput = GetText(texts, "test_part_4");

                label4.Visible = false;
            }
        }

        private string GetText(
            Dictionary<string, string> texts,
            string key)
        {
            string value;

            if (texts.TryGetValue(key, out value))
                return value ?? "";

            return "";
        }

        private string NormalizeOutput(string text)
        {
            return (text ?? "")
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .TrimEnd('\n', ' ', '\t');
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            label2.Text = "Выполнение программы...";
            label2.ForeColor = SystemColors.ControlText;

            string filePath = null;

            try
            {
                string folder = Path.Combine(
                    Path.GetTempPath(),
                    "PythonLocker"
                );

                Directory.CreateDirectory(folder);

                filePath = Path.Combine(
                    folder,
                    Guid.NewGuid().ToString("N") + ".py"
                );

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

                    if (!process.Start())
                    {
                        throw new InvalidOperationException(
                            "Не удалось запустить Python."
                        );
                    }

                    // Читаем оба потока одновременно,
                    // чтобы заполнение буфера не заблокировало процесс.
                    Task<string> outputTask =
                        process.StandardOutput.ReadToEndAsync();

                    Task<string> errorTask =
                        process.StandardError.ReadToEndAsync();

                    Task exitTask = Task.Run(
                        () => process.WaitForExit()
                    );

                    Task completed = await Task.WhenAny(
                        exitTask,
                        Task.Delay(ExecutionTimeoutMs)
                    );

                    if (completed != exitTask)
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {
                            // Процесс мог завершиться самостоятельно.
                        }

                        // Дожидаемся завершения процесса и чтения потоков.
                        await exitTask;
                        await outputTask;
                        await errorTask;

                        label2.Text =
                            "Превышено время выполнения (5 секунд).";

                        label2.ForeColor = Color.DarkOrange;
                        return;
                    }

                    string output = await outputTask;
                    string error = await errorTask;

                    if (process.ExitCode != 0)
                    {
                        label2.Text =
                            "В ПРОГРАММЕ ОШИБКА:\r\n" + error;

                        label2.ForeColor = Color.Red;
                        return;
                    }

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
            }
            catch (Exception ex)
            {
                label2.Text =
                    "Не удалось выполнить программу:\r\n" + ex.Message;

                label2.ForeColor = Color.Red;
            }
            finally
            {
                if (filePath != null)
                {
                    try
                    {
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    catch
                    {
                        // Ошибка очистки временного файла
                        // не должна ломать интерфейс.
                    }
                }

                button1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label4.Visible = !label4.Visible;
        }
    }
}
