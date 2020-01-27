using System;
using System.IO;
using System.Threading.Tasks;

namespace ConPty.Sample.Terminal.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var terminal = new ConsoleApi.Terminal())
                {
                    terminal.Start(@"ping localhost", 126, 32);
                    _ = Task.Run(() => CopyPipeToOutput(terminal.Output));
                    terminal.WaitToExit();
                }
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.Message);
            }
        }

        private static void CopyPipeToOutput(Stream output)
        {
            try
            {
                char[] buffer = new char[1024];

                using (StreamReader reader = new StreamReader(output))
                using (StreamWriter writer = new StreamWriter(System.Console.OpenStandardOutput()) { AutoFlush = true })
                using (StreamWriter fileWriter = new StreamWriter(File.Open("output.txt", FileMode.Create, FileAccess.Write)) { AutoFlush = true })
                {
                    while (true)
                    {
                        int readed = reader.Read(buffer, 0, buffer.Length);

                        if (readed == 0)
                        {
                            return;
                        }

                        writer.Write(buffer, 0, readed);
                        fileWriter.Write(buffer, 0, readed);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                System.Console.WriteLine("Pseudo Console has been terminated.");
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.Message);
            }
        }
    }
}
