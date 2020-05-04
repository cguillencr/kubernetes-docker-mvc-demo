using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIv2.Controllers;
using LogsProcesor;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APIv2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			#region log4net
			var logRepository = log4net.LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
			log4net.Config.XmlConfigurator.Configure(logRepository, new System.IO.FileInfo("log4net.config.xml"));

			Logger.SetLogger(log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), "demo"));
			Logger.Debug("App starting.");
			#endregion

			#region 
			// This block simulates an Actor who recovery state and becomes this project to a stateFull application.
			// Simulates 20secs to recover state.

			Thread.Sleep(20000);
			ValuesController.data.Add("value3");
			ValuesController.data.Add("value4");

			#endregion
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
