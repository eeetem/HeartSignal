using System;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;

namespace HeartSignal
{
	public class KeyBinds : IComponent
	{
		public KeyBinds()
		{
			SortOrder = 0;
			IsUpdate = true;
			IsRender = false;
			IsMouse = false;
			IsKeyboard = false;
		}


		public void Update(IScreenObject host, TimeSpan delta)
		{

			Keyboard keyboard = Game.Instance.Keyboard;


			foreach (AsciiKey key in keyboard.KeysPressed)
			{

				switch (key.Key)
				{
					case(Keys.NumPad8):
						NetworkManager.SendNetworkMessage("north");
						continue;
					case(Keys.NumPad7):
						NetworkManager.SendNetworkMessage("northwest");
						continue;
					case(Keys.NumPad9):
						NetworkManager.SendNetworkMessage("northeast");
						continue;
					case(Keys.NumPad4):
						NetworkManager.SendNetworkMessage("west");
						continue;
					case(Keys.NumPad6):
						NetworkManager.SendNetworkMessage("east");
						continue;
					case(Keys.NumPad2):
						NetworkManager.SendNetworkMessage("south");
						continue;
					case(Keys.NumPad1):
						NetworkManager.SendNetworkMessage("southwest");
						continue;
					case(Keys.NumPad3):
						NetworkManager.SendNetworkMessage("southeast");
						continue;

				}
			
			}
		}

		
		public void Render(IScreenObject host, TimeSpan delta)
		{
			throw new NotImplementedException();
		}

		public void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
		{
			throw new NotImplementedException();
		}

		public void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
		{
			throw new NotImplementedException();
		}

		public void OnAdded(IScreenObject host)
		{
			return;
		}

		public void OnRemoved(IScreenObject host)
		{
			return;
		}

		public uint SortOrder { get; }
		public bool IsUpdate { get; }
		public bool IsRender { get; }
		public bool IsMouse { get; }
		public bool IsKeyboard { get; }
	}
}