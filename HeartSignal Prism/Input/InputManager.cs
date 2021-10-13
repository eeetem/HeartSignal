using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HeartSignal
{
	static class InputManager
	{
		private static KeyboardState keyboardState, lastKeyboardState;
		//private static MouseState mouseState, lastMouseState;

		public static KeyboardState GetKeyboard()
		{
			lastKeyboardState = keyboardState;
			//lastMouseState = mouseState;


			keyboardState = Keyboard.GetState();
			//	mouseState = Mouse.GetState();
			return keyboardState;
		}
	}
}
