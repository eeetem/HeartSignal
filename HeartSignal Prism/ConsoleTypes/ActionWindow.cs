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
    class Actionwindow : SadConsole.UI.ControlsConsole
    {
        public Actionwindow(int width, int height,Point position) : base(width, height)
        {


            this.Position = position;
        }


        public void DisplayActions(string item, Point? newPosition = null) {
            if (focusitem != null) { item = focusitem; }
            if (!Program.actionDatabase.ContainsKey(item) || Program.actionDatabase[item] == null)
            {

                Program.SendNetworkMessage("ex " + item);
                return;
            }

            if (newPosition != null) {

                Position = (Point)newPosition;
            }
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Print(1, 0, item);
            this.Cursor.Position = new Point(1, 1);
            foreach (string action in Program.actionDatabase[item])
            {

                
                if (Cursor.Position.X + action.Length+1> Width) {
                    Cursor.NewLine().Right(1);
                }
                Point pos = this.Cursor.Position;
                var button = new Button(action.Length, 1)
                {
                    Text = action,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };
                button.MouseButtonClicked += (s, a) => DoAction(item, action);
                this.Controls.Add(button);
                this.Cursor.Right(action.Length + 1);

            }
            foreach (string action in Program.argactionDatabase[item])
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
                button.MouseButtonClicked += (s, a) => DoArgAction(item, action);
                this.Controls.Add(button);
                this.Cursor.Right(action.Length + 1);

            }
            this.IsVisible = true;
            this.IsEnabled = true;



        }
        private void DoAction(string item, string action)
        {

            Program.SendNetworkMessage(action + " " + item);
        }
        static bool awaitingItemClick = false;
        static string PendingArgMessage = "";
        private static void DoArgAction(string item, string action) {
            PendingArgMessage = action.Replace("[name]", item);
            awaitingItemClick = true;



        }

        /// <summary>
        /// returns true if focus was swaped - returns false if the click was used for something else
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// 
        string focusitem;
        public void SetFocus(string item)
        {
            if (!awaitingItemClick)
            {
                focusitem = item;
                DisplayActions(item);
                return;
            }
            Program.SendNetworkMessage(PendingArgMessage + " "+item);
            awaitingItemClick = false;
            return;

        }
        public void ClearFocus() {


            focusitem = null;
        
        }


    }

}
