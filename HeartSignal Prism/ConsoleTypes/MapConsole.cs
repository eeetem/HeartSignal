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
    class MapConsole : SadConsole.UI.ControlsConsole, IMouseInputReciver
    {
        public MapConsole(int width, int height) : base(width, height)
        {


            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            SadComponents.Add(new MouseHandler());


        }
        readonly Point middle = new Point(3,3);

       public void Clicked(Point loc) {
            loc = new Point((int)Math.Ceiling((double)loc.X / 2), loc.Y); //cells are 2 coordinates wide
            
            double angle = GetAngleOfLineBetweenTwoPoints(loc, middle);
            System.Console.WriteLine(angle);
            if (angle == 0) {
                Program.SendNetworkMessage("west");

            }
            else if (angle==180)
            {
                Program.SendNetworkMessage("east");

            }
            else if (angle == 90)
            {
                Program.SendNetworkMessage("north");

            }
            else if (angle == -90)
            {
                Program.SendNetworkMessage("south");

            }
            else if (angle > 0 && angle < 90)
            {
                Program.SendNetworkMessage("northwest");

            }
            else if (angle > 90 && angle < 180)
            {
                Program.SendNetworkMessage("northeast");

            }
            else if (angle > -90 && angle < 0)
            {
                Program.SendNetworkMessage("southwest");

            }
            else if (angle > -180 && angle < -90)
            {
                Program.SendNetworkMessage("southeast");

            }



        }
        //probably not the best spot to put a math fucntion into but whatever, once i have more than 1 use for this i'll seperate it out
        public static double GetAngleOfLineBetweenTwoPoints(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }
        public List<string> cexists = new List<string>();
        public List<string> mapdata = new List<string>();

        public void DrawMap()
        {
            
             this.Clear();
            Cursor.Position = new Point(2,1);
            foreach (string line in mapdata) {
                System.Console.WriteLine(line);
                Cursor.Print(line).NewLine().Right(2);
            
            
            
            }

            this.Print(0, 0, "NW");
            this.Print(6, 0, "NN");
            this.Print(12, 0, "NE");
            this.Print(0, 3, "WW");
            this.Print(12, 3, "EE");
            this.Print(0, 6, "SW");
            this.Print(6, 6, "SS");
            this.Print(12, 6, "SE");

        }




    }

}
