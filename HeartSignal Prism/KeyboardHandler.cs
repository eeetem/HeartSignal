using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;
using Keys = SadConsole.Input.Keys;
using System.Text;
using System.Windows.Forms;

namespace HeartSignal
{
    class KeyboardHandler : KeyboardConsoleComponent
    {


        // this is a callback for the owner of this keyboard handler. It is called when the user presses ENTER.
        public delegate void KeyDelegate();
        public event KeyDelegate EnterPressed;
      //  public event KeyDelegate BackPressed;

        public int CursorLastY = 0;
        public override void OnAdded(IScreenObject host)
        {
            var console = (Console)host;

        }

        public override void ProcessKeyboard(IScreenObject consoleObject, Keyboard info, out bool handled)
        {
            // Upcast this because we know we're only using it with a Console type.
            Console console = (Console)consoleObject;

            // Check each key pressed.
            foreach (AsciiKey key in info.KeysPressed)
            {
                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                {

                    if (key.Character == 'v' && info.IsKeyDown(Keys.LeftControl)) {

                        
                        console.Cursor.Print(Clipboard.GetText());

                    }
                    
                        
                    
                    
                    console.Cursor.Print(key.Character.ToString());
                }

                // Special character - BACKSPACE
                else if (key.Key == Keys.Back)
                {
                    

                    // Do not let them backspace into the prompt
                    if (console.Cursor.Position.Y != CursorLastY || console.Cursor.Position.X > 1)
                    {
                        console.Cursor.LeftWrap(1).Print(" ").LeftWrap(1);
                    }
                }

                // Special character - ENTER
                else if (key.Key == Keys.Enter)
                {
                    EnterPressed();
                }
            }

            handled = true;
        }

    }
}
