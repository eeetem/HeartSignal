using System;
using SadConsole;
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
        public ThingButtonTheme(Gradient grad = null) : base() {


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

        private bool inited = false;
        public void InitialDraw(Button button, TimeSpan time)
        {
            if (inited) return;
            inited = true;
            //do the underline animation regardless

      
            int middle = button.Surface.Height != 1 ? button.Surface.Height / 2 : 0;
            
            
            button.Surface.UsePrintProcessor = true;

            //button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), appearance);

            
            ColoredString parsedText = ColoredString.Parser.Parse(button.Text.Replace(";"," "));
            if (textcolor != Color.White)
            {
                parsedText.SetForeground(textcolor);
            }

            button.Surface.Print(0, middle, parsedText);

            
            
        }

        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            
            
            
            if (!(control is Button button)) return;
            
            InitialDraw(button,time);
            
            if (button.Surface.Effects.Count != 0)
            {
                button.Surface.Effects.UpdateEffects(time);
                button.IsDirty = true;
            }

            
            gradientCounter += (float)time.TotalSeconds/2;
            if (gradientCounter > 1)
            {

                gradientCounter = 0;
            }
            Color color = grad.Lerp(gradientCounter);
            button.Surface.SetDecorator(0, button.Surface.Width, new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(color));

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

            inited = false;

        }
        public void DefaultColor()
        {
            inited = false;
            textcolor = Color.White;


        }


    }
}
