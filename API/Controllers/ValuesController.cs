using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogsProcesor;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		public static List<string> data = new List<string>();
		// GET api/values
		[HttpGet]
		public ActionResult<IEnumerable<string>> Get()
		{
			Logger.Debug($"Request {HttpContext.Request.Path}");
			return data;
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
			data.Add(value);
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
