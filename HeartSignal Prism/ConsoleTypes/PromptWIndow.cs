using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadConsole.Input;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.UI;

namespace HeartSignal
{
    class PromptWindow : SadConsole.UI.ControlsConsole, IMouseInputReciver
    {
        public PromptWindow(int width, int height, Point position) : base(width, height)
        {


            this.Position = position;
            Cursor.DisableWordBreak = false;
            IsVisible = false;
            UsePrintProcessor = true;
            Cursor.UseStringParser = true;
            SadComponents.Add( new MouseHandler());
        }

        public List<string> args = new List<string>();

         public string toptext;
        public string middletext;
        public enum PopupType { 
        Choice,
        Text,
        Permanent,//has to be manually hid by other code
        
        }
        public PopupType Type;
        public bool needsDraw = false;


        public void DisplayPrompt()
        {
            NetworkManager.lockMessages = true;
               needsDraw = false;
            this.Resize(Width, 128, Width, 128, true);
            Controls.Clear();
        
            this.Cursor.Position = new Point(1, 1);
            string[] words = middletext.Split(' ');
            foreach (string word in words)
            {
                if (Cursor.Position.X + Utility.RemoveParserTags(word).Length + 1 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Cursor.Print(word.Replace(";", " ")).RightWrap(1);
            }
            if (Type == PopupType.Choice)
            {

                List<string> fullArgList = args.ToList();
                while (fullArgList.Count > 0)
                { 
                    Cursor.NewLine().Right(1);
                    
                
                    int argLenght = 4;//start accounting for borders and a little more just to be safe
                    List<string> argsToPrint = new List<string>();
                    foreach (var arg in fullArgList)
                    {
                        if (argLenght + arg.Length + 4 > Width)
                        {
                            break;
                        }
                    
                        argsToPrint.Add(arg);
                        argLenght += arg.Length+4;
                    }

                    int glyphsPerArg = (int)Math.Floor((float)Width / argsToPrint.Count);

                    foreach (string arg in argsToPrint)
                    {
                        int padding = (int) Math.Floor((double) (glyphsPerArg - (arg.Length + 2)) / 2);
                       
                        if (padding < 1)
                        {
                            Cursor.Right(1);//ensure atleast 1 space
                            padding = 0;//prevent negative values
                        }

                        Cursor.Right(padding );
                        

                        
                        var button = new Button(arg.Length+2, 1)
                        {
                            Text = arg,
                            Position = Cursor.Position
                            // Theme = new but
                        };
                        Cursor.Right(arg.Length + 2);
                        
                        Cursor.Right(padding); 

                        button.Click += (s, a) => NetworkManager.SendNetworkMessage(arg,true);
                        button.Click += (s, a) => this.IsVisible = false;
                        button.Click += (s, a) => NetworkManager.lockMessages = false;
                        this.Controls.Add(button);
                    
                    
                    
                    }
                    fullArgList.RemoveRange(0,argsToPrint.Count);
                    
                    
                   
                }
            }
            else if (Type == PopupType.Text)
            {
                Cursor.NewLine();
                var text = new TextBox(14)
                {
                    // Mask = '*',
                    Position = new Point(Width / 2 -14/2, Cursor.Position.Y)
                };
                Controls.Add(text);
                Cursor.NewLine();
                var button = new Button(4, 1)
                {
                    Text = "OK",
                    Position = new Point(Width / 2 - 4/2, Cursor.Position.Y)
                };
                button.Click += (s, a) => NetworkManager.SendNetworkMessage(text.Text,true);
                button.Click += (s, a) => this.IsVisible = false;
                button.Click += (s, a)  => NetworkManager.lockMessages = false;
                Controls.Add(button);







            }
            else if (Type == PopupType.Permanent)
            {




            }
            this.Resize(Width, Cursor.Position.Y + 2, Width, Cursor.Position.Y + 2, false);
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.LightGray, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Print(Width/2-toptext.Length/2, 0, toptext);
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


        public void Clicked(Point clickloc, MouseScreenObjectState state)
        {
           
        }

        public void RightClicked(Point clickloc, MouseScreenObjectState state)
        {
            if (Type == PopupType.Permanent)
            {
                Program.PromptWindow.IsVisible = false;
                ActionWindow.awaitingItemClick = false;
                
            }
        }
    }
}
