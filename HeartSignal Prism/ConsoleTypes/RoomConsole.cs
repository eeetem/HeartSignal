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

            Cursor.Print("There is ");
            foreach (string thing in ls)
            {
                index++;
                Point pos = Cursor.Position;
                var button = new Button(thing.Length, 1)
                {
                    Text = thing,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };
                button.MouseEnter += (s, a) => actionWindow.DisplayActions(thing, new Point(0, Cursor.Position.Y));
                button.Click += (s, a) => actionWindow.SetFocus(thing);
                Controls.Add(button);
                Cursor.Right(thing.Length);

               // this.SetDecorator(pos.X, pos.Y, thing.Length, new CellDecorator(Color.White, 95, Mirror.None));
                if (index >= ls.Count)
                {


                }
                else if (index == ls.Count - 1)
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
