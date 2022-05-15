using System;
using System.Collections.Generic;
using SadConsole;
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
            StartDelay = TimeSpan.Zero;
            IsFinished = false;
            _timeElapsed = TimeSpan.Zero;
        }

        public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
        {

          
                ColoredGlyph target = glyphs[(int) Counter];
                cell.Foreground = target.Foreground;
                cell.Background = target.Background;
                cell.Glyph = target.Glyph;
                return true;   
            

                
        }



        public override void Update(System.TimeSpan delta)
        {
           
            Counter = Math.Clamp(Counter +(float) delta.Milliseconds / 1000 * Utility.GlobalAnimationSpeed,0,glyphs.Count);
            if (Counter >= glyphs.Count)
            {
                Counter = 0;
            }
            

            base.Update(delta);

            



        }

        public override ICellEffect Clone() => new AnimGlyph(glyphs)
        {


        };



    }
}
