using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsOverhead
{
	class Program
	{
		static void Main(string[] args)
		{
			ThreadOverhead();
		}

		private static void ThreadOverhead()
		{
			const int OneMB = 1024 * 1024;

			using (ManualResetEvent wakeThreads = new ManualResetEvent(false))
			{
				int threadNum = 0;
				try
				{
					while (true)
					{
						Thread t = new Thread(WaitOnEvent);
						t.Start(wakeThreads);
						Console.WriteLine("{0}: {1}MB", ++threadNum,
							 Process.GetCurrentProcess().VirtualMemorySize64 / OneMB);
					}
				}
				catch (OutOfMemoryException)
				{
					Console.WriteLine("Out of memory after {0} threads.", threadNum);
					Debugger.Break();
					wakeThreads.Set();   // Release all the threads
				}
			}
		}

		private static void WaitOnEvent(Object eventObj)
		{
			((ManualResetEvent)eventObj).WaitOne();
		}
	}
}
