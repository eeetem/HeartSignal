using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace HeartSignal
{
    public class TabConsole : BaseConsole
    {

        public TabConsole(int width, int height) : base(width, height, true, true)
        {

            // Disable the cursor since our keyboard handler will do the work.
            SadComponents.Add(new MouseHandler());

        }

        public Dictionary<string, string> tabs = new Dictionary<string, string>();
        private string selectedTab = "";

        protected override void DrawConsole()
        {
            if (tabs.Count == 0)
            {
                return;
            }

            Resize(ViewWidth, ViewHeight, Width, 50, false);
            if (tabs.ContainsKey(selectedTab) && tabs[selectedTab].Length < 1)
            {
                tabs.Remove(selectedTab);
            }
            if (!tabs.ContainsKey(selectedTab))
            {
                selectedTab = tabs.Keys.ToList()[0];
            }
            foreach (var tab in tabs.Keys)
            {
                if (tab == selectedTab)
                {
                    Cursor.Print(tab);
                    continue;
                }
              
                
                
                var tabbtn = new Button(tab.Length, 1)
                {
                    Text = tab,
                    Position = Cursor.Position,
                    Theme = new ButtonLinesTheme()
                };
                tabbtn.MouseButtonClicked += (s, a) => selectedTab = tab;
                tabbtn.MouseButtonClicked += (s, a) => ReDraw();
                this.Controls.Add(tabbtn);
                Cursor.Right(tab.Length);
                
                
               
            }


            Cursor.Position = new Point(0, 1);
            Utility.PrintParseMessage(tabs[selectedTab], actionWindow, this, true);

        

         


            Resize(ViewWidth, ViewHeight, Width, Math.Max(Cursor.Position.Y + 15, ViewHeight), false);

        }



    }
}
