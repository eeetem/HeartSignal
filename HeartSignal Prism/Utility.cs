using System;
using System.Collections.Generic;
using SadRogue.Primitives;
using SadConsole.StringParser;
using SadConsole;
using SadConsole.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Console = SadConsole.Console;
using Point = SadRogue.Primitives.Point;
using Color = SadRogue.Primitives.Color;

namespace HeartSignal
{
	static class Utility
    {

        public static float GlobalAnimationSpeed = 1f;

        public static double GetAngleOfLineBetweenTwoPoints(SadRogue.Primitives.Point p1, SadRogue.Primitives.Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }


        public static string[] SplitThingId(string thingid) {

            if (thingid.IndexOf("(") <= 0)
            {
                Program.MainConsole.ReciveExternalInput("ERROR: attempted to get id from an item without id: "+thingid);
                return new string[]{thingid,"0"};
            }

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


         //   pos = new Point(Math.Clamp(pos.X,  0, console.Width), pos.Y);
            

            
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
            NetworkManager.SendNetworkMessage("ex " + id);


        }
        public static void PrintParseMessage(string message,ActionWindow ac,SadConsole.UI.ControlsConsole  con, bool explicitLook)
        {
            string[] words = message.Split(" ");
            try
            {
                foreach (string word in words)
                {
                    if (word.Contains("!+!"))
                    {
                        string text;
                        text = word.Replace("!+!", "").Replace("_", " ");
                        string tip = text.Substring(text.IndexOf('(') + 1, text.Length - (text.IndexOf('(') + 2));
                        text = text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));
                        tip = tip.Replace(")", "");

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
                        Utility.CreateButtonThingId(Utility.SplitThingId(text2.Replace("_", " ")), con, ac,
                            explicitLook,
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
            catch(Exception)
            {
                con.Cursor.Print("Error print parsing: " + message +
                                 " report this and make sure it does not have any odd symbols");
            }
        }

        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            by = Math.Clamp(by, 0, 1);
            return firstFloat * (1 - by) + secondFloat * by;
        }
         public static Texture2D[] SplitTexture(Texture2D original, int partWidth, int partHeight, out int xCount, out int yCount)
    {
        xCount = original.Width / partWidth + (original.Width % partWidth == 0 ? 0 : 1);//The number of textures in each horizontal row
        yCount = original.Height / partHeight + (original.Height % partHeight == 0 ? 0 : 1);//The number of textures in each vertical column
        Texture2D[] r = new Texture2D[xCount * yCount];//Number of parts = (area of original) / (area of each part).
        int dataPerPart = partWidth * partHeight;//Number of pixels in each of the split parts

        //Get the pixel data from the original texture:
        Color[] originalData = new Color[original.Width * original.Height];
        original.GetData<Color>(originalData);

        int index = 0;
        for (int y = 0; y < yCount * partHeight; y += partHeight)
            for (int x = 0; x < xCount * partWidth; x += partWidth)
            {
                //The texture at coordinate {x, y} from the top-left of the original texture
                Texture2D part = new Texture2D(original.GraphicsDevice, partWidth, partHeight);
                //The data for part
                Color[] partData = new Color[dataPerPart];

                //Fill the part data with colors from the original texture
                for (int py = 0; py < partHeight; py++)
                    for (int px = 0; px < partWidth; px++)
                    {
                        int partIndex = px + py * partWidth;
                        //If a part goes outside of the source texture, then fill the overlapping part with Color.Transparent
                        if (y + py >= original.Height || x + px >= original.Width)
                            partData[partIndex] = Color.Transparent;
                        else
                            partData[partIndex] = originalData[(x + px) + (y + py) * original.Width];
                    }

                //Fill the part with the extracted data
                part.SetData<Color>(partData);
                //Stick the part in the return array:                    
                r[index++] = part;
            }
        //Return the array of parts.
        return r;
    }
    }
}
