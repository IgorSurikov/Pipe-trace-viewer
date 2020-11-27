using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeTraceViewer.Models
{
	class Record
	{
		public Record(int? id, string disassembly, List<Event> events)
		{
			this.id = id;
			this.disassembly = disassembly;
			Events = events;
		}

		public int? id { get; set; }
		public string disassembly { get; set; }

		public List<Event> Events { get; set; }
	}
}