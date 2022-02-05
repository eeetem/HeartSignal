using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System.Collections.Generic;

namespace HeartSignal
{
    internal class DisplayConsole : BaseConsole
    {

        public DisplayConsole(int width, int height) : base(width, height,true,true)
        {

            // Disable the cursor since our keyboard handler will do the work.
            SadComponents.Add(new MouseHandler());
            
         
        }
        public bool ExplicitLook = false;
        public List<string> lines = new List<string>();

        protected override void DrawConsole()
        {
            Resize(ViewWidth,ViewHeight,Width,50,false);

            foreach (string fancy in new List<string>(lines))
            {
                Utility.PrintParseMessage(fancy,actionWindow,this,ExplicitLook);
            }
            //TODO ADD THIS BACK WHEN SADCONSOLE FIXED EFFECT CLEARING
            //Resize(ViewWidth,ViewHeight,Width,Math.Max(Cursor.Position.Y,ViewHeight),false);
            //temp fix
            
        }

      
    }
}
