using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using System.Collections.Generic;
using ImageProcessor.Processors;
using SadConsole.Input;

namespace HeartSignal
{
    public class InputConsole : Console{
    readonly KeyboardHandler keyboard;


        public InputConsole(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Gray;
            DefaultForeground = Color.White;
            keyboard = new KeyboardHandler();
            Cursor.IsVisible = true;
            keyboard.CursorLastY = Cursor.Position.Y;
            IsFocused = true;
            UseKeyboard = true;
            keyboard.EnterPressed += SendCommand;
            Cursor.PrintAppearanceMatchesHost = false;
            Cursor.PrintAppearance = new ColoredGlyph(Color.White, Color.Gray);

     
            SadComponents.Add(keyboard);

            Cursor.Print(">");



        }
        protected override void OnMouseEnter(MouseScreenObjectState state)
        {
            IsFocused = true;


        }
        private void SendCommand() {

            string data = this.GetString(0,( Width*Cursor.Position.Y)+Cursor.Position.X);
            
#if RELEASE


            data = data.Replace("[", "");
            data = data.Replace("]", "");
            data = data.Replace("{", "");
            data = data.Replace("}", "");
            data = data.Replace(Environment.NewLine, "");
            data = data.Replace("\n", "");
            data = data.Replace("\r", "");
            data = data.Replace("\t", "");
            data = data.Replace(">", "");
            data = data.Replace("<", "");
            data = data.Replace("+", "");
            data = data.Replace("-", "");
#endif
            
            
            
            
            
            Cursor.Position = new Point(0, 0);
            this.DefaultBackground = Color.Gray;
            DefaultForeground = Color.White;
            this.Clear();
            TimesShiftedUp = 0;
            Cursor.Print(">");
            
            




           // ReciveExternalInput("[c:r_f:darkgray]" + value);

            NetworkManager.SendNetworkMessage("say "+data.Trim());


        }



    }


}
