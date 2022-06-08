using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole;
using SadRogue.Primitives;
using Console = System.Console;

namespace HeartSignal
{
	public static class MessageParser
	{
		
		public static void ParseServerMessage(string input)
		{
		
			int idx = input.IndexOf(':');
			if (idx > 0 && input.Contains("[tag]"))
			{
				TagParse(input, idx);
			}
			else if (input.Contains("[JSON]"))
			{
				JsonParse(input);
			}
			else
			{
				if (Program.loginConsole != null)
				{
					Program.loginConsole.miniDisplay.Cursor.Print(input).NewLine();
				}
				else
				{

					Program.MainConsole.ReciveExternalInput(input);
				}
			}




		}

		private static void JsonParse(string input)
		{
			JObject jsonObj;
			try
			{
				input = input.Replace("[JSON]", "");
				jsonObj = JsonConvert.DeserializeObject(input) as JObject;

				if (jsonObj["type"] == null)
				{
					Program.MainConsole.ReciveExternalInput("Error parsing: " + input);
					return;
				}
			}
			catch (Exception e)
			{
				Program.MainConsole.ReciveExternalInput("failed to parse: "+input + e);
				return;
			}


			switch (jsonObj["type"].ToString())
			{
				
				case "inventory":

					Program.InventoryConsole.inventoryInfo = MakeNestedInfo((JObject)jsonObj["root"]);
					Program.InventoryConsole.ReDraw();
					break;
				case "holding":
					Program.MainConsole.ReciveExternalInput("obsolete parsing tag recived: holding");
					break;
				case "exam":

					Program.ExamInventoryConsole.inventoryInfo = MakeNestedInfo((JObject)jsonObj["root"]);
					Program.ExamInventoryConsole.ReDraw();
					break;
				case "login":

					if (Program.loginConsole != null)
					{
						Program.loginConsole.Tagline = jsonObj["tagline"].ToString();
						Settings.WindowTitle = Program.loginConsole.Tagline;
						Program.loginConsole.MakeSurfaceImage();
						
						
						Utility.GetImageOrDownload(jsonObj["background"].ToString());//just for the sake of download
						Program.loginConsole.baseData =  File.ReadAllBytes("img/" + jsonObj["background"]);
					}


					break;
				case "desc":
					Program.RoomConsole.Examine(jsonObj["contents"].ToString());
					break;
				case "room":
					if (jsonObj["contents"] != null)
					{
						Program.RoomConsole.contents = jsonObj["contents"].ToString();
					}

					if (jsonObj["things"] != null)
					{
						
						List<string> things = new List<string>();
						foreach (var jToken in jsonObj["things"])
						{

							things.Add(jToken.ToString());


						}

						Program.RoomConsole.things = things;
					}

				
					Program.RoomConsole.ReDraw();
					break;
				case "var":
					Utility.SetVar(jsonObj["name"].ToString(),jsonObj["value"].ToString());
					break;
				case "thingdata":
					string id = jsonObj["id"].ToString();
					string desc = " ";
					string name= " ";
					Color? color = null;
					
					if (jsonObj["name"] != null)
					{
						name = jsonObj["name"].ToString();
						
					}

					if (jsonObj["desc"] != null)
					{
						desc = jsonObj["desc"].ToString();
					}

					if (jsonObj["color"] != null)
					{
						bool keep1 = false;
						color = Color.White.FromParser(jsonObj["color"].ToString(), out keep1, out keep1, out keep1, out keep1, out keep1);
					}
					

					ThingDatabase.ThingData oldData;
					if (ThingDatabase.thingDatabase.TryGetValue(id, out oldData))
					{
						if (name == " ")
						{
							name = oldData.name;

						}

						if (desc == " ")
						{
							desc = oldData.desc;
						}

						if (color == null)
						{
							color = oldData.color;
						}
					}
					
					
					Dictionary<string, List<string>> actionDatabase = new Dictionary<string, List<string>>();
					foreach (var jToken in jsonObj["actions"])
					{
						var actionList = (JProperty) jToken;
						string tabName = actionList.Name;
						var array = (JArray) actionList.Value;
						List<string> actions = new List<string>();
						foreach (var action in array)
						{
							actions.Add(action.ToString());
						}
						actionDatabase.Add(tabName,actions);


					}

					lock (ThingDatabase.DatabaseSyncObj)
					{
						ThingDatabase.ThingUpdate updaters = null;
						
						
						
						if (ThingDatabase.thingDatabase.ContainsKey(id))
						{
							updaters = ThingDatabase.thingDatabase[id].updateEvent;
							ThingDatabase.thingDatabase.Remove(id);

						}


						ThingDatabase.ThingData data = new ThingDatabase.ThingData(name,desc);
						data.actionDatabase = actionDatabase;
						data.updateEvent = updaters;

						if (color != null)
						{
							
							data.color = (Color) color;
						}


						ThingDatabase.thingDatabase.Add(id, data);

						if (ThingDatabase.thingDatabase[id].updateEvent != null)
						{
							ThingDatabase.thingDatabase[id].updateEvent.Invoke();
						}
						
					}

					break;
				case "status":
					foreach (var jToken in jsonObj["tabs"])
					{
						JProperty property = (JProperty) jToken;
						string tabname = property.Name;
						string contents = property.Value.ToString();
						Program.StatusConsole.tabs.Remove(tabname);
						Program.StatusConsole.tabs.Add(tabname,contents);
						Program.StatusConsole.ReDraw();
					}
					break;

			}
			


		}

		private static NestedInfo MakeNestedInfo(JObject jsonObj)
		{
			NestedInfo info = new NestedInfo();
			info.Header = jsonObj["name"].ToString();
			info.Contents = new List<NestedInfo>();
			int index = 1;
			while (jsonObj[index.ToString()] != null)
			{
				info.Contents.Add(MakeNestedInfo((JObject)jsonObj[index.ToString()]));
				index++;
			}

			return info;

		}

		private static void TagParse(string input, int idx)
		{
#if Debug
				System.Console.WriteLine(input);
#endif
			string sub = input.Substring(0, idx).Replace("[tag]", "");
			string cutstring = input;
			string[] returned;
			List<string> args;
			//  System.Console.WriteLine(input);
			switch (sub)
			{
				//a lot of parse repeating - turn this into a function at some point - me from the future: turned some bits into functions however there is still shitload of repeating, needs quite a big refactor

				case "actions":
					return;
					/*
					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];

					args = returned[1].Split("-").ToList();
					if (!ActionWindow.actionDatabase.ContainsKey(args[0]))
					{
						ActionWindow.actionDatabase[args[0]] = new Dictionary<string, List<string>>();
					}

					ActionWindow.actionDatabase[args[0]][args[1]] =
						ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					foreach ( KeyValuePair<string,ActionWindow> ac in new Dictionary<string, ActionWindow>(ActionWindow.activeWindows))
					{
						if (ac.Key == args[0])
						{
							ac.Value.DisplayActions();
						}
					}*/
					break;
				//obsolete
				case "desc":
				case "room":
				case "argactions":
				case "bars":
					Program.MainConsole.ReciveExternalInput("obsolete parsing tag recived:");
					Program.MainConsole.ReciveExternalInput(cutstring);
					break;
				case "buttons":
					returned = RemoveParseTag(cutstring);
					args = ExtractQuotationStrings(returned[0]);
					Program.buttonConsole.MakeButtons(args);
					break;
				case "exits":

					//todo
					break;


				//[tag]delay:attack{"1235","1234","64:64:64}
				case "delay":
					return;
					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];

					args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					if (args.Count != 3)
					{
						Program.MainConsole.ReciveExternalInput("ERROR: incorrect arguments supplied to a delay");
						break;
					}

					bool keep1 = false;
					string[] colors = args[2].Split(".");
					Color[] Colors = new Color[colors.Length];
					int index = 0;
					foreach (string color in colors)
					{
						Colors[index] = Color.White.FromParser(color, out keep1, out keep1, out keep1, out keep1, out keep1);
						index++;
					}

					
					//Program.delayConsole.DisplayDelay(returned[1], new Gradient(Colors), float.Parse(args[0], CultureInfo.InvariantCulture), float.Parse(args[1], CultureInfo.InvariantCulture));
					break;
				case "map":


					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];
					try
					{
						Program.MapConsole.mapdata =
							ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					}
					catch (Exception E)
					{
						Program.MainConsole.ReciveExternalInput("Map Exception, please report this : " + E);
					}

					Program.MapConsole.ReDraw();
					break;
				case "cexits":

					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];


					Program.MapConsole.cexists = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					Program.MapConsole.ReDraw();
					break;
				case "version":


					string localversion = File.ReadAllText(@"Version.ver");
					string serverversion = cutstring.Remove(0, cutstring.IndexOf(":", StringComparison.Ordinal) + 1);
					if (serverversion != localversion)
					{
						Program.PromptWindow.toptext = "outdated version";
						Program.PromptWindow.middletext = "  Your version of the game is outdated, use the launcher to update";
						Program.PromptWindow.Type = PromptWindow.PopupType.Permanent;
						Program.PromptWindow.needsDraw = true;
					}


					break;
				case "sound":
					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];
					args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					AudioManager.ParseRequest(returned[1], args[0], args[1]);

					break;
				case "effect":
					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];
					string extraparam = returned[1];
					args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					PostPorcessing.AddTween(args[0], float.Parse(args[1], CultureInfo.InvariantCulture), float.Parse(args[2], CultureInfo.InvariantCulture), extraparam == "wipe");

					break;

				case "overlay":
					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];
					//string effectToAdjust = returned[1];//for when we get more effects
					string base64 = cutstring.Substring(0, cutstring.IndexOf('}'));
					PostPorcessing.SetOverlay(base64);
					break;

				case "border":
					cutstring = cutstring.Remove(0, cutstring.IndexOf(":") + 1);
					string[] settings = cutstring.Split("-");

					bool keep = false;

					AnimatedBorderComponent._borderCellStyle = new ColoredGlyph(
						Color.White.FromParser(settings[0], out keep, out keep, out keep, out keep, out keep),
						Color.Black);

					Utility.GlobalAnimationSpeed = float.Parse(settings[1], CultureInfo.InvariantCulture);


					break;
				case "prompt":

					returned = RemoveParseTag(cutstring);
					cutstring = returned[0];
					string[] titles = returned[1].Split(";");
					Program.PromptWindow.toptext = titles[0];
					Program.PromptWindow.middletext = titles[1];
					List<string> args4 = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
					if (args4.Count > 0)
					{
						Program.PromptWindow.Type = PromptWindow.PopupType.Choice;
						Program.PromptWindow.args = args4;
					}
					else
					{
						Program.PromptWindow.Type = PromptWindow.PopupType.Text;
						Program.PromptWindow.args = null;
					}


					Program.PromptWindow.needsDraw = true;


					break;

				
				case "accept":


					Program.loginConsole?.Delete();

					break;

				default:

					System.Console.WriteLine("unkown parsing tag: " + sub);

					//if we couldn't parse it - it's possibly not meant to be parsed - print it
					if (Program.loginConsole != null)
					{
						Program.loginConsole.miniDisplay.Cursor.Print(input).NewLine();
					}
					else
					{
						Program.MainConsole.ReciveExternalInput(input);
					}

					break;
			}
		}

		private static NestedInfo GetNestedBrackets(string text)
		{


			int[] indexes = GetOutermostBrackets(text);
			string thingid = text.Substring(0, indexes[0]);
			NestedInfo info = new NestedInfo(thingid, null);
			string innerbracket = text.Substring(indexes[0] + 1, indexes[1] - (indexes[0] + 1));

			while (innerbracket.Contains('{'))
			{
				NestedInfo innerinfo = GetNestedBrackets(innerbracket);
				info.Contents.Add(innerinfo);
				int[] innerindexes = GetOutermostBrackets(innerbracket);
				innerbracket = innerbracket.Remove(0, innerindexes[1] + 1).Replace(",", "").Trim();
			}



			if (innerbracket.Length > 1)
			{

				info.Contents.Add(new NestedInfo(innerbracket, null));
			}

			return info;
		}
		private static int[] GetOutermostBrackets(string text)
		{

			int first = text.IndexOf('{');
			int layers = -1;
			int counter = first;
			int last = -1;
			while (counter < text.Length)
			{
				if (text[counter] == '}')
				{
					if (layers == 0)
					{
						last = counter;
						break;

					}
					else
					{

						layers--;
					}


				}
				else if (text[counter] == '{')
				{

					layers++;

				}
				counter++;

			}
			return new int[] { first, last };

		}
		private static string[] RemoveParseTag(string s)
		{



			s = s.Remove(0, s.IndexOf(':') + 1);

			string name = s.Substring(0, s.IndexOf('{'));


			s = s.Remove(0, s.IndexOf('{') + 1);
			return new string[] { s, name };
		}

		
		public static List<string> ExtractQuotationStrings(string s)
		{

			List<string> strings = new List<string>();

			while (true)
			{
				int posFrom = s.IndexOf('"');
				if (posFrom != -1) //if found char
				{
					int posTo = s.IndexOf('"', posFrom + 1);
					if (posTo != -1) //if found char
					{
						strings.Add(s.Substring(posFrom + 1, posTo - posFrom - 1));

						s = s.Remove(0, posTo + 1);//+1 to cut the comma

						continue;

					}
				}
				break;
			}


			return strings;

		}

		
	}
}