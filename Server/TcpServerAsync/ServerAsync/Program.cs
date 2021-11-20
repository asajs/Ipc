using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerAsync
{
    class Program
    {
        const int PORT_NO = 5000;
        const string SERVER_IP = "127.0.0.1";
        static NetworkStream serverStream;
        static byte[] bytesToSend;

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
            bytesToSend = Encoding.ASCII.GetBytes(input);
            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            TcpListener listener = new TcpListener(localAdd, PORT_NO);
            listener.Start();

            //---incoming client connected---

            //---get the incoming data through a network stream---

            //---write back the text to the client---
            TcpClient client = listener.AcceptTcpClient();
            serverStream = client.GetStream();
            BeginWrite();
            Thread.Sleep(runTime);
            client.Close();
            listener.Stop();
        }

        static void BeginWrite()
		{
            object state = new object();
            serverStream.BeginWrite(bytesToSend, 0, bytesToSend.Length, EndWrite, state);
		}

        static void EndWrite(IAsyncResult ar)
		{
            serverStream.EndWrite(ar);
            BeginWrite();
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