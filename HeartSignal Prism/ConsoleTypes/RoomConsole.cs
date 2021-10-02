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
    class RoomConsole : BaseConsole
    {

        ///TODO: room console and thing console could be children to a singl console due to a lot of similarities
        public RoomConsole(int width, int height) : base(width, height, true, true)
        {

            name = "";


        }
        public string name { get; private set; }
        public List<string> roomInfo = new List<string>();
        public List<string> thingInfo = new List<string>();
        public List<string> fancyInfo = new List<string>();
        public List<string> bodyInfo = new List<string>();


        //setting name aka changing rooms wipes everything
        public void SetName(string n)
        {


            name = n;
            roomInfo = new List<string>();
            thingInfo = new List<string>();
            fancyInfo = new List<string>();
            bodyInfo = new List<string>();

        }




        protected override void DrawConsole()
        {

            Cursor.NewLine().Print(name).NewLine();
            foreach (string desc in new List<string>(roomInfo))
            {
                Cursor.Print(desc).NewLine();


            }



            foreach (string fancy in new List<string>(fancyInfo))
            {
                string[] words = fancy.Split(" ");
                foreach (string word in words)
                {
                    if (word.Contains("+"))
                    {
                        string text;
                        text = word.Replace("+", "");
                        string tip = text.Substring(text.IndexOf('(') + 1, text.Length - (text.IndexOf('(') + 3));
                        text = text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));

                        var button = new Button(text.Length, 1)
                        {
                            Text = text,
                            Position = Cursor.Position,
                            Theme = new ThingButtonTheme(new Gradient(Color.Green, Color.LimeGreen, Color.Green))
                        };


                        button.MouseEnter += (s, a) => actionWindow.ShowTooltip(tip, Cursor.Position + new Point(0, 0));

                        Controls.Add(button);
                        Cursor.Right(word.Length);
                    }
                    else if (word.Contains("<"))
                    {
                        string text2;
                        text2 = word.Replace("<", "").Replace(">", "");
                        Utility.CreateButtonThingId(Utility.SplitThingID(text2.Replace("_", " ")), this, actionWindow, false, null, true);
                        Cursor.Right(1);

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

            DrawList(new List<string>(thingInfo));

            DrawList(new List<string>(bodyInfo));
           

        }





        private void DrawList(List<string> ls)
        {

  


            if (ls.Count == 0) { return; }
            int index = 0;
            //int indexoffset = 0;
            List<string[]> thingids = new List<string[]>();
            foreach (string thingid in ls) {
                thingids.Add(Utility.SplitThingID(thingid));

            }

            Cursor.Print("There is ");
            foreach (string[] thingid in thingids)
            {
                

                index++;

                Utility.CreateButtonThingId(thingid, this, actionWindow);
               

                if (index >= thingids.Count())
                {


                }
                else if (index == thingids.Count() - 1)
                {

                    Cursor.Print(" and ");
                }

                else
                {
                    Cursor.Print(", ");

                }





            }
            Cursor.NewLine();
        }


      





        


    }

}
