using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace UI.Controllers
{
	public class PagesController : Controller
	{
		private IConfiguration configuration;
		public PagesController(IConfiguration iConfig)
		{
			configuration = iConfig;
		}

		[HttpGet("/page1")]
		public IActionResult Page1()
		{
			string apiServer = configuration.GetValue<string>("ApiServerUrl");

			var client = new RestClient($"http://{apiServer}/api/values");
			client.Timeout = 100;
			var request = new RestRequest(Method.GET);
			IRestResponse response = client.Execute(request);
			return Ok(response.Content);
		}
	}
}
