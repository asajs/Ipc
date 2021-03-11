using System;
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
        static UdpClient client;

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

            IPEndPoint sendTo = new IPEndPoint(IPAddress.Any, 5000);
            client = new UdpClient(sendTo);
            client.Receive(ref sendTo);

            byte[] buffer = Encoding.ASCII.GetBytes(input);
            Task.Run(() =>
            {
                while (true)
                {
                    client.Send(buffer, buffer.Length, sendTo);
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