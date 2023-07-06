﻿using Extensions;

using SkyveApp.Domain.Systems;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SkyveApp.Systems;

public class LoggerSystem : ILogger
{
	private bool failed;
	private readonly INotifier _notifier;
	private readonly bool _disabled;
	private readonly Stopwatch? _stopwatch;

	public string LogFilePath { get; }

	public LoggerSystem(ISettings _, INotifier notifier)
	{
		var folder = CrossIO.Combine(ISave.CustomSaveDirectory, ISave.AppName, "Logs");
		var previousLog = CrossIO.Combine(folder, $"SkyveApp_Previous.log");

		LogFilePath = CrossIO.Combine(folder, $"SkyveApp.log");

		_stopwatch = Stopwatch.StartNew();
		_notifier = notifier;

		try
		{
			Directory.CreateDirectory(folder);

			if (CrossIO.FileExists(previousLog))
			{
				CrossIO.DeleteFile(previousLog);
			}

			if (CrossIO.FileExists(LogFilePath))
			{
				File.Move(LogFilePath, previousLog);
			}

			File.WriteAllBytes(LogFilePath, new byte[0]);

			_stopwatch = Stopwatch.StartNew();

			var assembly = Assembly.GetExecutingAssembly();
			var details = assembly.GetName();

			Info($"Skyve v{details.Version}");
			Info($"Now  = {DateTime.Now:yyyy-MM-dd hh:mm:ss tt}");
			Info($"Here = {Application.StartupPath}");
		}
		catch
		{
			_disabled = true;
		}

		_notifier = notifier;
	}

	public void Debug(object message)
	{
		ProcessLog("DEBUG", message);
	}

	public void Info(object message)
	{
		ProcessLog("INFO ", message);
	}

	public void Warning(object message)
	{
		ProcessLog("WARN ", message);
	}

	public void Error(object message)
	{
		ProcessLog("ERROR", message);
	}

	public void Exception(Exception exception, object message)
	{
		ProcessLog("FATAL", $"{message}\r\n{exception}\r\n");
	}

	private void ProcessLog(string type, object content)
	{
		if (_disabled)
		{
			return;
		}

		var secs = _stopwatch!.ElapsedMilliseconds / 1000M;
		var sb = new StringBuilder();
		var time = secs.ToString("0.000");

		sb.AppendFormat("\r\n[{0} ", type);

		if (time.Length < 6)
		{
			sb.Append(' ', 10 - time.Length);
		}

		sb.Append(time);

		sb.Append("] ");

		sb.Append(content?.ToString().Replace("\n", "\n                      "));

		lock (LogFilePath)
		{
			try
			{
				File.AppendAllText(LogFilePath, sb.ToString());
			}
			catch (Exception ex)
			{
				if (!failed)
				{
					failed = true;

					_notifier.OnLoggerFailed(ex);
				}
			}
		}
	}
}