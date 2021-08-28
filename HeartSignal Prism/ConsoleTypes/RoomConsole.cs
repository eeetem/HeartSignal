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
            if (actionWidnow != null)
            {
                actionWidnow.Dispose();
                actionWidnow = null;//i'm not sure it is mentioned elsewhere in the sadconsole itself? if it is it might cause garbage collection issues, but for now i'll leave it as is
            }
            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine().NewLine().NewLine();
            Cursor.Print(name).NewLine();
            foreach (string desc in roomInfo)
            {
                Cursor.Print(desc);


            }
            Cursor.NewLine();
            DrawList(thingInfo);
            DrawList(bodyInfo);
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

        public Dictionary<string, List<string>> actionDatabase = new Dictionary<string, List<string>>();
        Window actionWidnow;
        private void RetriveActions(string item)
        {
            if (!actionDatabase.ContainsKey(item) || actionDatabase[item] == null)
            {

                Program.SendNetworkMessage("ex " + item);
            }
            else
            {if (actionWidnow != null)
                {
                    actionWidnow.Dispose();
                }
                actionWidnow = new Window(30, 5);
                actionWidnow.Title = "Actions";


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
                    actionWidnow.Cursor.Right(action.Length+1);

                }
                actionWidnow.IsVisible = true;
                actionWidnow.IsEnabled = true;
                actionWidnow.Show();
            }

        }
        private void DoAction(string item,string action)
        {

            Program.SendNetworkMessage(action +" "+ item);
        }


    }

}
