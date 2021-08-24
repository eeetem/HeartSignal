using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using PrimS.Telnet;
using System.Threading.Tasks;

namespace HeartSignal
{
    internal class InputConsole : Console
    {//ONLY EVER SHOULD EXIST AS A CHILD OF ANOTHER CONSOLE
        readonly KeyboardHandler keyboard;
        ITextInputReciver ReciverParent;
        public InputConsole(int width, int height,Console parent) : base(width, height)
        {
            DefaultBackground = Color.Gray;
            keyboard = new KeyboardHandler();
            Cursor.IsVisible = true;
            keyboard.CursorLastY = Cursor.Position.Y;
            IsFocused = true;
            UseKeyboard = true;
            keyboard.EnterPressed += SendCommand;
            this.
   
                ReciverParent = (ITextInputReciver)parent;
        
                
            SadComponents.Add(keyboard);
            Cursor.Print(">");



        }


        private void SendCommand() {
            int startingIndex = 0;
            string data = this.GetString(startingIndex, Cursor.Position.ToIndex(Width) - startingIndex);
            ReciverParent.ReciveInput(data);
            Cursor.Position = new Point(0, 0);
            this.Clear();
            Cursor.Print(">");


        }

    }


}
