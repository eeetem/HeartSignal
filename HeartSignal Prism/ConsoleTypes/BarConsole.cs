using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;

namespace HeartSignal
{
	class BarConsole : Console
	{
		public BarConsole(int width, int height) : base(width, height)
		{


            SadComponents.Add(new AnimatedBorderComponent());

        }



        private Dictionary<string, Bar> bars = new Dictionary<string, Bar>();

        public void AddBar(string barName, List<string> affs)
        {
            
            float capacity = float.Parse(barName.Substring(barName.IndexOf("[")+1, barName.IndexOf("]") - barName.IndexOf("[")-1));
            barName = barName.Remove(barName.IndexOf("["), barName.IndexOf("]") - barName.IndexOf("[")+1);
            bars[barName] = new Bar(barName, capacity);

            if (affs.Count > 0 && affs[0] == "delete") {

                bars.Remove(barName);
                return;
            }
            foreach (string arg in affs) {
                string[] args = arg.Split(":");
                if (args.Length != 3) {
                    Program.MainConsole.ReciveExternalInput("incorrect arguments were passed into the bar console - please inform the developers");
                    Program.MainConsole.ReciveExternalInput(arg);
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
                
                bars[barName].affs.Add(new Affliction(new Gradient(Colors), args[1], Int32.Parse(args[2])));

            }
      

        }
        float counter = 0;
	    public override void Update(TimeSpan delta)
		{
			base.Update(delta);
            DrawConsole(delta);
            
		}
		protected void  DrawConsole(TimeSpan delta) {
            if (bars.Count == 0) return;
            this.Clear();
            this.Effects.RemoveAll();

            counter += (float)delta.TotalSeconds/4 * Utility.GlobalAnimationSpeed;
            if (counter > 1) {
                counter = 0;
            
            }
            // ReSharper disable once PossibleLossOfFraction -accuracy not highly important
            float glyphsPerBar = (int)Math.Floor((double) ((Width-4) / bars.Count));
            //all the functions take in a int - but this var is a float, the reason is it being an int fucks with math

            int index = 0;
            bool even = true;
            foreach (KeyValuePair<string, Bar> bar in bars.ToList())
            {
                even = !even;
                ShapeParameters shape = ShapeParameters.CreateBorder(new ColoredGlyph(Color.Green, Color.Black, 176));
                int barStart =  1+(int)((glyphsPerBar+1) * index);
                Surface.DrawBox(new Rectangle(barStart, 1, (int)glyphsPerBar, 2), shape);
                
               
              //  Surface.DrawBox(new Rectangle(glyphsPerBar * index+1, 1, glyphsPerBar -3, 2), shape);
                
                int percentage = 0;
                foreach (Affliction a in bar.Value.affs) {

                    shape = ShapeParameters.CreateStyledBoxFilled(ICellSurface.ConnectedLineThin, new ColoredGlyph(a.color.Lerp(counter), a.color.Lerp(counter)), new ColoredGlyph(a.color.Lerp(counter), a.color.Lerp(counter)));
                    int afflictionStart = (int) (barStart + glyphsPerBar / bar.Value.capacity * percentage);
                    int afflictionLenght = (int) Math.Ceiling(glyphsPerBar / bar.Value.capacity * a.percentage);
                    Surface.DrawBox(new Rectangle(afflictionStart , 1, afflictionLenght, 2), shape);
                    

               

                    if (a.name.Length < afflictionLenght)
                    {
                        Surface.Print(afflictionStart, 2, new ColoredString(a.name, a.color.Lerp(counter), Color.Black));

                    }
                    else
                    {
                        
                        //print below if mousing over
                        
                        
                        if (Game.Instance.Mouse.ScreenPosition.X >= afflictionStart*Font.GlyphWidth &&
                            Game.Instance.Mouse.ScreenPosition.X <= (afflictionStart + afflictionLenght)*Font.GlyphWidth &&
                            Game.Instance.Mouse.ScreenPosition.Y < 4 * Font.GlyphHeight
                            )
                        {
                            Surface.Print(afflictionStart, 3, new ColoredString(a.name, a.color.Lerp(counter), Color.Black));
                        }

                    }
                        
                    percentage += a.percentage;   
                }
                Surface.Print((int) ((glyphsPerBar + 1) * index + 1), 0, bar.Key+" ("+percentage+"/"+bar.Value.capacity+")");


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
    public struct Bar
    {
        public Bar(string n, float c)
        {

            affs = new List<Affliction>();
            name = n;
            capacity = c;
  


        }
        public List<Affliction> affs;
        readonly string name;
        public float capacity;
 

    }
}
