using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PipeTraceViewer.TagHelpers
{
	public class EventTagHelper : TagHelper
	{
		public string Description { get; set; }
		public override async void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "div";
			string colorClass = String.Empty;
			switch (Description)
			{
				case "Fetch":
					colorClass = "bg-primary";
					break;

				case "Decode":
					colorClass = "bg-success";
					break;

				case "Execute":
					colorClass = "bg-danger";
					break;

				case "Memory":
					colorClass = "bg-warning";
					break;

				case "Writeback":
					colorClass = "bg-info";
					break;
			}
			output.AddClass(colorClass, HtmlEncoder.Default);
			output.AddClass("child", HtmlEncoder.Default);
			output.AddClass("border-top", HtmlEncoder.Default);
			output.AddClass("border-bottom", HtmlEncoder.Default);

			output.Attributes.SetAttribute("data-toggle", "tooltip");
			output.Attributes.SetAttribute("title", Description);
			output.Content.SetContent($"{Description[0]}");
		}
    }


}
