using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.StringParser;
using SadConsole.UI;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using Color = SadRogue.Primitives.Color;
using Game = SadConsole.Game;
using Point = SadRogue.Primitives.Point;

namespace HeartSignal
{
	static class Program
	{
		public static ClassicConsole MainConsole;
		public static DisplayConsole RoomConsole;
		public static DisplayConsole ThingConsole;
		public static MapConsole MapConsole;
		public static PromptWindow PromptWindow;
		public static InventoryConsole InventoryConsole;
		public static InventoryConsole ExamInventoryConsole;
		public static InventoryConsole GrasperConsole;
		public static DelayConsole delayConsole;
		public static ButtonConsole buttonConsole;
		public static Console root;
		public static LoginConsole loginConsole;

		//public static bool verboseDebug;


		[STAThread]
		private static void Main()
		{
	
			AppDomain currentDomain = default(AppDomain);
			currentDomain = AppDomain.CurrentDomain;
			// Handler for unhandled exceptions.
			currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

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

			
			SadConsole.Host.Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			SadConsole.Host.Global.GraphicsDeviceManager.ApplyChanges();
			Game.Instance.OnStart = Init;
			Game.Instance.Run();
			
			
			Game.Instance.Dispose();

		}



		private static void Init()
		{
			
			
			Game.Instance.MonoGameInstance.Components.Add(new PostPorcessing());
			Default d = (Default) ColoredString.Parser;
			d.CustomProcessor = Utility.CustomParseCommand;
			
			
			
			root = new Console(1, 1);
			root.SadComponents.Add(new KeyBinds());

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


			root.Children.Add(InventoryConsole);
			ExamInventoryConsole = new InventoryConsole(1, 1);

			root.Children.Add(ExamInventoryConsole);
			GrasperConsole = new InventoryConsole(1, 1);

			root.Children.Add(GrasperConsole);
			buttonConsole = new ButtonConsole(1, 1);
			root.Children.Add(buttonConsole);
			delayConsole = new DelayConsole(1, 1);
			root.Children.Add(delayConsole);

			loginConsole = new LoginConsole(1, 1);
			
   
            

			PromptWindow = new PromptWindow(76, 10, new Point(Width / 2 - 38, Height / 2 - 5));

			
			loginConsole.Children.Add(PromptWindow);

			
			PositionConsoles();

			Game.Instance.Screen = loginConsole;

            
			// This is needed because we replaced the initial screen object with our own.
			Game.Instance.DestroyDefaultStartingConsole();


			Settings.ResizeMode = Settings.WindowResizeOptions.None;
			Game.Instance.MonoGameInstance.WindowResized += (s, a) => PositionConsoles();
		
			NetworkManager.ConnectToServer();





		}

		public static int Width = 0;
		public static int Height;


		public static void PositionConsoles()
		{
            

			Program.Width = Game.Instance.MonoGameInstance.WindowWidth / root.FontSize.X;
			Program.Height = Game.Instance.MonoGameInstance.WindowHeight / root.FontSize.Y;

			
			PromptWindow.Position = new Point(Program.Width / 2 - (PromptWindow.Width/2), Program.Height / 2 - 5);
			
			if (loginConsole != null)
			{
				//LoginConsole.ImageDrawThread?.Interrupt();//bad things happen if we dont due to texture size and surface size mismatch
				PromptWindow.Position = PromptWindow.Position - new Point((Program.Width / 2) - Program.Height,0);

				loginConsole.Resize(Program.Height*2, Program.Height, Height*2, Program.Height, false);
				loginConsole.MakeControlls();
				PostPorcessing.RemakeRenderTarget();
				return;
			}

            
			

			root.Resize(Program.Width, Program.Height, Program.Width, Program.Height, false);


			int MapConsoleHeight = 7;
			int inventoryWidth = 30;
			int roomConsoleWidth = (Program.Width - (inventoryWidth * 3)) / 2;
			int barConsoleHeight = 6;//ONLY EVEN due to map console size increase
			int topConsoleRowHeight = 15 + (int)(Program.Height* 0.2);
			if (topConsoleRowHeight % 2 != 0)
			{
				topConsoleRowHeight += 1;
			}

			int width = Program.Width - (inventoryWidth * 2) - 2;
			int height = Program.Height - (topConsoleRowHeight + barConsoleHeight+3);
			InputConsole input = MainConsole.GetInputSource();
			input.Resize(width, 2, width, 2, false);//fun fact: input console is gigantic - just hidden under
			input.Position = new Point(0,  height);
			input.ClearInput();
			if (height > 24)
			{
				MainConsole.FontSize = MainConsole.Font.GetFontSize(IFont.Sizes.Two);
				width = width / 2;
				height = height / 2;
				MainConsole.Position = new Point((inventoryWidth + 2) / 2, (topConsoleRowHeight + barConsoleHeight) / 2);

			}
			else
			{
				MainConsole.FontSize = MainConsole.Font.GetFontSize(IFont.Sizes.One);
				MainConsole.Position = new Point((inventoryWidth + 2), (topConsoleRowHeight + barConsoleHeight));
				
			}

			MainConsole.Resize(width , height , width , Math.Max(height,MainConsole.Height), false);
			
			MainConsole.SetRelevantViewPos();


			width = (inventoryWidth / 2) + 1;
			height = MapConsoleHeight;
			MapConsole.Resize(width, height, width, height, false);
			MapConsole.Position = new Point((Program.Width / 2) - (inventoryWidth / 2) , (barConsoleHeight) / 2);
			MapConsole.ReDraw();

			width = inventoryWidth;
			height = MapConsoleHeight*2;
			delayConsole.Resize(width, height, width, 100, false);
			delayConsole.Position = new Point(0 , barConsoleHeight);
			delayConsole.ReDraw();

			width = roomConsoleWidth - 1;
			height = topConsoleRowHeight;
			int posx = inventoryWidth + 1;
			int posy = barConsoleHeight;
			if (height > 31)
			{
				ThingConsole.FontSize = ThingConsole.Font.GetFontSize(IFont.Sizes.Two);
				ThingConsole.actionWindow.FontSize = ThingConsole.actionWindow.Font.GetFontSize(IFont.Sizes.Two);
				width = width / 2;
				height = height / 2;
				posx = posx / 2;
				posy = posy / 2;
			}
			else
			{
				ThingConsole.FontSize = ThingConsole.Font.GetFontSize(IFont.Sizes.One);
				ThingConsole.actionWindow.FontSize = ThingConsole.actionWindow.Font.GetFontSize(IFont.Sizes.One);
			}

			ThingConsole.Resize(width, height, width, 100, true);
			ThingConsole.Position = new Point(posx, posy);
			ThingConsole.ReDraw();


			width = roomConsoleWidth - 3;
			height = topConsoleRowHeight;
			posx = inventoryWidth * 2 + roomConsoleWidth + 2;
			posy = barConsoleHeight;
			if (height > 31)
			{
				RoomConsole.FontSize = RoomConsole.Font.GetFontSize(IFont.Sizes.Two);
				RoomConsole.actionWindow.FontSize = RoomConsole.actionWindow.Font.GetFontSize(IFont.Sizes.Two);
				width = width / 2;
				height = height / 2;
				posx = posx / 2;
				posy = posy / 2;
			}
			else
			{
				RoomConsole.FontSize = RoomConsole.Font.GetFontSize(IFont.Sizes.One);
				RoomConsole.actionWindow.FontSize = RoomConsole.actionWindow.Font.GetFontSize(IFont.Sizes.One);
			}

			RoomConsole.Resize(width, height, width, 100, true);
			RoomConsole.Position = new Point(posx, posy);
			RoomConsole.ReDraw();
			
			
			
			width = inventoryWidth;
			height = topConsoleRowHeight;
			posx = inventoryWidth + roomConsoleWidth + 1;
			posy = barConsoleHeight;
			if (height > 31)
			{
				height += 1;
			}
			else
			{
				GrasperConsole.FontSize = GrasperConsole.Font.GetFontSize(IFont.Sizes.One);
				GrasperConsole.actionWindow.FontSize = GrasperConsole.actionWindow.Font.GetFontSize(IFont.Sizes.One);
			}
			GrasperConsole.Resize(width, height, width, 100, false);
			GrasperConsole.Position = new Point(posx,posy );
			GrasperConsole.ActionOffset = new Point(0, 1);
			GrasperConsole.ReDraw();

			
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






			width = Program.Width;
			height = barConsoleHeight - 1;
			buttonConsole.Resize(width, height, width, height, true);

			PostPorcessing.RemakeRenderTarget();





		}







		private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{

			DateTime date = DateTime.Now;
			
			File.WriteAllText("CrashDump"+date.ToFileTime()+".txt", e.ExceptionObject.ToString());
			string debug = File.ReadAllText("debuglog.txt");
			File.WriteAllText("debuglog"+date.ToFileTime()+".txt", debug);

		}


	}
}