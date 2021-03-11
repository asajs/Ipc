using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PipeServer
{
	class Program
    {
        static string input;
        static NamedPipeServerStream server;
        static void Main(string[] args)
        {
            int runTime = 0;
            input = "";
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
            StartServer();
            Thread.Sleep(runTime);
            server.Dispose();
        }

        static void StartServer()
        {
            Task.Run(() =>
            {
                server = new NamedPipeServerStream("PipesOfPiece");
                server.WaitForConnection();
                StreamWriter writer = new StreamWriter(server);
                writer.AutoFlush = true;
                while (true)
                {
                    writer.Write(input);
                }
            });
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