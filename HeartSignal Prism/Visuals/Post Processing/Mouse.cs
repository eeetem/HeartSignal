using System;
using SadConsole.Host;
using SadRogue.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Color = SadRogue.Primitives.Color;


namespace HeartSignal
{
	public class CustomMouseFX: DrawableGameComponent
	{
		
		
		
		public CustomMouseFX() : base(SadConsole.Game.Instance.MonoGameInstance)
		{

			SadConsole.Game.Instance.MonoGameInstance.IsMouseVisible = false;//hide default mouse
			cursorTexture2D = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Texture2D>("cursor");

			DrawOrder = 6;

		}


		private Texture2D cursorTexture2D;
			

		//When we need to draw to the screen, it's done here.
		public override void Draw(GameTime gameTime)
		{
				Global.SharedSpriteBatch.Begin();
	

				Global.SharedSpriteBatch.Draw(Global.RenderOutput, Global.RenderOutput.Bounds, Microsoft.Xna.Framework.Color.White);
				Global.SharedSpriteBatch.Draw(cursorTexture2D,new Vector2(SadConsole.Game.Instance.Mouse.ScreenPosition.X,SadConsole.Game.Instance.Mouse.ScreenPosition.Y),Microsoft.Xna.Framework.Color.White);
				Global.SharedSpriteBatch.End();
	

		}
		
	}
}