﻿using System;
using System.Collections.Generic;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.Input;
using Console = System.Console;

namespace HeartSignal
{
    class MapConsole : BaseConsole, IMouseInputReciver
    {
        public MapConsole(int width, int height) : base(width, height,false)
        {//todo fix border scaling



            FontSize = Font.GetFontSize(IFont.Sizes.Two);
            SadComponents.Add(new MouseHandler());



        }
        readonly Point middle = new Point(3,3);

        public void SetMapData(List<string> data)
        {
            mapdata = data;
        }
       public void Clicked(Point loc, MouseScreenObjectState state) {

            loc = new Point((int)Math.Floor((double)loc.X / 2), loc.Y); //cells are 2 coordinates wide

            Point relative = loc - middle;

            string msg = "map";
            if (state.Mouse.LeftButtonDown)
            {
                msg += "lmb";
            }
            else if (state.Mouse.RightButtonDown)
            {
                msg += "rmb";
            }

            if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift))
            {
                msg += "shift";
            }
            if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl))
            {
                msg += "ctrl";
            }

            msg += " " + relative.X + " " + relative.Y;


            NetworkManager.SendNetworkMessage(msg);

       }

       public void RightClicked(Point clickloc, MouseScreenObjectState state)
       {
           Clicked(clickloc,state);
       }

       public List<string> cexists = new List<string>();
        private List<string> mapdata = new List<string>();

        protected override void DrawConsole()
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

                Cursor.Print(line).NewLine().Right(2);



            }


        }



    }

}
