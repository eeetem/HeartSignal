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
            actionWidnow = new ControlsConsole(30, 5);
            Children.Add(actionWidnow);
            actionWidnow.IsVisible = false;

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

            this.Clear();
            focusitem = null;
            actionWidnow.Clear();
            actionWidnow.Controls.Clear();

            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine().NewLine().NewLine();
            Cursor.Print(name).NewLine();
            foreach (string desc in roomInfo)
            {
                Cursor.Print(desc);


            }
            Cursor.NewLine();
            List<string> copylist;
            copylist = new List<string>(thingInfo);
            DrawList(copylist);
            copylist = new List<string>(bodyInfo);
            DrawList(copylist);
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
                    Theme = new RoomButtonTheme()
                };
                button.MouseEnter += (s, a) => RetriveActions(thing);
                button.Click += (s, a) => SetFocus(thing);
                Controls.Add(button);
                Cursor.Right(thing.Length);

                this.SetDecorator(pos.X, pos.Y, thing.Length, new CellDecorator(Color.White, 95, Mirror.None));
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
        private void SetFocus(string item) {
            focusitem = item;
            RetriveActions(item);
        
        }
        public Dictionary<string, List<string>> actionDatabase = new Dictionary<string, List<string>>();
        ControlsConsole actionWidnow;
        string focusitem;
        private void RetriveActions(string item)
        {
            if(focusitem != null)
            {


                item = focusitem;
            }
            if (!actionDatabase.ContainsKey(item) || actionDatabase[item] == null)
            {

                Program.SendNetworkMessage("ex " + item);
            }
            else
            {
                actionWidnow.Clear();
                actionWidnow.Controls.Clear();
                var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
                actionWidnow.DrawBox(new Rectangle(0, 0, 30, 5), boxShape);
                actionWidnow.Position = new Point(0, Cursor.Position.Y);
                actionWidnow.Cursor.Position = new Point(1, 3);
                foreach (string action in actionDatabase[item])
                {

                    Point pos = actionWidnow.Cursor.Position;
                    var button = new Button(action.Length, 1)
                    {
                        Text = action,
                        Position = pos,
                        Theme = new RoomButtonTheme()
                    };
                    button.MouseButtonClicked += (s, a) => DoAction(item, action);
                    actionWidnow.Controls.Add(button);
                    actionWidnow.Cursor.Right(action.Length + 1);

                }
                actionWidnow.IsVisible = true;
                actionWidnow.IsEnabled = true;

                //   actionWidnow.Show();
            }

        }
        private void DoAction(string item, string action)
        {

            Program.SendNetworkMessage(action + " " + item);
        }

    }

}
