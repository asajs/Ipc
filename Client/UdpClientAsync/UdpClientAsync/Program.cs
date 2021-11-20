using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UdpClientAsync
{
    class Program
    {
        const int PORT_NO = 5000;
        static IPEndPoint receiveFrom;
        static UdpClient client;
        static Stopwatch stopwatch;
        static long count;
        static byte[] bytesToRead;
        static void Main(string[] args)
        {
            int runTime = 30000;
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
            receiveFrom = new IPEndPoint(IPAddress.Loopback, PORT_NO);
            client = new UdpClient();
            client.Connect(receiveFrom);
            client.Send(bytesToRead, bytesToRead.Length);
            count = 0;
            stopwatch = new Stopwatch();
            bytesToRead = new byte[bytesToRead.Length];
            stopwatch.Start();
            BeginRead();
            Thread.Sleep(runTime);
            client.Dispose();
        }

        static void BeginRead()
        {
            object state = new object();
            client.BeginReceive(EndRead, state);
        }

        static void EndRead(IAsyncResult ar)
        {
            client.EndReceive(ar, ref receiveFrom);
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
