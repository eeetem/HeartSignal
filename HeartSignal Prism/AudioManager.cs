using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Media;
using System.ComponentModel;
using csvorbis;
using csogg;
namespace HeartSignal
{
	static class AudioManager
	{


		
		public static void ParseRequest(string ID, string sfxFile, string request)
		{


			switch (request)
			{
				case "preload":
					download(sfxFile);
					break;
				case "play":
					if (download(sfxFile))
					{
						playSound(ID, sfxFile);
					}
					break;
				case "forceplay":
					if (download(sfxFile, true))
					{
						playSound(ID, sfxFile);
					}
					else
					{
						DownloadAwaiters.Add(new string[] { ID, sfxFile });

					}
					break;


			}


		}
		static List<string[]> DownloadAwaiters = new List<string[]>();
		/// <summary>
		/// returns true if file already exists, false if dowload was started instead
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public static bool download(string file, bool doCallback = false)
		{
			if (File.Exists("sfx/" + file)) { return true; }

			string dir = Directory.GetCurrentDirectory() + "/sfx/" + file.Remove(file.LastIndexOf("/"), file.Length - file.LastIndexOf("/"));
			Directory.CreateDirectory(dir);
			using (var client = new WebClient())
			{
				if (doCallback)
				{
					client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
				}
				client.DownloadFileAsync(new Uri("http://deathcult.today/soundhell/" + file), "sfx/" + file);
			}
			return false;

		}


		private static void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
		{
			foreach (string[] downloadAwaiter in new List<string[]>(DownloadAwaiters))
			{
				if (File.Exists("sfx/" + downloadAwaiter[1]))
				{
					playSound(downloadAwaiter[0], downloadAwaiter[1]);
					DownloadAwaiters.Remove(downloadAwaiter);


				}


			}

		}



		private static void playSound(string id, string path)
		{
			path = "sfx/" + path;
			//TODO IMPLEMENT OTHER PLATFORMS ALSO TURN THIS INTO COMPILE TIME IF RATHER THAN RUNTIME
			if (OperatingSystem.IsWindows())
			{
				SoundPlayer player;
				if (path.EndsWith(".ogg"))
				{
					using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
					{
						player = new SoundPlayer(new OggDecoder.OggDecodeStream(file));

					}
				}
				else
				{

					player = new System.Media.SoundPlayer();
					player.SoundLocation = path;
				}
				player.Load();
				player.Play();
			}

		}
		



	}
}