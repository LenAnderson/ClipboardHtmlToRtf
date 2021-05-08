using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardHtmlToRtf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private TaskbarIcon notifyIcon;
		private ClipboardService clipSvc;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
			clipSvc = new ClipboardService();
			clipSvc.Start();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			clipSvc.Stop();
			notifyIcon.Dispose();
			base.OnExit(e);
		}
	}
}
