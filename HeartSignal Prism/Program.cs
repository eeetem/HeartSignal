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

namespace HeartSignal
{
    static class Program
    {
       static ClassicConsole MainConsole;
        static RoomConsole RoomConsole;
        static MapConsole MapConsole;
        static PromptWindow PromptWindow;
        static InventoryConsole InventoryConsole;
        static ThingConsole ThingConsole;
        public static ScreenObject root;

        public static Client TelnetClient;
        [STAThread]
        private static void Main(string[] args)
        {
            var SCREEN_WIDTH = 96*2;
            var SCREEN_HEIGHT = 54;

            SadConsole.Settings.WindowTitle = "HeartSignal Prism";
            SadConsole.Settings.UseDefaultExtendedFont = true;
           
           // SadConsole.Settings.AllowWindowResize = false;
            SadConsole.UI.Themes.Library.Default.Colors.Lines = new AdjustableColor(Color.Red, "red");


            SadConsole.Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();//STILL NOT FIXED TODO FIX
            SadConsole.Game.Instance.Dispose();

        }



        private static void Init()
        {
            root = new ScreenObject();
            
            int MapConsoleWidth = 14;
            int roomConsoleWidth = (Game.Instance.ScreenCellsX - MapConsoleWidth) / 2;
            int inventoryWidth = 30;
            int topconsolerowheight = 15;
            int inputHeigth = 2;

            ///todo: replace all hardcoded coordinates with variables since a lot of them counterdepend on other console sizes

            MainConsole = new ClassicConsole(Game.Instance.ScreenCellsX, SadConsole.Game.Instance.ScreenCellsY- (topconsolerowheight+ inputHeigth));
            MainConsole.Position = new Point(0, topconsolerowheight);


            InputConsole input = new InputConsole(Game.Instance.ScreenCellsX- inventoryWidth, inputHeigth, MainConsole);
            MainConsole.Children.Add(input);
            input.Position = new Point(0, Game.Instance.ScreenCellsY - (topconsolerowheight + inputHeigth));

            root.Children.Add(MainConsole);
            
            
            RoomConsole = new RoomConsole(roomConsoleWidth, topconsolerowheight);
            root.Children.Add(RoomConsole);
            ThingConsole = new ThingConsole(roomConsoleWidth, topconsolerowheight);
            ThingConsole.Position = new Point(roomConsoleWidth, 0);
            root.Children.Add(ThingConsole);

            ///currently there is a single static window however this could be later turned in multiple dymanically created ones
            PromptWindow = new PromptWindow(30,10,new Point(Game.Instance.ScreenCellsX/2-15, Game.Instance.ScreenCellsY/2-5));
            root.Children.Add(PromptWindow);

            MapConsole = new MapConsole(MapConsoleWidth, 7);
            MapConsole.Position = new Point(Game.Instance.ScreenCellsX-14,0);
            root.Children.Add(MapConsole);

            InventoryConsole = new InventoryConsole(inventoryWidth, Game.Instance.ScreenCellsY-7);
            InventoryConsole.Position = new Point(Game.Instance.ScreenCellsX - inventoryWidth, 7);
            root.Children.Add(InventoryConsole);




            Game.Instance.Screen = root;

          


            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();

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
       
                        RoomConsole.needRedraw = true;
                        InventoryConsole.needRedraw = true;
                        break;
                    case "argactions":
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        ActionWindow.argactionDatabase[returned[1]] = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));

                        RoomConsole.needRedraw = true;
                        InventoryConsole.needRedraw = true;
                        break;
                    case "map":


                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];

                        MapConsole.mapdata = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        MapConsole.DrawMap();
                        break;
                    case "cexits":

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];


                        MapConsole.cexists = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        MapConsole.DrawMap();
                        break;
                    case "inventory":

                        Dictionary<string, List<string>> inventory = new Dictionary<string, List<string>>();
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];
                        while (cutstring.Contains('{'))
                        {

                            returned = RemoveParseTag(cutstring);
                            cutstring = returned[0];

                            inventory[returned[1]] = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));

                            if (cutstring.Contains('{'))///this if statement is horrible however the entire parser is a bit of a mess already and will need a refactor in the future to be a bit more consistent
                            {
                                cutstring = cutstring.Remove(0, cutstring.IndexOf('}') + 3);
                            }
                            else
                            {
                                cutstring = cutstring.Remove(0, cutstring.IndexOf('}') + 2);
                            }
                        }
                        InventoryConsole.inventoryInfo = inventory;
                        InventoryConsole.needRedraw = true;
                        break;
                   case "holding":
                        Dictionary<string, List<string>> holding = new Dictionary<string, List<string>>();
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];
                        while (cutstring.Contains('{')) { 

                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];



                        holding[returned[1]] = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));

                            if (cutstring.Contains('{'))///this if statement is horrible however the entire parser is a bit of a mess already and will need a refactor in the future to be a bit more consistent
                            {
                                cutstring = cutstring.Remove(0, cutstring.IndexOf('}') + 3);
                            }
                            else {
                                cutstring = cutstring.Remove(0, cutstring.IndexOf('}') + 2);
                            }
                        }
                        InventoryConsole.holdingInfo = holding;
                        InventoryConsole.needRedraw = true;
                        break;
                    case "sound":
                        returned = RemoveParseTag(cutstring);
                        cutstring = returned[0];
                        List<string> args = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        AudioManager.ParseRequest(returned[1],args[0],args[1]);

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
        private static string[] RemoveParseTag(string s) {



            s = s.Remove(0, s.IndexOf(':') + 1);

            string name = s.Substring(0, s.IndexOf('{'));


            s = s.Remove(0, s.IndexOf('{')+1);
            return new string[] { s, name };
        }

        private static async void ServerLoop() {


            

            string login = await MainConsole.AskForInput("Enter Login");
            string pass = await MainConsole.AskForInput("Enter Password");
            MainConsole.Clear();
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
                        foreach (string message in messageQueue) {
                            await client.WriteLine(message);
                        }
                        messageQueue.Clear();
                        needToSendMessage = false;

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


