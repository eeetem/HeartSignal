using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI.Controls;
using SadRogue.Primitives;

namespace HeartSignal
{
    class InventoryConsole : BaseConsole
    {
        public InventoryConsole(int width, int height) : base(width, height,true,true)
        {
            Cursor.DisableWordBreak = false;
            SadComponents.Add(new MouseHandler());
        }
        public string name { get; private set; }


        private bool[] RevealIndex = new bool[20];
        private int tempRevealIndex = 0;
        public NestedInfo inventoryInfo = new NestedInfo();

        public Point ActionOffset = new Point(0,0);



        protected override void DrawConsole()
        {
            Resize(ViewWidth,ViewHeight,Width,100,false);

            DrawContents(inventoryInfo, 0);

            Resize(ViewWidth,ViewHeight,Width,Math.Max(Cursor.Position.Y,ViewHeight),false);
        }


        private void DrawContents(NestedInfo info,int layer) {
            if(info.Header == null) return;

            this.DrawLine(Cursor.Position, Cursor.Position + new Point(layer, 0), ICellSurface.ConnectedLineThin[1]);
            Cursor.Right(layer);
            Utility.PrintParseMessage(info.Header,actionWindow,this,false,1);
            int index = 0;
            foreach (NestedInfo innerinfo in info.Contents)
            {
                
                
                if (RevealIndex[index] || layer > 0 || tempRevealIndex == index) 
                {
                    if (layer == 0)
                    {
                        var closeAction = new Button("[X]".Length, 1)
                        {
                            Text = "[X]",
                            Position = Cursor.Position,
                            Theme = new ThingButtonTheme(new Gradient(Color.Red,Color.Pink,Color.Red))
                        };
                        int index1 = index;
                        closeAction.MouseButtonClicked += (s, a) => RevealIndex[index1] = !RevealIndex[index1];
                        closeAction.MouseButtonClicked += (s, a) => ReDraw();
         
                        this.Controls.Add(closeAction);
                      Cursor.Right(3);
                    }
               
                    DrawContents(innerinfo, layer + 1);
                    
                }
                else
                {
                    if (layer == 0)
                    {
                        var closeAction = new Button("[O]".Length, 1)
                        {
                            Text = "[O]",
                            Position = Cursor.Position,
                            Theme = new ThingButtonTheme(new Gradient(Color.Pink, Color.Red, Color.Pink))
                        };
                        int index1 = index;
                        closeAction.MouseEnter += (s, a) => tempRevealIndex = index1;
                        closeAction.MouseEnter += (s, a) => ReDraw();
                        this.Controls.Add(closeAction);
                        Cursor.Right(3);
                    }

                    Utility.PrintParseMessage(innerinfo.Header,actionWindow,this,false,1);
                }
                
                
                index++;


            }

        }

        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            return base.ProcessMouse(state);
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
