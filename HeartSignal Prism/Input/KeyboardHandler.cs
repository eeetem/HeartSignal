using System;
using System.Drawing;
using System.Linq;
using SadConsole.Components;
using SadConsole.Input;
using SadConsole;
using Color = SadRogue.Primitives.Color;
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
           
            
            if(handled) return;


            // Upcast this because we know we're only using it with a Console type.
            Console console = (Console)consoleObject;

            // Check each key pressed.
            foreach (AsciiKey key in info.KeysPressed)
            {
                // If the character associated with the key pressed is a printable character, print it
                if (key.Character != '\0')
                {
                   
                    ColoredGlyph[] SurfaceArray = (ColoredGlyph[]) console.Surface.ToArray();
                    Array.Reverse(SurfaceArray);
                    for(int i = 1; i < SurfaceArray.Length; i++)
                    {
                        if (i == SurfaceArray.Length - (console.Cursor.Position.ToIndex(console.Surface.Width)))
                        {
                          //  SurfaceArray[i].Glyph = 32;
                            break;
                        }

                        SurfaceArray[i].CopyAppearanceTo(SurfaceArray[i - 1]);
                        //if (SurfaceArray[i + 1].Glyph != 0)
                        //{
                            
                       //}

                    }

                    Array.Reverse(SurfaceArray);
                    

                    console.Surface = new CellSurface(console.Surface.Width,console.Surface.Height,SurfaceArray );
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
                    
                    ColoredGlyph[] SurfaceArray = (ColoredGlyph[]) console.Surface.ToArray();
                   // Array.Reverse(SurfaceArray);
                    for(int i = console.Cursor.Position.ToIndex(console.Surface.Width)+1; i < SurfaceArray.Length; i++)
                    {

                        SurfaceArray[i].CopyAppearanceTo(SurfaceArray[i - 1]);
                        //if (SurfaceArray[i + 1].Glyph != 0)
                        //{
                            
                        //}

                    }

                  //  Array.Reverse(SurfaceArray);
                    

                    console.Surface = new CellSurface(console.Surface.Width,console.Surface.Height,SurfaceArray );
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
