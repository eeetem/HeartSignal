﻿using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadRogue.Primitives;

namespace HeartSignal
{
    class DelayConsole : BaseConsole
    {
        public DelayConsole(int width, int height) : base(width, height,false)
        {
            
           SadComponents.Add(new MouseHandler());



        }

        public void DisplayDelay(string name, Gradient color, float duartion, float delay)
        {
	        double unixTimestamp = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()/1000;
	        delays[name] = (new DelayBar(name, unixTimestamp+delay, unixTimestamp +duartion+delay, color));

        }

        private Dictionary<string,DelayBar> delays = new Dictionary<string,DelayBar>();

        private float counter = 0;

        public override void Render(TimeSpan delta)
        {
	        counter += (float)delta.Milliseconds / 1000  * Utility.GlobalAnimationSpeed;
	        if (counter > 1)
	        {
		        counter = 0;
	        }

	        this.Clear();

	        
	        Cursor.Position = new Point(0, 1);
             foreach (DelayBar db in delays.Values.ToList())
             {

	             Utility.PrintParseMessage(db.name,this.actionWindow,this,false);
	             int barLenght = Width - (Cursor.Position.X + 2);
	             Surface.DrawLine(Cursor.Position + new Point(1, 0),Cursor.Position + new Point(barLenght, 0),176,db.color.Lerp(counter),Color.Black);
	             double duration = db.endtime - db.startime;
	             double unixTimestamp = (double)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()/1000;
	             double timePassed = Math.Max(unixTimestamp - db.startime,0);
	             double timePerGlyph = duration/barLenght;
	             int glyphsToFill = (int)Math.Max(Math.Round(timePassed / timePerGlyph),1);
	             Surface.DrawLine(Cursor.Position + new Point(1, 0),Cursor.Position + new Point(glyphsToFill, 0),178,db.color.Lerp(counter),db.color.Lerp(counter));

	             if (duration < timePassed)
	             {
		             delays.Remove(db.name);
	             }

	             Cursor.NewLine();
	             Cursor.NewLine();



             }

            
             base.Render(delta);

        }
        public struct DelayBar
        {
	        public DelayBar(string name, double startime, double endtime, Gradient color)
	        {
		        this.name = name;
		        this.startime = startime;
		        this.endtime = endtime;
		        this.color = color;
	        }

	        public string name;
	        public double startime;
	        public double endtime;
	        public Gradient color;



        }


    }

}
