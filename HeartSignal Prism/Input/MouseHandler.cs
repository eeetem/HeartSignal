﻿using SadConsole.Components;
using SadConsole.Input;
using SadConsole;
using SadRogue.Primitives;

//using System.Windows.Forms;

namespace HeartSignal
{
    class MouseHandler : MouseConsoleComponent
    {



        IMouseInputReciver owner;
        public override void OnAdded(IScreenObject host)
        {
            if (host is IMouseInputReciver)
            {

                owner = (IMouseInputReciver) host;
            }
        }
        bool holding;

        
        public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
        {
            handled = false;
            if (!state.IsOnScreenObject)
            {
 
                return;
            }
            if (state.Mouse.RightButtonDown)
            {
                owner?.RightClicked(state.CellPosition,state);


            }
            if (state.Mouse.LeftButtonDown && !holding)
            {
                holding = true;
                owner?.Clicked(state.CellPosition,state);


            }
            else if (holding && !state.Mouse.LeftButtonDown)
            {

                holding = false;
                

            }
            
            BaseConsole consolehost = host as BaseConsole;
            if (consolehost != null)
            {
                ICellSurface surface = consolehost.Surface;
                if (state.CellPosition.Y-surface.ViewPosition.Y < 4 )
                {
                    surface.ViewPosition = surface.ViewPosition.Translate(new Point(0, -1));
                }else  if (state.CellPosition.Y-surface.ViewPosition.Y  > surface.ViewHeight-4)
                {
                    surface.ViewPosition = surface.ViewPosition.Translate(new Point(0, 1));
                }


                surface.ViewPosition = surface.ViewPosition.Translate(new Point(0, (int)(state.Mouse.ScrollWheelValueChange*0.01)));
                if (surface.ViewPosition.Y > consolehost.MaxScroll)
                {

                    surface.ViewPosition = new Point(0, consolehost.MaxScroll);
                }
            }
        }









    }
}
