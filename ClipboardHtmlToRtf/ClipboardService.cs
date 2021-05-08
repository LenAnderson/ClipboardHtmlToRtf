using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardHtmlToRtf
{
	class ClipboardService
	{
		bool isRunning = false;
		IDataObject dataObject;

		string AppDir { get { return Regex.Replace(System.Reflection.Assembly.GetExecutingAssembly().Location, @"(^.+)\\[^\\]+$", "$1"); } }

		public void Start()
		{
			isRunning = true;
			Run();
		}

		async void Run()
		{
			while (isRunning)
			{
				// check if dataObject is current
				if (dataObject == null || !Clipboard.IsCurrent(dataObject))
				{
					// if not: get current dataObject
					dataObject = Clipboard.GetDataObject();
					// check if dataObject contains html but not rtf
					if (dataObject.GetDataPresent(DataFormats.Html) && !dataObject.GetDataPresent(DataFormats.Rtf))
					{
						// if yes: convert html to rtf
						var src = System.IO.Path.GetTempFileName();
						var dst = System.IO.Path.GetTempFileName();
						var html = Regex.Replace(dataObject.GetData(DataFormats.Html).ToString(), "^.*(<html>.+</html>).*$", "$1", RegexOptions.Singleline);
						System.IO.File.WriteAllText(src, html);
						var info = new ProcessStartInfo($@"{AppDir}\Resources\pandoc-2.13\pandoc.exe");
						info.UseShellExecute = false;
						info.Arguments = $@"""{src}"" -f html -t rtf -s -o ""{dst}""";
						var p = Process.Start(info);
						p.WaitForExit();
						var rtf = System.IO.File.ReadAllText(dst);
						// add rtf to dataObject
						var data = new DataObject();
						data.SetData(DataFormats.Rtf, rtf);
						// add all existing formats to dataObject
						var formats = dataObject.GetFormats();
						foreach (var format in formats)
						{
							data.SetData(format, dataObject.GetData(format));
						}
						// set clipboard to dataObject
						Clipboard.SetDataObject(data);
					}
				}
				await Task.Delay(100);
			}
		}


		public void Stop()
		{
			isRunning = false;
		}
	}
}
