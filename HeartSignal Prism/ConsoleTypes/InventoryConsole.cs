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
    class InventoryConsole : SadConsole.UI.ControlsConsole
    {
        public InventoryConsole(int width, int height) : base(width, height)
        {


            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            actionWidnow = new ControlsConsole(30, 5);
            Program.root.Children.Add(actionWidnow);
            actionWidnow.IsVisible = false;

        }
        public string name { get; private set; }
        public Dictionary<string,List<string>> inventoryInfo = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> holdingInfo = new Dictionary<string, List<string>>();








        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawInventory()
        {
            needRedraw = false;
            this.Clear();
           // focusitem = null;
            actionWidnow.Clear();
            actionWidnow.Controls.Clear();

            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            //this could be re-used for self-description
            /* Cursor.NewLine().NewLine().NewLine();
             Cursor.Print(name).NewLine();
             foreach (string desc in new List<string>(roomInfo))
             {
                 Cursor.Print(desc);


             }
             Cursor.NewLine();*/
            Cursor.Print("My Body:").NewLine();
            DrawList(new Dictionary<string, List<string>>(inventoryInfo));
            Cursor.Print("I can hold with:").NewLine();
            DrawList(new Dictionary<string, List<string>>(holdingInfo));
            this.IsFocused = true; 
        }





        private void DrawList(Dictionary<string, List<string>> ls)
        {





            if (ls.Count == 0) { return; }

            foreach (KeyValuePair<string, List<string>> info in ls)
            {
               
                
                Cursor.Print(info.Key + ":").NewLine();
                foreach (string item in info.Value) {
                    var button = new Button(item.Length, 1)
                    {
                        Text = item,
                        Position = Cursor.Position,
                        Theme = new ThingButtonTheme()
                    };
                    button.MouseEnter += (s, a) => RetriveActions(item);
                    button.Click += (s, a) => SetFocus(item);
                    Controls.Add(button);
                    Cursor.NewLine();
                  //  this.SetDecorator(pos.X, pos.Y, thing.Length, new CellDecorator(Color.White, 95, Mirror.None));


                }





            }
            Cursor.NewLine();
        }
        
        private void SetFocus(string item) {
            focusitem = item;
            RetriveActions(item);
        
        }
        
        ControlsConsole actionWidnow;
        string focusitem;
        public bool needRedraw = false;
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                DrawInventory();



            }

        }
        private void RetriveActions(string item)
        {
           if(focusitem != null)
            {


                item = focusitem;
            }
            if (!Program.actionDatabase.ContainsKey(item) || Program.actionDatabase[item] == null)
            {

                Program.SendNetworkMessage("ex " + item);
            }
            else
            {
                actionWidnow.Clear();
                actionWidnow.Controls.Clear();
                var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
                actionWidnow.DrawBox(new Rectangle(0, 0, 30, 5), boxShape);
                actionWidnow.Position = new Point(Game.Instance.ScreenCellsX-(20+30), Game.Instance.ScreenCellsY-(5+2));

                actionWidnow.Cursor.Position = new Point(1, 3);
                foreach (string action in Program.actionDatabase[item])
                {

                    Point pos = actionWidnow.Cursor.Position;
                    var button = new Button(action.Length, 1)
                    {
                        Text = action,
                        Position = pos,
                        Theme = new ThingButtonTheme()
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
