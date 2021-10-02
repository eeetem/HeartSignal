using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.Effects;
using SadConsole.StringParser;
using System.Collections;

namespace HeartSignal
{
	class BarConsole : BaseConsole
	{
		public BarConsole(int width, int height) : base(width, height)
		{




		}


        Dictionary<string,List<Affliction>> Bars = new Dictionary<string,List<Affliction>>();
		public void AddBar(string barname, List<string> affs) {

            List<Affliction> afflictions = new List<Affliction>();
            foreach (string arg in affs) {
                string[] args = arg.Split(":");
                if (args.Length != 3) {
                    Program.MainConsole.DrawMessage("incorrect arguments were passed into the bar console - please inform the developers");
                    continue;

                }
                bool keep = false;
                Color c = Color.White.FromParser(args[0], out keep, out keep, out keep, out keep, out keep);
                afflictions.Add(new Affliction(c, args[1], Int32.Parse(args[2])));

            }
            Bars[barname] = afflictions;
            ReDraw();
        }
 
        protected override void  DrawConsole() {
            if (Bars.Count == 0) return;
           int glyphsPerBar = Width / Bars.Count;

            int index = 0;

            foreach (KeyValuePair<string, List<Affliction>> Bar in Bars) {
                ShapeParameters param = ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick,new ColoredGlyph(Color.Green, Color.Green), new ColoredGlyph(Color.Green, Color.Green));
                Surface.DrawBox(new Rectangle((glyphsPerBar + 1) * index+1, 1, glyphsPerBar -3, 2), param);
                Surface.Print((glyphsPerBar + 1) * index + 1, 0, Bar.Key);
                int percentage = 0;
                foreach (Affliction a in Bar.Value) {

                    param = ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick, new ColoredGlyph(a.color, a.color), new ColoredGlyph(a.color, a.color));
                    Surface.DrawBox(new Rectangle(((glyphsPerBar + 1) * index + 1)+ (glyphsPerBar - 3) / 100 * percentage, 1, (glyphsPerBar - 3)/100*a.percentage, 2), param);

                    if (a.name.Length < (glyphsPerBar - 3) / 100 * a.percentage)
                    {
                        Surface.Print(((glyphsPerBar + 1) * index + 1) + (glyphsPerBar - 3) / 100 * percentage, 2, new ColoredString(a.name, a.color, Color.Black));

                    }


                    percentage += a.percentage;

                }
                index++;
            }
        }


	}

    public struct Affliction
    {
        public Affliction(Color c,string n, int p)
        {

            color = c;
            name = n;
            percentage = p;
    
            

        }
        public Color color;
        public string name;
        public int percentage;
  
    }
}
