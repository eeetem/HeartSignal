using System;
using System.Collections.Generic;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace HeartSignal
{
    public class DisplayConsole : BaseConsole
    {

        public DisplayConsole(int width, int height) : base(width, height,true,true)
        {

            // Disable the cursor since our keyboard handler will do the work.
            SadComponents.Add(new MouseHandler());
            
         
        }
        public bool ExplicitLook = false;
        public string contents = "";
        public List<string> things = new List<string>();
        public List<ThingWindow> subWindows = new List<ThingWindow>();

        protected override void DrawConsole()
        {
            Resize(ViewWidth,ViewHeight,Width,50,false);

         
            Utility.PrintParseMessage(contents.Replace("[nl]","\r\n"),actionWindow,this,ExplicitLook);

            
            //todo search for big windows and print em first
           
             subWindows = new List<ThingWindow>();
            foreach(string thing in things)
            {
                ThingWindow tw = new ThingWindow(this, thing);
                subWindows.Add(tw);
                Children.Add(tw);
            }
           
            
          
            WindowSort();
            
        }

        public void WindowSort()
        {
            Resize(ViewWidth,ViewHeight,Width,100,false);
            Point nextWindowPos = Cursor.Position + new Point(0,1);
            int highestHeigth = 0;
            subWindows.Sort(new WindowHeightCompare());



            foreach (var window in subWindows)
            {
                Children.MoveToBottom(window);
                window.Position = nextWindowPos;
                nextWindowPos += new Point(window.Width,0);
                if (window.Height > highestHeigth)
                {
                    highestHeigth = window.Height;
                }
                if (nextWindowPos.X + 35 > this.Width)
                {
                    nextWindowPos = new Point(0, nextWindowPos.Y + highestHeigth);
                    highestHeigth = 0;
                }
            }
            
            Resize(ViewWidth,ViewHeight,Width,Math.Max(nextWindowPos.Y+15,ViewHeight),false);

        }
    }

    public class WindowHeightCompare : Comparer<ThingWindow>
    {
        // Compares by Length, Height, and Width.
        public override int Compare(ThingWindow x, ThingWindow y)
        {
            return y.Height.CompareTo(x.Height);
        }
    }
}
