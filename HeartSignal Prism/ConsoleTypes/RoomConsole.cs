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
    class RoomConsole : SadConsole.UI.ControlsConsole
    {
        public RoomConsole(int width, int height) : base(width, height)
        {


            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            //since both inventory and room consoles use very similar actionWindows - turn it into a class at some point
            actionWindow = new Actionwindow(30, 5, new Point(0, Cursor.Position.Y));
            Children.Add(actionWindow);
            actionWindow.IsVisible = false;

        }
        public string name { get; private set; }
        public List<string> roomInfo = new List<string>();
        public List<string> thingInfo = new List<string>();
        public List<string> bodyInfo = new List<string>();


        //setting name aka changing rooms wipes everything
        public void SetName(string n)
        {


            name = n;
            roomInfo = new List<string>();
            thingInfo = new List<string>();
            bodyInfo = new List<string>();

        }




        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawRoom()
        {
           needRedraw = false;
            this.Clear();

            actionWindow.Clear();
            actionWindow.Controls.Clear();

            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine().NewLine().NewLine();
            Cursor.Print(name).NewLine();
            foreach (string desc in new List<string>(roomInfo))
            {
                Cursor.Print(desc);


            }
            Cursor.NewLine();

            DrawList(new List<string>(thingInfo));

            DrawList(new List<string>(bodyInfo));
            this.IsFocused = true; 
        }





        private void DrawList(List<string> ls)
        {

  


            if (ls.Count == 0) { return; }
            int index = 0;
            int indexoffset = 0;
            List<string[]> thingids = new List<string[]>();
            foreach (string thingid in ls) {
                thingids.Add(Utility.SplitThingID(thingid));

            }

            Cursor.Print("There is ");
            List<string> processed = new List<string>();
            foreach (string[] thingid in thingids)
            {
                
                if (processed.Contains(thingid[0])) {
                    continue;
                
                }
                index++;
                ///if there is other things with same name process them at the same time
                List<string> sameThingsIDs = new List<string>();
                foreach (string[] innerthingid in thingids)
                {
                    if (thingid[0] == innerthingid[0]) {

                        processed.Add(innerthingid[0]);
                        sameThingsIDs.Add(innerthingid[1]);
                                        
                    }
                
                }


                    bool multiple = false;
                if (sameThingsIDs.Count() > 1) {
                    multiple = true;
                    indexoffset += sameThingsIDs.Count() - 1;
                }

                if (thingid[0].Length + Cursor.Position.X + 5 > Width) {
                    Cursor.NewLine().Right(1);
                }


                if (!multiple) {
                    if (thingid[0].ToLower()[0] == 'a' || thingid[0].ToLower()[0] == 'e')
                    {

                        Cursor.Print("an ");
                    }
                    else
                    {

                        Cursor.Print("a ");
                    }
                }
                else
                {

                    Cursor.Print(Utility.ConvertWholeNumber(sameThingsIDs.Count().ToString())+" ");

                }
                Point pos = Cursor.Position;
                if (!multiple)
                {
                    
                    var button = new Button(thingid[0].Length, 1)
                    {
                        Text = thingid[0],
                        Position = pos,
                        Theme = new ThingButtonTheme()
                    };
                    button.MouseEnter += (s, a) => actionWindow.DisplayActions(thingid[0]+"("+thingid[1] + ")", pos + new Point(-3, 1));
                    button.Click += (s, a) => actionWindow.ClickItem(thingid[1]);
                    Controls.Add(button);
                    Cursor.Right(thingid[0].Length);
                }
                else {
                    var button = new Button(thingid[0].Length+1, 1)
                    {
                        Text = thingid[0] + "s",
                        Position = pos,
                        Theme = new ThingButtonTheme()
                    };
                    button.MouseEnter += (s, a) => actionWindow.DisplayMultiItem(thingid[0], pos + new Point(-3, 1), sameThingsIDs);
                   // button.Click += (s, a) => actionWindow.SetFocus(thing.Key);
                    Controls.Add(button);
                    Cursor.Right(thingid[0].Length+ 1);



                }

                if (index >= thingids.Count()-indexoffset)
                {


                }
                else if (index == thingids.Count() - indexoffset - 1)
                {

                    Cursor.Print(" and ");
                }

                else
                {
                    Cursor.Print(", ");

                }





            }
            Cursor.NewLine();
        }
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                DrawRoom();



            }

        }

        Actionwindow actionWindow;

        public bool needRedraw = false;






        


    }

}
