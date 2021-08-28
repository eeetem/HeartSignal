using System;
using System.Collections.Generic;
using SadConsole;
using System.Text;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace HeartSignal
{
    class RoomButtonTheme : ButtonLinesTheme
    {

        Gradient grad;
        float gradientCounter;
        public override void UpdateAndDraw(ControlBase control, TimeSpan time)
        {
            if (!(control is Button button)) return;
            if (!button.IsDirty) return;
            if (grad == null) 
            {
            grad = new Gradient(Color.Red, Color.Pink,Color.Red);
            }
            RefreshTheme(control.FindThemeColors(), control);
            ColoredGlyph appearance;


            appearance = ControlThemeState.GetStateAppearance(control.State);



            appearance = ControlThemeState.GetStateAppearance(control.State);
            int middle = button.Surface.Height != 1 ? button.Surface.Height / 2 : 0;
            gradientCounter += (float)time.TotalSeconds/2;
            if (gradientCounter > 1)
            {

                gradientCounter = 0;
            }
            Color color = grad.Lerp(gradientCounter);

            // Redraw the control
            button.Surface.Fill(appearance.Foreground, appearance.Background,
                                appearance.Glyph, Mirror.None);

            button.Surface.Print(0, middle, button.Text.Align(button.TextAlignment, button.Width), appearance);

           
                button.Surface.SetDecorator(0, button.Surface.Width,/* new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[1], Mirror.None).CreateCellDecorator(topleftcolor),*/ new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(color));
              ///  button.Surface.AddDecorator(0, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-left", topleftcolor));
               // button.Surface.AddDecorator(button.Surface.Width - 1, 1, button.Parent.Host.ParentConsole.Font.GetDecorator("box-edge-right", bottomrightcolor));


               button.IsDirty = true;
        }
        
    }
}
