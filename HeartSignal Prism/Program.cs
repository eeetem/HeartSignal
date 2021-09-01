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
        static InventoryConsole InventoryConsole;
        public static ScreenObject root;
        public static Dictionary<string, List<string>> actionDatabase = new Dictionary<string, List<string>>();
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
            SadConsole.Game.Instance.Run();//exception is likely caused by async bullshit and foreach in consoles - fix before playtest by coppying the lists before for-each-ing them
            SadConsole.Game.Instance.Dispose();

        }


        private static void Init()
        {
            root = new ScreenObject();
            

            
            MainConsole = new ClassicConsole(Game.Instance.ScreenCellsX, SadConsole.Game.Instance.ScreenCellsY-12);
            MainConsole.Position = new Point(0, 10);


            InputConsole input = new InputConsole(Game.Instance.ScreenCellsX-20 , 2, MainConsole);
            MainConsole.Children.Add(input);
            input.Position = new Point(0, Game.Instance.ScreenCellsY - 12);

            root.Children.Add(MainConsole);
            
            
            RoomConsole = new RoomConsole(Game.Instance.ScreenCellsX, 10);
            root.Children.Add(RoomConsole);


            MapConsole = new MapConsole(14, 7);
            MapConsole.Position = new Point(Game.Instance.ScreenCellsX-14,0);
            root.Children.Add(MapConsole);

            InventoryConsole = new InventoryConsole(20, Game.Instance.ScreenCellsY-7);
            InventoryConsole.Position = new Point(Game.Instance.ScreenCellsX - 20, 7);
            root.Children.Add(InventoryConsole);

            Game.Instance.Screen = root;

          


            // This is needed because we replaced the initial screen object with our own.
            Game.Instance.DestroyDefaultStartingConsole();


            ServerLoop();





        }
        public static bool SendNetworkMessage(string message) {
            if (needToSendMessage) {


                return false;
            }
            networkMessage = message;
            needToSendMessage = true;
            System.Console.WriteLine(message);
            return true;

        }
        static string networkMessage = "";
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
            if (idx > 0 && idx <8)//hardcoded max lenght, kinda cringe but whatever for now
            {

                string sub = input.Substring(0, idx);
                string cutstring = input;
                string[] returned;
              //  System.Console.WriteLine(input);
                switch (sub)
                {

                    ///a lot of parse repeating - turn this into a function at some point
                    case "room":

                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];

                        RoomConsole.SetName(returned[1]);


                        RoomConsole.roomInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.DrawRoom();
                        break;
                    case "things":

                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];


                        RoomConsole.thingInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.DrawRoom();
                        break;
                    case "bodies":

                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];

                        RoomConsole.bodyInfo = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        RoomConsole.needRedraw = true;
                        break;


                    case "actions":
                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];


                        actionDatabase[returned[1]] = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        //possibly turn this into a delegate later
                        RoomConsole.needRedraw = true;
                        InventoryConsole.needRedraw = true;
                        break;
                    case "map":


                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];

                        MapConsole.mapdata = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        MapConsole.DrawMap();
                        break;
                    case "cexits":

                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];


                        MapConsole.cexists = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        MapConsole.DrawMap();
                        break;
                    /*case "inventory":

                        cutstring = cutstring.Remove(0, cutstring.IndexOf(':') + 1);


                        cutstring = cutstring.Remove(0, cutstring.IndexOf('{'));

        
                        MapConsole.cexists = ExtractQuotationStrings(cutstring.Substring(0, cutstring.IndexOf('}')));
                        MapConsole.DrawMap();
                        break;*/
                   case "holding":
                        Dictionary<string, List<string>> holding = new Dictionary<string, List<string>>();
                        returned = ParseUntillBracket(cutstring);
                        cutstring = returned[0];
						while (cutstring.Contains('{')) { 

                        returned = ParseUntillBracket(cutstring);
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
        private static string[] ParseUntillBracket(string s) {



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

                        await client.WriteLine(networkMessage);
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


