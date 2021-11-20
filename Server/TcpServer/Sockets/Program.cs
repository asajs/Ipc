using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static TcpClient client;

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
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            TcpListener listener = new TcpListener(localAdd, PORT_NO);
            listener.Start();

            //---incoming client connected---

            //---get the incoming data through a network stream---

            //---write back the text to the client---
            byte[] buffer = Encoding.ASCII.GetBytes(input);
            client = listener.AcceptTcpClient();
            Task.Run(() =>
            {
                NetworkStream nwStream = client.GetStream();
                while (true)
                {
                    nwStream.Write(buffer, 0, buffer.Length);
                }
            });
            Thread.Sleep(runTime);
            client.Close();
            listener.Stop();
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