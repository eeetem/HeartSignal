using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using System.Collections.Generic;
using SadConsole.Input;

namespace HeartSignal
{
    public class InputConsole : Console
    {//ONLY EVER SHOULD EXIST AS A CHILD OF ANOTHER CONSOLE
        readonly KeyboardHandler keyboard;


        public InputConsole(int width, int height) : base(width, height)
        {
            DefaultBackground = Color.Gray;
            keyboard = new KeyboardHandler();
            Cursor.IsVisible = true;
            keyboard.CursorLastY = Cursor.Position.Y;
            IsFocused = true;
            UseKeyboard = true;
            keyboard.EnterPressed += SendCommand;

     
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
            data = data.Replace(">", "");
            data = data.Replace("<", "");
#endif
            
            
            
            
            
            Cursor.Position = new Point(0, 0);
            this.Clear();
            TimesShiftedUp = 0;
            Cursor.Print(">");
            
            




           // ReciveExternalInput("[c:r_f:darkgray]" + value);

            NetworkManager.SendNetworkMessage("say "+data);


        }



    }


}
