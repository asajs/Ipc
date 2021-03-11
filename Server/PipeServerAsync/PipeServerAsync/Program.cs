using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace PipeServerAsync
{
    class Program
    {
        static string input;
        static byte[] buffer;
        static int runTime;
        static NamedPipeServerStream serverStream;
        static void Main(string[] args)
        {
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

            StartServer();
            Thread.Sleep(runTime);
            serverStream.Dispose();
        }

        static void StartServer()
        {
            serverStream = new NamedPipeServerStream("PipesOfPiece");
            serverStream.WaitForConnection();
            BeginWrite();
        }

        static void EndWrite(IAsyncResult ar)
        {
            serverStream.EndWrite(ar);
            BeginWrite();
        }

        static void BeginWrite()
        {
            object state = new object();
            serverStream.BeginWrite(buffer, 0, buffer.Length, EndWrite, state);
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