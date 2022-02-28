using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using SadConsole;

namespace UpdateUnpacker
{
	public static class Updater
	{
		
		public static void BeginUpdate()
		{
			string url = "";
			
				Program.root.Print(Program.root.Width / 2 - 11,Program.root.Height / 2-1,"Downloading New Update");

				Program.root.Print(Program.root.Width / 2 - 20,Program.root.Height / 2, ColoredString.Parser.Parse(GetProgressBarString(0)));
				

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				url = "http://deathcult.today/stillborn/heartsignal_linux.zip";
			}else  if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				url = "http://deathcult.today/stillborn/heartsignal_win64.zip";
			}else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				url = "http://deathcult.today/stillborn/heartsignal_osx.zip";
			}else
			{
				//error
			}
			
			using (var client = new WebClient())
			{

				client.DownloadFileAsync(new Uri(url), "downloaded.zip");
				client.DownloadProgressChanged += UpdateProgress;
				client.DownloadFileCompleted += Finalise;
			}

		}

		private static string GetProgressBarString(float percentage)
		{
			
			string bar = "";


			int cutoff =  (int)((40f/100f) * percentage);
			
			
			bar += "[c:r f:green]";
			for(int i = 0; i < cutoff; i++)
			{
		
					bar += "#";
					

				
			}
			bar += "[c:u]";
			bar += "[c:r f:red]";
			for(int i = cutoff; i < 40; i++)
			{
		
				bar += "-";
					

				
			}
			bar += "[c:u]";

			

			return bar;
		}

		private static void UpdateProgress(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
		{

			Program.root.Print(Program.root.Width / 2 - 20,Program.root.Height / 2, ColoredString.Parser.Parse(GetProgressBarString(downloadProgressChangedEventArgs.ProgressPercentage)));

		}

		private static void Finalise(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
		{
			
			if (asyncCompletedEventArgs.Cancelled || asyncCompletedEventArgs.Error != null)
			{
				Program.root.Print(Program.root.Width / 2 - 11,Program.root.Height / 2+1,ColoredString.Parser.Parse("[c:r f:red]There has been an error[c:u]"));
				return;
			}


			
			ZipFile.ExtractToDirectory("downloaded.zip",Directory.GetCurrentDirectory());
			Program.root.Clear(Program.root.Width / 2 - 30,Program.root.Height / 2,100);
			Program.root.Print(Program.root.Width / 2 - 8,Program.root.Height / 2,ColoredString.Parser.Parse("[c:r f:green]Ready To Launch![c:u]"));
			Program.ReadyToLaunch = true;
			File.Delete("downloaded.zip");

		}
	}
}