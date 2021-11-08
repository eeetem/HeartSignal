using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System.Threading.Tasks;


namespace HeartSignal
{
    public class ClassicConsole : SadConsole.UI.ControlsConsole, ITextInputReciver
    {
        public string Prompt { get; set; }


        //a lot of repeating of other things here should be parented to another console at some point
        public ClassicConsole(int width,int height): base(width, height)
        {



            ClearText();
            FontSize = Font.GetFontSize(IFont.Sizes.Two);
            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            
            Cursor.UseStringParser = true;
           // UseKeyboard = true;

            input = new InputConsole(width, 2, this);

            input.Position = new Point(0, 0);
            Children.Add(input);

            actionWindow = new ActionWindow(30, 5, new Point(0, 0));
            Children.Add(actionWindow);

            actionWindow.IsVisible = false;
            actionWindow.IsEnabled = false;

        }
        ActionWindow actionWindow;

        InputConsole input;
        //this may be elegant or maybe be omega shitcode
        public InputConsole GetInputSource() {

            return input;
        
        
        }
        public void ClearText()
        {
            this.Clear();
            Surface.ViewPosition = new Point(0, 0);
            Cursor.Position = new Point(0, 0);
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
        
        //probably should be renamed to something better
        public void ReciveExternalInput(string value) {

            if (value.Contains("[clear]")){
                ClearText();
                return;
            }

            Utility.PrintParseMessage(value, actionWindow, this, false);
            if (!Program.verboseDebug) //if verbose debug is not set then all other prints would be disbaled so do a debug print here
            {

                System.Console.WriteLine(value);
            }

            Surface.ViewPosition = new Point(0, Math.Max(0,Cursor.Position.Y-ViewHeight));


        }

    

        public void ReciveInput(string value)
        {
            //sanitize stuff if not in dev mode
#if RELEASE


            value = value.Replace("[", "");
            value = value.Replace("]", "");
            value = value.Replace("{", "");
            value = value.Replace("}", "");
            value = value.Replace(Environment.NewLine, "");
            value = value.Replace("\n", "");
            value = value.Replace("\r", "");
#endif



            ReciveExternalInput("[c:r_f:darkgray]" + value);

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
