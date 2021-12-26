using SadConsole.Components;
using SadConsole.Input;
using SadConsole;
using Console = SadConsole.Console;
using Keys = SadConsole.Input.Keys;
//using System.Windows.Forms;

namespace HeartSignal
{
    class KeyboardHandler : KeyboardConsoleComponent
    {



        // this is a callback for the owner of this keyboard handler. It is called when the user presses ENTER.
        public delegate void KeyDelegate();
        public event KeyDelegate EnterPressed;
        public event KeyDelegate UpPressed;
        public event KeyDelegate DownPressed;
        //  public event KeyDelegate BackPressed;

        public int CursorLastY = 0;
        public override void OnAdded(IScreenObject host)
        {
            //  var console = (Console)host;
            host.UseKeyboard = true;
        }
        
        public override void ProcessKeyboard(IScreenObject consoleObject, Keyboard info, out bool handled)
        {
            handled = false;
            KeyBinds.Process(consoleObject,info, out handled);
            
            if(handled) return;


            // Upcast this because we know we're only using it with a Console type.
            Console console = (Console)consoleObject;

            // Check each key pressed.
            foreach (AsciiKey key in info.KeysPressed)
            {
                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                {
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
                else if (key.Key == Keys.Left)
                {


                    // Do not let them backspace into the prompt
                    if (console.Cursor.Position.Y != CursorLastY || console.Cursor.Position.X > 1)
                    {
                        console.Cursor.LeftWrap(1);
                    }
                }
                else if (key.Key == Keys.Right)
                {


                    
                        console.Cursor.RightWrap(1);
                    
                }

                // Special character - ENTER
                else if (key.Key == Keys.Enter)
                {
                    EnterPressed?.Invoke();
                }
                else if (key.Key == Keys.Up)
                {
                    UpPressed?.Invoke();
                }
                else if (key.Key == Keys.Down)
                {
                    DownPressed?.Invoke();
                }
            }

            handled = true;
        }

    }
}
