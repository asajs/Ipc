using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UdpServerAsync
{
	class Program
    {
        const int PORT_NO = 5000;
        static IPEndPoint sendTo;
        static UdpClient server;
        static byte[] bytesToSend;

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
            bytesToSend = Encoding.ASCII.GetBytes(input);

            sendTo = new IPEndPoint(IPAddress.Any, PORT_NO);
            server = new UdpClient(sendTo);
            server.Receive(ref sendTo);

            BeginWrite();
            Thread.Sleep(runTime);
            server.Dispose();
        }

        static void BeginWrite()
        {
            object state = new object();
            server.BeginSend(bytesToSend, bytesToSend.Length, sendTo, EndWrite, state);
        }

        static void EndWrite(IAsyncResult ar)
        {
            server.EndSend(ar);
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