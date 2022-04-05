using System;
using System.Collections.Generic;
using System.Globalization;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.StringParser;

namespace HeartSignal
{
	class ParseCommandTimedGradient : ParseCommandBase
    {
        private ColoredGlyph initial;
        private ColoredGlyph final;
        private float delay;
        private float duration;
        private int lenght;
        private int counter = 1;
        public ParseCommandTimedGradient(string parameters, ParseCommandStacks commandStack)
        {
            string[] param = parameters.Split(':');
            bool keep = false;
            initial = new ColoredGlyph(Color.White.FromParser(param[0], out keep, out keep, out keep, out keep, out keep), Color.White.FromParser(param[1], out keep, out keep, out keep, out keep, out keep),0);
            final = new ColoredGlyph(Color.White.FromParser(param[2], out keep, out keep, out keep, out keep, out keep), Color.White.FromParser(param[3], out keep, out keep, out keep, out keep, out keep),0);
            delay = float.Parse(param[4], CultureInfo.InvariantCulture);
            duration = float.Parse(param[5], CultureInfo.InvariantCulture);
            lenght = int.Parse(param[6], CultureInfo.InvariantCulture);
            
            
            
            CommandType = CommandTypes.Effect;

        }


        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex, ICellSurface surface, ref int stringIndex, ReadOnlySpan<char> processedString, ParseCommandStacks commandStack)
        {
            float timePerGlyph = duration / lenght;
            double unixTimestamp = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()/1000;
            double turnTime = unixTimestamp + delay + timePerGlyph * counter;
            counter++;
            
            glyphState.Effect = new TimedGradient(initial,final,turnTime);

        }
    }
}

