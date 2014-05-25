using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
	class Program
	{
		static void Main(string[] args)
		{
			FirstAsyncFunction();
		}

		private static void FirstAsyncFunction()
		{
			int id1 = Environment.CurrentManagedThreadId;
			Task<int> task = HttpLengthAsync("http://abbyy.com/");

			Debugger.Break();
			int id3 = Environment.CurrentManagedThreadId; // Same as id1
			Debugger.Break();
			var length = task.Result;  // Waits for Task complete to get its result
		}

		private static async Task<int> HttpLengthAsync(String uri)
		{
			int id2 = Environment.CurrentManagedThreadId; // Same as FirstAsyncFunction's id1
			Debugger.Break();
			Task<String> task = new HttpClient(){ Timeout = TimeSpan.FromSeconds(30)}.GetStringAsync(uri);

			// await lets calling thread return to caller (Main)
			// Code after await resumes via thread pool thread
			String text = await task;
			Debugger.Break();
			int id3 = Environment.CurrentManagedThreadId; // A thread pool thread
			return text.Length;  // Sets Task's Result property
		}
	}
}
