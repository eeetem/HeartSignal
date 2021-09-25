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
    class RoomConsole : SadConsole.UI.ControlsConsole
    {
        public RoomConsole(int width, int height) : base(width, height)
        {

            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
 
            actionWindow = new ActionWindow(30, 5, new Point(0, Cursor.Position.Y));
            Children.Add(actionWindow);
            actionWindow.IsVisible = false;
            ColoredString.CustomProcessor = Utility.CustomParseCommand;
            SadComponents.Add(new AnimatedBorderComponent());


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




        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawRoom()
        {
           needRedraw = false;
            this.Clear();

            actionWindow.IsVisible = false;
            actionWindow.Clear();
            actionWindow.Controls.Clear();
            
            Controls.Clear();
            Cursor.Position = new Point(0, 0);
            Cursor.NewLine().Print(name).NewLine();
            foreach (string desc in new List<string>(roomInfo))
            {
                Cursor.Print(desc).NewLine();


            }



            foreach (string fancy in new List<string>(fancyInfo)) {

                string[] words = fancy.Split(" ");
                bool combining = false;
                string combined = "";
                foreach (string word in words) {


                    if (word[0] == '<')
                    {
                        combining = true;
                        combined = "";
                        combined += word;
                    }
                    else if(word[word.Length-1] == '>'|| word[word.Length - 2] == '>')//possibly just change it to "contains()"
                    {
                        combining = false;
                        string thingid = word.Substring(0, word.IndexOf(">"));
                        string unreleated = word.Substring(word.IndexOf(">")+1, word.Length - word.IndexOf(">")-1);
                        
                        combined += " " + thingid;
                        thingid = combined.Replace("<", "").Replace(">", "");

                        Utility.CreateButtonThingId(Utility.SplitThingID(thingid), this, actionWindow);
                        Cursor.Print(unreleated + " ");
        

                    }
                    else if (combining) {
                        combined += " " + word;
                    
                    }
                    else {
                        if (Cursor.Position.X + word.Length > Width) {
                            Cursor.NewLine();
                        }
                        Cursor.Print(word + " ");

                    }

                }
                Cursor.NewLine();
            
            }

            DrawList(new List<string>(thingInfo));

            DrawList(new List<string>(bodyInfo));
            Cursor.NewLine(); Cursor.NewLine(); Cursor.NewLine(); Cursor.NewLine(); Cursor.NewLine(); Cursor.NewLine(); Cursor.NewLine();
            this.IsFocused = true; 
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
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                DrawRoom();



            }

        }

        ActionWindow actionWindow;

        public bool needRedraw = false;






        


    }

}
