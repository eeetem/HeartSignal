using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.UI;

namespace HeartSignal
{
    class PromptWindow : SadConsole.UI.ControlsConsole
    {
        public PromptWindow(int width, int height, Point position) : base(width, height)
        {


            this.Position = position;
            Cursor.DisableWordBreak = false;
        }


         public string toptext;
        public string middletext;
        public bool binary;///posibly turn this into an enum later if we get multiple popup types

        public bool needsDraw = false;


        public void DisplayPrompt()
        {
            needsDraw = false;
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.LightGray, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Print(Width / 2 - toptext.Length, 0, toptext);
            this.Cursor.Position = new Point(1, 1);
            string[] words = middletext.Split(' ');
            foreach (string word in words)
            {
                if (Cursor.Position.X + word.Length + 1 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Cursor.Print(word).RightWrap(1);
            }
            if (binary)
            {
                var button = new Button(5, 1)
                {
                    Text = "yes",
                    Position = new Point(1, Height - 2),
                    // Theme = new but
                };

                button.MouseButtonClicked += (s, a) => Program.SendNetworkMessage("prompt:yes");
                button.MouseButtonClicked += (s, a) => this.IsVisible = false;
                this.Controls.Add(button);
                button = new Button(4, 1)
                {
                    Text = "no",
                    Position = new Point(Width - 5, Height - 2)
                    // Theme = new but
                };

                button.MouseButtonClicked += (s, a) => Program.SendNetworkMessage("prompt:no");
                button.MouseButtonClicked += (s, a) => this.IsVisible = false;
                this.Controls.Add(button);
            }
            this.IsVisible = true;









        }
        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
            if (needsDraw)
            {
                DisplayPrompt();



            }





        }

    }
}
