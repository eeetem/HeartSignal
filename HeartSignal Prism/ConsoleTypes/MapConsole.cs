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
            UsePrintProcessor = true;
            Cursor.DisableWordBreak = true;
            SadComponents.Add(new MouseHandler());


        }
        readonly Point middle = new Point(3,3);

       public void Clicked(Point loc) {
            System.Console.WriteLine(loc);
            loc = new Point((int)Math.Floor((double)loc.X / 2), loc.Y); //cells are 2 coordinates wide

            double angle = Utility.GetAngleOfLineBetweenTwoPoints(loc, middle);
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

        public List<string> cexists = new List<string>();
        public List<string> mapdata = new List<string>();

        public void DrawMap()
        {
            
             this.Clear();

            

            //not a super big fan of the large ammount of repeating here, but it will do for now
            this.Print(0, 0, "[c:r f:lightgray]NW[c:u]");
            this.Print(6, 0, "[c:r f:lightgray]NN[c:u]");
            this.Print(12, 0, "[c:r f:lightgray]NE[c:u]");
            this.Print(0, 3, "[c:r f:lightgray]WW[c:u]");
            this.Print(12, 3, "[c:r f:lightgray]EE[c:u]");
            this.Print(0, 6, "[c:r f:lightgray]SW[c:u]");
            this.Print(6, 6, "[c:r f:lightgray]SS[c:u]");
            this.Print(12, 6, "[c:r f:lightgray]SE[c:u]");
            foreach (string dir in cexists) {

                switch (dir) {
                    case "NW":
                       this.Print(0, 0, "[c:r f:red]NW[c:u]");
                        break;
                    case "NN":
                        this.Print(6, 0, "[c:r f:red]NN[c:u]");
                        break;
                    case "NE":
                        this.Print(12, 0, "[c:r f:red]NE[c:u]");
                        break;
                    case "WW":
                        this.Print(0, 3, "[c:r f:red]WW[c:u]");
                        break;
                    case "EE":
                        this.Print(12, 3, "[c:r f:red]EE[c:u]");
                        break;
                    case "SW":
                        this.Print(0, 6, "[c:r f:red]SW[c:u]");
                        break;
                    case "SS":
                        this.Print(6, 6, "[c:r f:red]SS[c:u]");
                        break;
                    case "SE":
                        this.Print(12,6, "[c:r f:red]SE[c:u]");
                        break;





                }
            
            
            
            }
            Cursor.Position = new Point(2, 1);
            foreach (string line in mapdata)
            {
                System.Console.WriteLine(line);
                Cursor.Print(line).NewLine().Right(2);



            }


        }




    }

}
