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

		}

		private Effect crtEffect;

		//When we need to draw to the screen, it's done here.
		public override void Draw(GameTime gameTime)
		{
			try
			{
				crtEffect.Parameters["hardScan"]?.SetValue(-20f);
				crtEffect.Parameters["hardPix"]?.SetValue(-20f);
				crtEffect.Parameters["warpX"]?.SetValue(0.5f);
				crtEffect.Parameters["warpY"]?.SetValue(0.5f);
				crtEffect.Parameters["maskDark"]?.SetValue(2f);
				crtEffect.Parameters["maskLight"]?.SetValue(1.5f);
				crtEffect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
				crtEffect.Parameters["shadowMask"]?.SetValue(1.0f);
				crtEffect.Parameters["brightboost"]?.SetValue(2.0f);
				crtEffect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
				crtEffect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
				crtEffect.Parameters["bloomAmount"]?.SetValue(0.15f);
				crtEffect.Parameters["shape"]?.SetValue(1.0f);
				crtEffect.Parameters["textureSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				crtEffect.Parameters["videoSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				crtEffect.Parameters["outputSize"].SetValue(new Vector2(SadConsole.Settings.Rendering.RenderRect.Width, SadConsole.Settings.Rendering.RenderRect.Height));

				Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);


				crtEffect.CurrentTechnique.Passes[0].Apply();

				Global.SharedSpriteBatch.Draw(Global.RenderOutput, Global.RenderOutput.Bounds, Microsoft.Xna.Framework.Color.White);
				Global.SharedSpriteBatch.End();
			}
			finally
			{
				
			}

		}
		
	}
}