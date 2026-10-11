using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PythonLocker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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
                        label2.Text = string.IsNullOrWhiteSpace(output)
                            ? "Программа выполнена без вывода."
                            : output;
                    }
                    else
                    {
                        label2.Text = error;
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
    }
}
