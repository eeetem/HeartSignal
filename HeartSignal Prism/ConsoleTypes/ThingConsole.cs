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
    class ThingConsole : SadConsole.UI.ControlsConsole
    {
        public ThingConsole(int width, int height) : base(width, height)
        {


            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;


        }

        public List<string> lines = new List<string>();
        public void DrawThing()
        {
            needRedraw = false;
            this.Clear();


            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine().NewLine().NewLine();
            foreach (string desc in new List<string>(lines))
            {
                Cursor.Print(desc);
                Cursor.NewLine();

            }

        }





        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                DrawThing();



            }

        }



        public bool needRedraw = false;






        


    }

}
