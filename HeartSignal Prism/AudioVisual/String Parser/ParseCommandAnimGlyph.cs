using System.Collections.Generic;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.StringParser;

namespace HeartSignal
{
	class ParseCommandAnimGlyph : ParseCommandBase
    {

        List<ColoredGlyph> Glyphs = new List<ColoredGlyph>();
        public ParseCommandAnimGlyph(string parameters, ParseCommandStacks commandStack)
        {
            string[] glyphs = parameters.Split(':');
            foreach (string glyph in glyphs) { 
               string[] param = glyph.Split("-");
                bool keep = false;
                Glyphs.Add(new ColoredGlyph(Color.White.FromParser(param[0], out keep, out keep, out keep, out keep, out keep), Color.White.FromParser(param[1], out keep, out keep, out keep, out keep, out keep),(int)param[2][0]));
            
            }
            CommandType = CommandTypes.Effect;
            commandStack.TurnOnEffects = true;
        }


        public override void Build(ref ColoredString.ColoredGlyphEffect glyphState, ColoredString.ColoredGlyphEffect[] glyphString, int surfaceIndex,
            ICellSurface surface, ref int stringIndex, string processedString, ParseCommandStacks commandStack)
        {

            glyphState.Effect = new AnimGlyph(Glyphs);

            commandStack.RemoveSafe(this);
        }
    }
}

