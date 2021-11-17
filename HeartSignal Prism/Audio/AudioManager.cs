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


		
		public static void ParseRequest(string ID, string request, string param)
		{
			if (ID == null) {

			 ID = 	new Random().Next(0, 1000).ToString();//this isnt gonna happend often so i dont want to store a random in memory
			}

			switch (request)
			{
				case "preload":
					InitiateDownload(param,"",ID);
					break;
				case "play":
					if (Sounds.ContainsKey(ID)) {
						Sounds[ID].Play();
					
					
					}
					else if (InitiateDownload(param,"",ID))
					{
						playSound(ID, param);
					}
					break;
				case "stopall":
					StopAllSounds();
					break;
				case "forceplay":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Play();


					}
					else if(InitiateDownload(param, "play", ID))
					{
						playSound(ID, param);
					}
					break;
				case "loop":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].IsLooped = true;
						Sounds[ID].Play();


					}
					else if(InitiateDownload(param, "loop", ID))
					{
						playSound(ID, param, true);
					}
					break;
				case "pause":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[ID].Pause();


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
				case "reassign":
					if (Sounds.ContainsKey(ID))
					{
						Sounds[param.ToString()] = Sounds[ID];
						Sounds.Remove(ID);
					}
					break;
				case "pan":
					if (Sounds.ContainsKey(ID))
					{
						float setting =  Math.Clamp(float.Parse(param), -1, 1);
						Sounds[ID].Pan = setting;
					}
					break;

				case "pitch":
					if (Sounds.ContainsKey(ID))
					{
						float setting =  Math.Clamp(float.Parse(param), -1, 1);
						Sounds[ID].Pitch =setting;
					}
					break;
				case "volume":
					if (Sounds.ContainsKey(ID))
					{
						float setting =  Math.Clamp(float.Parse(param), -1, 1);
						Sounds[ID].Volume = setting;
					}
					break;



			}


		}
		static List<string[]> DownloadQueue= new List<string[]>();
		static bool DownloadInProgress = false;

		/// <summary>
		/// returns true if file already exists, false if dowload was started instead
		/// </summary>
		public static bool InitiateDownload(string file, string afteraction, string ID) {
			if (File.Exists("sfx/" + file)) { return true; }
			foreach (string[] param in DownloadQueue) {
				if (param[0] == file) {

					return false;//return since the same file is already in queue, however dont return true as then the yet to be downloaded sound will attempt to be played
				}
			
			
			
			}
			DownloadQueue.Add(new string[] { file, afteraction, ID });
			//if nothing is downloading - start the download otherwise the other download will check the download queue after it's finished
			if (!DownloadInProgress) {

				Download();
			}
			return false;
		}


		
		public static void Download()
		{
			DownloadInProgress = true;
			string file = DownloadQueue[0][0];
			string dir = "";
			string filename ="";
			if (file.Contains("/"))
			{
				dir = Directory.GetCurrentDirectory() + "/sfx/" + file.Remove(file.LastIndexOf("/"), file.Length - file.LastIndexOf("/"));
				filename = file.Substring(file.LastIndexOf("/")+1);
			}
			else {
				//base sfx folder is used for temp files
				Program.MainConsole.ReciveExternalInput("warning inaproporiate download path for a sound file was specified: " + file);
				return;

			}

			Directory.CreateDirectory(dir);
			using (var client = new WebClient())
			{
				client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadCompleted);
				
				client.DownloadFileAsync(new Uri("http://deathcult.today/soundhell/" + file), "sfx/" + filename);
			}
			return;

		}


		private static void DownloadCompleted (object sender, AsyncCompletedEventArgs e)
		{
			string[] finishedDownload = DownloadQueue[0];
			File.Move("sfx" + finishedDownload[0].Substring(finishedDownload[0].LastIndexOf("/")), "sfx/"+ finishedDownload[0]);//move the downloaded file from temp location to proper one
			if (finishedDownload[1].Length >1)//if an after action was supplied now that the sound is downloaded perform that actions
			{
				ParseRequest(finishedDownload[2], finishedDownload[1], finishedDownload[0]);
			}
			DownloadQueue.Remove(DownloadQueue[0]);
			if (DownloadQueue.Count() > 0)
			{
				Download();
			}
			else {
				DownloadInProgress = false;
			
			}

		}
		static Dictionary<string, SoundEffectInstance> Sounds = new Dictionary<string, SoundEffectInstance>();

		public static void StopAllSounds() {

			foreach (KeyValuePair<string, SoundEffectInstance> sound in Sounds) {

				ParseRequest(sound.Key, "stop",null);
			
			}
		
		
		}
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
				Program.MainConsole.ReciveExternalInput("An error has occured trying to play audio file: " + path);
				Program.MainConsole.ReciveExternalInput("clearing audio cache may fix this - if the problem persists please inform the developers");
#if DEBUG
				Program.MainConsole.ReciveExternalInput("the exception has been printed into dev console.");
#endif

			}
			
			

		}
		



	}
}