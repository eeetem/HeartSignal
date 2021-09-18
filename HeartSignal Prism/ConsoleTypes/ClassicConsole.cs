using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using PrimS.Telnet;
using System.Threading.Tasks;

namespace HeartSignal
{
    internal class ClassicConsole : Console, ITextInputReciver
    {
        public string Prompt { get; set; }

        // This console domonstrates a classic MS-DOS or Windows Command Prompt style console.
        public ClassicConsole(int width,int height): base(width, height)
        {

            

                        // Startup description
             ClearText();

           // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            ColoredString.CustomProcessor = Utility.CustomParseCommand;
            Cursor.UseStringParser = true;
            SadComponents.Add(new CopyPasteMouse());

        }

        public void ClearText()
        {
            this.Clear();
        }


        bool awaitingInput = true;
        string inputToReturn = "";
        public async Task<string> AskForInput(string prompt)
        {
            ReciveExternalInput(prompt+":");
            awaitingInput = true;
            
            while (awaitingInput)
            {
                await Task.Delay(10);
            }
            return inputToReturn;
        }
        public void ReciveExternalInput(string value) {
            DrawMessage(value);
           // DrawMessage("[c:ga f:black:red:white:5:b:red:orange:yellow:green:blue:purple:blue:green:yellow:orange:red:200]Wow, a strip of human meat![c:u]");




        }
        public void DrawMessage(string value) {
            Cursor.Print(value).NewLine();

        
        
        }
    

        public void ReciveInput(string value)
        {
            ///sanitize stuff if not in dev mode
#if RELEASE


            value = value.Replace("[", "");
            value = value.Replace("]", "");
            value = value.Replace("{", "");
            value = value.Replace("}", "");
            value = value.Replace(Environment.NewLine, "");
            value = value.Replace("\n", "");
            value = value.Replace("\r", "");
#endif



            DrawMessage(value);

            value = value.Replace(">", "");
            if (awaitingInput) {
                inputToReturn = value;
                awaitingInput = false;
                
                return;
            
            
            }
           



/*
            if (value.ToLower() == "exit" || value.ToLower() == "quit")
            {
#if WINDOWS_UAP
                //Windows.UI.Xaml.Application.Current.Exit();       Not working?
#else
                Environment.Exit(0);
#endif
            }*/
            if (Program.TelnetClient == null|| (Program.TelnetClient!=null&& !Program.TelnetClient.IsConnected)) {

                Cursor.Print("no connection to server").NewLine();
                return;

            }

            Program.SendNetworkMessage(value);

        }
    }
}
