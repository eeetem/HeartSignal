using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using PrimS.Telnet;
using SadConsole;
using SadConsole.StringParser;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Console = SadConsole.Console;
using Color = SadRogue.Primitives.Color;
using Game = SadConsole.Game;
using Point = SadRogue.Primitives.Point;


namespace UpdateUnpacker
{
	class Program
	{
		
		public static ControlsConsole root;
		public static string version;
		static void Main(string[] args)
		{



			try
			{
				version = File.ReadAllText(@"HeartSignal Prism/Version.ver");
			}
			catch (Exception e)
			{
				version = "NONE";
			}

			Settings.UseDefaultExtendedFont = true;
			Settings.WindowTitle = "HeartSignal Launcher";
			Settings.AllowWindowResize = false;
		
			Library.Default.Colors.Lines = new AdjustableColor(Color.Red, "red");

			Game.Create(50, 20);

			
			SadConsole.Host.Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			SadConsole.Host.Global.GraphicsDeviceManager.ApplyChanges();
			Game.Instance.OnStart = Init;
			Game.Instance.Run();
			
			
			Game.Instance.Dispose();

		}
		
		public static int Width = 0;
		public static int Height;

		public static bool ReadyToLaunch = false;
            

		private static void Init()
		{
			
			

			root = new ControlsConsole(1,1);
			Program.Width = Game.Instance.MonoGameInstance.WindowWidth / root.FontSize.X;
			Program.Height = Game.Instance.MonoGameInstance.WindowHeight / root.FontSize.Y;
			
			root.Resize(Width,Height,true);


			root.Fill(Color.White, Color.Black);
			var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
			root.DrawBox(new Rectangle(0,0,root.Width,root.Height),boxShape);
			
			root.Print(2,2,ColoredString.Parser.Parse("Current Version: [c:r f:red]"+version+"[c:u]"));
			
			
			var button = new Button(10)
			{
				Text = "Launch",
				Position = new Point(root.Width / 2 - 5, (root.Height / 2) + 2),
			};
			button.MouseButtonClicked += (s, a) => LaunchGame();
	
			root.Controls.Add(button);
			root.Print(root.Width / 2 - 12,root.Height / 2,ColoredString.Parser.Parse(" [c:r f:yellow]Checking For Updates...[c:u]"));
			
			Game.Instance.Screen = root;

            
			// This is needed because we replaced the initial screen object with our own.
			Game.Instance.DestroyDefaultStartingConsole();


			Settings.ResizeMode = Settings.WindowResizeOptions.None;


			ServerLoop();
		}

		private static void LaunchGame()
		{
			if(!ReadyToLaunch) return;
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.FileName = "HeartSignal Prism/HeartSignal.exe";
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "/HeartSignal Prism";
			Process p = Process.Start(startInfo);

			Game.Instance.MonoGameInstance.Exit();
		}

		private static void PraseServerMSG(string MSG)
		{
			if (MSG.IndexOf("version:", StringComparison.Ordinal) < 0)
			{
				return;
			}

			MSG = MSG.Substring(MSG.IndexOf("version:") + 8, MSG.Length - (MSG.IndexOf("version:") + 8));
			
			string serverVersion = MSG.Substring(0, MSG.IndexOf("\r"));

			if (serverVersion != version)
			{
				Updater.BeginUpdate();
				version = serverVersion;
				root.Clear(2,2,40);
				root.Print(2,2,ColoredString.Parser.Parse("Current Version: [c:r f:red]"+version+"[c:u]"));

			}
			else
			{
				ReadyToLaunch = true;
				root.Clear(root.Width / 2 - 12,root.Height / 2,30);
				root.Print(root.Width / 2 - 8,root.Height / 2,ColoredString.Parser.Parse("[c:r f:green]Ready To Launch![c:u]"));
				
			}
		}

		private static async void ServerLoop()
        		{
        
        
        			
        			using (Client client = new Client("deathcult.today", 6666, new CancellationToken()))
                    {
	                    await client.TryLoginAsync("", "", 1000);
        
        
	                    TelnetClient = client;
        					
        
        
	                    while (TelnetClient.IsConnected)
	                    {
        				
        					
        
		                    string response = "";
		                    while (true)
		                    {
			                    string recived = await client.ReadAsync(TimeSpan.FromMilliseconds(100));
			                    if (recived.Length > 1)
			                    {
				                    //if we recived something - try reciving again since the message might have been cut halfway
				                    response += recived;
				                    continue;
			                    }
			                    //if nothing was recived in last 100ms process it
			                    break;
        
		                    }
        
        						
		                    if (response.Length > 1)
		                    {
			                    PraseServerMSG(response);

		                    }
        
		                    if (!client.IsConnected)
		                    {
			                    break;
		                    }
		                    //await Task.Delay(50);
	                    }
                    }
        			

        		}

		   public static Client TelnetClient { get; set; }
	}
}