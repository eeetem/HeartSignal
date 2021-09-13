using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using SadConsole.StringParser;
using SadConsole;

namespace HeartSignal
{
	static class Utility
	{


        public static double GetAngleOfLineBetweenTwoPoints(Point p1, Point p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * (180 / Math.PI);
        }


        public static string[] SplitThingID(string thingid) {


            
            string idstring = thingid.Substring(thingid.IndexOf("("), thingid.Length - thingid.IndexOf("("));
            idstring = idstring.Replace("(", "").Replace(")", "");
            string[] ids = idstring.Split(',');
                
           string thing = thingid.Remove(thingid.IndexOf("("), thingid.Length - thingid.IndexOf("("));

            string[] output = new string[ids.Length + 1];
            output[0] = thing;
            ids.CopyTo(output, 1);

            return output;


        }




        public static ParseCommandBase CustomParseCommand(string command, string parameters, ColoredGlyph[] glyphString,
                                                  ICellSurface surface, ParseCommandStacks commandStacks)
        {
            
            switch (command)
            {
                case "gradanim":
                case "ga":
                    return new ParseCommandGradientAnim(parameters, commandStacks);
                default:
                    return null; 
            }
        }


    }
}
