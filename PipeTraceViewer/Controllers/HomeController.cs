using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PipeTraceViewer.Models;

namespace PipeTraceViewer.Controllers
{
	public class HomeController : Controller
	{
		private readonly IWebHostEnvironment _env;

		public HomeController(IWebHostEnvironment env)
		{
			_env = env;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Viewer()
		{
			var records = await ExtractDataAsync();
			return View(records.Where(i => i.id < 10).ToList());
		}


		private async Task<List<Record>> ExtractDataAsync()
		{
			string path = Path.Combine(_env.ContentRootPath, "wwwroot", "data", "test.json");
			IEnumerable<JsonObject> rawData;
			using (FileStream fs = new FileStream(path, FileMode.Open))
			{
				rawData = await JsonSerializer.DeserializeAsync<List<JsonObject>>(fs);
			}

			var stageList = from i in rawData
				where i.type == "Stage"
				select i;
			rawData = rawData.Except(stageList);
			var rawRecords = rawData.Where(i => i.type == "Record").GroupJoin(
				rawData.Where(i => i.type == "Event"),
				r => r.id,
				e => e.id,
				(record, events) => new
				{
					record.type,
					record.id,
					record.disassembly,
					events = events
						.Join(stageList,
							e => e.stage,
							s => s.id,
							(e, s) => new {e.cycle, s.description})
				});

			var records = new List<Record>();
			foreach (var rec in rawRecords)
			{
				var events = new List<Event>();
				var rawEvents = rec.events.ToList();
				for (int i = 0; i < rawEvents.Count(); i++)
				{
					if (rawEvents[i].description == "Fetch")
					{
						events.Add(new Event(rawEvents[i].cycle, rawEvents[i].cycle, rawEvents[i].description));
					}
					else if (rawEvents[i].description == "Writeback")
					{
						events.Add(new Event(rawEvents[i].cycle, rawEvents[i].cycle, rawEvents[i].description));
					}
					else
					{
						events.Add(new Event(rawEvents[i - 1].cycle + 1, rawEvents[i].cycle,
							rawEvents[i].description));
					}
				}

				records.Add(new Record(rec.id, rec.disassembly, events));
			}

			return records;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
		}
	}
}