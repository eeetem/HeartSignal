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
            
            


        }
        public void DrawMessage(string value) {
            Cursor.Print(value).NewLine();
            
        
        
        }
    

        public void ReciveInput(string value)
        {
#if DEBUG
    
#else
            value = value.Replace("[", "");
            value = value.Replace("]", "");
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

            bool succses = Program.SendNetworkMessage(value);
            if (!succses) {
                Cursor.Print("Message wasn't sent as previous one was processing, slow down").NewLine();


            }

        }
    }
}
