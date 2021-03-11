using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace RunOtherProcesses
{
	class Program
	{
		static List<string> clientFolderPaths = new List<string>() { @"C:\Users\Asa\source\repos\IPC\Client\MemorySharingClient\MemorySharingClient\bin\Release", @"C:\Users\Asa\source\repos\IPC\Client\MemorySharingClientMutex\MemorySharingClientMutex\bin\Release" };
		static List<string> serverFolderPaths = new List<string>() { @"C:\Users\Asa\source\repos\IPC\Server\MemorySharingServer\MemorySharingServer\bin\Release", @"C:\Users\Asa\source\repos\IPC\Server\MemorySharingServerMutex\MemorySharingServerMutex\bin\Release" };
		static string dataPath = @"C:\Users\Asa\source\repos\IPC\Data";
		static void Main(string[] args)
		{
			List<string> clientExePaths = new List<string>();
			List<string> serverExePaths = new List<string>();
			List<string> dataPaths = new List<string>();

			foreach(string folderPath in clientFolderPaths)
			{
				clientExePaths.AddRange(Directory.EnumerateFiles(folderPath, "*.exe", SearchOption.AllDirectories));
			}

			foreach (string folderPath in serverFolderPaths)
			{
				serverExePaths.AddRange(Directory.EnumerateFiles(folderPath, "*.exe", SearchOption.AllDirectories));
			}

			dataPaths.AddRange(Directory.EnumerateFiles(dataPath, "*.txt", SearchOption.TopDirectoryOnly));

			foreach(string data in dataPaths)
			{
				RunExes(clientExePaths, serverExePaths, data);
			}
		}

		static void RunExes(List<string> clientExes, List<string> serverExes, string dataPath)
		{
			string dataFileName = Path.GetFileNameWithoutExtension(dataPath);

			for(int i = 0; i < clientExes.Count; i++)
			{
				string clientFileName = Path.GetFileNameWithoutExtension(clientExes[i]);
				string serverFileName = Path.GetFileNameWithoutExtension(serverExes[i]);

				string parentDirectoryName = Path.GetFileName(Path.GetDirectoryName(clientExes[i]));

				string outputPath = Path.Combine(@"C:\Temp", $"{parentDirectoryName}-{clientFileName}_{dataFileName}.txt");

				StartProcesses(clientExes[i], serverExes[i], dataPath, outputPath);
			}
		}

		static void StartProcesses(string clientPath, string serverPath, string dataPath, string outputPath)
		{
			File.Delete(outputPath);
			string output = string.Empty;
			int runTime = 60000;

			string arguments = $"{dataPath} {runTime}";

			ProcessStartInfo serverProcessInfo = new ProcessStartInfo(serverPath, arguments);
			serverProcessInfo.UseShellExecute = false;
			serverProcessInfo.CreateNoWindow = false;

			Process serverProcess = new Process();
			serverProcess.StartInfo = serverProcessInfo;
			serverProcess.Start();

			Thread.Sleep(100);

			ProcessStartInfo clientProcessInfo = new ProcessStartInfo(clientPath, arguments);
			clientProcessInfo.UseShellExecute = false;
			clientProcessInfo.CreateNoWindow = false;
			clientProcessInfo.RedirectStandardOutput = true;
			clientProcessInfo.StandardOutputEncoding = Encoding.ASCII;

			Process clientProcess = new Process();
			clientProcess.StartInfo = clientProcessInfo;
			clientProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) => { output += $"calls/sec: {e.Data}\n"; });
			clientProcess.Start();
			clientProcess.BeginOutputReadLine();

			List<double> clientCpuTime = new List<double>();
			List<double> serverCpuTime = new List<double>();
			List<double> totalTime = new List<double>();

			double cpuCount = Convert.ToDouble(Environment.ProcessorCount);
			DateTime endTime = DateTime.Now.AddMilliseconds(runTime);
			DateTime lastTime = new DateTime();
			DateTime curTime;
			TimeSpan curServerprocTime;
			TimeSpan curClientProcTime;
			TimeSpan lastClientProcTime = clientProcess.TotalProcessorTime;
			TimeSpan lastServerProcTime = serverProcess.TotalProcessorTime;

			while(DateTime.Now < endTime)
			{
				if(!serverProcess.HasExited && !clientProcess.HasExited)
				{
					curTime = DateTime.Now;
					curServerprocTime = serverProcess.TotalProcessorTime;
					curClientProcTime = clientProcess.TotalProcessorTime;

					double serverCpuUsage = (curServerprocTime.TotalMilliseconds - lastServerProcTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / cpuCount;
					double clientCpuUsage = (curClientProcTime.TotalMilliseconds - lastClientProcTime.TotalMilliseconds) / curTime.Subtract(lastTime).TotalMilliseconds / cpuCount;

					clientCpuTime.Add(clientCpuUsage * 100);
					serverCpuTime.Add(serverCpuUsage * 100);
					totalTime.Add((clientCpuUsage + serverCpuUsage) * 100);

					lastTime = curTime;
					lastServerProcTime = curServerprocTime;
					lastClientProcTime = curClientProcTime;
				}
				Thread.Sleep(1000);
			}

			clientProcess.WaitForExit();
			serverProcess.WaitForExit();
			clientProcess?.Dispose();
			serverProcess?.Dispose();
			Thread.Sleep(300);

			File.AppendAllText(outputPath, output);
			File.AppendAllLines(outputPath, clientCpuTime.Select(e => $"client: {Math.Round(e)}"));
			File.AppendAllLines(outputPath, serverCpuTime.Select(e => $"server: {Math.Round(e)}"));
			File.AppendAllLines(outputPath, totalTime.Select(e => $"total: {Math.Round(e)}"));
		}
	}
}
