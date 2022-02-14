using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.Effects;

namespace HeartSignal
{

    public class AnimGlyph : CellEffectBase
    {
        private float Counter = 0;


        List<ColoredGlyph> glyphs;

        public AnimGlyph(List<ColoredGlyph>  g) 
        {
            glyphs = g;

            Counter = 0;
            RemoveOnFinished = false;
            StartDelay = 0d;
            IsFinished = false;
            _timeElapsed = 0d;
        }

        public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
        {

            ColoredGlyph target = glyphs[(int)Counter];
            cell.Foreground = target.Foreground;
            cell.Background = target.Background;
            cell.Glyph = target.Glyph;

            return true;        
        }



        public override void Update(double timeElapsed)
        {
            

            Counter += (float)timeElapsed *Utility.GlobalAnimationSpeed;
            if (Counter > glyphs.Count)
            {

                Counter = 0;
            }
            base.Update(timeElapsed);

            



        }

        public override ICellEffect Clone() => new AnimGlyph(glyphs)
        {


        };


       // public override string ToString() => string.Format("BLINK-{0}-{1}-{2}-{3}-{4}", BlinkOutColor.PackedValue, BlinkSpeed, UseCellBackgroundColor, StartDelay, BlinkCount);
    }
}
