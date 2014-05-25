﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AsyncInClient
{
	/// <summary>
	///   Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonSync_Click(object sender, RoutedEventArgs e)
		{
			ContentTextBlock.Text = string.Empty;
			try
			{
				//string result = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext()).Result;

				#region Nested Message Loop

				//string result = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext()).WaitWithNestedMessageLoop();

				#endregion

				#region GetAwaiter

				//string result = TaskMethod().GetAwaiter().GetResult();

				#endregion

				string result = TaskMethod().Result;
				ContentTextBlock.Text = result;
			}
			catch (Exception ex)
			{
				ContentTextBlock.Text = ex.Message;
			}
		}

		private void ButtonAsync_Click(object sender, RoutedEventArgs e)
		{
			ContentTextBlock.Text = string.Empty;
			Mouse.OverrideCursor = Cursors.Wait;
			Task<string> task = TaskMethod();
			task.ContinueWith(t =>
			{
				ContentTextBlock.Text = t.Exception.InnerException.Message;
				Mouse.OverrideCursor = null;
			},
				CancellationToken.None,
				TaskContinuationOptions.OnlyOnFaulted,
				TaskScheduler.FromCurrentSynchronizationContext());
		}

		private void ButtonAsyncOK_Click(object sender, RoutedEventArgs e)
		{
			ContentTextBlock.Text = string.Empty;
			Mouse.OverrideCursor = Cursors.Wait;
			Task<string> task = TaskMethod(TaskScheduler.FromCurrentSynchronizationContext());
			task.ContinueWith(t => Mouse.OverrideCursor = null,
				CancellationToken.None,
				TaskContinuationOptions.None,
				TaskScheduler.FromCurrentSynchronizationContext());
		}

		private Task<string> TaskMethod()
		{
			return TaskMethod(TaskScheduler.Default);
		}

		private Task<string> TaskMethod(TaskScheduler scheduler)
		{
			Task delay = Task.Delay(TimeSpan.FromSeconds(5));

			return delay.ContinueWith(t =>
			{
				string str = string.Format("Task is running on a thread id {0}. Is thread pool thread: {1}",
					Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread);
				ContentTextBlock.Text = str;
				return str;
			}, scheduler);
		}
	}
}