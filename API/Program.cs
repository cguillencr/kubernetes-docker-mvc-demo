using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			#region 
			// This block simulates an Actor who recovery state and becomes this project to a stateFull application.
			// Simulates 20secs to recover state.

			Thread.Sleep(20000);
			ValuesController.data.Add("value1");
			ValuesController.data.Add("value2");

			#endregion
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
