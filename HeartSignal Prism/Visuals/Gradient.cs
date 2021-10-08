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
        public Gradient foregradient;
        public Gradient backgradient;
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
            if(foregradient != null)
			{
                cell.Foreground = foregradient.Lerp(Counter);

            }


            if (backgradient != null)
            {
                cell.Background = backgradient.Lerp(Counter);

            }




            return true;        
        }



        public override void Update(double timeElapsed)
        {
            

            Counter += (float)timeElapsed * AnimatedBorderComponent.speed * 0.8f;
            if (Counter > 1)
            {

                Counter = 0;
            }
            base.Update(timeElapsed);

            



        }

        public override ICellEffect Clone() => new GradientEffect()
        {
            Speed = Speed,
            foregradient = foregradient,
            backgradient = backgradient,
            IsForeground = IsForeground,

        };


       // public override string ToString() => string.Format("BLINK-{0}-{1}-{2}-{3}-{4}", BlinkOutColor.PackedValue, BlinkSpeed, UseCellBackgroundColor, StartDelay, BlinkCount);
    }
}
