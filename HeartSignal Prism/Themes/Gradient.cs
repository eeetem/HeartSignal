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

    public class GradientEffect : CellEffectBase
    {
        private float Counter = 0;

        public double Speed { get; set; }
        public Gradient gradient;
        public bool IsForeground;


        public GradientEffect() 
        {
 
            Counter = 0;
            RemoveOnFinished = false;
            StartDelay = 0d;
            IsFinished = false;
            _timeElapsed = 0d;
        }

        public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
        {
    
                  Color color = gradient.Lerp(Counter);

            if (IsForeground)
            {

                cell.Foreground = color;


            }
            else {

     
                cell.Background = color;
            }

            return true;        
        }



        public override void Update(double timeElapsed)
        {
            

            Counter += (float)timeElapsed * (float)Speed * 1f;
            if (Counter > 1)
            {

                Counter = 0;
            }
            base.Update(timeElapsed);

            



        }

        public override ICellEffect Clone() => new GradientEffect()
        {
            Speed = Speed,
            gradient = gradient,
            IsForeground = IsForeground,

        };


       // public override string ToString() => string.Format("BLINK-{0}-{1}-{2}-{3}-{4}", BlinkOutColor.PackedValue, BlinkSpeed, UseCellBackgroundColor, StartDelay, BlinkCount);
    }
}
