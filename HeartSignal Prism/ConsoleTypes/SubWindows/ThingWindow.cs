using System;
using System.Collections.Generic;
using System.Drawing;
using SadConsole;
using SadConsole.Quick;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;
using Console = SadConsole.Console;
using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace HeartSignal
{
	public class ThingWindow : SadConsole.UI.ControlsConsole
	{
		private ActionWindow _actionWindow;
		private string _id;
		private DisplayConsole parent;
		private static readonly object T = new object();

		private Button interactibleThingId;
		public ThingWindow(DisplayConsole parent, string ID) : base(34, 100)
		{
			this.parent = parent;
			this.UsePrintProcessor = true;
			Cursor.UseStringParser = true;
			parent.Children.Add(this);

			
			_actionWindow = new ActionWindow(30,5,new Point(0,0));
			Children.Add(_actionWindow);
			_id = ID;
			lock (ThingDatabase.DatabaseSyncObj)
			{
				if (!ThingDatabase.thingDatabase.ContainsKey(_id))
				{
					ThingDatabase.ThingData th = new ThingDatabase.ThingData("error","Loading...");
					ThingDatabase.GetData(_id);
					ThingDatabase.thingDatabase.Add(_id, th);
				}
				//todo possibly move the update event into display console
				var thingData = ThingDatabase.thingDatabase[_id];
				thingData.updateEvent += Draw;
				ThingDatabase.thingDatabase.Remove(_id);
				ThingDatabase.thingDatabase.Add(_id, thingData);
			}
			this.MouseButtonClicked += (s, a) => interactibleThingId?.InvokeClick();

			Draw();
		}
		public void Draw() {
			this.Resize(33, 7, true);
			this.Clear();
			Controls.Clear();
			var thingData =ThingDatabase.thingDatabase[_id];
			
		
	
			this.Cursor.Position = new Point(0, 1);
		
			Utility.PrintParseMessage( thingData.desc.Replace("[nl]","\r\n"),_actionWindow,this,true);


		
			this.Resize(35, (int) MathF.Min(Cursor.Position.Y + 2,7), false);
			foreach (var control in Controls)
			{
				control.Position = new Point(control.Position.X + 1, control.Position.Y);
			}
			this.Surface.ShiftRight();
			var boxShape = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin, new ColoredGlyph(AnimatedBorderComponent._borderCellStyle.Foreground, Color.Transparent));
			this.DrawBox(new Rectangle(0, 0, Width, Height), boxShape);
			Cursor.Position = new Point(1,0);
			if (thingData.actionDatabase.ContainsKey("basic"))
			{
				List<string> basicActionList = new List<string>(thingData.actionDatabase["basic"]);
				foreach (var act in basicActionList)
				{
					var action = new Button(act.Length, 1)
					{
						Text = act,
						Position = Cursor.Position,
						Theme = new ThingButtonTheme(new Gradient(Color.Black, Color.White, Color.Black))
					};
					action.MouseButtonClicked += (s, a) => _actionWindow.DoAction(_id, act);
					this.Controls.Add(action);
					Cursor.Right(act.Length + 1);
				}
			}

			var examineAction = new Button("examine".Length, 1)
			{
				Text = "examine",
				Position = Cursor.Position,
				Theme = new ThingButtonTheme(new Gradient(Color.Black,Color.White,Color.Black))
			};
			examineAction.MouseButtonClicked += (s, a) => ThingDatabase.Examine(_id);
			
			this.Controls.Add(examineAction);
			Cursor.Right("examine".Length+1);

			Cursor.Position = new Point(Width - 8, Height-1);
			interactibleThingId = Utility.CreateButtonThingId(new[]{"interact",_id},this,_actionWindow,true);
			
			
			
			this.IsVisible = true;
			this.IsEnabled = true;
			
			parent.WindowSort();//this gets run too many times, optimise perhaps
		}

		protected override void Dispose(bool disposing)
		{
			_actionWindow.Dispose();
			base.Dispose(disposing);
		}
	}
}