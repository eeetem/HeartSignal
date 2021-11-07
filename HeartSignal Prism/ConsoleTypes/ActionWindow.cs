using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.UI;
using SadConsole.Input;

namespace HeartSignal
{
    class ActionWindow : SadConsole.UI.ControlsConsole
    {
        public ActionWindow(int width, int height, Point position) : base(width, height)
        {

            this.UsePrintProcessor = true;
            this.Position = position;

        }
        public static Dictionary<string, Dictionary<string, List<string>>> actionDatabase = new Dictionary<string, Dictionary<string, List<string>>>();
       // public static Dictionary<string, Dictionary<string, List<string>>> argactionDatabase = new Dictionary<string, Dictionary<string, List<string>>>();
        static string lastitem = "";
        public string selectedTab = "";
        protected override void OnMouseExit(MouseScreenObjectState state) {
            IsVisible = false;
            IsEnabled = false;
            lastitem = "";
        
        
        }

        //a lot fo reapeating code in here, integrate this better at some point
        public void ShowTooltip(string text,ICellSurface surface, Point? newPosition = null) {
            this.Resize(40, 12, 40, 12, false);
            AudioManager.ParseRequest(null, "play", "misc/tooltip.ogg");
            if (newPosition != null)
            {

                Position = (Point) newPosition - new Point(0, surface.ViewPosition.Y);
            }
            this.Clear();
            Controls.Clear();

            this.Cursor.Position = new Point(1, 1);
            string[] words = text.Split(" ");
            foreach (string w in words)
            {
                string Word = w;
                if (!Word.Contains("[") && Cursor.Position.X + Word.Length + 2 > Width || Word.Contains("[newline]"))
                {

                    Word = Word.Replace("[newline]", "");
                    Cursor.NewLine().Right(1);
                }
                Cursor.Print(Word.Replace(";"," ")).Right(1);
            }
            this.Resize(Width, Cursor.Position.Y + 2, Width, Cursor.Position.Y + 2, false);
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Green, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);

            this.IsVisible = true;
            this.IsEnabled = true;
        }



        public void DisplayActions(string item,  ICellSurface surface,Point? newPosition = null, bool expilcitlook = false)
        {
            //if (focusitem != null) { item = focusitem; }
            if (awaitingItemClick) { return; }

            this.Resize(40, 5, 40, 5, false);

            string[] returned = Utility.SplitThingId(item);
            string thing = returned[0];
            string id = returned[1];

            if (lastitem != item)
            {
                if (!expilcitlook)
                {
                    GetDesc(id);
                }

                lastitem = item;
            }
            if (!actionDatabase.ContainsKey(id)) {
                return;
            }

            if (newPosition != null)
            {

                Position = (Point) newPosition - new Point(0, surface.ViewPosition.Y);
            }
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Cursor.Position = new Point(0, 0);
            Cursor.Right(1).Print("Actions").Right(1);
            //if the action menu does not contain currently selected tab - switch to the first one
            if (!actionDatabase[id].ContainsKey(selectedTab))
			{
                var first = actionDatabase[id].First();
                selectedTab = first.Key;


            }


            foreach (KeyValuePair<string, List<string>> tabs in actionDatabase[id])
            {

                if (tabs.Key == selectedTab)
                {
                    Surface.SetDecorator(Cursor.Position.X, Cursor.Position.Y, tabs.Key.Length, new GlyphDefinition(ICellSurface.ConnectedLineThinExtended[7], Mirror.None).CreateCellDecorator(Color.White));
                    Cursor.Print(tabs.Key).Right(1);
                   

                }
                else
                {
                    var tab = new Button(tabs.Key.Length, 1)
                    {
                        Text = tabs.Key,
                        Position = Cursor.Position,
                        Theme = new ThingButtonTheme()
                    };
                    tab.MouseButtonClicked += (s, a) => selectedTab = tabs.Key;
                    tab.MouseButtonClicked += (s, a) => DisplayActions(item, surface,newPosition, expilcitlook);
                    this.Controls.Add(tab);
                    Cursor.Right(tabs.Key.Length+1);
                }


            }
         

            if (expilcitlook)
            {
                var look = new Button(4, 1)
                {
                    Text = "look",
                    Position = Cursor.Position,
                    Theme = new ThingButtonTheme()
                };
                look.MouseButtonClicked += (s, a) => IsVisible = false;
                look.MouseButtonClicked += (s, a) => IsEnabled = false;
                look.MouseButtonClicked += (s, a) => GetDesc(id);
                this.Controls.Add(look);


            }
            Cursor.NewLine().Right(1);
            if (actionDatabase[id].ContainsKey(selectedTab))
            {
                foreach (string action in actionDatabase[id][selectedTab])
                {
                    bool argFlag = action.Contains("[whatever]");
                    string parsedAction = action.Replace(" [this]", "").Replace("[whatever]", "...").Replace("_", " ");
                    if (Cursor.Position.X + parsedAction.Length + 1 > Width)
                    {
                        Cursor.NewLine().Right(1);
                    }
                    Point pos = this.Cursor.Position;
                    var button = new Button(parsedAction.Length, 1)
                    {
                        Text = parsedAction,
                        Position = pos,
                        Theme = new ThingButtonTheme()
                    };
                    if (argFlag)
                    {
                        button.MouseButtonClicked += (s, a) => DoArgAction(id, action, thing);
                    }
                    else
                    {
                        button.MouseButtonClicked += (s, a) => DoAction(id, action);
                    }
                    this.Controls.Add(button);
                    this.Cursor.Right(parsedAction.Length + 1);

                }
            }
           
                /*
                var exit = new Button(1, 1)
                {
                    Text = "x",
                    Position = new Point(Width-1,0),
                    Theme = new ThingButtonTheme()
                };
                exit.MouseButtonClicked += (s, a) => IsVisible = false;
                exit.MouseButtonClicked += (s, a) => IsEnabled = false;
                this.Controls.Add(exit);
                 */

            
            this.IsVisible = true;
            this.IsEnabled = true;



        }

        public void DisplayMultiItem(string name, ICellSurface surface,Point? newPosition = null,List<string> IDs = null)
        {
            //if (awaitingItemClick) { return; }
            
            if (newPosition != null)
            {
                Position = (Point) newPosition - new Point(0, surface.ViewPosition.Y);
            }
            
            this.Clear();
            Controls.Clear();
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Red, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
            this.Print(1, 0, name);
            this.Cursor.Position = new Point(1, 1);
            Cursor.Print("Which one?").NewLine().Right(1);
            for (int i = 1; i < IDs.Count() + 1; i++)
            {



                if (Cursor.Position.X + 2 > Width)
                {
                    Cursor.NewLine().Right(1);
                }
                Point pos = this.Cursor.Position;
                string buttontext = i.ToString();
                if (buttontext[buttontext.Length - 1] == '1')
                {
                    buttontext = buttontext + "st";

                }
                else if (buttontext[buttontext.Length - 1] == '2')
                {
                    buttontext = buttontext + "nd";

                }
                else if (buttontext[buttontext.Length - 1] == '3')
                {
                    buttontext = buttontext + "rd";

                }
                else
                {
                    buttontext = buttontext + "th";

                }
                var button = new Button(buttontext.Length, 1) 
                {
                    Text = buttontext,
                    Position = pos,
                    Theme = new ThingButtonTheme()
                };

                int foo = i - 1;
                button.MouseEnter += (s, a) => DisplayActions(name + "(" + IDs[foo] + ")", null);
                button.MouseButtonClicked += (s, a) => ClickItem(IDs[foo],a);
                this.Controls.Add(button);
                this.Cursor.Right(buttontext.Length + 1);

            }
            /*
            var exit = new Button(1, 1)
            {
                Text = "x",
                Position = new Point(Width - 1, 0),
                Theme = new ThingButtonTheme()
            };
            exit.MouseButtonClicked += (s, a) => IsVisible = false;
            exit.MouseButtonClicked += (s, a) => IsEnabled = false;
            this.Controls.Add(exit);
            */

            this.IsVisible = true;
            this.IsEnabled = true;

            

        }

        private void DoAction(string id, string action)
        {
            //index++;///arrays starting at 1 momment
            Program.SendNetworkMessage(action + " " + id);

        }

        public static bool awaitingItemClick = false;
        static string PendingArgMessage = "";
        private static void DoArgAction(string id, string action,string name)
        {
            // index++;//arrays starting at 1 momment
            PendingArgMessage = action.Replace("[this]", id);
            awaitingItemClick = true;
            Program.PromptWindow.toptext = "Click a thing to complete";
            Program.PromptWindow.middletext = action.Replace("[this]", name).Replace("[whatever]", "...?");
            Program.PromptWindow.Type = PromptWindow.PopupType.Permanent;

            Program.PromptWindow.needsDraw = true;

        }


        public void ClickItem(string item, ControlBase.ControlMouseState mouse)
        {


            if (awaitingItemClick)
            {
                Program.PromptWindow.IsVisible = false;
                Program.SendNetworkMessage(PendingArgMessage.Replace("[whatever]",item));
                awaitingItemClick = false;
             
                return;
            }
            
            if (mouse.OriginalMouseState.Mouse.LeftClicked) {
                if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift)){
                    Program.SendNetworkMessage("lmbshift " + item);

                } else if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl)){
                    Program.SendNetworkMessage("lmbctrl " + item);
                } 
                else {
                    Program.SendNetworkMessage("lmb " + item);
                }
            }
            if (mouse.OriginalMouseState.Mouse.RightClicked)
            {
                if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift))
                {
                    Program.SendNetworkMessage("rmbshift " + item);

                }
                else if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl))
                {
                    Program.SendNetworkMessage("rmbctrl " + item);
                }
                else
                {
                    Program.SendNetworkMessage("rmb " + item);
                }

            }
        }

        public static void GetDesc(string id)
        {
            Program.SendNetworkMessage("look " + id);

        }


    }
}
