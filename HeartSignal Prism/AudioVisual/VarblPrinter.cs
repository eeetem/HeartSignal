using System;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;

namespace HeartSignal
{
	public class VarblPrinterTheme : ThemeBase
	{
		private string varkey = "";
		public VarblPrinterTheme( string varkey)
		{
			this.varkey = varkey;
		}


		public override void UpdateAndDraw(ControlBase control, TimeSpan time)
		{
			Label L = control as Label;
			L.Surface.Clear();
			if (control.Width != Utility.GetVar(varkey).Length)
			{
				L.Resize(Utility.GetVar(varkey).Length, 1);
				BaseConsole dc = control.Parent.Host.ParentConsole as BaseConsole;
				if (dc != null)
				{
					dc.ReDraw();
				}
			}

			L.Surface.Print(0,0,Utility.GetVar(varkey));

		}

		public override ThemeBase Clone()
		{
			throw new NotImplementedException();
		}
	}
}