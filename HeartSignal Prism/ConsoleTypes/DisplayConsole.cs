using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.UI.Controls;
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

        private bool examineMode = false;
        private string examineText = "";

        public void Examine(string contents)
        {
            examineMode = true;
            examineText = contents;
            ReDraw();
        }
        protected override void DrawConsole()
        {
            Resize(ViewWidth,ViewHeight,Width,50,false);

         
            Utility.PrintParseMessage(contents,actionWindow,this,ExplicitLook);

            
            //todo search for big windows and print em first
            foreach (var window in subWindows)
            {
                window.IsEnabled = false;
                window.Dispose();

            }
             subWindows = new List<ThingWindow>();
             if (examineMode)
             {
                 Cursor.NewLine();
               
                 var closeAction = new Button("[X]".Length, 1)
                 {
                     Text = "[X]",
                     Position = new Point(this.Width-4,Cursor.Position.Y+1),
                     Theme = new ThingButtonTheme(new Gradient(Color.Red,Color.White,Color.Red))
                 };
                 Utility.PrintParseMessage(examineText,actionWindow,this,ExplicitLook);
                 closeAction.MouseButtonClicked += (s, a) => examineMode = false;
                 closeAction.MouseButtonClicked += (s, a) => ReDraw();
                 this.Controls.Add(closeAction);
             }
             else
             {
                 foreach(string thing in things)
                 {
                     ThingWindow tw = new ThingWindow(this, thing);
                     subWindows.Add(tw);
                     Children.Add(tw);
                 }
           
            
          
                 WindowSort();
             }
 
            
        }

        private bool needSort = false;
        public void WindowSort()
        {
            needSort = true;
        }

        public override void Update(TimeSpan delta)
        {
            if (needSort)
            {
                _windowSort();
                needSort = false;
            }
            
            base.Update(delta);
        }

        public void _windowSort()
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
