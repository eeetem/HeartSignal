using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using SadRogue.Primitives;
using SadConsole.StringParser;
using SadConsole;
using SadConsole.UI.Controls;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Components;
using SadConsole.Host;
using SadConsole.UI.Themes;
using Point = SadRogue.Primitives.Point;
using Color = SadRogue.Primitives.Color;

namespace HeartSignal
{
	static class Utility
    {

        public static float GlobalAnimationSpeed = 1f;
        private static Dictionary<string, string> globalVars = new Dictionary<string, string>();


        public static void SetVar(string name, string value)
        {
            if (!globalVars.ContainsKey(name))
            {
                globalVars.Add(name,value);
            }

            globalVars[name] = value;
            
            
            
        }

        public static string GetVar(string name)
        {
            if (globalVars.ContainsKey(name))
            {
                
                return globalVars[name];
            }

            return " ";

        }

        public static double GetAngleOfLineBetweenTwoPoints(SadRogue.Primitives.Point p1, SadRogue.Primitives.Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }


        public static string[] SplitThingId(string thingid) {

            if (thingid.IndexOf("(") <= 0)
            {
        //        Program.MainConsole.ReciveExternalInput("ERROR: attempted to get id from an item without id: "+thingid);
                return new string[]{thingid,"none"};
            }

            string idstring = thingid.Substring(thingid.IndexOf("("), thingid.IndexOf(")") - thingid.IndexOf("("));
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
                case "timed":
                    return new ParseCommandTimedGradient(parameters, commandStacks);
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

        public static int GetTaglessLenght(string name)
        {
            ColoredString parsedName = ColoredString.Parser.Parse(name);//just to get the lenght
            return parsedName.ToString().Length;
        }

        public static void CreateToolTip(string name, string tip, SadConsole.UI.ControlsConsole console, ActionWindow ac) {
            string realtext = RemoveParserTags(name);


            if (realtext.Length + console.Cursor.Position.X > console.Width)
            {
                console.Cursor.NewLine().Right(1);
            }
            Point pos = console.Cursor.Position;


         //   pos = new Point(Math.Clamp(pos.X,  0, console.Width), pos.Y);
            
           

            var button = new Button(realtext.Length, 1)
            {
                Text = name,
                Position = console.Cursor.Position,
                Theme = new ThingButtonTheme(new Gradient(Color.Green, Color.LimeGreen, Color.Green))
            };


            button.MouseEnter += (s, a) => ac.ShowTooltip(tip, console.Surface,pos);

            console.Controls.Add(button);
            console.Cursor.Right(realtext.Length + 1);


        }

        public static void CreateButton(string title, string output, string theme, SadConsole.UI.ControlsConsole console)
        {
            
            if (title.Length +2 + console.Cursor.Position.X > console.Width)
            {
                console.Cursor.NewLine().Right(1);
            }

            

           bool offset3d = false;
            ButtonTheme TbuttonTheme;
            switch (theme)
            {
                case "3d":
                    TbuttonTheme = new Button3dTheme();
                    offset3d = true;
                    break;
                case "line":
                    TbuttonTheme = new ButtonLinesTheme();
                    break;
                case "bracket":
                    TbuttonTheme = new ButtonTheme();
                    title = " " + title + " ";
                    break;
                default:
                    TbuttonTheme = new ThingButtonTheme(new Gradient(Color.Blue, Color.Cyan, Color.Blue));
                    break;

            }

            if (offset3d)
            {
                console.Cursor.Position = console.Cursor.Position.WithY(console.Cursor.Position.Y - 1);
            }


            var button = new Button(Utility.RemoveParserTags(title).Length, 1)
            {
                Text = title,
                Position = console.Cursor.Position,
                Theme = TbuttonTheme
            };
            if (offset3d)
            {
                console.Cursor.Position = console.Cursor.Position.WithY(console.Cursor.Position.Y +1);
            }


            button.MouseButtonClicked += (s, a) => NetworkManager.SendNetworkMessage(output);

            console.Controls.Add(button);
            console.Cursor.Right(title.Length + 1);
        }

        public static Button CreateButtonThingId(string[] thingid, SadConsole.UI.ControlsConsole console, ActionWindow ac,bool explicitlook = false, Point? offset = null,bool clampactionwindow = false) {

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
                    if (!ThingDatabase.thingDatabase.ContainsKey(id)){

                        ThingDatabase.GetData(id);

                    }

                }


            } else if(!ThingDatabase.thingDatabase.ContainsKey(thingid[1]))
            {

                ThingDatabase.GetData(thingid[1]);
            }


            string realtext = RemoveParserTags(thingid[0]);




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
                    Theme = new ThingButtonTheme(null)
                };


                button.MouseEnter += (s, a) => ac.DisplayActions(thingid[0] + "(" + thingid[1] + ")", console.Surface,pos, explicitlook);
                button.MouseMove += (s, a) => (button.Theme as ThingButtonTheme).AdjustColor();
                button.MouseExit += (s, a) => (button.Theme as ThingButtonTheme).DefaultColor();
                button.MouseButtonClicked += (s, a) => button.InvokeClick();
                button.Click += (s, a) => ac.ClickItem(thingid[1]);
                console.Controls.Add(button);
                console.Cursor.Right(realtext.Length);
                return button;
            }
            else
            {

                var button = new Button(realtext.Length, 1)
                {
                    Text = thingid[0],
                    Position = console.Cursor.Position,
                    Theme = new ThingButtonTheme()
                };

                button.MouseEnter += (s, a) => ac.DisplayMultiItem(thingid[0],  console.Surface, pos, sameThingsIDs);

                // button.Click += (s, a) => actionWindow.SetFocus(thing.Key);
                console.Controls.Add(button);
                console.Cursor.Right(realtext.Length);

                return button;

            }

        }

        public static Texture2D GetImageOrDownload(string file)
        {
            if (File.Exists("img/" + file))
            {
                return Texture2D.FromFile(Global.GraphicsDevice, "img/" + file);
            }

            string dir = "";
            string filename ="";
            if (file.Contains("/"))
            {
                dir = Directory.GetCurrentDirectory() + "/img/" + file.Remove(file.LastIndexOf("/"), file.Length - file.LastIndexOf("/"));
                filename = file.Substring(file.LastIndexOf("/")+1);
            }
            else {
                //base sfx folder is used for temp files
                Program.MainConsole.ReciveExternalInput("warning inaproporiate download path for a image file was specified: " + file);
                return null;

            }

            Directory.CreateDirectory(dir);
            using (var client = new WebClient())
            {

                client.DownloadFile(new Uri("http://deathcult.today/snufftv/" + file), "img/" + filename);
            }

            File.Move("img" + file.Substring(file.LastIndexOf("/")), "img/"+ file);//move the downloaded file from temp location to proper one


            return Texture2D.FromFile(Global.GraphicsDevice, "img/" + file);
        }
        public static void PrintParseMessage(string message,ActionWindow ac,SadConsole.UI.ControlsConsole  con, bool explicitLook = false,int buffer = 0)
        {
            message = message.Replace("[nl]", " \r\n");
            string[] words = message.Split(" ");
            try
            {
                foreach (string word in words)
                {

                    string spaced = word.Replace("¦", " ",StringComparison.InvariantCultureIgnoreCase);
                    
                    if (spaced.Contains("{"))
                    {
                        string varbl = spaced.Substring(spaced.IndexOf("{")+1, spaced.IndexOf("}")-spaced.IndexOf("{")-1);
                        var varble = new Label(RemoveParserTags(Utility.GetVar(varbl)).Length)
                        {
                            Position = con.Cursor.Position,
                            Theme = new VarblPrinterTheme(varbl)
                        };
                        con.Controls.Add(varble);
                        con.Cursor.Right(RemoveParserTags(Utility.GetVar(varbl)).Length + 1);
                        spaced = spaced.Remove(spaced.IndexOf("{"), spaced.IndexOf("}") - spaced.IndexOf("{")+1);

                    }
                    if (spaced.Contains("!+!"))
                    {
                        string text;
                        text = spaced.Replace("!+!", "").Replace("_", " ");
                        string tip = text.Substring(text.IndexOf('(') + 1, text.Length - (text.IndexOf('(') + 2));
                        text = text.Remove(text.IndexOf('('), text.Length - text.IndexOf('('));
                        tip = tip.Replace(")", "");

                        Utility.CreateToolTip(text, tip, con, ac);
                    }
                    //"!/!click_me(output,theme)!/!"
                   else if (spaced.Contains("!/!"))
                    {
                        string title = spaced.Substring(spaced.IndexOf("!/!")+3, spaced.IndexOf('(') - (spaced.IndexOf("!/!")+3));
                        string output = spaced.Substring(spaced.IndexOf("(")+1, spaced.IndexOf(',') - (spaced.IndexOf("(")+1));
                        string theme = spaced.Substring(spaced.IndexOf(",") + 1,   spaced.IndexOf(')') - (spaced.IndexOf(",")+1));
                        Utility.CreateButton(title, output, theme, con);
                    }
                    else if (spaced.Contains("<"))
                    {
                    
                        string leftover = "";
                        if (spaced.IndexOf("<") > 0)
                        {
                            string beginingbit = spaced.Substring(0, spaced.IndexOf("<"));
                            con.Cursor.Print(beginingbit);
                            spaced = spaced.Remove(0, spaced.IndexOf("<"));
                        }

                        if (spaced.Length > spaced.IndexOf('>'))
                        {
                            leftover = spaced.Substring(spaced.IndexOf('>') + 1, spaced.Length - (spaced.IndexOf('>') + 1));
                        }

                        spaced = spaced.Remove(spaced.IndexOf('>'), spaced.Length - spaced.IndexOf('>'));
                        spaced = spaced.Replace("<", "").Replace(">", "");
                        Utility.CreateButtonThingId(Utility.SplitThingId(spaced), con, ac,
                            explicitLook,
                            null, true);
                        con.Cursor.Print(leftover).Right(1);
                    }

                    else
                    {
                        if (con.Cursor.Position.X + spaced.Length+buffer > con.Width && !spaced.Contains("["))
                        {
                            con.Cursor.NewLine().Right(buffer);
                        }

                        con.Cursor.Print(spaced).Right(1);
                    }
                }

                con.Cursor.NewLine().Right(buffer);
            }
            catch(Exception e)
            {
                File.AppendAllText("debuglog.txt",
                    "Error print parsing: " + message + e);
                con.Cursor.Print("Error print parsing: " + message);
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
