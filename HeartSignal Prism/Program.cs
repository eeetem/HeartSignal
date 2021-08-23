using System;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using PrimS.Telnet;
using System.Threading.Tasks;


namespace HeartSignal
{
    static class Program
    {
       static ClassicConsole MainConsole;
        public static Client TelnetClient;
        private static void Main(string[] args)
        {
            var SCREEN_WIDTH = 100;
            var SCREEN_HEIGHT = 50;

            SadConsole.Settings.WindowTitle = "HeartSignal Prism";
            SadConsole.Settings.UseDefaultExtendedFont = true;

            SadConsole.Game.Create(SCREEN_WIDTH, SCREEN_HEIGHT);
            SadConsole.Game.Instance.OnStart = Init;
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }


        private static void Init()
        {

            //creates a new instance of classic console
            MainConsole = new ClassicConsole();

            //assings the console to the main screen
            Game.Instance.Screen = MainConsole;

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

        private static async void ServerLoop() {


            

            string login = await MainConsole.AskForInput("Enter Login");
            string pass = await MainConsole.AskForInput("Enter Password");
            MainConsole.Cursor.Print("Attempting server connection....").NewLine();
            using (Client client = new Client("deathcult.today", 6666, new System.Threading.CancellationToken()))
            {
                await client.TryLoginAsync("", "", 1000);

                await client.WriteLine("connect "+login+" "+pass);
                await Task.Delay(50);
                string response = await client.ReadAsync();
                MainConsole.DisplayMessageFromServer(response);
                TelnetClient = client;
                while (true) {

                    if (needToSendMessage) {

                        await client.WriteLine(networkMessage);
                        needToSendMessage = false;

                    }
                    response = await client.ReadAsync(TimeSpan.FromMilliseconds(50));
                    if (response.Length >1) {
                        MainConsole.DisplayMessageFromServer(response);
                    }
                    

                    //await Task.Delay(50);
                }
            }
            MainConsole.Cursor.Print("Server Connection Ended").NewLine();

        }

    }
}


