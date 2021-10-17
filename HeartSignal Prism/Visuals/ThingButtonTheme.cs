using System;
using System.Collections.Generic;
using SadConsole;
using System.Text;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.Input;

namespace HeartSignal
{
    class ThingButtonTheme : ButtonLinesTheme
    {

        Gradient grad;
        float gradientCounter;
        Color textcolor;
        bool ForceApreanace = true;
        public ThingButtonTheme(Gradient grad = null, bool forceapreaance = true) : base() {

            ForceApreanace = forceapreaance;
            if (grad != null)
            {
                this.grad = grad;
            }
           else
            {
                this.grad = new Gradient(Color.Red, Color.Pink, Color.Red);
            }
            textcolor = Color.White;
        }
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;

            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance;



            appearance = ControlThemeState.GetStateAppearance(control.State);
            int middle = button.Surface.Height != 1 ? button.Surface.Height / 2 : 0;
            gradientCounter += (float)time.TotalSeconds/2;
            if (gradientCounter > 1)
            {

                gradientCounter = 0;
            }


            // Redraw the control
            button.Surface.Fill(appearance.Foreground, appearance.Background,
                                appearance.Glyph, Mirror.None);

            appearance.Foreground = textcolor;
            
            button.Surface.UsePrintProcessor = true;
            //button.Surface.Print(0, 0, button.Text);
            if (ForceApreanace)
            {
                button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), appearance);

            }
            else {
                ColoredString parsedText = ColoredString.Parse(button.Text);
                parsedText.IgnoreEffect = false;
               
                 button.Surface.Print(0, middle, parsedText);

            }
           
            Color color = grad.Lerp(gradientCounter);
            button.Surface.SetDecorator(0, button.Surface.Width, new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(color));
              ///  button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
               // button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));


               button.IsDirty = true;
        }
        public void AdjustColor() {

            if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift))
            {

                textcolor = Color.Red;
            }
            else if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl))
            {

                textcolor = Color.Blue;
            }
            else {

                textcolor = Color.White;
            }
        
        }
        public void DefaultColor()
        {
            textcolor = Color.White;


        }


    }
}
