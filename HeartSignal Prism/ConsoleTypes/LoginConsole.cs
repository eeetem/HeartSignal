using SadConsole;
using System;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace HeartSignal
{
    public class LoginConsole : SadConsole.UI.ControlsConsole
    {
        public LoginConsole(int width, int height) : base(width, height)
        {

            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;

            Cursor.DisableWordBreak = true;

            Cursor.UseStringParser = true;
            UsePrintProcessor = true;
            miniDisplay = new Console(30,15);
            
           // miniDisplay.SadComponents.Add(new AnimatedBorderComponent());
            Children.Add(miniDisplay);
            

        }

        public Console miniDisplay;
  
        public string tagline ="";
        public void ReDraw()
        {
          //  this.Clear();
            this.Controls.Clear();
   

            miniDisplay.Position = new Point(Width / 2 - 15, (Program.Height / 2) + 10);
            this.Print(0, (Program.Height/2)-8,tagline.Align(HorizontalAlignment.Center,Width));
            
            var input = new TextBox(26)
            {
                Text = "login",
                
                Position = new Point(Width/2 -13 , (Program.Height/2)-4)
            };

            void Handler(object sender, ControlBase.ControlMouseState args)
            {
                input.Text = "";
                input.MouseButtonClicked -= Handler;
            }

            input.MouseButtonClicked += Handler;
            
            Controls.Add(input);

            var password = new TextBox(26)
            {
                Text = "password",
                Mask = '*',
                Position = new Point(Width/2 -13 ,(Program.Height/2))
            };

            void Handler2(object sender, ControlBase.ControlMouseState args)
            {
                password.Text = "";
                password.MouseButtonClicked -= Handler2;
            }

            password.MouseButtonClicked += Handler2;
            
            Controls.Add(password);
            
            
           
            var button = new Button(11)
            {
                Text = "Be Born",
                Position = new Point(Width/2 -5 ,(Program.Height/2)-2),
            };
            button.MouseButtonClicked += (s, a) => input.FocusLost();
            button.MouseButtonClicked += (s, a) => password.FocusLost();
            button.MouseButtonClicked += (s,a) => Program.SendNetworkMessage("connect " + input.Text + " " + password.Text);
            Controls.Add(button);
        }

    }
}