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
        public InventoryConsole(int width, int height) : base(width, height, true, true)
        {
            Cursor.DisableWordBreak = false;
            SadComponents.Add(new MouseHandler());
        }

        public string name { get; private set; }


        private bool[,] RevealIndex = new bool[20, 20];
        private int[] tempRevealIndex = new int[20];
        public NestedInfo inventoryInfo = new NestedInfo();

        public Point ActionOffset = new Point(0, 0);



        protected override void DrawConsole()
        {
            Resize(ViewWidth, ViewHeight, Width, 100, false);

            DrawContents(inventoryInfo, 0, 0, true);

            Resize(ViewWidth, ViewHeight, Width, Math.Max(Cursor.Position.Y, ViewHeight), false);
        }


        private void DrawContents(NestedInfo info, int layer, int index, bool isLast, int ignoreLines = 0)
        {
            if (info.Header == null) return;

            bool quitFlag = false;
            
          // this.DrawLine(Cursor.Position, Cursor.Position + new Point(layer, 0), ICellSurface.ConnectedLineThin[1]);
          if (layer != 0)
          {
              
              Cursor.Right(1);
          }
          
          for (int i = 1; i < layer; i++)
            {
                if (i <= ignoreLines)
                {
                    Cursor.Right(1);
                    continue;
                }

                Cursor.Print(((char)179).ToString());
        
            }

          
            
            if (isLast)
            {
                Cursor.Print(((char)192).ToString());
            }
            else
            {
                Cursor.Print(((char)195).ToString());
            }
            

          
            if (info.Contents.Count > 0 && layer > 0)
            {
                
            
                if (RevealIndex[layer, index])
                {
                    var closeAction = new Button("[-]".Length, 1)
                    {
                        Text = "[-]",
                        Position = Cursor.Position,
                        Theme = new ThingButtonTheme(new Gradient(Color.Red, Color.Pink, Color.Red))
                    };
                    int index1 = index;
                    closeAction.MouseButtonClicked += (s, a) => RevealIndex[layer,index1] = !RevealIndex[layer,index1];
                    closeAction.MouseButtonClicked += (s, a) => ReDraw();
                    closeAction.MouseButtonClicked += (s, a) => AudioManager.ParseRequest(null, "play", "interface/nest_close.ogg");

                    this.Controls.Add(closeAction);
                
                }else if (tempRevealIndex[layer] == index)
                {
                
                    var closeAction = new Button("[?]".Length, 1)
                    {
                        Text = "[?]",
                        Position = Cursor.Position,
                        Theme = new ThingButtonTheme(new Gradient(Color.Red, Color.Pink, Color.Red))
                    };
                    int index1 = index;
                    closeAction.MouseButtonClicked += (s, a) => RevealIndex[layer,index1] = !RevealIndex[layer,index1];
                    closeAction.MouseButtonClicked += (s, a) => ReDraw();
                    closeAction.MouseButtonClicked += (s, a) => AudioManager.ParseRequest(null, "play", "interface/nest_open.ogg");
                    closeAction.MouseExit += (s, a) => tempRevealIndex[layer] = -1;
                    closeAction.MouseExit += (s, a) => AudioManager.ParseRequest(null, "play", "interface/nest_close.ogg");
                    closeAction.MouseExit += (s, a) => ReDraw();
                    this.Controls.Add(closeAction);
                }
                else
                {
                    var closeAction = new Button("[+]".Length, 1)
                    {
                        Text = "[+]",
                        Position = Cursor.Position,
                        Theme = new ThingButtonTheme(new Gradient(Color.Pink, Color.Red, Color.Pink))
                    };
                    int index1 = index;
                    closeAction.MouseEnter += (s, a) => tempRevealIndex[layer] = index1;
                    closeAction.MouseEnter += (s, a) => ReDraw();
                    closeAction.MouseEnter += (s, a) => AudioManager.ParseRequest(null, "play", "interface/nest_open.ogg");
                    this.Controls.Add(closeAction);
                    quitFlag = true;

                }
                Cursor.Right(3);
            }

            
            Utility.PrintParseMessage(info.Header, actionWindow, this, false, 1);
            if(quitFlag) return;

            index = 0;
            foreach (NestedInfo innerinfo in info.Contents)
            {
                Cursor.Left(100);
                bool last = index+1  == info.Contents.Count;
                if (last && ignoreLines >= layer)
                {
                    ignoreLines++;
                }

                DrawContents(innerinfo, layer + 1, index, last,ignoreLines );


                //    Cursor.NewLine();


                index++;
                if (layer == 0 && !last)
                {
                    Cursor.Print(((char)179).ToString());
                    Cursor.NewLine();
                    //
                    //Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y + 1);
                }

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
