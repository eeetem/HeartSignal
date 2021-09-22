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
    class ThingConsole : SadConsole.UI.ControlsConsole
    {
        public ThingConsole(int width, int height) : base(width, height)
        {


            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            ColoredString.CustomProcessor = Utility.CustomParseCommand;
            Cursor.UseStringParser = true;
            actionWindow = new ActionWindow(30, 5, new Point(0, Cursor.Position.Y));
            Children.Add(actionWindow);
            actionWindow.IsVisible = false;
            SadComponents.Add(new AnimatedBorderComponent());


        }
        ActionWindow actionWindow;
        public List<string> lines = new List<string>();
        public void DrawThing()
        {
            needRedraw = false;
            this.Clear();
            this.Effects.RemoveAll();
            actionWindow.Clear();
            actionWindow.Controls.Clear();
            actionWindow.IsVisible = false;
            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine().NewLine().NewLine();
            foreach (string fancy in new List<string>(lines))
            {
                if (fancy.Contains("<"))
                {
                    string[] words = fancy.Split(" ");
                    bool combining = false;
                    string combined = "";

                    foreach (string word in words)
                    {
                        if (word.Length < 1) { continue; }

                        if (word[0] == '<')
                        {
                            combining = true;
                            combined = "";
                        }
                        if (word.Contains('>'))
                        {
                            combining = false;
                            string thingid = word.Substring(0, word.IndexOf(">"));
                            string unreleated = word.Substring(word.IndexOf(">") + 1, word.Length - word.IndexOf(">") - 1);

                            combined += " " + thingid;
                            combined = combined.Trim();
                            thingid = combined.Replace("<", "").Replace(">", "");

                            Utility.CreateButtonThingId(Utility.SplitThingID(thingid), this, actionWindow,true);
                            Cursor.Print(unreleated + " ");


                        }
                        else if (combining)
                        {
                            combined += " " + word;

                        }
                        else
                        {
                            if (Cursor.Position.X + word.Length > Width)
                            {
                                Cursor.NewLine();
                            }
                            Cursor.Print(word + " ");

                        }

                    }
                    
                }
                else {

                    Cursor.Print(fancy);

                }
                Cursor.NewLine();

            }


        }





        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                DrawThing();



            }

        }



        public bool needRedraw = false;






        


    }

}
