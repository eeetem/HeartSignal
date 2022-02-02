using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System.Threading.Tasks;
using SadConsole.Input;


namespace HeartSignal
{
    public class ClassicConsole : SadConsole.UI.ControlsConsole
    {
        public string Prompt { get; set; }


        //a lot of repeating of other things here should be parented to another console at some point
        public ClassicConsole(int width,int height): base(width, height)
        {



            
            
            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            
            Cursor.UseStringParser = true;
           // UseKeyboard = true;

            input = new InputConsole(width, 2);

            input.Position = new Point(0, 0);
            Children.Add(input);

            actionWindow = new ActionWindow(30, 5, new Point(0, 0));
            Children.Add(actionWindow);

            actionWindow.IsVisible = false;
            actionWindow.IsEnabled = false;
            ClearText();

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
            this.Effects.RemoveAll();
            Surface.ViewPosition = new Point(0, 0);
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine();
        }




        //probably should be renamed to something better
        public void ReciveExternalInput(string value) {

            if (value.Contains("[clear]")){
                ClearText();
                return;
            }

            if (Height < Cursor.Position.Y + 10)
            {
                Resize(ViewWidth,ViewHeight,Width,Height+50,false);
            }

            Utility.PrintParseMessage(value, actionWindow, this, false);

                System.Console.WriteLine(value);
            

            Surface.ViewPosition = new Point(0, Math.Max(0,Cursor.Position.Y-ViewHeight));


        }

    
        

    }
}
