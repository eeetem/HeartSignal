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
    class ThingConsole : BaseConsole
    {
        public ThingConsole(int width, int height) : base(width, height)
        {


            

            
        }

        public List<string> lines = new List<string>();
        protected override void DrawConsole()
        {


            foreach (string fancy in new List<string>(lines))
            {
                string[] words = fancy.Split(" ");
                foreach (string word in words) {
                    if (word.Contains("+"))
                    {
                        string text;
                        text = word.Replace("+", "");
                        string tip = text.Substring(text.IndexOf('(')+1, text.Length - (text.IndexOf('(')+3));
                        text =  text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));

                        var button = new Button(text.Length, 1)
                        {
                            Text = text,
                            Position = Cursor.Position,
                            Theme = new ThingButtonTheme(new Gradient(Color.Green, Color.LimeGreen,Color.Green))
                        };


                        button.MouseEnter += (s, a) => actionWindow.ShowTooltip(tip,Cursor.Position + new Point(0,0));

                        Controls.Add(button);
                        Cursor.Right(word.Length);
                    }
                    else if (word.Contains("<"))
                    {
                        string text2;
                        text2 = word.Replace("<", "").Replace(">", "");
                        Utility.CreateButtonThingId(Utility.SplitThingID(text2.Replace("_", " ")),this,actionWindow,true,null,true);
                        Cursor.Right(1);

                    }
                    else {

                        if (Cursor.Position.X + word.Length > Width && !word.Contains("["))
                        {
                            Cursor.NewLine();
                        }
                        Cursor.Print(word.Replace("_"," ") + " ");

                    }
                
                
                
                
                
                }
                Cursor.NewLine();
            }


        }




        


    }

}
