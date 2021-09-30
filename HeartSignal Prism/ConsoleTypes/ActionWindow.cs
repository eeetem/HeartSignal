using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.UI;
using SadConsole.Input;

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
        static string lastitem = "";

        protected override void OnMouseExit(MouseScreenObjectState state) {
            IsVisible = false;
            IsEnabled = false;
        
        
        }

        //a lot fo reapeating code in here, integrate this better at some point
        public void ShowTooltip(string text, Point? newPosition = null) {
            this.Resize(40, 12, 40, 12, false);
            if (newPosition != null)
            {

                Position = (Point)newPosition;
            }
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Green, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Cursor.Position = new Point(1, 1);
            string[] words = text.Split("_");
            foreach (string word in words)
            {
                if (Cursor.Position.X + word.Length + 2 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Cursor.Print(word).Right(1);
            }

            this.IsVisible = true;
            this.IsEnabled = true;
        }
        public void DisplayActions(string item, Point? newPosition = null, bool expilcitlook = false)
        {
            //if (focusitem != null) { item = focusitem; }
            if (awaitingItemClick) { return; }

            this.Resize(30, 5, 30, 5, false);

            string[] returned = Utility.SplitThingID(item);
            string thing = returned[0];
            string id = returned[1];

            if (lastitem != item)
            {
                if (!expilcitlook)
                {
                    GetDesc(id);
                }

                lastitem = item;
            }
            if (!actionDatabase.ContainsKey(id)) {
                return;
            }

            if (newPosition != null)
            {

                Position = (Point)newPosition;
            }
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
           this.Print(1, 0, "Actions");
            this.Cursor.Position = new Point(1, 1);
            foreach (string action in actionDatabase[id])
            {

                string parsedAction = action.Replace(" [name]", "").Replace("_", " ");
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
                button.MouseButtonClicked += (s, a) => DoAction(id, action);
                this.Controls.Add(button);
                this.Cursor.Right(parsedAction.Length + 1);

            }
            foreach (string action in argactionDatabase[id])
            {

                string parsedAction = action.Replace(" [name]", "").Replace("_"," ") + "...";
                if (Cursor.Position.X + parsedAction.Length+1 > Width)
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
                this.Cursor.Right(parsedAction.Length + 1);

            }
            /*
            var exit = new Button(1, 1)
            {
                Text = "x",
                Position = new Point(Width-1,0),
                Theme = new ThingButtonTheme()
            };
            exit.MouseButtonClicked += (s, a) => IsVisible = false;
            exit.MouseButtonClicked += (s, a) => IsEnabled = false;
            this.Controls.Add(exit);
             */
            if (expilcitlook)
            {
                var look = new Button(4, 1)
                {
                    Text = "look",
                    Position = new Point(9, 0),
                    Theme = new ThingButtonTheme()
                };
                look.MouseButtonClicked += (s, a) => IsVisible = false;
                look.MouseButtonClicked += (s, a) => IsEnabled = false;
                look.MouseButtonClicked += (s, a) => GetDesc(id);
                this.Controls.Add(look);


            }
            
            this.IsVisible = true;
            this.IsEnabled = true;



        }

        public void DisplayMultiItem(string name, Point? newPosition = null, List<string> IDs = null)
        {
            //if (awaitingItemClick) { return; }

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
            /*
            var exit = new Button(1, 1)
            {
                Text = "x",
                Position = new Point(Width - 1, 0),
                Theme = new ThingButtonTheme()
            };
            exit.MouseButtonClicked += (s, a) => IsVisible = false;
            exit.MouseButtonClicked += (s, a) => IsEnabled = false;
            this.Controls.Add(exit);


            this.IsVisible = true;
            this.IsEnabled = true;

            */

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

        public static void GetDesc(string id)
        {
            Program.SendNetworkMessage("look " + id);

        }

    }
}
