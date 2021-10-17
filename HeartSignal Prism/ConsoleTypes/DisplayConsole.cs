using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using System.Collections.Generic;

namespace HeartSignal
{
    internal class DisplayConsole : BaseConsole
    {

        public DisplayConsole(int width, int height) : base(width, height,true,true)
        {

            // Disable the cursor since our keyboard handler will do the work.
         
        }
        public bool ExplicitLook = false;
        public List<string> lines = new List<string>();
        protected override void DrawConsole()
        {


            foreach (string fancy in new List<string>(lines))
            {
                string[] words = fancy.Split(" ");
                foreach (string word in words)
                {
                    if (word.Contains("!+!"))
                    {
                        string text;
                        text = word.Replace("!+!", "").Replace("_", " ");
                        string tip = text.Substring(text.IndexOf('(') + 1, text.Length - (text.IndexOf('(') + 2));
                        text = text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));


                        Utility.CreateToolTip(text, tip, this, actionWindow);
                    }
                    else if (word.Contains("<"))
                    {
                        string text2 = word;
                        string leftover = "";
                        if (text2.Length > text2.IndexOf('>'))
                        {
                            leftover = text2.Substring(text2.IndexOf('>') + 1, text2.Length - (text2.IndexOf('>') + 1));
                        }
                        text2 = text2.Remove(text2.IndexOf('>'), text2.Length - text2.IndexOf('>'));
                        text2 = text2.Replace("<", "").Replace(">", "");
                        Utility.CreateButtonThingId(Utility.SplitThingID(text2.Replace("_", " ")), this, actionWindow, ExplicitLook, null, true);
                        Cursor.Print(leftover).Right(1);

                    }
                    else
                    {

                        if (Cursor.Position.X + word.Length > Width && !word.Contains("["))
                        {
                            Cursor.NewLine();
                        }
                        Cursor.Print(word.Replace("_", " ") + " ");

                    }





                }
                Cursor.NewLine();
            }


        }

    }
}
