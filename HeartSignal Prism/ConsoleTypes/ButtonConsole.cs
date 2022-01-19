using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;
using System.IO;
using SadConsole.UI;
using SadConsole.UI.Themes;

namespace HeartSignal
{
	class ButtonConsole : ControlsConsole
	{
		public ButtonConsole(int width, int height) : base(width, height)
		{


			SadComponents.Add(new AnimatedBorderComponent());
			

		}

		public void MakeButtons(List<String> args)
		{

			verbs = args;
			this.Controls.Clear();
			this.Clear();

			int xpos = 1;
			foreach (var verb in verbs)
			{
				var button = new Button(verb.Length,3)
				{
					Text = verb.Replace("_"," ").ToUpper(),
					Position = new Point(xpos , Height/2 -1),
					Theme = new Button3dTheme()
					
				};
				button.MouseButtonClicked += (s, a) => NetworkManager.SendNetworkMessage(verb);
				button.MouseButtonClicked +=  (s, a) => AudioManager.ParseRequest(null, "play", "misc/buttonclick.ogg");
				button.MouseEnter +=  (s, a) => AudioManager.ParseRequest(null, "play", "misc/buttonhover.ogg");
				this.Controls.Add(button);
				xpos = xpos + 1 + verb.Length;

			}
		}

		public List<String> verbs = new List<string>();
	


	}
}