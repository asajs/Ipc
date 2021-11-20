using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace PipeClientAsync
{
	class Program
    {
        static byte[] buffer;
        static long count = 0;
        static Stopwatch stopwatch = new Stopwatch();
        static NamedPipeClientStream clientStream;

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
            buffer = Encoding.ASCII.GetBytes(input);

            //Client
            clientStream = new NamedPipeClientStream("PipesOfPiece");
            clientStream.Connect();
            
            stopwatch.Start();
            BeginRead();
            Thread.Sleep(runTime);
            clientStream.Dispose();
        }

        static void EndRead(IAsyncResult ar)
        {
            clientStream.EndRead(ar);
            count++;
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                Console.WriteLine($"{count}");
                count = 0;
                stopwatch.Restart();
            }
            BeginRead();
        }

        static void BeginRead()
        {
            object state = new object();
            byte[] byteToRead = new byte[buffer.Length];

            clientStream.BeginRead(byteToRead, 0, buffer.Length, EndRead, state);
        }

        static string GetInputString()
        {
            string path = @"C:\Users\Asa\source\repos\IPC";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo file = directoryInfo.GetFiles().First();
            return File.ReadAllText(file.FullName);
        }
    }
}
