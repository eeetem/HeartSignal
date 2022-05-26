using System;
using System.Collections.Generic;
using Console = SadConsole.Console;

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
        public string contents = "";

        protected override void DrawConsole()
        {
            Resize(ViewWidth,ViewHeight,Width,50,false);

         /*   string[] words = contents.Split(" ");
            string toPrint = "";
            foreach (var word in words)
            {
                if (word.Contains("[nl]"))
                {

                    Utility.PrintParseMessage(toPrint,actionWindow,this,ExplicitLook);
                    Cursor.NewLine();
                    toPrint = "";
                }

                toPrint +=word+" ";
                toPrint = toPrint.Replace("[nl]", "");
            }*/
         
            Utility.PrintParseMessage(contents.Replace("[nl]","\r\n"),actionWindow,this,ExplicitLook);
            
            //TODO ADD THIS BACK WHEN SADCONSOLE FIXED EFFECT CLEARING
            Resize(ViewWidth,ViewHeight,Width,Math.Max(Cursor.Position.Y,ViewHeight),false);
            //temp fix
            
        }


    }
}
