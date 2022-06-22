using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using SadConsole.Input;

namespace HeartSignal
{
    public class InputConsole : Console{
    readonly KeyboardHandler keyboard;


        public InputConsole(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Gray;
            DefaultForeground = Color.Black;
            
            Cursor.IsVisible = true;
            IsFocused = true;
            Cursor.PrintAppearanceMatchesHost = false;
            Cursor.PrintAppearance = new ColoredGlyph(Color.Black, Color.Gray);

           keyboard = new KeyboardHandler();
           keyboard.CursorLastY = Cursor.Position.Y;
          keyboard.EnterPressed += SendCommand;
            SadComponents.Add(keyboard);

            Cursor.Print(">");



        }
        protected override void OnMouseEnter(MouseScreenObjectState state)
        {
            IsFocused = true;


        }

        public void ClearInput()
        {
            Cursor.Position = new Point(0, 0);
            this.DefaultBackground = Color.Gray;
            DefaultForeground = Color.White;
            this.Clear();
            TimesShiftedUp = 0;
            Cursor.Print(">");
        }

        private void SendCommand() {

            string data = this.GetString(1,( Width*Cursor.Position.Y)+Cursor.Position.X);
            
#if RELEASE


            data = data.Replace("[", "");
            data = data.Replace("]", "");
            data = data.Replace("{", "");
            data = data.Replace("}", "");
            data = data.Replace("\n", "");
            data = data.Replace("\r", "");
            data = data.Replace("\t", "");
           data = data.Replace(">", "");
            data = data.Replace("<", "");
            data = data.Replace("+", "");
            data = data.Replace("-", "");
            data = data.Replace("\\", "");
            data = data.Replace("!/", "");
#endif



            ClearInput();
            

            
            




           // ReciveExternalInput("[c:r_f:darkgray]" + value);
           if (data[0] == '/')
           {
               NetworkManager.SendNetworkMessage(data.Trim().Remove(0,1));
           }
           else
           {
               NetworkManager.SendNetworkMessage("say "+data.Trim());
           }


        }



    }


}
