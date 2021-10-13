using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using PrimS.Telnet;
using System.Threading.Tasks;
using SadConsole.Input;

namespace HeartSignal
{
    internal class BaseConsole : SadConsole.UI.ControlsConsole
    {

        public BaseConsole(int width, int height, bool border= true, bool actionW=false) : base(width, height)
        {

            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;
            ColoredString.CustomProcessor = Utility.CustomParseCommand;
            Cursor.UseStringParser = true;
            UsePrintProcessor = true;
            // SadComponents.Add(new CopyPasteMouse());
            if (border)
            {
                SadComponents.Add(new AnimatedBorderComponent());
            }
            if (actionW) {
                actionWindow = new ActionWindow(30, 5, new Point(0, 0));
                Children.Add(actionWindow);

                actionWindow.IsVisible = false;
                actionWindow.IsEnabled = false;
                }
        }
        protected ActionWindow actionWindow;

        private bool needRedraw = false;


        public void ReDraw() {
            needRedraw = true;
        
        }

        protected virtual void DrawConsole() { }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needRedraw)
            {
                this.Clear();
                if (actionWindow != null)
                {
                    actionWindow.Clear();
                    actionWindow.Controls.Clear();
                    actionWindow.IsVisible = false;
                }

                Controls.Clear();
                Cursor.Position = new Point(0, 0);
                this.Effects.RemoveAll();
                DrawConsole();
                needRedraw = false;


            }

        }
       


        }
}
