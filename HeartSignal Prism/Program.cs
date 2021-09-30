using System;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using PrimS.Telnet;
using System.Threading.Tasks;
using System.Collections.Generic;

using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.UI;
using System.Text.RegularExpressions;

namespace HeartSignal
{
    static class Program
    {
        public static ClassicConsole MainConsole;
        public static RoomConsole RoomConsole;
        static MapConsole MapConsole;
        static PromptWindow PromptWindow;
        public static InventoryConsole InventoryConsole;
        public static InventoryConsole ExamInventoryConsole;
        public static InventoryConsole GrasperConsole;
        static ThingConsole ThingConsole;
        public static Console root;
        public static bool verboseDebug = false;

        public static Client TelnetClient;
        [STAThread]
        private static void Main(string[] args)
        {
            var SCREEN_WIDTH = (96*2)+30;
            var SCREEN_HEIGHT = 54+5;

            SadConsole.Settings.WindowTitle = "HeartSignal Prism";
            SadConsole.Settings.UseDefaultExtendedFont = true;

            SadConsole.Settings.AllowWindowResize = true;
            SadConsole.UI.Themes.Library.Default.Colors.Lines = new AdjustableColor(Color.Red, "red");

            

            SadConsole.Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();

            SadConsole.Game.Instance.Dispose();

        }



        private static void Init()
        {

            root = new Console(1, 1);
            MainConsole = new ClassicConsole(1,1);
            root.Children.Add(MainConsole);
            MapConsole = new MapConsole(1, 1);
            root.Children.Add(MapConsole);
            RoomConsole = new RoomConsole(1, 1);
            root.Children.Add(RoomConsole);
            ThingConsole = new ThingConsole(1,1);
            root.Children.Add(ThingConsole);
            InventoryConsole = new InventoryConsole(1,1);
            InventoryConsole.tagline = "My Body";
            InventoryConsole.self = true;
            root.Children.Add(InventoryConsole);
            ExamInventoryConsole = new InventoryConsole(1, 1);
            ExamInventoryConsole.tagline = "Their Body";
            root.Children.Add(ExamInventoryConsole);
            GrasperConsole = new InventoryConsole(1, 1);
            GrasperConsole.tagline = "I can hold with";
            root.Children.Add(GrasperConsole);







            PromptWindow = new PromptWindow(30, 10, new Point(WIDTH / 2 - 15, HEIGHT / 2 - 5));

            root.Children.Add(PromptWindow);



            PositionConsoles();
            Game.Instance.Screen = root;
            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();

           // SadConsole.WIDTH = 100;
            Settings.ResizeMode = Settings.WindowResizeOptions.None;
            SadConsole.Game.Instance.MonoGameInstance.WindowResized += (s,a) => PositionConsoles();
            ServerLoop();





        }

        public static int WIDTH = 0;
        public static int HEIGHT = 0;


        ///TODO: TURN needRedraw into something that's part of a one inheritable console instead of each console having that
        static void PositionConsoles() {

            WIDTH = Game.Instance.MonoGameInstance.WindowWidth / root.FontSize.X;
            HEIGHT = Game.Instance.MonoGameInstance.WindowHeight / root.FontSize.Y;
            root.Resize(WIDTH, HEIGHT, WIDTH, HEIGHT, false);


            int MapConsoleHeight = 7;
            int inventoryWidth = 28;
            int roomConsoleWidth = (WIDTH - (inventoryWidth * 3)) / 2;

            int topconsolerowheight = 20;


            ///todo: replace all hardcoded coordinates with variables since a lot of them counterdepend on other console sizes
            //ScreenSurface.
            int width = WIDTH - (inventoryWidth * 2) - 2;
            int height =HEIGHT - (topconsolerowheight + 2) - 2;
            MainConsole.Resize(width,height,width,height,false);
            MainConsole.Position = new Point(inventoryWidth + 1, topconsolerowheight);

            //cringus
            MainConsole.GetInputSource().Resize(width, 30, width, 30, false);//fun fact: input console is gigantic - just hidden under
            MainConsole.GetInputSource().Position = new Point(0, height +2);
            MainConsole.GetInputSource().Cursor.Position = new Point(0, 0);
            MainConsole.GetInputSource().Clear();
            MainConsole.GetInputSource().Cursor.Print(">");

            width = inventoryWidth / 2;
            height = MapConsoleHeight;
            MapConsole.Resize(width,height,width,height,false);
            MapConsole.Position = new Point((WIDTH / 2) - (inventoryWidth / 2), 0);
            MapConsole.ReDraw();

            width = roomConsoleWidth - 1;
            height = topconsolerowheight;
            RoomConsole.Resize(width, height,width,height,true);
            RoomConsole.Position = new Point(inventoryWidth + 1, 0);
            RoomConsole.ReDraw();


            PromptWindow.Position = new Point(WIDTH / 2 - 15, HEIGHT / 2 - 5);

            width = roomConsoleWidth - 3;
            height = topconsolerowheight;
            ThingConsole.Resize(width, height, width, height, true);
            ThingConsole.Position = new Point(inventoryWidth * 2 + roomConsoleWidth + 2, 0);
            ThingConsole.ReDraw();


            width = inventoryWidth;
            height = HEIGHT;
            InventoryConsole.Resize(width, height, width, height, true);
            InventoryConsole.Position = new Point(0, 0);
            InventoryConsole.ActionOffset = new Point(10, 1);
            InventoryConsole.ReDraw();


            width = inventoryWidth;
            height = HEIGHT - (MapConsoleHeight * 2) - 1;
            ExamInventoryConsole.Resize(width, height, width, height, true);
            ExamInventoryConsole.Position = new Point(WIDTH - inventoryWidth, (MapConsoleHeight * 2) + 1);
            ExamInventoryConsole.ActionOffset = new Point(-30, 1);
            ExamInventoryConsole.ReDraw();


            width = inventoryWidth;
            height = topconsolerowheight;
            GrasperConsole.Resize(width, height, width, height, false);
            GrasperConsole.Position = new Point(inventoryWidth + roomConsoleWidth + 1, 0);
            GrasperConsole.ActionOffset = new Point(0, 1);
            GrasperConsole.clickableFirstLayer = false;
            GrasperConsole.ReDraw();








        }

       

        static List<string> messageQueue = new List<string>();
        public static bool SendNetworkMessage(string message) {

            messageQueue.Add(message);
            needToSendMessage = true;
            if (verboseDebug)
            {
                System.Console.WriteLine("Sending Message: " + message);
            }
            return true;

        }

        static bool needToSendMessage = false;


        public static List<string> ExtractQuotationStrings(string s) {

            List<string> stringlist = new List<string>();

            while (true)
            {
                int posFrom = s.IndexOf('"');
                if (posFrom != -1) //if found char
                {
                    int posTo = s.IndexOf('"', posFrom + 1);
                    if (posTo != -1) //if found char
                    {
                        stringlist.Add(s.Substring(posFrom + 1, posTo - posFrom - 1));

                        s = s.Remove(0,posTo+1);//+1 to cut the comma
           
                        continue;

                    }
                }
                break;
            }
            

            return stringlist;

        }
        private static void SplitInput(string input)
        {



            int idx = input.IndexOf(Environment.NewLine);
            if (idx < 1) {


                idx = input.IndexOf('\n');

                if (idx < 1)
                {


                    idx = input.IndexOf('\r');



                }

            }
            if (idx > 0)
            {
                ParseServerInput(input.Substring(0,idx));
                input = input.Remove(0,idx+2);//i'm not exactly sure why the +2 is needed, but it works, dont touch it
                if (input.Length > 1)
                {

                    SplitInput(input);


                }
                return;
            }

            ParseServerInput(input);




        }
        private static void ParseServerInput(string input) {
            

        int idx = input.IndexOf(':');
            if (idx > 0 && idx < 11)//hardcoded max lenght, kinda cringe but whatever for now
            {

                string sub = input.Substring(0, idx);
                string cutstring = input;
                string[] returned;
                //  System.Console.WriteLine(input);
                switch (sub)
                {

                    ///a lot of parse repeating - turn this into a function at some point - me from the future: turned some bits into functions however there is still shitload of repeating, needs quite a big refactor
                    case "desc":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];




                        ThingConsole.lines = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        ThingConsole.ReDraw();
                        break;
                    case "room":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        RoomConsole.SetName(returned[1]);


                        RoomConsole.roomInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.ReDraw();
                        break;
                    case "fancy":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        RoomConsole.fancyInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.ReDraw();
                        break;
                    case "things":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        RoomConsole.thingInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.ReDraw();
                        break;
                    case "bodies":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        RoomConsole.bodyInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.ReDraw();
                        break;


                    case "actions":
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        ActionWindow.actionDatabase[returned[1]] = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        break;
                    case "argactions":
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        ActionWindow.argactionDatabase[returned[1]] = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        break;
                    case "map":


                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        MapConsole.mapdata = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
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

                        NestedInfo info2 = new NestedInfo(null,null);

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
                        List<string> args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        AudioManager.ParseRequest(returned[1], args[0], args[1]);

                        break;

                    case "recolor":
                        cutstring = cutstring.Remove(0, cutstring.IndexOf(":")+1);
                       
                        bool keep = false;
                        AnimatedBorderComponent._borderCellStyle = new ColoredGlyph(Color.White.FromParser(cutstring,out keep, out keep, out keep, out keep, out keep), Color.Black);


                        break;
                    case "prompt":
                       
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        PromptWindow.toptext = returned[1];
                        List<string> args2 = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        PromptWindow.middletext = args2[0];
                        //cringe
                        if (args2[1] == "binary") {

                            PromptWindow.binary = true;
						}
						else { PromptWindow.binary = false; }
                        PromptWindow.needsDraw = true;
                        

                        break;

                    case "exits":

                        //todo
                        break;
                
                    default:
                        if (verboseDebug)
                        {
                            System.Console.WriteLine("unkown parsing tag: " + sub);
                        }
							//if we couldn't parse it - it's possibly not meant to be parsed - print it
							MainConsole.ReciveExternalInput(input);
                        break;

                }

            }
            else {
                MainConsole.ReciveExternalInput(input);
            }
 
         

                
            
        }
        private static NestedInfo GetNestedBrackets(string text) {

            
            int[] indexes = GetOutermostBrackets(text);
            string thingid = text.Substring(0, indexes[0]);
            NestedInfo info = new NestedInfo(thingid,null);
            string innerbracket = text.Substring(indexes[0]+1, indexes[1] - (indexes[0]+1));

           while (innerbracket.Contains('{'))
            {
               NestedInfo innerinfo = GetNestedBrackets(innerbracket);
                info.Contents.Add(innerinfo);
                int[] innerindexes = GetOutermostBrackets(innerbracket);
                innerbracket = innerbracket.Remove(0, innerindexes[1]+1).Replace(",", "").Trim();
            }



            if (innerbracket.Length > 1) {

                info.Contents.Add(new NestedInfo(innerbracket,null));
            }

            return info;
        }
        private static int[] GetOutermostBrackets(string text) {

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
        private static string[] RemoveParseTag(string s) {



            s = s.Remove(0, s.IndexOf(':') + 1);

            string name = s.Substring(0, s.IndexOf('{'));


            s = s.Remove(0, s.IndexOf('{')+1);
            return new string[] { s, name };
        }

        private static async void ServerLoop() {



            MainConsole.Cursor.NewLine();
#if DEBUG
            MainConsole.ReciveExternalInput("This is a debug build of HeartSignal, report to developers if you see this message");
            string ans = await MainConsole.AskForInput("Do you want verbose logging?(y/n)");
            if (ans == "y") {
                verboseDebug = true;
            }

#endif
            string login = await MainConsole.AskForInput("Enter Login");
            string pass = await MainConsole.AskForInput("Enter Password");
            MainConsole.Clear();
            MainConsole.Cursor.NewLine();
            MainConsole.Cursor.Print("Attempting server connection....").NewLine();
            using (Client client = new Client("deathcult.today", 6666, new System.Threading.CancellationToken()))
            {
                await client.TryLoginAsync("", "", 1000);

                await client.WriteLine("connect "+login+" "+pass);
                string response = await client.ReadAsync(TimeSpan.FromMilliseconds(50));
                MainConsole.ReciveExternalInput(response);
                TelnetClient = client;
                while (true) {

                    if (needToSendMessage) {
                        foreach (string message in new List<string>(messageQueue)) {
                            await client.WriteLine(message);
                            messageQueue.Remove(message);
                        }
                        if (messageQueue.Count < 1)
                        {
                            needToSendMessage = false;
                        }
                    }
                    response = await client.ReadAsync(TimeSpan.FromMilliseconds(50));
                    if (response.Length >1) {
                        SplitInput(response);
#if DEBUG

                        if (verboseDebug)
                        {
                            System.Console.WriteLine(response);
                        }


#endif
                    }

                    if (!client.IsConnected) { break; }
                    //await Task.Delay(50);
                }
            }
            MainConsole.Cursor.Print("Server Connection Ended").NewLine();

        }

    }
}


