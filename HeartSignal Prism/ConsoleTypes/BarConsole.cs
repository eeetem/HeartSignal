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
	class BarConsole : Console
	{
		public BarConsole(int width, int height) : base(width, height)
		{


            SadComponents.Add(new AnimatedBorderComponent());

        }


      Dictionary<string,List<Affliction>> Bars = new Dictionary<string,List<Affliction>>();
		public void AddBar(string barname, List<string> affs) {

            List<Affliction> afflictions = new List<Affliction>();
            if (affs[0] == "delete") {

                Bars.Remove(barname);
                return;
            }
            foreach (string arg in affs) {
                string[] args = arg.Split(":");
                if (args.Length != 3) {
                    Program.MainConsole.DrawMessage("incorrect arguments were passed into the bar console - please inform the developers");
                    Program.MainConsole.DrawMessage(arg);
                    continue;

                }
                bool keep = false;
                string[] colors = args[0].Split(".");
                Color[] Colors = new Color[colors.Length];
                int index=0;
                foreach (string color in colors) {

                    Colors[index] = Color.White.FromParser(color, out keep, out keep, out keep, out keep, out keep);
                    index++;
                }
                
                afflictions.Add(new Affliction(new Gradient(Colors), args[1], Int32.Parse(args[2])));

            }
            Bars[barname] = afflictions;

        }
        float counter = 0;
	    public override void Update(TimeSpan delta)
		{
			base.Update(delta);
            DrawConsole(delta);
            
		}
		protected void  DrawConsole(TimeSpan delta) {
            if (Bars.Count == 0) return;

            counter += (float)delta.TotalSeconds/4 * AnimatedBorderComponent.speed;
            if (counter > 1) {
                counter = 0;
            
            }
           int glyphsPerBar = Width / Bars.Count;

            int index = 0;

            foreach (KeyValuePair<string, List<Affliction>> Bar in Bars) {
                ShapeParameters param = ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick,new ColoredGlyph(Color.Green, Color.Green), new ColoredGlyph(Color.Green, Color.Green));
                Surface.DrawBox(new Rectangle((glyphsPerBar + 1) * index+1, 1, glyphsPerBar -3, 2), param);
                Surface.Print((glyphsPerBar + 1) * index + 1, 0, Bar.Key);
                int percentage = 0;
                foreach (Affliction a in Bar.Value) {

                    param = ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThick, new ColoredGlyph(a.color.Lerp(counter), a.color.Lerp(counter)), new ColoredGlyph(a.color.Lerp(counter), a.color.Lerp(counter)));
                    Surface.DrawBox(new Rectangle(((glyphsPerBar + 1) * index + 1)+ (glyphsPerBar - 3) / 100 * percentage, 1, (glyphsPerBar - 3)/100*a.percentage, 2), param);
                    

                    if (a.name.Length < (glyphsPerBar - 3) / 100 * a.percentage)
                    {
                        Surface.Print(((glyphsPerBar + 1) * index + 1) + (glyphsPerBar - 3) / 100 * percentage, 2, new ColoredString(a.name, a.color.Lerp(counter), Color.Black));

                    }


                    percentage += a.percentage;

                }
                index++;
            }
        }


	}

    public struct Affliction
    {
        public Affliction(Gradient c,string n, int p)
        {

            color = c;
            name = n;
            percentage = p;
    
            

        }
        public Gradient color;
        public string name;
        public int percentage;
  
    }
}
