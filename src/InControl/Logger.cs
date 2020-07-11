using System;

namespace InControl
{
	public class Logger
	{
		public static event Action<LogMessage> OnLogMessage;

		public static void LogInfo(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage logMessage = default(LogMessage);
				logMessage.text = text;
				logMessage.type = LogMessageType.Info;
				LogMessage obj = logMessage;
				Logger.OnLogMessage(obj);
			}
		}

		public static void LogWarning(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage logMessage = default(LogMessage);
				logMessage.text = text;
				logMessage.type = LogMessageType.Warning;
				LogMessage obj = logMessage;
				Logger.OnLogMessage(obj);
			}
		}

		public static void LogError(string text)
		{
			if (Logger.OnLogMessage != null)
			{
				LogMessage logMessage = default(LogMessage);
				logMessage.text = text;
				logMessage.type = LogMessageType.Error;
				LogMessage obj = logMessage;
				Logger.OnLogMessage(obj);
			}
		}
	}
}
