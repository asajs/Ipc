﻿using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemorySharingServer
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

            using (MemoryMappedFile client = MemoryMappedFile.CreateOrOpen("memorymapping", bytesToRead.Length))
            {
				using (MemoryMappedViewStream stream = client.CreateViewStream(0, bytesToRead.Length))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(bytesToRead);
                    }
                }

				Task.Run(() =>
                {
                    using (MemoryMappedViewStream stream = client.CreateViewStream(0, bytesToRead.Length))
					{
                        while (true)
                        {
							stream.Write(bytesToRead, 0, bytesToRead.Length);
                            stream.Flush();
                            stream.Seek(0, SeekOrigin.Begin);
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
