﻿using System;
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
            actionWindow = new ActionWindow(30, 5, new Point(0,0));
            Program.root.Children.Add(actionWindow);
            actionWindow.IsVisible = false;
            ColoredString.CustomProcessor = Utility.CustomParseCommand;
            SadComponents.Add(new AnimatedBorderComponent());

        }
        public string name { get; private set; }



        public NestedInfo inventoryInfo = new NestedInfo();
        public List<NestedInfo> holdingInfo = new List<NestedInfo>();








        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawInventory()
        {
            needRedraw = false;
            this.Clear();
          //  actionWindow.ClearFocus();
            actionWindow.Clear();
            actionWindow.Controls.Clear();
            actionWindow.IsVisible = false;

            Controls.Clear();
            Cursor.Position = new Point(0, 0);

            Cursor.Print("My Body:").NewLine();
            DrawNestedInfo(inventoryInfo);
            this.DrawLine(Cursor.Position, Cursor.Position + new Point(Width, 0), ICellSurface.ConnectedLineThin[1]);
            Cursor.NewLine();
            Cursor.Print("I can hold with:").NewLine();
            DrawNestedInfo(holdingInfo);
            this.IsFocused = true; 
            
        }


        private void DrawNestedInfo(NestedInfo info)
            
        {
            if (info.Contents == null) { return; }
            foreach (NestedInfo item in info.Contents)
            {
                DrawContents(item, 0);
                Cursor.NewLine();


            }


        }
            private void DrawNestedInfo(List<NestedInfo> ls)
        {





            if (ls.Count == 0) { return; }

            foreach (NestedInfo info in ls)
            {

               
                    Cursor.Print(info.Header + ":").NewLine();

                    foreach (NestedInfo item in info.Contents)
                    {
                        DrawContents(item, 1);


                    }
                
            }
            Cursor.NewLine();
        }
        private void DrawContents(NestedInfo info,int layer) {
   
                string[] returned = Utility.SplitThingID(info.Header);
                string thing = returned[0];
                string id = returned[1];
                this.DrawLine(Cursor.Position, Cursor.Position + new Point(layer, 0), ICellSurface.ConnectedLineThin[1]);
                 Cursor.Right(layer);
            int y = Cursor.Position.Y;
                 var button = new Button(thing.Length, 1)
                {
                    Text = thing,
                    Position = Cursor.Position,
                    Theme = new ThingButtonTheme()
                };
                button.MouseEnter += (s, a) => actionWindow.DisplayActions(info.Header, new Point(Game.Instance.ScreenCellsX - (Width + 30), y + 4));
                button.Click += (s, a) => actionWindow.ClickItem(id);

                Controls.Add(button);
            if (layer == 0)
            {
                Cursor.Right(thing.Length).Print(":");
            }
            Cursor.NewLine();
            foreach (NestedInfo innerinfo in info.Contents) {
                DrawContents(innerinfo, layer + 1);


            }

        }

        ActionWindow actionWindow;

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
    public struct NestedInfo
    {
        public NestedInfo(string header = null, List<NestedInfo> contents = null)
        {
            Header = header;
            if (contents != null)
            {
                Contents = contents;



            }
            else {
                Contents = new List<NestedInfo>();



            }
        }

        public string Header;
        public List<NestedInfo> Contents;

       // public override string ToString() => $"({X}, {Y})";
    }

}
