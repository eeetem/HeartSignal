using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace HeartSignal
{
    public class AnimatedBorderComponent : LogicComponent
    {
        private Console _borderConsole;
        public static ColoredGlyph _borderCellStyle = new ColoredGlyph(Color.DarkGray,Color.Black);
        int[][] _borderGlyphs = new int[][]
{
new int[] { 176,176,176,176,176,176,176,176,176,176,176,176,176,},
new int[] {177,177,177,177,177,177,177,177,177,177,177,177,177,},
new int[] { 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, 178, },
new int[] {177,177,177,177,177,177,177,177,177,177,177,177,177,},
};
        public static float speed = 1f;

        public bool IsUpdate => false;

        public bool IsRender => false;

        public bool IsMouse => false;

        public bool IsKeyboard => false;


        public AnimatedBorderComponent()
        {
           
        }



        public override void OnAdded(IScreenObject screenObject)
        {
            if (screenObject is Console console)
            {
                _borderConsole = new Console(console.Width + 2, console.Height + 2);
                _borderConsole.Font = console.Font;
                _borderConsole.DrawBox(new Rectangle(0, 0, _borderConsole.ViewWidth, _borderConsole.ViewHeight), _borderCellStyle, null, _borderGlyphs[0]);
                _borderConsole.Position = new Point(-1, -1);
                _borderConsole.UseMouse = false;
                console.Children.Add(_borderConsole);
            }
            else
                throw new Exception("Can only be added to a console");
        }

        public override void OnRemoved(IScreenObject console)
        {
            if (_borderConsole.Parent != null)
            {
                _borderConsole.Parent = null;
            }

            _borderConsole = null;
        }

        public override void Render(IScreenObject console, TimeSpan delta) { }

        public void ProcessKeyboard(IScreenObject console, Keyboard info, out bool handled) => throw new NotImplementedException();

        public void ProcessMouse(IScreenObject console, MouseScreenObjectState state, out bool handled) => throw new NotImplementedException();

        double counter = 0;
        public override void Update(IScreenObject console, TimeSpan delta) {
            Console con = (Console)console;
            if (_borderConsole.Width != con.ViewWidth+2 || _borderConsole.Height != con.ViewHeight+2)
            {
                _borderConsole.Resize(con.ViewWidth + 2, con.ViewHeight + 2, con.ViewWidth + 2, con.ViewHeight + 2,
                    true); //this might cause performance issues
             
     
                
            }
            counter += (delta.TotalSeconds / 2) * speed;
            if (counter > 4)
            {
                counter = 0;
            }

            _borderConsole.DrawBox(new Rectangle(0, 0, _borderConsole.ViewWidth, _borderConsole.ViewHeight),
                _borderCellStyle, null, _borderGlyphs[(int) counter]);
        }
    }
    /*
    internal class BorderedConsole : Console
    {
        public BorderedConsole()
            : base(80, 25)
        {
            IsVisible = false;

            this.Print(1, 1, "Example of using a component to draw a border around consoles");

            var console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(176, Color.Red, Color.Black));
            console.Position = new Point(2, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(177, Color.Red, Color.Black));
            console.Position = new Point(17, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(178, Color.Red, Color.Black));
            console.Position = new Point(32, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Glyph line");
            console.SadComponents.Add(new BorderComponent(219, Color.Red, Color.Black));
            console.Position = new Point(47, 5);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Thin line");
            console.SadComponents.Add(new BorderComponent(ICellSurface.ConnectedLineThin, Color.Green, Color.Black));
            console.Position = new Point(17, 12);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Thick line");
            console.SadComponents.Add(new BorderComponent(ICellSurface.ConnectedLineThick, Color.Orange, Color.Black));
            console.Position = new Point(2, 12);
            Children.Add(console);

            console = new Console(12, 4);
            console.Print(1, 1, "Extd. line");
            console.SadComponents.Add(new BorderComponent(console.Font.IsSadExtended ? ICellSurface.ConnectedLineThinExtended : ICellSurface.ConnectedLineThin, Color.Purple, Color.Black));
            console.Position = new Point(32, 12);
            Children.Add(console);
        }

    }*/
}
