using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
	class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static void Main(string[] args)
        {
            int runTime = 0;
            string input = "";
            if(args.Length != 0)
			{
                if(File.Exists(args[0]))
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


            TcpClient client = new TcpClient(SERVER_IP, PORT_NO);

            //---read back the text---
            long count = 0;
            Stopwatch stopwatch = new Stopwatch();
            NetworkStream nwStream = client.GetStream();
            Task.Run(() =>
            {
                stopwatch.Start();
                while (true)
                {
                    nwStream.Read(bytesToRead, 0, bytesToRead.Length);
                    nwStream.Flush();
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
            nwStream.Dispose();
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
