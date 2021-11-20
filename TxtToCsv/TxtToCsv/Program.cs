using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace TxtToCsv
{
	class Program
	{
		static void Main(string[] args)
		{
			string dataLocation = @"C:\temp";
			List<int> callsPerSec = new List<int>();
			List<int> clientCpus = new List<int>();
			List<int> serverCpus = new List<int>();
			List<int> totalCpus = new List<int>();
			List<CsvEntry> csvEntries = new List<CsvEntry>();

			foreach(string filePath in Directory.EnumerateFiles(dataLocation, "*.txt", SearchOption.TopDirectoryOnly))
			{
				string[] lines = File.ReadAllLines(filePath);
				foreach(string line in lines)
				{
					string[] results = line.Split(' ', StringSplitOptions.None);
					if (results.Length > 1 && !string.IsNullOrWhiteSpace(results[1]))
					{
						if(int.TryParse(results[1], out int number))
						if (results[0].StartsWith("calls"))
						{
							callsPerSec.Add(number);
						}
						else if(results[0].StartsWith("client"))
						{
							clientCpus.Add(number);
						}
						else if(results[0].StartsWith("server"))
						{
							serverCpus.Add(number);
						}
						else if(results[0].StartsWith("total"))
						{
							totalCpus.Add(number);
						}
					}
				}

				// Remove first and last to avoid odd issues.
				callsPerSec.RemoveAt(0);
				callsPerSec.RemoveAt(callsPerSec.Count - 1);
				clientCpus.RemoveAt(0);
				clientCpus.RemoveAt(clientCpus.Count - 1);
				serverCpus.RemoveAt(0);
				serverCpus.RemoveAt(serverCpus.Count - 1);
				totalCpus.RemoveAt(0);
				totalCpus.RemoveAt(totalCpus.Count - 1);

				List<int> countLengths = new List<int>()
				{
					callsPerSec.Count,
					clientCpus.Count,
					serverCpus.Count,
					totalCpus.Count
				};

				// find shortest list
				int shortest = countLengths.Min();

				// Make sure all the lists are the same range. We are assuming that the errent values are at the end
				if(shortest < callsPerSec.Count)
				{
					callsPerSec.RemoveRange(shortest, callsPerSec.Count - shortest);
				}
				if(shortest < clientCpus.Count)
				{
					clientCpus.RemoveRange(shortest, clientCpus.Count - shortest);
				}
				if(shortest < serverCpus.Count)
				{
					serverCpus.RemoveRange(shortest, serverCpus.Count - shortest);
				}
				if(shortest < totalCpus.Count)
				{
					totalCpus.RemoveRange(shortest, totalCpus.Count - shortest);
				}

				// Compute the normal values...
				int callAverage = (int)Math.Round(callsPerSec.Average());
				int clientCpuAverage = (int)Math.Round(clientCpus.Average());
				int serverCpuAverage = (int)Math.Round(serverCpus.Average());
				int totalCpuAverage = (int)Math.Round(totalCpus.Average());

				double callStdDev = StandardDevation(callsPerSec);
				double clientStdDev = StandardDevation(clientCpus);
				double serverStdDev = StandardDevation(serverCpus);
				double totalStdDev = StandardDevation(totalCpus);

				int minCall = callsPerSec.Min();
				int maxCall = callsPerSec.Max();
				int minClient = clientCpus.Min();
				int maxClient = clientCpus.Max();
				int minServer = serverCpus.Min();
				int maxServer = serverCpus.Max();
				int minTotal = totalCpus.Min();
				int maxTotal = totalCpus.Max();

				for(int i = 0; i < callsPerSec.Count; i++)
				{
					int call = callsPerSec[i];
					int client = clientCpus[i];
					int server = serverCpus[i];
					int total = totalCpus[i];
					csvEntries.Add(new CsvEntry(call, client, server, total));
				}

				csvEntries.Add(new CsvEntry(callAverage, clientCpuAverage, serverCpuAverage, totalCpuAverage));
				csvEntries.Add(new CsvEntry(callStdDev, clientStdDev, serverStdDev, totalStdDev));
				csvEntries.Add(new CsvEntry(minCall, minClient, minServer, minTotal));
				csvEntries.Add(new CsvEntry(maxCall, maxClient, maxServer, maxTotal));

				string csvPath = @$"{dataLocation}\{Path.GetFileNameWithoutExtension(filePath)}.csv";

				using (StreamWriter writer = new StreamWriter(csvPath))
				using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
				{
					csvWriter.WriteRecords(csvEntries);
				}

				csvEntries.Clear();
				callsPerSec.Clear();
				clientCpus.Clear();
				serverCpus.Clear();
				totalCpus.Clear();
			}
		}

		static double StandardDevation(IEnumerable<int> values)
		{
			double average = values.Average();
			return Math.Sqrt(values.Average(v => (v - average) * (v - average)));
		}

		public class CsvEntry
		{
			public double CallCount { get; set; }
			public double ClientCpuUsage { get; set; }
			public double ServerCpuUsage { get; set; }
			public double TotalCpuUsage { get; set; }

			public CsvEntry(double callCount, double clientCpuUsage, double serverCpuUsage, double totalCpuUsage)
			{
				CallCount = callCount;
				ClientCpuUsage = clientCpuUsage;
				ServerCpuUsage = serverCpuUsage;
				TotalCpuUsage = totalCpuUsage;
			}
		}
	}
	
}
