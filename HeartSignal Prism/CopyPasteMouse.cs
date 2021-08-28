using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using System.Windows.Forms;

namespace HeartSignal
{
    class CopyPasteMouse : MouseConsoleComponent
    {



        Console owner;
        public override void OnAdded(IScreenObject host)
        {
            owner = (Console)host;

        }


        int clickindex = 0;
        int releaseindex = 0;
        bool holding = true;

        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {


            if (state.Mouse.LeftButtonDown&& !holding) {
                holding = true;
                clickindex = state.SurfaceCellPosition.ToIndex(owner.Width);
 
            }
            else if (!state.Mouse.LeftButtonDown && holding)
            {
                try
                {
                    holding = false;
                    releaseindex = state.SurfaceCellPosition.ToIndex(owner.Width);
                    string str = owner.GetString(clickindex, releaseindex - clickindex);
                    if (str.Length > 1)
                    {
                        Clipboard.SetText(str);

                    }
                }
                catch ( Exception e)///kinda cringe but i am not bothered fixing this exception for now
                { 
                
                
                
                
                
                }
                
                

            }


            if (holding) {
  
                //todo graphics here


            }




            handled = true;
        }









    }
}
