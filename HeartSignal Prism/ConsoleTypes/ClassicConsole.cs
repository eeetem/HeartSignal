using System;
using System.Runtime.InteropServices;
using SadConsole;
using SadRogue.Primitives;



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

        private bool needsClear = false;
        
        private static readonly object syncObj = new object();
        public override void Update(TimeSpan delta)
        {

            lock (syncObj)
            {
                if (needsClear)
                {
                    this.Clear();
                    Controls.Clear();
                    this.Effects.RemoveAll();
                    Surface.ViewPosition = new Point(0, 0);
                    Cursor.Position = new Point(0, 0);
                    Cursor.NewLine();
                    needsClear = false;
                }

                base.Update(delta);
            }
        }

        public void ClearText()
        {
            needsClear = true;
        }




        //probably should be renamed to something better
        public void ReciveExternalInput(string value) {

            lock (syncObj)
            {
                if (value.Contains("[clear]"))
                {
                    ClearText();
                    return;
                }
                //this probably isnt the best place for these
           

                if (Height < Cursor.Position.Y + 10)
                {
                    Resize(ViewWidth, ViewHeight, Width, Height + 50, false);
                }

                Utility.PrintParseMessage(value, actionWindow, this, false);
#if DEBUG
                System.Console.WriteLine(value);


                System.Console.WriteLine(Cursor.Position);
#endif
             
                SetRelevantViewPos();
            }

        }

     
            

        public void SetRelevantViewPos()
        {
            Surface.ViewPosition = new Point(0, Math.Max(0,Cursor.Position.Y-ViewHeight));
            
        }

    
        

    }

}
