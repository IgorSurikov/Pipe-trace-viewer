using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeTraceViewer.Models
{
	class JsonObject
	{
		public string type { get; set; }
		public int? id { get; set; }
		public string description { get; set; }
		public string disassembly { get; set; }
		public int? cycle { get; set; }
		public int? stage { get; set; }
	}