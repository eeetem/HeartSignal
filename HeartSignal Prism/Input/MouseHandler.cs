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
    class MouseHandler : MouseConsoleComponent
    {



        IMouseInputReciver owner;
        public override void OnAdded(IScreenObject host)
        {
            owner = (IMouseInputReciver)host;

        }


        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            handled = false;
            if (!state.IsOnScreenObject)
            {
                
                return;
            }

            if (state.Mouse.LeftButtonDown ) {
                owner.Clicked(state.CellPosition);
            
            
            }


        }









    }
}
