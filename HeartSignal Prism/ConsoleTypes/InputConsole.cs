﻿using System;
using SadConsole;
using Console = SadConsole.Console;
using SadRogue.Primitives;
using PrimS.Telnet;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;

namespace HeartSignal
{
    internal class InputConsole : Console
    {//ONLY EVER SHOULD EXIST AS A CHILD OF ANOTHER CONSOLE
        readonly KeyboardHandler keyboard;

        ITextInputReciver ReciverParent;
        List<string> commands = new List<string>();
        public InputConsole(int width, int height, ITextInputReciver parent) : base(width, height)
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
            ReciverParent = parent;
          //  TimesShiftedDown
     
            SadComponents.Add(keyboard);

            Cursor.Print(">");



        }
        protected override void OnMouseEnter(MouseScreenObjectState state)
        {
            IsFocused = true;


        }
        private void SendCommand() {

            string data = this.GetString(0,( Width*(Cursor.Position.Y+ 1))+Cursor.Position.X);

            ReciverParent.ReciveInput(data);

            commands.Add(data);
            commandindex = 0;
            Cursor.Position = new Point(0, 0);
            this.Clear();
            TimesShiftedUp = 0;
            Cursor.Print(">");
            


        }

        int commandindex;
        private void UpCommand() {

            
            if (commandindex+1 <= commands.Count) {
                commandindex++;
                this.Clear();
                Cursor.Position = new Point(0, 0);
                Cursor.Print(commands[commands.Count - commandindex]);
            }

        }



        private void DownCommand() {

            
            if (commandindex-1 > 0)
            {
                commandindex--;
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
