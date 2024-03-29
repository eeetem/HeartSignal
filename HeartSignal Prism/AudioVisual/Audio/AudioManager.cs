﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace HeartSignal
{
	static class AudioManager
	{

		private static EventWaitHandle downloadThreadsWairWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
		private static readonly object syncObj = new object();
		public static bool ProcessingSound = false;
		public static void ParseRequest(string ID, string request, string param)
		{
			
			//System.Console.WriteLine("recived " +request+param);
			if (ID == null) {

			 ID = 	new Random().Next(0, 9999).ToString();//this isnt gonna happend often so i dont want to store a random in memory
			}

	
			while (DownloadQueue.FindIndex(x => x.ID == ID) >= 0 ) //if the soundeffect with this id is downloading - wait
			{
				//	System.Console.WriteLine("stopped by download"+request+param);
					
				downloadThreadsWairWaitHandle.WaitOne();
				//after this even is called the sound might still be in the queue so this is a while loop to double check
			}


			lock (syncObj)
			{
				
	
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
							float setting =  Math.Clamp(float.Parse(param,CultureInfo.InvariantCulture), -1, 1);
							Sounds[ID].Pan = setting;
						}
						break;

					case "pitch":
						if (Sounds.ContainsKey(ID))
						{
							float setting =  Math.Clamp(float.Parse(param,CultureInfo.InvariantCulture), -1, 1);
							Sounds[ID].Pitch =setting;
						}
						break;
					case "volume":
						if (Sounds.ContainsKey(ID))
						{
							float setting =  Math.Clamp(float.Parse(param,CultureInfo.InvariantCulture), -1, 1);
							Sounds[ID].Volume = setting;
						}
						break;

					
				}

				downloadThreadsWairWaitHandle.Set();//make all the download queue threads check is they should proceed and get caught by thread lock
				downloadThreadsWairWaitHandle.Reset();//reset the event so nothing gets through when we set processing to false
			}




		}
		static List<DownloadStruct> DownloadQueue= new List<DownloadStruct>();
		static bool DownloadInProgress = false;

		/// <summary>
		/// returns true if file already exists, false if dowload was started instead
		/// </summary>
		public static bool InitiateDownload(string file, string afteraction, string ID) {
			if (File.Exists("sfx/" + file)) { return true; }
			foreach (DownloadStruct param in DownloadQueue) {
				if (param.file == file) {

					return false;//return since the same file is already in queue, however dont return true as then the yet to be downloaded sound will attempt to be played
				}
				
			
			}
			DownloadQueue.Add(new DownloadStruct( file, afteraction, ID ));
			//if nothing is downloading - start the download otherwise the other download will check the download queue after it's finished
			if (!DownloadInProgress) {

				Download();
			}
			return false;
		}


		
		public static void Download()
		{
			DownloadInProgress = true;
			string file = DownloadQueue[0].file;
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
				client.DownloadFileCompleted += DownloadCompleted;
				
				client.DownloadFileAsync(new Uri("http://deathcult.today/soundhell/" + file), "sfx/" + filename);
			}

		}


		private static void DownloadCompleted (object sender, AsyncCompletedEventArgs e)
		{
			DownloadStruct finishedDownload = DownloadQueue[0];
			File.Move("sfx" + finishedDownload.file.Substring(finishedDownload.file.LastIndexOf("/")), "sfx/"+ finishedDownload.file);//move the downloaded file from temp location to proper one

			
			DownloadQueue.Remove(DownloadQueue[0]);
			if (finishedDownload.afteraction.Length >1)//if an after action was supplied now that the sound is downloaded perform that actions
			{
				ParseRequest(finishedDownload.ID, finishedDownload.afteraction, finishedDownload.file);
			}
			
			
			
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

			List<DownloadStruct> newDownloadQueue = new List<DownloadStruct>();
			foreach (var download in DownloadQueue)
			{

				newDownloadQueue.Add( new DownloadStruct(download.file,"",download.ID));
			}

			DownloadQueue = newDownloadQueue;


		}
		private static void playSound(string id, string path,bool loop =false)
		{
			try
			{
				path = "sfx/" + path;

			
					SoundEffectInstance player;
					if (path.EndsWith(".ogg"))
					{
						using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
						{
							//todo caches loaded sound effects instead of loading them every time
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
			catch (Exception e) {

				System.Console.WriteLine("audio error:" + e);
				Program.MainConsole.ReciveExternalInput("An error has occured trying to play audio file: " + path);
				Program.MainConsole.ReciveExternalInput("clearing audio cache may fix this - if the problem persists please inform the developers");
#if DEBUG
				Program.MainConsole.ReciveExternalInput("the exception has been printed into dev console.");
#endif

			}
			
			

		}
		

		public struct DownloadStruct
		{
			public DownloadStruct(string file, string afteraction, string id)
			{
				ID = id;
				this.afteraction = afteraction;
				this.file = file;
			}

			public string file;
			public string afteraction;
			public string ID;



		}

	}
}