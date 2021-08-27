﻿using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;

namespace HeartSignal
{
    class RoomConsole : Console
    {
        
        public RoomConsole(int width, int height) : base(width, height)
        {
            

            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;

        }
        public string name { get; private set; }
        public List<string> roomInfo = new List<string>();
        public List<string> thingInfo = new List<string>();
        public List<string> bodyInfo = new List<string>();


        //setting name aka changing rooms wipes everything
        public void SetName(string n) {


            name = n;
        roomInfo = new List<string>();
        thingInfo = new List<string>();
        bodyInfo = new List<string>();

    }


        

        //a bunch of repeating code, might be worth moving into "drawlist" fucntion
        public void DrawRoom() {

            this.Clear();
            Cursor.NewLine().NewLine().NewLine();
            Cursor.Print(name).NewLine();
            foreach (string desc in roomInfo)
            {
                Cursor.Print(desc);


            }
            Cursor.NewLine();
            DrawList(thingInfo);
            DrawList(bodyInfo);
        }





        private void DrawList(List<string> ls) {


            int index = 0;
     

         
                Cursor.Print("There is ");
                foreach (string thing in ls)
                {
                    index++;
                    Cursor.Print(thing);
                    if (index >= ls.Count)
                    {


                    }
                    else if (index == ls.Count - 1)
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
