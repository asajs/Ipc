using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemorySharingClientMutex
{
    class Program
    {
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
            byte[] bytesToRead = Encoding.ASCII.GetBytes(input);

            using (MemoryMappedFile server = MemoryMappedFile.CreateOrOpen("memorymapping", bytesToRead.Length))
            {
                Mutex mutex = Mutex.OpenExisting("mutex");
                Stopwatch stopwatch = new Stopwatch();
                long count = 0;
                Task.Run(() =>
                {
                    using (MemoryMappedViewStream stream = server.CreateViewStream(0, bytesToRead.Length))
                    {
                        stopwatch.Start();
                        while (true)
                        {
                            mutex.WaitOne();
                            stream.Read(bytesToRead, 0, bytesToRead.Length);
                            count++;
                            if (stopwatch.ElapsedMilliseconds > 1000)
                            {
                                Console.WriteLine($"{count}");
                                count = 0;
                                stopwatch.Restart();
                            }
                            mutex.ReleaseMutex();
                        }
                    }
                });
                Thread.Sleep(runTime);
            }
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
