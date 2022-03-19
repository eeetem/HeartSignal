using System;
using System.Collections.Generic;
using SadConsole;
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



        public NestedInfo inventoryInfo = new NestedInfo();

        public Point ActionOffset = new Point(0,0);



        protected override void DrawConsole()
        {
            Resize(ViewWidth,ViewHeight,Width,100,false);

            DrawContents(inventoryInfo, 0);

            Resize(ViewWidth,ViewHeight,Width,Math.Max(Cursor.Position.Y,ViewHeight),false);
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





            if (ls ==null || ls.Count == 0) { return; }

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
            if(info.Header == null) return;
            string[] returned = Utility.SplitThingId(info.Header); string thing = returned[0];
            string id = returned[1];
            this.DrawLine(Cursor.Position, Cursor.Position + new Point(layer, 0), ICellSurface.ConnectedLineThin[1]);
            Cursor.Right(layer);
            Utility.CreateButtonThingId(returned, this, actionWindow, false,ActionOffset);
            if (layer == 0)
            {
                Cursor.Print(":");
            }
            Cursor.NewLine();
            foreach (NestedInfo innerinfo in info.Contents) {
                DrawContents(innerinfo, layer + 1);


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
