using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipeTraceViewer.Models
{
	public class Event
	{
		public Event(int? startCycle, int? endCycle, string description)
		{
			EndCycle = endCycle;
			StartCycle = startCycle;
			CycleCount = endCycle - startCycle + 1;
			this.description = description;
		}

		public int? EndCycle { get; set; }
		public int? StartCycle { get; set; }
		public int? CycleCount { get; set; }
		public string description { get; set; }
	}
}
