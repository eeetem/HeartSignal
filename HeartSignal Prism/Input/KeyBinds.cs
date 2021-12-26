using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;

namespace HeartSignal
{
	public static class KeyBinds 
	{


		
		public static void Process(IScreenObject host, Keyboard keyboard, out bool handled)
		{
			handled = true;
			foreach (AsciiKey key in keyboard.KeysPressed)
			{

				switch (key.Key)
				{
					case(Keys.NumPad8):
						NetworkManager.SendNetworkMessage("north");
						return;
					case(Keys.NumPad7):
						NetworkManager.SendNetworkMessage("northwest");
						return;
					case(Keys.NumPad9):
						NetworkManager.SendNetworkMessage("northeast");
						return;
					case(Keys.NumPad4):
						NetworkManager.SendNetworkMessage("west");
						return;
					case(Keys.NumPad6):
						NetworkManager.SendNetworkMessage("east");
						return;
					case(Keys.NumPad2):
						NetworkManager.SendNetworkMessage("south");
						return;
					case(Keys.NumPad1):
						NetworkManager.SendNetworkMessage("southwest");
						return;
					case(Keys.NumPad3):
						return;

				}
			
			}

			handled = false; //if none of the keybinds triggered - let other keyboard components handle stuff
		}


	}
}