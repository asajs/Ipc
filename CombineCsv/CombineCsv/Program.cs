using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace CombineCsv
{
	class Program
	{
		static void Main(string[] args)
		{
			string dataPath = @"C:\Temp";
			string extraPath = @"C:\Temp\CombinedCsv\";

			Directory.CreateDirectory(extraPath);
			List<string> filePaths = Directory.EnumerateFiles(dataPath, "*.csv").ToList();

			Dictionary<string, List<string>> combinedFiles = new Dictionary<string, List<string>>();

			foreach (string filePath in filePaths)
			{
				string[] splitPath = Path.GetFileName(filePath).Split('_');

				if (combinedFiles.TryGetValue(splitPath[0], out List<string> collection))
				{
					collection.Add(filePath);
				}
				else
				{
					combinedFiles.Add(splitPath[0], new List<string>() { filePath });
				}
			}

			List<AverageCount> callPerCpuSmall = new List<AverageCount>();
			List<AverageCount> callPerCpuMedium = new List<AverageCount>();
			List<AverageCount> callPerCpuLarge = new List<AverageCount>();
			foreach (KeyValuePair<string, List<string>> valuePair in combinedFiles)
			{
				foreach(string path in valuePair.Value)
				{
					List<CsvEntry> valuesCombined = new List<CsvEntry>();
					using(StreamReader reader = new StreamReader(path))
					using(CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
					{
						List<CsvEntry> test = csv.GetRecords<CsvEntry>().ToList();
						List<CsvEntry> values = test.TakeLast(4).ToList();
						valuesCombined.AddRange(values);
						string finalPath = MassagePath(path);
						CsvEntry averageValues = values.First();
						AverageCount averageCount = new AverageCount(finalPath, averageValues.CallCount, Math.Round(averageValues.CallCount / averageValues.TotalCpuUsage));
						if(path.Contains("19"))
						{
							callPerCpuSmall.Add(averageCount);
						}
						else if(path.Contains("108"))
						{
							callPerCpuMedium.Add(averageCount);
						}
						else if(path.Contains("59892"))
						{
							callPerCpuLarge.Add(averageCount);
						}
					}
					string combinedPath = $@"{extraPath}{valuePair.Key}_combined.csv";
					using (StreamWriter writer = new StreamWriter(combinedPath, true))
					using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
					{
						csvWriter.WriteRecords(valuesCombined);
					}
				}
			}

			string smallPath = $@"{extraPath}small_values_aggregrated.csv";
			using (StreamWriter writer = new StreamWriter(smallPath))
			using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csvWriter.WriteRecords(callPerCpuSmall);
			}

			string mediumPath = $@"{extraPath}medium_values_aggregrated.csv";
			using (StreamWriter writer = new StreamWriter(mediumPath))
			using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csvWriter.WriteRecords(callPerCpuMedium);
			}

			string largePath = $@"{extraPath}large_values_aggregrated.csv";
			using (StreamWriter writer = new StreamWriter(largePath))
			using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csvWriter.WriteRecords(callPerCpuLarge);
			}
		}

		static string MassagePath(string path)
		{
			string basicPath = Path.GetFileName(path).Split('_')[0];
			if(basicPath.Contains("-Client"))
			{
				return basicPath.Replace("-Client", "-Tcp");
			}
			else
			{
				return basicPath.Replace("Client", "");
			}
		}

		public class CsvEntry
		{
			public double CallCount { get; set; }
			public double ClientCpuUsage { get; set; }
			public double ServerCpuUsage { get; set; }
			public double TotalCpuUsage { get; set; }

			public CsvEntry(double CallCount, double ClientCpuUsage, double ServerCpuUsage, double TotalCpuUsage)
			{
				this.CallCount = CallCount;
				this.ClientCpuUsage = ClientCpuUsage;
				this.ServerCpuUsage = ServerCpuUsage;
				this.TotalCpuUsage = TotalCpuUsage;
			}
		}

		public class AverageCount
		{
			public string Path { get; set; }
			public double CallCountPerSecond { get; set; }
			public double CallsPer1CpuPercent { get; set; }

			public AverageCount(string path, double callCountPerSecond, double callsPer1CpuPercent)
			{
				Path = path;
				CallCountPerSecond = callCountPerSecond;
				CallsPer1CpuPercent = callsPer1CpuPercent;
			}
		}

	}
}
