using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using SadConsole.StringParser;
using SadConsole;
using SadConsole.UI.Controls;

namespace HeartSignal
{
	static class Utility
	{


        public static double GetAngleOfLineBetweenTwoPoints(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }


        public static string[] SplitThingID(string thingid) {


            
            string idstring = thingid.Substring(thingid.IndexOf("("), thingid.Length - thingid.IndexOf("("));
            idstring = idstring.Replace("(", "").Replace(")", "").Replace(".", "").Trim();
            string[] ids = idstring.Split(',');
                
           string thing = thingid.Remove(thingid.IndexOf("("), thingid.IndexOf(")") - thingid.IndexOf("(")+1);

            string[] output = new string[ids.Length + 1];
            output[0] = thing;
            ids.CopyTo(output, 1);

            return output;


        }




        public static ParseCommandBase CustomParseCommand(string command, string parameters, ColoredGlyph[] glyphString,
                                                  ICellSurface surface, ParseCommandStacks commandStacks)
        {
            
            switch (command)
            {
                case "gradanim":
                case "ga":
                    return new ParseCommandGradientAnim(parameters, commandStacks);
                default:
                    return null; 
            }
        }

        public static void CreateButtonThingId(string[] thingid, SadConsole.UI.ControlsConsole console, ActionWindow ac) {

            ///if there is other things with same name process them at the same time
            List<string> sameThingsIDs = new List<string>();

            bool multiple = false;
            if (thingid.Length > 2)
            {
                multiple = true;
                bool first = true;
                foreach (string id in thingid)
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    sameThingsIDs.Add(id);


                }
            }


            if (thingid[0].Length + console.Cursor.Position.X > console.Width)
            {
                console.Cursor.NewLine().Right(1);
            }

            Point pos = console.Cursor.Position;
            if (!multiple)
            {

                var button = new Button(thingid[0].Length, 1)
                {
                    Text = thingid[0],
                    Position = console.Cursor.Position,
                    Theme = new ThingButtonTheme()
                };
                pos = new Point(Math.Clamp(pos.X - 5, 0, console.Width - ac.Width),pos.Y+1);
                
                button.MouseEnter += (s, a) => ac.DisplayActions(thingid[0] + "(" + thingid[1] + ")", pos);
                button.Click += (s, a) => ac.ClickItem(thingid[1]);
                console.Controls.Add(button);
                console.Cursor.Right(thingid[0].Length);
            }
            else
            {

                var button = new Button(thingid[0].Length, 1)
                {
                    Text = thingid[0],
                    Position = console.Cursor.Position,
                    Theme = new ThingButtonTheme()
                };
                pos = new Point(Math.Clamp(pos.X - 5, 0, console.Width - ac.Width), pos.Y +1);
                button.MouseEnter += (s, a) => ac.DisplayMultiItem(thingid[0], pos, sameThingsIDs);
                // button.Click += (s, a) => actionWindow.SetFocus(thing.Key);
                console.Controls.Add(button);
                console.Cursor.Right(thingid[0].Length);



            }

        }
    }
}
