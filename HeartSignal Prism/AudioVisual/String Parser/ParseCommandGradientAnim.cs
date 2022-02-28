using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.StringParser;
using System.Globalization;
namespace HeartSignal
{
	class ParseCommandGradientAnim : ParseCommandBase
    {

        private int foregroundLenght;
        private int? backroundLenght;


        public ColoredString GradientString;

        Gradient foregrad;
        Gradient backgrad;
        public ParseCommandGradientAnim(string parameters, ParseCommandStacks commandStack)
        {

            var badCommandException = new ArgumentException("command is invalid for Recolor: " + parameters);

            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length > 3)
            {
                CommandType = CommandTypes.Effect;
                int index = 0;
                int? backroundindex = null;
                foreach (string param in parametersArray) {
                    if (param == "b") {

                        backroundindex = index;
                        break;



                    }

                    index++;
                }

                if (backroundindex != null)
                {
                    backroundLenght = int.Parse(parametersArray[parametersArray.Length - 1], CultureInfo.InvariantCulture);
                    foregroundLenght = int.Parse(parametersArray[(int)backroundindex - 1], CultureInfo.InvariantCulture);
                }
                else {
                    foregroundLenght = int.Parse(parametersArray[parametersArray.Length - 1], CultureInfo.InvariantCulture);
                    backroundLenght = null;
                }

                List<Color> steps = new List<Color>();


                if (backroundindex != null)
                {
                    for (int i = 1; i < backroundindex - 1; i++)
                        steps.Add(Color.White.FromParser(parametersArray[i], out bool keep, out keep, out keep, out keep, out bool useDefault));

                }
                else {



                    for (int i = 1; i < parametersArray.Length - 1; i++)
                        steps.Add(Color.White.FromParser(parametersArray[i], out bool keep, out keep, out keep, out keep, out bool useDefault));


                }


                GradientString = new Gradient(steps.ToArray()).ToColoredString(new string(' ', foregroundLenght));
                Color[] colors = new Color[foregroundLenght];
               index = 0;
                foreach (ColoredString.ColoredGlyphEffect C in GradientString)
				{
                    colors[index] = C.Foreground;
                    index++;
     

				}
                foregrad = new Gradient(colors);
                steps = new List<Color>();
                if (backroundindex != null)
                {
                    for (int i = (int)backroundindex + 1; i < parametersArray.Length - 1; i++)
                        steps.Add(Color.White.FromParser(parametersArray[i], out bool keep, out keep, out keep, out keep, out bool useDefault));

                    GradientString = new Gradient(steps.ToArray()).ToColoredString(new string(' ', (int)backroundLenght));
                    colors = new Color[(int)backroundLenght];
                    index = 0;
                    foreach (ColoredString.ColoredGlyphEffect c in GradientString)
                    {
                        colors[index] = c.Foreground;
                        index++;


                    }
                    backgrad = new Gradient(colors);
                }
                

            }

            else
                throw badCommandException;
        }


        public ParseCommandGradientAnim()
        {

        }
        GradientEffect GenerateEffect(int foreoffset,int backoffset) {
            Gradient foregengrad = foregrad;
            if (foreoffset > 0)
            {
                Color[] colors = foregrad.ToColorArray(foregrad.Count());
                Color[] loopedAround = new Color[foreoffset];
                Array.Copy(colors, loopedAround, foreoffset);
                Array.Copy(colors, foreoffset, colors, 0, colors.Length - foreoffset);
                Array.Copy(loopedAround, 0, colors, colors.Length - foreoffset, foreoffset);
                foregengrad = new Gradient(colors);
            }
            Gradient backgengrad = backgrad;
            if (backoffset > 0)
            {
                Color[] colors = backgrad.ToColorArray(backgrad.Count());
                Color[] loopedAround = new Color[backoffset];
                Array.Copy(colors, loopedAround, backoffset);
                Array.Copy(colors, backoffset, colors, 0, colors.Length - backoffset);
                Array.Copy(loopedAround, 0, colors, colors.Length - backoffset, backoffset);
                backgengrad = new Gradient(colors);
                
            }

            GradientEffect gradEffect = new GradientEffect();
            gradEffect.Speed = 1;
            gradEffect.foregradient = foregengrad;
            gradEffect.backgradient = backgengrad;

            return gradEffect;
        }
        int backcounter = 0;
        int forecounter = 0;
        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex, ICellSurface surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
        {

            /*
            if (CommandType == CommandTypes.Background)
                glyphState.Background = GradientString[Length - _counter].Foreground;
            else
                glyphState.Foreground = GradientString[Length - _counter].Foreground;

            */

            glyphState.Effect = GenerateEffect(foregroundLenght - forecounter, backroundLenght == null ? 0: (int)backroundLenght - backcounter);


            forecounter++;
            backcounter++;
            if (backcounter > backroundLenght) {

                backcounter = 0;
            }
            if (forecounter > foregroundLenght)
            {

                forecounter = 0;
            }


        }
    }
}

