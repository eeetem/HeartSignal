using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.UI;

namespace HeartSignal
{
    class MapConsole : SadConsole.UI.ControlsConsole
    {
        public MapConsole(int width, int height) : base(width, height)
        {


            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;


        }







        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawMap(List<string> lines)
        {
            
             this.Clear();
            Cursor.Position = new Point(1,1);
            foreach (string line in lines) {
                System.Console.WriteLine(line);
                Cursor.Print(line).NewLine();
            
            
            
            }

        }




    }

}
