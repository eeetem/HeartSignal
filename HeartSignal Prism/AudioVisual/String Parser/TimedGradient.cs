using System;
using System.Collections.Generic;
using SadConsole;
using SadConsole.Effects;

namespace HeartSignal
{

    public class TimedGradient : CellEffectBase
    {
     
        private double turnTime;
        private ColoredGlyph inital;
        private ColoredGlyph final;
        public TimedGradient(ColoredGlyph _initial, ColoredGlyph _final, double _turnTime)
        {

            turnTime = _turnTime;

            inital = _initial;
            final = _final;
            
            RemoveOnFinished = true;
            StartDelay = TimeSpan.Zero;
            IsFinished = false;
            _timeElapsed = TimeSpan.Zero;
            
            
            
        }
		private static readonly object syncObj = new object();
        public override bool ApplyToCell(ColoredGlyph cell, ColoredGlyphState originalState)
        {

            cell.Background = inital.Background;
            cell.Foreground = inital.Foreground;
            
            double unixTimestamp = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()/1000;
            if (unixTimestamp > turnTime)
            {
                cell.Background = final.Background;
                cell.Foreground = final.Foreground;
                IsFinished = true;
            }

            return true;        
        }



   
        

        public override ICellEffect Clone() => new TimedGradient(inital,final,turnTime)
        {


        };
    }
}
