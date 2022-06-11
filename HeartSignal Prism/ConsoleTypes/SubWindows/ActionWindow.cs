using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.Input;
using Keys = SadConsole.Input.Keys;

namespace HeartSignal
{
    public class ActionWindow : SadConsole.UI.ControlsConsole
    {
        public ActionWindow(int width, int height, Point position) : base(width, height)
        {

            this.UsePrintProcessor = true;
            Cursor.UseStringParser = true;
            this.Position = position;
            IsVisible = false;
            IsEnabled = false;
         

        }
     

        public static Dictionary<string, ActionWindow> activeWindows = new Dictionary<string, ActionWindow>();
       // public static Dictionary<string, Dictionary<string, List<string>>> argactionDatabase = new Dictionary<string, Dictionary<string, List<string>>>();
        static string lastitem = "";
        public string selectedTab = "";
        protected override void OnMouseExit(MouseScreenObjectState state) {
            IsVisible = false;
            IsEnabled = false;
            lastitem = "";
            activeWindows.Remove(Utility.SplitThingId(lastitem)[1]);

        }

        private ICellSurface referenceSurface;


        //a lot fo reapeating code in here, integrate this better at some point
        public void ShowTooltip(string text,ICellSurface surface, Point? newPosition = null) {
            this.Resize(60, 12, 60, 12, false);
            AudioManager.ParseRequest(null, "play", "misc/tooltip.ogg");
            if (newPosition != null)
            {

                Position = (Point) newPosition - new Point(0, surface.ViewPosition.Y);
            }
            Position = new Point(Math.Clamp(Position.X,  0, Math.Max(0,surface.ViewWidth-this.Width-1)), Position.Y);
            

            this.Clear();
            Controls.Clear();

            this.Cursor.Position = new Point(0, 1);

            Cursor.Print(text);
            this.Surface.ShiftRight();
            this.Resize(Width, Cursor.Position.Y + 2, Width, Cursor.Position.Y + 2, false);
            var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(Color.Green, Color.Transparent));
            this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);

            this.IsVisible = true;
            this.IsEnabled = true;
        }

        private bool GexplicitLook;
        public void DisplayActions(string item = null,  ICellSurface surface = null,Point? newPosition = null, bool? _expilcitLook = null)
        {
            _expilcitLook = true;
            if (item == null)
            {
                item = lastitem;
            }

            if (surface == null)
            {
                surface = referenceSurface;
            }
            
            
            referenceSurface = surface;

            bool explicitlook = false;
            
            if (_expilcitLook == null)
            {
                explicitlook = GexplicitLook;
            }
            else
            {
                explicitlook = (bool) _expilcitLook;
                GexplicitLook = (bool) _expilcitLook;
            }
            
            //if (focusitem != null) { item = focusitem; }
            if (awaitingItemClick) { return; }

            this.Resize(40, 5, 40, 5, false);

            string[] returned = Utility.SplitThingId(item);
            string thing = returned[0];
            string id = returned[1];
            

            if (lastitem != item)
            {
                if (!explicitlook)
                {
                    ThingDatabase.GetData(id);
                }

                lastitem = item;
            }
            if (!ThingDatabase.thingDatabase.ContainsKey(id) || ThingDatabase.thingDatabase[id].name == "error" || ThingDatabase.thingDatabase[id].actionDatabase.Count == 0) {
                ThingDatabase.GetData(id);
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
            if (!ThingDatabase.thingDatabase[id].actionDatabase.ContainsKey(selectedTab))
			{
                var first = ThingDatabase.thingDatabase[id].actionDatabase.First();
                selectedTab = first.Key;


            }


            foreach (KeyValuePair<string, List<string>> tabs in ThingDatabase.thingDatabase[id].actionDatabase)
            {
                if (tabs.Key == "basic") continue;
                
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
                    tab.MouseButtonClicked += (s, a) => DisplayActions(item, surface,newPosition, explicitlook);
                    this.Controls.Add(tab);
                    Cursor.Right(tabs.Key.Length+1);
                }


            }
         

            if (explicitlook)
            {
                var look = new Button(4, 1)
                {
                    Text = "look",
                    Position = Cursor.Position,
                    Theme = new ThingButtonTheme()
                };
                look.MouseButtonClicked += (s, a) => IsVisible = false;
                look.MouseButtonClicked += (s, a) => IsEnabled = false;
                look.MouseButtonClicked += (s, a) => ThingDatabase.Examine(id);
                this.Controls.Add(look);


            }
            Cursor.NewLine().Right(1);
            if (ThingDatabase.thingDatabase[id].actionDatabase.ContainsKey(selectedTab))
            {
                foreach (string action in ThingDatabase.thingDatabase[id].actionDatabase[selectedTab])
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
                        button.MouseButtonClicked += (s, a) => DoArgAction(id, action);
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

            activeWindows.Remove(id);    
            activeWindows.Add(id,this);
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
            this.Print(1, 0, Utility.RemoveParserTags(name).CreateColored(Color.Gray));
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
                button.MouseButtonClicked += (s, a) => button.InvokeClick();
                button.Click += (s, a) => ClickItem(IDs[foo]);
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

        public void DoAction(string id, string action)
        {
            //index++;///arrays starting at 1 momment
            NetworkManager.SendNetworkMessage(action + " " + id);

        }

        public static bool awaitingItemClick = false;
        static string pendingArgMessage = "";
        private static void DoArgAction(string id, string action)
        {
            // index++;//arrays starting at 1 momment
            pendingArgMessage = action.Replace("[this]", id);
            awaitingItemClick = true;
            Program.PromptWindow.toptext = "Click a thing to complete";
            Program.PromptWindow.middletext = action.Replace("[this]", ThingDatabase.thingDatabase[id].name).Replace("[whatever]", "...?");
            Program.PromptWindow.Type = PromptWindow.PopupType.Permanent;

            Program.PromptWindow.needsDraw = true;

        }

        public bool clickedThisFrame = false;
        public void ClickItem(string item)
        {
            if(clickedThisFrame) return;
            clickedThisFrame = true;    //probably not the most elegant solution


            if (awaitingItemClick)
            {
                Program.PromptWindow.Close();
                NetworkManager.SendNetworkMessage(pendingArgMessage.Replace("[whatever]",item));
                awaitingItemClick = false;
             
                return;
            }
            
            if (Game.Instance.Mouse.LeftClicked) {
                if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift)){
                    NetworkManager.SendNetworkMessage("lmbshift " + item);

                } else if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl)){
                    NetworkManager.SendNetworkMessage("lmbctrl " + item);
                } 
                else {
                    NetworkManager.SendNetworkMessage("lmb " + item);
                }
            }
            if (Game.Instance.Mouse.RightClicked)
            {
                if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift))
                {
                    NetworkManager.SendNetworkMessage("rmbshift " + item);

                }
                else if (Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl))
                {
                    NetworkManager.SendNetworkMessage("rmbctrl " + item);
                }
                else
                {
                    NetworkManager.SendNetworkMessage("rmb " + item);
                }

            }
        }

        public override void Update(TimeSpan delta)
        {
            clickedThisFrame = false;
            base.Update(delta);
        }
    }
}
