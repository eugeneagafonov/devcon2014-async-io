using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devcon2014.AsyncIO
{
	internal class Program
	{
		private const int BUFFER_SIZE = 4096;

		private static void Main(string[] args)
		{
			Task t = ProcessAsynchronousIO();
			t.GetAwaiter().GetResult();
		}

		private static async Task ProcessAsynchronousIO()
		{
			using (var stream = new FileStream(
				"test1.txt", FileMode.Create, FileAccess.ReadWrite,
				FileShare.None, BUFFER_SIZE))
			{
				Console.WriteLine("1. Uses I/O Threads: {0}", stream.IsAsync);

				byte[] buffer = Encoding.UTF8.GetBytes(CreateFileContent());
				Task t = stream.WriteAsync(buffer, 0, buffer.Length);
				await t;
			}

			using (var stream =
			#region SECRET
 new FileStream("test2.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None,
					BUFFER_SIZE, FileOptions.Asynchronous
			#endregion
))
			{
				Console.WriteLine("2. Uses I/O Threads: {0}", stream.IsAsync);

				byte[] buffer = Encoding.UTF8.GetBytes(CreateFileContent());
				Task t = stream.WriteAsync(buffer, 0, buffer.Length);
				await t;
			}

			using (FileStream stream = File.Create("test3.txt",
			#region SECRET
 BUFFER_SIZE, FileOptions.Asynchronous
			#endregion
))
			using (var sw = new StreamWriter(stream))
			{
				Console.WriteLine("3. Uses I/O Threads: {0}", stream.IsAsync);
				await sw.WriteAsync(CreateFileContent());
			}

			using (var sw = new StreamWriter("test4.txt", true))
			{
				Console.WriteLine("4. Uses I/O Threads: {0}", 
					((FileStream)sw.BaseStream).IsAsync);
				await sw.WriteAsync(CreateFileContent());
			}

			Console.WriteLine("Starting parsing files in parallel");

			var readTasks = new Task<long>[4];
			for (int i = 0; i < 4; i++)
			{
				readTasks[i] = SumFileContent(string.Format("test{0}.txt", i + 1));
			}

			long[] sums = await Task.WhenAll(readTasks);

			Console.WriteLine("Sum in all files: {0}", sums.Sum());

			Console.WriteLine("Deleting files...");

			var deleteTasks = new Task[4];
			for (int i = 0; i < 4; i++)
			{
				string fileName = string.Format("test{0}.txt", i + 1);
				deleteTasks[i] = SimulateAsynchronousDelete(fileName);
			}

			await Task.WhenAll(deleteTasks);

			Console.WriteLine("Deleting complete.");
		}

		private static string CreateFileContent()
		{
			var sb = new StringBuilder();
			for (int i = 0; i < 100000; i++)
			{
				sb.AppendFormat("{0}", new Random(i).Next(0, 99999));
				sb.AppendLine();
			}
			return sb.ToString();
		}

		private static async Task<long> SumFileContent(string fileName)
		{
			using (var stream = new FileStream(fileName, FileMode.Open,
				FileAccess.Read, FileShare.None, BUFFER_SIZE, FileOptions.Asynchronous))
			using (var sr = new StreamReader(stream))
			{
				long sum = 0;
				while (sr.Peek() > -1)
				{
					string line = await sr.ReadLineAsync();
					sum += long.Parse(line);
				}

				return sum;
			}
		}

		private static Task SimulateAsynchronousDelete(string fileName)
		{
			return Task.Run(() => File.Delete(fileName));
		}
	}
}