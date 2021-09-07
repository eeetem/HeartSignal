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


        public void DisplayActions(string item, Point? newPosition = null,int index =1) {
            //if (focusitem != null) { item = focusitem; }
            if (!Program.actionDatabase.ContainsKey(item) || Program.actionDatabase[item] == null)
            {
               // index++;///arrays starting at 1 momment
                Program.SendNetworkMessage("ex " + index+"."+item);
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
                button.MouseButtonClicked += (s, a) => DoAction(item, action, index);
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
                button.MouseButtonClicked += (s, a) => DoArgAction(item, action,index);
                this.Controls.Add(button);
                this.Cursor.Right(action.Length + 1);

            }
            this.IsVisible = true;
            this.IsEnabled = true;



        }
        public void DisplayMultiItem(string item, Point? newPosition = null, int count = 1)
        {
            if (awaitingItemClick) { return; }
            //if (focusitem != null && focusitem != item) { return; }
            if (!Program.actionDatabase.ContainsKey(item) || Program.actionDatabase[item] == null)
            {

                Program.SendNetworkMessage("ex " + "1."+item);
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
            this.Print(1, 0, item);
            this.Cursor.Position = new Point(1, 1);
            Cursor.Print("Which " + item + "?").NewLine().Right(1);
            for (int i = 1; i < count+1; i++)
            {
               
                

                if (Cursor.Position.X + 2> Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Point pos = this.Cursor.Position;
                string buttontext = i.ToString();
                if (buttontext[buttontext.Length-1] == '1')
                {
                    buttontext = buttontext + "st";

                }
                else if (buttontext[buttontext.Length-1] == '2')
                {
                    buttontext = buttontext + "nd";

                }
                else if (buttontext[buttontext.Length-1] == '3')
                {
                    buttontext = buttontext + "rd";

                }
                else {
                    buttontext = buttontext + "th";

                }
                var button = new Button(buttontext.Length, 1)
                {
                    Text = buttontext,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };

                int foo = i;
                button.MouseEnter += (s, a) => DisplayActions(item, null, foo);
                button.Click += (s, a) => SetFocus(item, foo);
                this.Controls.Add(button);
                this.Cursor.Right(buttontext.Length + 1);

            }
          
            
            this.IsVisible = true;
            this.IsEnabled = true;



        }

        private void DoAction(string item, string action, int index = 1)
        {
            //index++;///arrays starting at 1 momment
            Program.SendNetworkMessage(action + " " + index+"."+item);
        }

        static bool awaitingItemClick = false;
        static string PendingArgMessage = "";
        private static void DoArgAction(string item, string action,int index = 1) {
           // index++;///arrays starting at 1 momment
            PendingArgMessage = action.Replace("[name]", index+"."+item);
            awaitingItemClick = true;



        }

        /// <summary>
        /// returns true if focus was swaped - returns false if the click was used for something else
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// 
      
        public void SetFocus(string item, int index = 1)
        {

          
            if (awaitingItemClick)
            {
                Program.SendNetworkMessage(PendingArgMessage + " " + index + "." + item);
                awaitingItemClick = false;
                return;
            }
        }



    }

}
