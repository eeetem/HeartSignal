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
            actionWindow = new Actionwindow(30, 5, new Point(Game.Instance.ScreenCellsX - (20 + 30), Game.Instance.ScreenCellsY - (5 + 2)));
            Program.root.Children.Add(actionWindow);
            actionWindow.IsVisible = false;

        }
        public string name { get; private set; }
        public Dictionary<string,List<string>> inventoryInfo = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> holdingInfo = new Dictionary<string, List<string>>();








        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawInventory()
        {
            needRedraw = false;
            this.Clear();
            actionWindow.ClearFocus();
            actionWindow.Clear();
            actionWindow.Controls.Clear();

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
                    button.MouseEnter += (s, a) => actionWindow.DisplayActions(item);
                    button.Click += (s, a) => actionWindow.SetFocus(item);
                    Controls.Add(button);
                    Cursor.NewLine();
                  //  this.SetDecorator(pos.X, pos.Y, thing.Length, new CellDecorator(Color.White, 95, Mirror.None));


                }





            }
            Cursor.NewLine();
        }
        

        Actionwindow actionWindow;

        public bool needRedraw = false;
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                DrawInventory();



            }

        }



    }

}
