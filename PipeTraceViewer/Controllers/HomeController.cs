using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
			List<String> files = (from a in Directory.GetFiles(Path.Combine(_env.ContentRootPath, "wwwroot", "data"))
				select Path.GetFileName(a)).ToList();
			ViewBag.files = new SelectList(files);
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(string fileName)
		{
			return RedirectToAction("Viewer", new { fileName = fileName});
		}

		public async Task<IActionResult> Viewer(String fileName)
		{
			ViewData["Title"] = fileName;
			var records = await ExtractDataAsync(fileName);
			var cycleCount = records.Max(r => r.Events.Max(e => e.EndCycle));
			ViewBag.cycleCount = cycleCount;
			return View(records);
		}


		private async Task<List<Record>> ExtractDataAsync(String fileName)
		{
			string path = Path.Combine(_env.ContentRootPath, "wwwroot", "data", fileName);
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