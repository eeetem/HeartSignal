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
using Microsoft.Xna.Framework.Audio;

namespace HeartSignal
{
	static class AudioManager
	{


		
		public static void ParseRequest(string ID, string sfxFile, string request)
		{
			float param = 0;
			if (request.Contains("-")) {
				param = float.Parse(request.Substring(request.IndexOf("-"), request.Length - request.IndexOf("-")));
				request = request.Remove(request.IndexOf("-"), request.Length - request.IndexOf("-"));
			}
			switch (request)
			{
				case "preload":
					download(sfxFile);
					break;
				case "play":
					if (Sounds.ContainsKey(ID)) {
						Sounds[ID].Play();
					
					
					}
					else if (download(sfxFile))
					{
						playSound(ID, sfxFile);
					}
					break;
				case "forceplay":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Play();


					}
					else if(download(sfxFile, true))
					{
						playSound(ID, sfxFile);
					}
					else
					{
						DownloadAwaiters.Add(new string[] { ID, sfxFile });

					}
					break;
				case "loop":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].IsLooped = true;
						Sounds[ID].Play();


					}
					else if(download(sfxFile, true))
					{
						playSound(ID, sfxFile,true);
					}
					else
					{
						DownloadAwaiters.Add(new string[] { ID, sfxFile ,"loop"});

					}
					break;
				case "pause":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Stop();


					}
					break;
				case "stop":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Stop();
						Sounds[ID].Dispose();
						Sounds.Remove(ID);

					}
					break;
				case "reasign":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[param.ToString()] = Sounds[ID];
						Sounds[ID] = null;
					}
					break;
				case "pan":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Pan = param;
					}
					break;

				case "pitch":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Pitch = param;
					}
					break;
				case "volume":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Volume = param;
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
			string dir;
			if (file.Contains("/"))
			{
				dir = Directory.GetCurrentDirectory() + "/sfx/" + file.Remove(file.LastIndexOf("/"), file.Length - file.LastIndexOf("/"));
			}
			else {
				dir = Directory.GetCurrentDirectory() + "/sfx/";


			}
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
					playSound(downloadAwaiter[0], downloadAwaiter[1], downloadAwaiter[2] =="loop");
					DownloadAwaiters.Remove(downloadAwaiter);


				}


			}

		}
		static Dictionary<string, SoundEffectInstance> Sounds = new Dictionary<string, SoundEffectInstance>();


		private static void playSound(string id, string path,bool loop =false)
		{
			try
			{
				path = "sfx/" + path;
				//TODO IMPLEMENT OTHER PLATFORMS ALSO TURN THIS INTO COMPILE TIME IF RATHER THAN RUNTIME

				if (OperatingSystem.IsWindows())
				{
					SoundEffectInstance player;
					if (path.EndsWith(".ogg"))
					{
						using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
						{
							player = SoundEffect.FromStream((new OggDecoder.OggDecodeStream(file))).CreateInstance();

						}
					}
					else
					{

						player = SoundEffect.FromFile(path).CreateInstance();
					}
					if (loop)
					{
						player.IsLooped = true;
					}
				
						player.Play();

					

					Sounds.Add(id, player);

				}
			}
			catch (Exception e) {

				System.Console.WriteLine("audio error:" + e);
				Program.MainConsole.DrawMessage("An error has occured trying to play audio file: " + path);
				Program.MainConsole.DrawMessage("clearing audio cache may fix this - if the problem persists please inform the developers");
#if DEBUG
				Program.MainConsole.DrawMessage("the exception has been printed into dev console.");
#endif

			}
			
			

		}
		



	}
}