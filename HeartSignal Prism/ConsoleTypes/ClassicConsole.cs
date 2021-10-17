using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System.Threading.Tasks;


namespace HeartSignal
{
    internal class ClassicConsole : SadConsole.UI.ControlsConsole, ITextInputReciver
    {
        public string Prompt { get; set; }


        //a lot of repeating of other things here should be parented to another console at some poin
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

            if (value.Contains("[clear]")){
                ClearText();
                return;
            }
            DrawMessage(value);
           // DrawMessage("[c:ga f:black:red:white:5:b:red:orange:yellow:green:blue:purple:blue:green:yellow:orange:red:200]Wow, a strip of human meat![c:u]");




        }
        private void DrawMessage(string value) {


            string[] words = value.Split(" ");
            foreach (string word in words)
            {
                if (word.Contains("+"))
                {
                    string text;
                    text = word.Replace("+", "").Replace("_", " ");
                    string tip = text.Substring(text.IndexOf('(') + 1, text.Length - (text.IndexOf('(') + 2));
                    text = text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));

                    var button = new Button(text.Length, 1)
                    {
                        Text = text,
                        Position = Cursor.Position,
                        Theme = new ThingButtonTheme(new Gradient(Color.Green, Color.LimeGreen, Color.Green))
                    };


                    button.MouseEnter += (s, a) => actionWindow.ShowTooltip(tip, Cursor.Position + new Point(0, 0));

                    Controls.Add(button);
                    Cursor.Right(text.Length + 1);
                }
                else if (word.Contains("<")&& word.Contains(">"))
                {
                    string text2 = word;
                    string leftover = "";
                    if (text2.Length > text2.IndexOf('>'))
                    {
                        leftover = text2.Substring(text2.IndexOf('>') + 1, text2.Length - (text2.IndexOf('>') + 1));
                    }
                    text2 = text2.Remove(text2.IndexOf('>'), text2.Length - text2.IndexOf('>'));
                    text2 = text2.Replace("<", "").Replace(">", "");
                    Utility.CreateButtonThingId(Utility.SplitThingID(text2.Replace("_", " ")), this, actionWindow, false, null, true);
                    Cursor.Print(leftover).Right(1);

                }
                else
                {

                    if (Cursor.Position.X + word.Length > Width && !word.Contains("["))
                    {
                        Cursor.NewLine();
                    }
                    Cursor.Print(word.Replace("_", " ") + " ");

                }





            }
            Cursor.NewLine();
            if (!Program.verboseDebug) //if verbose debug is not set then all other prints would be disbaled so do a debug print here
            {

                System.Console.WriteLine(value);
            }

        
        
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



            DrawMessage("[c:r_f:darkgray]" + value);

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
