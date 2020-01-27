using System;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConPty.Sample.ConsoleApi;

namespace ConPty.Sample.Terminal.Forms
{
    public partial class TerminalForm : Form
    {
        //private NativeConsole console;
        private ConsoleApi.Terminal terminal;
        private bool terminalIsExiting;

        public TerminalForm()
        {
            InitializeComponent();
            InitialiseTerminal();
        }

        private void InitialiseTerminal()
        {
            //console = new NativeConsole(false);
            terminal = new ConsoleApi.Terminal();
            terminal.Start(@"ping localhost", 126, 32);

            _ = Task.Run(CopyConsoleToWindow);
        }

        private void CopyConsoleToWindow()
        {
            var buffer = ArrayPool<char>.Shared.Rent(1024);

            try
            {
                using (StreamReader reader = new StreamReader(terminal.Output))
                using (StreamWriter fileWriter = new StreamWriter(File.Open("output.txt", FileMode.Create, FileAccess.Write)) { AutoFlush = true })
                {
                    while (true)
                    {
                        int readed = reader.Read(buffer, 0, buffer.Length);

                        if (readed == 0)
                        {
                            return;
                        }

                        fileWriter.Write(buffer, 0, readed);
                        OutputCharacters(buffer, readed);
                    }
                }
            }
            catch (ObjectDisposedException) { /* Pseudo terminal is terminated. */ }
            catch (Exception exception)
            {
                string message = Environment.NewLine + "Error: " + exception.Message;
                OutputCharacters(message.ToCharArray(), message.Length);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(buffer);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            terminalIsExiting = true;
            terminal?.Dispose();
            //console?.Dispose();
        }

        private void OutputCharacters(char[] buffer, int length)
        {
            if (terminalIsExiting)
            {
                return;
            }

            string text = new string(buffer.Take(length).ToArray());

            if (CheckForIllegalCrossThreadCalls)
            {
                Invoke(new Action(() => { tbOutput.Text += text; }));
            }
            else
            {
                tbOutput.Text += text;
            }
        }
    }
}
