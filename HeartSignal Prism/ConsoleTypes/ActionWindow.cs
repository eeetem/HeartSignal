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
    class ActionWindow : SadConsole.UI.ControlsConsole
    {
        public ActionWindow(int width, int height, Point position) : base(width, height)
        {


            this.Position = position;
        }
        public static Dictionary<string, List<string>> actionDatabase = new Dictionary<string, List<string>>();
        public static Dictionary<string, List<string>> argactionDatabase = new Dictionary<string, List<string>>();
        string lastitem = "";
        public void DisplayActions(string item, Point? newPosition = null)
        {
            //if (focusitem != null) { item = focusitem; }
            if (awaitingItemClick) { return; }



            string[] returned = Utility.SplitThingID(item);
            string thing = returned[0];
            string id = returned[1];

            if (!GetActions(id))
            {
                return;

            }
            if (lastitem != item)
            {
                GetDesc(id);

                lastitem = item;
            }

            if (newPosition != null)
            {

                Position = (Point)newPosition;
            }
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
           // this.Print(1, 0, thing);
            this.Cursor.Position = new Point(1, 1);
            foreach (string action in actionDatabase[id])
            {


                if (Cursor.Position.X + action.Length + 1 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Point pos = this.Cursor.Position;
                var button = new Button(action.Length, 1)
                {
                    Text = action,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };
                button.MouseButtonClicked += (s, a) => DoAction(id, action);
                this.Controls.Add(button);
                this.Cursor.Right(action.Length + 1);

            }
            foreach (string action in argactionDatabase[id])
            {

                string parsedAction = action.Replace(" [name]", "") + "...";
                if (Cursor.Position.X + parsedAction.Length + 1 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Point pos = this.Cursor.Position;
                var button = new Button(parsedAction.Length, 1)
                {
                    Text = parsedAction,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };
                button.MouseButtonClicked += (s, a) => DoArgAction(id, action);
                this.Controls.Add(button);
                this.Cursor.Right(action.Length + 1);

            }
            this.IsVisible = true;
            this.IsEnabled = true;



        }
        public void DisplayMultiItem(string name, Point? newPosition = null, List<string> IDs = null)
        {
            //if (awaitingItemClick) { return; }

            foreach (string id in IDs)
            {

                GetActions(id);


            }

            if (newPosition != null)
            {

                Position = (Point)newPosition;
            }
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Print(1, 0, name);
            this.Cursor.Position = new Point(1, 1);
            Cursor.Print("Which one?").NewLine().Right(1);
            for (int i = 1; i < IDs.Count() + 1; i++)
            {



                if (Cursor.Position.X + 2 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Point pos = this.Cursor.Position;
                string buttontext = i.ToString();
                if (buttontext[buttontext.Length - 1] == '1')
                {
                    buttontext = buttontext + "st";

                }
                else if (buttontext[buttontext.Length - 1] == '2')
                {
                    buttontext = buttontext + "nd";

                }
                else if (buttontext[buttontext.Length - 1] == '3')
                {
                    buttontext = buttontext + "rd";

                }
                else
                {
                    buttontext = buttontext + "th";

                }
                var button = new Button(buttontext.Length, 1) 
                {
                    Text = buttontext,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };

                int foo = i - 1;
                button.MouseEnter += (s, a) => DisplayActions(name + "(" + IDs[foo] + ")", null);
                button.Click += (s, a) => ClickItem(IDs[foo]);
                this.Controls.Add(button);
                this.Cursor.Right(buttontext.Length + 1);

            }


            this.IsVisible = true;
            this.IsEnabled = true;



        }

        private void DoAction(string id, string action)
        {
            //index++;///arrays starting at 1 momment
            Program.SendNetworkMessage(action + " " + id);
        }

        static bool awaitingItemClick = false;
        static string PendingArgMessage = "";
        private static void DoArgAction(string id, string action)
        {
            // index++;///arrays starting at 1 momment
            PendingArgMessage = action.Replace("[name]", id);
            awaitingItemClick = true;



        }


        public void ClickItem(string item)
        {


            if (awaitingItemClick)
            {
                Program.SendNetworkMessage(PendingArgMessage + " " + item);
                awaitingItemClick = false;
                return;
            }
        }
        public static bool GetActions(string id)
        {

            //this being outside the IF causes ex spam but currently it's needed for descriptions, definatelly possible to optimise this if needed

            if (!actionDatabase.ContainsKey(id) || actionDatabase[id] == null)
            {

                Program.SendNetworkMessage("ex " + id);
                return false;
            }
            return true;

        }
        public static void GetDesc(string id)
        {
            Program.SendNetworkMessage("look " + id);

        }

    }
}
