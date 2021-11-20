using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipeClient
{
	class Program
    {
        static void Main(string[] args)
        {
            int runTime = 0;
            string input = "";
            if (args.Length != 0)
            {
                if (File.Exists(args[0]))
                {
                    input = File.ReadAllText(args[0]);
                }
                int.TryParse(args[1], out runTime);
            }
            else
            {
                input = GetInputString();
            }

            //Client
            var client = new NamedPipeClientStream("PipesOfPiece");
            client.Connect();
            StreamReader reader = new StreamReader(client);
            Stopwatch stopwatch = new Stopwatch();
            long count = 0;
            char[] charsRead = new char[input.Length];
            Task.Run(() =>
            {
                stopwatch.Start();
                while (true)
                {
                    reader.Read(charsRead, 0, input.Length);
                    count++;
                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        Console.WriteLine($"{count}");
                        count = 0;
                        stopwatch.Restart();
                    }
                }
            });
            Thread.Sleep(runTime);
            reader.Dispose();
            client.Dispose();
        }

        static string GetInputString()
        {
            string path = @"C:\Users\Asa\source\repos\IPC\Data";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo file = directoryInfo.GetFiles().First();
            return File.ReadAllText(file.FullName);
        }
    }
}