using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientAsync
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static NetworkStream clientStream;
        static Stopwatch stopwatch;
        static long count;
        static byte[] bytesToRead;
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
            bytesToRead = Encoding.ASCII.GetBytes(input);

            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);

            //---read back the text---
            count = 0;
            stopwatch = new Stopwatch();
            clientStream = client.GetStream();
            bytesToRead = new byte[bytesToRead.Length];
            stopwatch.Start();
            BeginRead();
            Thread.Sleep(runTime);
            client.Dispose();
        }

        static void BeginRead()
		{
            object state = new object();
            clientStream.BeginRead(bytesToRead, 0, bytesToRead.Length, EndRead, state);
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

        static string GetInputString()
		{
            string path = @"C:\Users\Asa\source\repos\IPC\Data";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo file = directoryInfo.GetFiles().First();
            return File.ReadAllText(file.FullName);
        }
    }
}
