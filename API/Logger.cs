using log4net;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsProcesor
{
	public sealed class Logger
	{
		private static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		internal Logger()
		{
		}

		public static void Error(Exception e)
		{
			logger.Error(e);
		}

		public static void Error(string message, Exception e)
		{
			logger.Error(message, e);
		}
		public static void Debug(string message)
		{
			logger.Debug(message);
		}

		public static void Debug(int message)
		{
			logger.Debug(message);
		}


		public static void SetLogger(ILog log)
		{
			logger = log;
		}
	}
}
