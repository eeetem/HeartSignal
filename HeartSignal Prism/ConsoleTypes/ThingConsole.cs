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
    class ThingConsole : BaseConsole
    {
        public ThingConsole(int width, int height) : base(width, height)
        {




        }

        public List<string> lines = new List<string>();
        protected override void DrawConsole()
        {

         
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

                            Utility.CreateButtonThingId(Utility.SplitThingID(thingid), this, actionWindow,true,null,true);
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




        


    }

}
