using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SadConsole;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.UI;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using Color = SadRogue.Primitives.Color;
using Point = SadRogue.Primitives.Point;

namespace HeartSignal
{
	static class Program
	{
		public static ClassicConsole MainConsole;
		public static DisplayConsole RoomConsole;
		static DisplayConsole ThingConsole;
		static MapConsole MapConsole;
		public static PromptWindow PromptWindow;
		public static InventoryConsole InventoryConsole;
		public static InventoryConsole ExamInventoryConsole;
		public static InventoryConsole GrasperConsole;
		public static DelayConsole delayConsole;
		static ButtonConsole buttonConsole;
		private static Console root;
		public static LoginConsole loginConsole;

		public static bool verboseDebug;


		[STAThread]
		private static void Main()
		{
	


			var SCREEN_WIDTH = (96 * 2) + 30;
			var SCREEN_HEIGHT = 54 + 5;
			

			Settings.WindowTitle = File.ReadAllText("tagline.txt");
			File.WriteAllText("debuglog.txt", "Beginning log for current session:\n");

			Settings.UseDefaultExtendedFont = true;

			Settings.AllowWindowResize = true;
			
			Settings.DoFinalDraw = false;

			
			Library.Default.Colors.Lines = new AdjustableColor(Color.Red, "red");

			

			Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
			//Game.Instance.DefaultFont = new fon
			
			Game.Instance.OnStart = Init;
			Game.Instance.Run();

			
			Game.Instance.Dispose();

		}



		private static void Init()
		{
			SadConsole.Host.Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			Game.Instance.MonoGameInstance.Components.Add(new PostPorcessing());
			ColoredString.CustomProcessor = Utility.CustomParseCommand;
			root = new Console(1, 1);

			MainConsole = new ClassicConsole(1, 1);
			root.Children.Add(MainConsole);
			MapConsole = new MapConsole(1, 1);
			root.Children.Add(MapConsole);
			RoomConsole = new DisplayConsole(1, 1);
			root.Children.Add(RoomConsole);
			ThingConsole = new DisplayConsole(1, 1);
			ThingConsole.ExplicitLook = true;
			root.Children.Add(ThingConsole);
			InventoryConsole = new InventoryConsole(1, 1);
			InventoryConsole.tagline = "My Body";
			InventoryConsole.self = true;
			root.Children.Add(InventoryConsole);
			ExamInventoryConsole = new InventoryConsole(1, 1);
			ExamInventoryConsole.tagline = "Their Body";
			root.Children.Add(ExamInventoryConsole);
			GrasperConsole = new InventoryConsole(1, 1);
			GrasperConsole.tagline = "I can hold with";
			root.Children.Add(GrasperConsole);
			buttonConsole = new ButtonConsole(1, 1);
			root.Children.Add(buttonConsole);
			delayConsole = new DelayConsole(1, 1);
			root.Children.Add(delayConsole);
            
#if RELEASE

			loginConsole = new LoginConsole(1, 1);
			
#endif
            
            

			PromptWindow = new PromptWindow(40, 10, new Point(Width / 2 - 15, Height / 2 - 5));

			root.Children.Add(PromptWindow);



			PositionConsoles();
#if RELEASE
			Game.Instance.Screen = loginConsole;
#else

            Game.Instance.Screen = root;
#endif
            
			// This is needed because we replaced the initial screen object with our own.
			Game.Instance.DestroyDefaultStartingConsole();

  
			Settings.ResizeMode = Settings.WindowResizeOptions.None;
			Game.Instance.MonoGameInstance.WindowResized += (s, a) => PositionConsoles();
			
			NetworkManager.ConnectToServer();





		}

		public static int Width = 0;
		public static int Height;


		static void PositionConsoles()
		{
            

			Program.Width = Game.Instance.MonoGameInstance.WindowWidth / root.FontSize.X;
			Program.Height = Game.Instance.MonoGameInstance.WindowHeight / root.FontSize.Y;
#if  RELEASE
			if (loginConsole != null)
			{
				//LoginConsole.ImageDrawThread?.Interrupt();//bad things happen if we dont due to texture size and surface size mismatch
				
				loginConsole.Resize(Program.Height*2, Program.Height, Height*2, Program.Height, false);
				loginConsole.MakeControlls();
				PostPorcessing.RemakeRenderTarget();
				return;
			}
#endif

            
            
			root.Resize(Program.Width, Program.Height, Program.Width, Program.Height, false);


			int MapConsoleHeight = 7;
			int inventoryWidth = 29;
			int roomConsoleWidth = (Program.Width - (inventoryWidth * 3)) / 2;
			int barConsoleHeight = 6;//ONLY EVEN due to map console size increase
			int topConsoleRowHeight = 20;


			int width = Program.Width - (inventoryWidth * 2) - 2;
			int height = Program.Height - (topConsoleRowHeight + barConsoleHeight + 4);
			InputConsole input = MainConsole.GetInputSource();
			input.Resize(width, 30, width, 30, false);//fun fact: input console is gigantic - just hidden under
			input.Position = new Point(0, height + 2);
			input.Cursor.Position = new Point(0, 0);
			input.Clear();
			input.Cursor.Print(">");
			if (height > 24)
			{
				MainConsole.FontSize = MainConsole.Font.GetFontSize(IFont.Sizes.Two);
				width = width / 2;
				height = height / 2;
			}
			else
			{
				MainConsole.FontSize = MainConsole.Font.GetFontSize(IFont.Sizes.One);
			}

			MainConsole.Resize(width , height , width , 256, false);
			MainConsole.Position = new Point((inventoryWidth + 2) / 2, (topConsoleRowHeight + barConsoleHeight) / 2);


			width = (inventoryWidth / 2) + 1;
			height = MapConsoleHeight;
			MapConsole.Resize(width, height, width, height, false);
			MapConsole.Position = new Point((Program.Width / 2) - (inventoryWidth / 2) , (barConsoleHeight) / 2);
			MapConsole.ReDraw();

			width = inventoryWidth;
			height = MapConsoleHeight/2;
			delayConsole.Resize(width, height, width, 100, false);
			delayConsole.Position = new Point(0 , barConsoleHeight);
			delayConsole.ReDraw();

			width = roomConsoleWidth - 1;
			height = topConsoleRowHeight;
			ThingConsole.Resize(width, height, width, 100, true);
			ThingConsole.Position = new Point(inventoryWidth + 1, barConsoleHeight);
			ThingConsole.ReDraw();


			PromptWindow.Position = new Point(Program.Width / 2 - 15, Program.Height / 2 - 5);

			width = roomConsoleWidth - 3;
			height = topConsoleRowHeight;
			RoomConsole.Resize(width, height, width, 100, true);
			RoomConsole.Position = new Point(inventoryWidth * 2 + roomConsoleWidth + 2, barConsoleHeight);
			RoomConsole.ReDraw();


			
			width = inventoryWidth;
			height = Program.Height - (MapConsoleHeight * 2) - barConsoleHeight-1;
			ExamInventoryConsole.Resize(width, height, width, 100, true);
			ExamInventoryConsole.Position = new Point(Program.Width - inventoryWidth, (MapConsoleHeight * 2) + 1 + barConsoleHeight);
			ExamInventoryConsole.ActionOffset = new Point(-30, 1);
			ExamInventoryConsole.ReDraw();

			InventoryConsole.Resize(width, height, width, 100, true);
			InventoryConsole.Position = new Point(0, (MapConsoleHeight * 2) + 1 + barConsoleHeight);
			InventoryConsole.ActionOffset = new Point(10, 1);
			InventoryConsole.ReDraw();





			width = inventoryWidth;
			height = topConsoleRowHeight;
			GrasperConsole.Resize(width, height, width, 100, false);
			GrasperConsole.Position = new Point(inventoryWidth + roomConsoleWidth + 1, barConsoleHeight);
			GrasperConsole.ActionOffset = new Point(0, 1);
			GrasperConsole.clickableFirstLayer = false;
			GrasperConsole.ReDraw();

			width = Program.Width;
			height = barConsoleHeight - 1;
			buttonConsole.Resize(width, height, width, height, true);

			PostPorcessing.RemakeRenderTarget();





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

		public static void ParseServerMessage(string input)
		{


			int idx = input.IndexOf(':');
			if (idx > 0 && input.Contains("[tag]"))
			{
				
				string sub = input.Substring(0, idx).Replace("[tag]","");
				string cutstring = input;
				string[] returned;
				List<string> args;
				//  System.Console.WriteLine(input);
				switch (sub)
				{

					//a lot of parse repeating - turn this into a function at some point - me from the future: turned some bits into functions however there is still shitload of repeating, needs quite a big refactor
					case "desc":
						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];
						
						ThingConsole.lines = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						ThingConsole.ReDraw();
						break;
					case "room":
						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];

						RoomConsole.lines = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						RoomConsole.ReDraw();
						break;
					case "actions":
						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];

						args = returned[1].Split("-").ToList();
						if (!ActionWindow.actionDatabase.ContainsKey(args[0]))
						{
							ActionWindow.actionDatabase[args[0]] = new Dictionary<string, List<string>>();
						}

						ActionWindow.actionDatabase[args[0]][args[1]] =
							ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						break;
					//obsolete
					case "argactions":
						MainConsole.ReciveExternalInput("obsolete parsing tag recived:");
						MainConsole.ReciveExternalInput(cutstring);
						break;
					case "buttons":
						returned = RemoveParseTag(cutstring);
						args = ExtractQuotationStrings(returned[0]);
						buttonConsole.MakeButtons(args);
						break;

					case "bars":
						MainConsole.ReciveExternalInput("obsolete parsing tag recived:");
						MainConsole.ReciveExternalInput(cutstring);
						/*	returned = RemoveParseTag(cutstring);
							cutstring = returned[0];
	
	
							BarConsole.AddBar(returned[1],
								ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}'))));*/
						break;
					//[tag]delay:attack{"1235","1234","64:64:64}
					case "delay":
						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];

						args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						if (args.Count != 3)
						{
							MainConsole.ReciveExternalInput("ERROR: incorrect arguments supplied to a delay");
							return;
						}

						bool keep1 = false;
						string[] colors = args[2].Split(".");
						Color[] Colors = new Color[colors.Length];
						int index=0;
						foreach (string color in colors) {

							Colors[index] = Color.White.FromParser(color, out keep1, out keep1, out keep1, out keep1, out keep1);
							index++;
						}

						;
						delayConsole.DisplayDelay(returned[1],new Gradient(Colors) ,float.Parse(args[0]),float.Parse(args[1]));
						break;
					case "map":
						

						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];
						try
						{
							MapConsole.mapdata =
								ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						}
						catch (Exception E)
						{
							MainConsole.ReciveExternalInput("Map Exception, please report this : "+E);
							
						}

						MapConsole.ReDraw();
						break;
					case "cexits":

						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];


						MapConsole.cexists = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						MapConsole.ReDraw();
						break;
					case "inventory":


						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];

						NestedInfo info2 = new NestedInfo(null, null);

						while (cutstring.Contains('{'))
						{
							NestedInfo innerinfo = GetNestedBrackets(cutstring);
							info2.Contents.Add(innerinfo);
							int[] innerindexes = GetOutermostBrackets(cutstring);
							cutstring = cutstring.Remove(0, innerindexes[1] + 2).Replace(",", "").Trim();
						}
						InventoryConsole.tagline = returned[1];
						InventoryConsole.inventoryInfo = info2;
						InventoryConsole.ReDraw();
						break;
					case "examine":


						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];

						NestedInfo info3 = new NestedInfo(null, null);

						while (cutstring.Contains('{'))
						{
							NestedInfo innerinfo = GetNestedBrackets(cutstring);
							info3.Contents.Add(innerinfo);
							int[] innerindexes = GetOutermostBrackets(cutstring);
							cutstring = cutstring.Remove(0, innerindexes[1] + 2).Replace(",", "").Trim();
						}
						ExamInventoryConsole.tagline = returned[1];
						ExamInventoryConsole.inventoryInfo = info3;
						ExamInventoryConsole.ReDraw();
						break;
					case "holding":

						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];
						NestedInfo info = new NestedInfo(null, null);

						while (cutstring.Contains('{'))
						{
							NestedInfo innerinfo = GetNestedBrackets(cutstring);
							info.Contents.Add(innerinfo);
							int[] innerindexes = GetOutermostBrackets(cutstring);
							cutstring = cutstring.Remove(0, innerindexes[1] + 2).Replace(",", "").Trim();
						}
						GrasperConsole.tagline = returned[1];
						GrasperConsole.inventoryInfo = info;
						GrasperConsole.ReDraw();
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
						string effectToAdjust = returned[1];//for when we get more effects
						args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						PostPorcessing.AddTween(args[0],float.Parse(args[1]),float.Parse(args[2]));
					
						break;

					case "border":
						cutstring = cutstring.Remove(0, cutstring.IndexOf(":") + 1);
						string[] settings = cutstring.Split("-");

						bool keep = false;
						AnimatedBorderComponent._borderCellStyle = new ColoredGlyph(Color.White.FromParser(settings[0], out keep, out keep, out keep, out keep, out keep), Color.Black);
						Utility.GlobalAnimationSpeed = float.Parse(settings[1]);


						break;
					case "prompt":

						returned = RemoveParseTag(cutstring);
						cutstring = returned[0];
						string[] titles = returned[1].Split(";");
						PromptWindow.toptext = titles[0];
						PromptWindow.middletext = titles[1];
						List<string> args4 = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
						if (args4.Count > 0)
						{
							PromptWindow.Type = PromptWindow.PopupType.Choice;
							PromptWindow.args = args4;
						}
						else
						{
							PromptWindow.Type = PromptWindow.PopupType.Text;
							PromptWindow.args = null;

						}


						PromptWindow.needsDraw = true;


						break;



					case "exits":

						//todo
						break;
					case "accept":
#if RELEASE
			
						Game.Instance.Screen = root;
						Thread.Sleep(100);//let all the renders and whatever finidh up before removing the login console 
						if (loginConsole != null)
						{
							loginConsole.Dispose();
							loginConsole = null;
						}
						
						PositionConsoles();
						
#endif
						break;
					case "tagline":

						if (loginConsole != null)
						{
							loginConsole.Tagline = cutstring.Remove(0, cutstring.IndexOf(":", StringComparison.Ordinal)+1);
							Settings.WindowTitle = loginConsole.Tagline;
							loginConsole.MakeSurfaceImage();
						}

						
						break;

					default:
						if (verboseDebug)
						{
							System.Console.WriteLine("unkown parsing tag: " + sub);
						}
						//if we couldn't parse it - it's possibly not meant to be parsed - print it
						if (loginConsole != null)
						{
							loginConsole.miniDisplay.Cursor.Print(input).NewLine();
						}
						else
						{

							MainConsole.ReciveExternalInput(input);
						}

						break;

				}

			}
			else
			{
				if (loginConsole != null)
				{
					loginConsole.miniDisplay.Cursor.Print(input).NewLine();
				}
				else
				{

					MainConsole.ReciveExternalInput(input);
				}
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


	}
}