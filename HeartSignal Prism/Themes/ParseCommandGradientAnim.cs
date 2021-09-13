using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.StringParser;
using System.Globalization;
namespace HeartSignal
{
	class ParseCommandGradientAnim : ParseCommandBase
    {

        private int _counter;


        public ColoredString GradientString;

        public int Length;
        Gradient grad;
 
        public ParseCommandGradientAnim(string parameters, ParseCommandStacks commandStack)
        {

            var badCommandException = new ArgumentException("command is invalid for Recolor: " + parameters);

            string[] parametersArray = parameters.Split(':');

            if (parametersArray.Length > 3)
            {
                
                CommandType = parametersArray[0] == "b" ? CommandTypes.Background : CommandTypes.Foreground;
                _counter = Length = int.Parse(parametersArray[parametersArray.Length - 1], CultureInfo.InvariantCulture);


                List<Color> steps = new List<Color>();

                for (int i = 1; i < parametersArray.Length - 1; i++)
                    steps.Add(Color.White.FromParser(parametersArray[i], out bool keep, out keep, out keep, out keep, out bool useDefault));

                GradientString = new ColorGradient(steps.ToArray()).ToColoredString(new string(' ', Length));
                Color[] colors = new Color[Length];
                int index = 0;
                foreach (ColoredString.ColoredGlyphEffect C in GradientString)
				{
                    colors[index] = C.Foreground;
                    index++;
     

				}
                grad = new Gradient(colors);
                commandStack.TurnOnEffects = true;

            }

            else
                throw badCommandException;
        }


        public ParseCommandGradientAnim()
        {

        }
        GradientEffect GenerateEffect(int offset) {
            Gradient gengrad = grad;
            if (offset > 0)
            {
                Color[] colors = grad.ToColorArray(grad.Count());
                Color[] loopedAround = new Color[offset];
                Array.Copy(colors, loopedAround, offset);
                Array.Copy(colors, offset, colors, 0, colors.Length - offset);
                Array.Copy(loopedAround, 0, colors, colors.Length - offset, offset);
                gengrad = new Gradient(colors);
            }

            GradientEffect gradEffect = new GradientEffect();
            gradEffect.Speed = 1;
            gradEffect.gradient = gengrad;
            gradEffect.IsForeground = CommandType == CommandTypes.Foreground;
            return gradEffect;
        }

        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex,
            ICellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {

            /*
            if (CommandType == CommandTypes.Background)
                glyphState.Background = GradientString[Length - _counter].Foreground;
            else
                glyphState.Foreground = GradientString[Length - _counter].Foreground;
*/

            glyphState.Effect = GenerateEffect(Length - _counter);
           

            _counter--;
            if (_counter == 0)
                commandStack.RemoveSafe(this);
            
        }
    }
}

