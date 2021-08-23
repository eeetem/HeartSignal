using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using PrimS.Telnet;
using System.Threading.Tasks;

namespace HeartSignal
{
    internal class ClassicConsole : Console
    {
        public string Prompt { get; set; }

        private readonly KeyboardHandler _keyboardHandlerObject;


        // This console domonstrates a classic MS-DOS or Windows Command Prompt style console.
        public ClassicConsole(): base(80, 23)
        {
           
            // This is our cusotmer keyboard handler we'll be using to process the cursor on this console.
            _keyboardHandlerObject = new KeyboardHandler();

            // Our custom handler has a call back for processing the commands the user types. We could handle
            // this in any method object anywhere, but we've implemented it on this console directly.
            _keyboardHandlerObject.EnterPressedAction = EnterPressedActionHandler;

            // Enable the keyboard and setup the prompt.
            UseKeyboard = true;
            Cursor.IsVisible = true;
            Prompt = ">";
            IsFocused = true;
            

                        // Startup description
                        ClearText();

                        // Disable the cursor since our keyboard handler will do the work.
                        Cursor.IsEnabled = false;
                        Cursor.Position = new Point(0, 24);
                       _keyboardHandlerObject.CursorLastY = 24;
                        TimesShiftedUp = 0;

                        Cursor.DisableWordBreak = true;
                        Cursor.Print(Prompt);
                        Cursor.DisableWordBreak = false;

                        // Assign our custom handler method from our handler object to this consoles keyboard handler.
                        // We could have overridden the ProcessKeyboard method, but I wanted to demonstrate how you
                        // can use your own handler on any console type.
            SadComponents.Add(_keyboardHandlerObject);
        }

        public void ClearText()
        {
            this.Clear();
            Cursor.Position = new Point(0, 24);
            _keyboardHandlerObject.CursorLastY = 24;
        }


        bool awaitingInput = true;
        string inputToReturn = "";
        public async Task<string> AskForInput(string prompt)
        {
           Cursor.NewLine().Print(prompt+":");
            awaitingInput = true;
            IniatePrompt();
            while (awaitingInput)
            {
                await Task.Delay(10);
            }

            return inputToReturn;
        }
        public void DisplayMessageFromServer(string value) {
            Cursor.Print(value);
            IniatePrompt();



        }
        public int promptIndex;
        public void IniatePrompt()
        {
            Cursor.NewLine().Print(Prompt);
            _keyboardHandlerObject.CursorLastY = Cursor.Position.Y;
            promptIndex = new Point(Cursor.Position.X, Cursor.Position.Y).ToIndex(Width);



        }

        private void EnterPressedActionHandler(string value)
        {
            
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
