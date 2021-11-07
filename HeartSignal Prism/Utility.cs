using System;
using System.Collections.Generic;
using SadRogue.Primitives;
using SadConsole.StringParser;
using SadConsole;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;

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


        public static string[] SplitThingId(string thingid) {


            
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
                case "glyph":
                    return new ParseCommandAnimGlyph(parameters, commandStacks);
                default:
                    return null; 
            }
        }
        public static string RemoveParserTags(string text) {

            while (text.Contains("]"))
            {
                text = text.Remove(text.IndexOf("["), text.IndexOf("]") - text.IndexOf("[") + 1);


            }
            return text.Replace(";"," ");


        }

        public static void CreateToolTip(string name, string tip, SadConsole.UI.ControlsConsole console, ActionWindow ac) {
            name = name.Replace("_", " ");
            string realtext = RemoveParserTags(name);

            bool forceAppearance = realtext == name;


            if (realtext.Length + console.Cursor.Position.X > console.Width)
            {
                console.Cursor.NewLine().Right(1);
            }
            Point pos = console.Cursor.Position;


            pos = new Point(Math.Clamp(pos.X,  0, console.Width), pos.Y);
            

            
            var button = new Button(name.Length, 1)
            {
                Text = name,
                Position = console.Cursor.Position,
                Theme = new ThingButtonTheme(new Gradient(Color.Green, Color.LimeGreen, Color.Green), forceAppearance)
            };


            button.MouseEnter += (s, a) => ac.ShowTooltip(tip, console.Surface,pos);

            console.Controls.Add(button);
            console.Cursor.Right(name.Length + 1);


        }
        public static void CreateButtonThingId(string[] thingid, SadConsole.UI.ControlsConsole console, ActionWindow ac,bool explicitlook = false, Point? offset = null,bool clampactionwindow = false) {

            //if there is other things with same name process them at the same time
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
            Point _offset = new Point(-5, 1);//default offset
            if (offset != null) {
                _offset = (Point)offset;


            }
            


            if (multiple)
            {
                foreach (string id in sameThingsIDs)
                {
                    if (!ActionWindow.actionDatabase.ContainsKey(id)){

                        InitThingId(id);

                    }

                }


            } else if(!ActionWindow.actionDatabase.ContainsKey(thingid[1]))
            {

                InitThingId(thingid[1]);
            }


            string realtext = RemoveParserTags(thingid[0]);

            bool forceApearance = realtext == thingid[0];


            if (realtext.Length + console.Cursor.Position.X > console.Width)
            {
                console.Cursor.NewLine().Right(1);
            }

            Point pos = console.Cursor.Position;
            if (clampactionwindow)
            {
                pos = new Point(Math.Clamp(pos.X + _offset.X, 0, console.Width), pos.Y + _offset.Y);
            }
            else
            {
                pos = new Point(pos.X + _offset.X, pos.Y + _offset.Y);


            }
            if (!multiple)
            {
  
                var button = new Button(realtext.Length, 1)
                {
                    Text = thingid[0],
                    Position = console.Cursor.Position,
                    Theme = new ThingButtonTheme(null, forceApearance)
                };


                button.MouseEnter += (s, a) => ac.DisplayActions(thingid[0] + "(" + thingid[1] + ")", console.Surface,pos, explicitlook);
                button.MouseMove += (s, a) => (button.Theme as ThingButtonTheme).AdjustColor();
                button.MouseExit += (s, a) => (button.Theme as ThingButtonTheme).DefaultColor();
                button.MouseButtonClicked += (s, a) => ac.ClickItem(thingid[1],a);
                console.Controls.Add(button);
                console.Cursor.Right(realtext.Length);
            }
            else
            {

                var button = new Button(thingid[0].Length, 1)
                {
                    Text = thingid[0],
                    Position = console.Cursor.Position,
                    Theme = new ThingButtonTheme()
                };

                button.MouseEnter += (s, a) => ac.DisplayMultiItem(thingid[0],  console.Surface, pos, sameThingsIDs);

                // button.Click += (s, a) => actionWindow.SetFocus(thing.Key);
                console.Controls.Add(button);
                console.Cursor.Right(thingid[0].Length);



            }

        }
        public static void InitThingId(string id)
        {

            //this being outside the IF causes ex spam but currently it's needed for descriptions, definatelly possible to optimise this if needed
            Program.SendNetworkMessage("ex " + id);


        }
        public static void PrintParseMessage(string message,ActionWindow ac,SadConsole.UI.ControlsConsole  con, bool explicitLook)
        {
            string[] words = message.Split(" ");
            foreach (string word in words)
            {
                if (word.Contains("!+!"))
                {
                    string text;
                    text = word.Replace("!+!", "").Replace("_", " ");
                    string tip = text.Substring(text.IndexOf('(') + 1, text.Length - (text.IndexOf('(') + 2));
                    text = text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));


                    Utility.CreateToolTip(text, tip, con, ac);
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
                    Utility.CreateButtonThingId(Utility.SplitThingId(text2.Replace("_", " ")), con, ac, explicitLook,
                        null, true);
                    con.Cursor.Print(leftover).Right(1);
                }
                else
                {
                    if (con.Cursor.Position.X + word.Length > con.Width && !word.Contains("["))
                    {
                        con.Cursor.NewLine();
                    }

                    con.Cursor.Print(word.Replace("_", " ").Replace(";", " ") + " ");
                }
            }

            con.Cursor.NewLine();
        }
    }
}
