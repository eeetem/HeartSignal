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
            root = new Console(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
            
  
            int MapConsoleHeight = 7;
            int inventoryWidth = 28;
            int roomConsoleWidth = (Game.Instance.ScreenCellsX  - (inventoryWidth*3)) / 2;
            
            int topconsolerowheight = 16;
            int inputHeigth = 2;

            ///todo: replace all hardcoded coordinates with variables since a lot of them counterdepend on other console sizes


            MainConsole = new ClassicConsole(Game.Instance.ScreenCellsX - (inventoryWidth * 2)-2, SadConsole.Game.Instance.ScreenCellsY- (topconsolerowheight+ inputHeigth)-2);
            MainConsole.Position = new Point(inventoryWidth+1, topconsolerowheight);


            InputConsole input = new InputConsole(Game.Instance.ScreenCellsX- (inventoryWidth * 2)-2, inputHeigth, MainConsole);
            MainConsole.Children.Add(input);
            input.Position = new Point(0, Game.Instance.ScreenCellsY - (topconsolerowheight + inputHeigth));

            root.Children.Add(MainConsole);
            MapConsole = new MapConsole(inventoryWidth / 2, MapConsoleHeight);
            MapConsole.Position = new Point((Game.Instance.ScreenCellsX / 2) - (inventoryWidth / 2), 0);
            root.Children.Add(MapConsole);

            RoomConsole = new RoomConsole(roomConsoleWidth-1, topconsolerowheight);
            RoomConsole.Position = new Point(inventoryWidth+1,0);
            root.Children.Add(RoomConsole);

            ///currently there is a single static window however this could be later turned in multiple dymanically created ones
            PromptWindow = new PromptWindow(30,10,new Point(Game.Instance.ScreenCellsX/2-15, Game.Instance.ScreenCellsY/2-5));
            root.Children.Add(PromptWindow);

            ThingConsole = new ThingConsole(roomConsoleWidth - 1, topconsolerowheight);
            ThingConsole.Position = new Point(inventoryWidth * 2 + roomConsoleWidth, 0);
            root.Children.Add(ThingConsole);


            InventoryConsole = new InventoryConsole(inventoryWidth, Game.Instance.ScreenCellsY);
            InventoryConsole.Position = new Point(0, 0);
            InventoryConsole.tagline = "My Body:";
            InventoryConsole.ActionOffset = new Point(10, 1);
            root.Children.Add(InventoryConsole);

            ExamInventoryConsole = new InventoryConsole(inventoryWidth, Game.Instance.ScreenCellsY - (MapConsoleHeight * 2)-1);
            ExamInventoryConsole.Position = new Point(Game.Instance.ScreenCellsX - inventoryWidth, (MapConsoleHeight * 2)+1);
            ExamInventoryConsole.tagline = "Their Body:";
            ExamInventoryConsole.ActionOffset = new Point(Game.Instance.ScreenCellsX - inventoryWidth, 4);
            root.Children.Add(ExamInventoryConsole);


            GrasperConsole = new InventoryConsole(inventoryWidth, topconsolerowheight);
            GrasperConsole.Position = new Point(inventoryWidth+roomConsoleWidth+1,0);
            GrasperConsole.tagline = "I can hold with:";
            GrasperConsole.ActionOffset = new Point(0, 1);
            GrasperConsole.clickableFirstLayer = false;

            root.Children.Add(GrasperConsole);




            Game.Instance.Screen = root;

          


            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();


            Settings.ResizeMode = Settings.WindowResizeOptions.Fit;

            ServerLoop();





        }

       

        static List<string> messageQueue = new List<string>();
        public static bool SendNetworkMessage(string message) {

            messageQueue.Add(message);
            needToSendMessage = true;
            System.Console.WriteLine(message);
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
                        ThingConsole.needRedraw = true;
                        break;
                    case "room":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        RoomConsole.SetName(returned[1]);


                        RoomConsole.roomInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.needRedraw = true;
                        break;
                    case "fancy":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        RoomConsole.fancyInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.needRedraw = true;
                        break;
                    case "things":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        RoomConsole.thingInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.needRedraw = true;
                        break;
                    case "bodies":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        RoomConsole.bodyInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.needRedraw = true;
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
                        MapConsole.needRedraw = true;
                        break;
                    case "cexits":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        MapConsole.cexists = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        MapConsole.needRedraw = true;
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
                        InventoryConsole.needRedraw = true;
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
                        ExamInventoryConsole.needRedraw = true;
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
                        GrasperConsole.needRedraw = true;
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
                        System.Console.WriteLine("unkown parsing tag: " + sub);
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


                        System.Console.WriteLine(response);



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


