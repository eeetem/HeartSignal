using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;

namespace HeartSignal
{
	public class KeyBinds : LogicComponent
	{

		public override void Render(IScreenObject host, TimeSpan delta)
		{

		}

		public override void Update(IScreenObject host, TimeSpan delta)
		{
			foreach (AsciiKey key in SadConsole.Game.Instance.Keyboard.KeysPressed)
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
						NetworkManager.SendNetworkMessage("southeast");
						return;
					case(Keys.Enter):
						Program.MainConsole.GetInputSource().IsFocused = true;
						return;

				}
			
			}
		}
	}
}