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
        bool holding;

        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            if (state.Mouse.LeftButtonDown && !holding)
            {
                holding = true;
                owner.Clicked(state.CellPosition);


            }
            else if (holding && !state.Mouse.LeftButtonDown)
            {

                holding = false;


            }


            handled = false;


        }









    }
}
