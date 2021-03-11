using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Udp
{
    class Program
    {
        static void Main(string[] args)
        {
            int runTime = 10000;
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
            byte[] bytesToRead = Encoding.ASCII.GetBytes(input);

            IPEndPoint recieveFrom = new IPEndPoint(IPAddress.Loopback, 5000);
            UdpClient client = new UdpClient();
            client.Connect(recieveFrom);
            client.Send(bytesToRead, bytesToRead.Length);
            long count = 0;
            Stopwatch stopwatch = new Stopwatch();
            Task.Run(() =>
            {
                stopwatch.Start();
                while (true)
                {
                    bytesToRead = client.Receive(ref recieveFrom);
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
