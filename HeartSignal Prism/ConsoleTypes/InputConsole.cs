using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using PrimS.Telnet;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace HeartSignal
{
    internal class InputConsole : Console
    {//ONLY EVER SHOULD EXIST AS A CHILD OF ANOTHER CONSOLE
        readonly KeyboardHandler keyboard;
        ITextInputReciver ReciverParent;
        List<string> commands = new List<string>();
        public InputConsole(int width, int height,Console parent) : base(width, height)
        {
            DefaultBackground = Color.Gray;
            keyboard = new KeyboardHandler();
            Cursor.IsVisible = true;
            keyboard.CursorLastY = Cursor.Position.Y;
            IsFocused = true;
            UseKeyboard = true;
            keyboard.EnterPressed += SendCommand;
            keyboard.UpPressed += UpCommand;
            keyboard.DownPressed += DownCommand;
            this.
   
                ReciverParent = (ITextInputReciver)parent;
        
                
            SadComponents.Add(keyboard);
            Cursor.Print(">");



        }


        private void SendCommand() {
            int startingIndex = 0;
            string data = this.GetString(startingIndex, Cursor.Position.ToIndex(Width) - startingIndex);
            ReciverParent.ReciveInput(data);

            commands.Add(data);
            commandindex = 0;
            Cursor.Position = new Point(0, 0);
            this.Clear();
            Cursor.Print(">");


        }

        int commandindex;
        private void UpCommand() {

            commandindex++;
            if (commandindex <= commands.Count) { 
            this.Clear();
                Cursor.Position = new Point(0, 0);
                Cursor.Print(commands[commands.Count - commandindex]);
            }

        }



        private void DownCommand() {

            commandindex--;
            if (commandindex > 0)
            {
                this.Clear();
                Cursor.Position = new Point(0, 0);
                Cursor.Print(commands[commands.Count - commandindex]);
            }
            else{

                this.Clear();
                Cursor.Position = new Point(0, 0);
                Cursor.Print(">");

            }



        }


    }


}
