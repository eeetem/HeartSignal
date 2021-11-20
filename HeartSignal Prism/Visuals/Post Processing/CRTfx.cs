using System;
using SadConsole.Host;
using SadRogue.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Color = SadRogue.Primitives.Color;


namespace HeartSignal
{
	public class CRTfx: DrawableGameComponent
	{
		
		
		
		public CRTfx() : base(SadConsole.Game.Instance.MonoGameInstance)
		{


			crtEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("CRT");


			DrawOrder = 6;

			gradient = new ColorGradient(Color.Red, Color.Black, Color.Blue, Color.Black, Color.Green, Color.Black,Color.Red);


		}

		private Effect crtEffect;

		private ColorGradient gradient;

		private float counter;
		//When we need to draw to the screen, it's done here.
		public override void Draw(GameTime gameTime)
		{


			counter += (float) gameTime.ElapsedGameTime.Milliseconds / 5000;
			if (counter > 1)
				counter = 0;
			Color c = gradient.Lerp(counter);
			crtEffect.Parameters["tint"].SetValue(new Vector4((float)c.R/255,(float)c.B/255,(float)c.G/255,1));
			
			Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);


			crtEffect.CurrentTechnique.Passes[0].Apply();

			Global.SharedSpriteBatch.Draw(Global.RenderOutput, Global.RenderOutput.Bounds, Microsoft.Xna.Framework.Color.White);
			Global.SharedSpriteBatch.End();

		}
		
	}
}