using System;
using SadConsole.Host;
using SadRogue.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Color = SadRogue.Primitives.Color;


namespace HeartSignal
{
	public class PostPorcessing: DrawableGameComponent
	{
		
		
		
		public PostPorcessing() : base(SadConsole.Game.Instance.MonoGameInstance)
		{


			crtEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("CRT");
			SadConsole.Game.Instance.MonoGameInstance.IsMouseVisible = false;//hide default mouse
			cursorTexture2D = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Texture2D>("cursor");

			combineSpriteBatch = new SpriteBatch(Global.SharedSpriteBatch.GraphicsDevice);


			DrawOrder = 6;
			

		}
		
		private Texture2D cursorTexture2D;

		private SpriteBatch combineSpriteBatch;
		
		private Effect crtEffect;

		//When we need to draw to the screen, it's done here.
		public override void Draw(GameTime gameTime)
		{
			try
			{
				///init effects
				crtEffect.Parameters["hardScan"]?.SetValue(-8.0f);
				crtEffect.Parameters["hardPix"]?.SetValue(-3.0f);
				crtEffect.Parameters["warpX"]?.SetValue(0.031f);
				crtEffect.Parameters["warpY"]?.SetValue(0.041f);
				crtEffect.Parameters["maskDark"]?.SetValue(0.5f);
				crtEffect.Parameters["maskLight"]?.SetValue(1.5f);
				crtEffect.Parameters["scaleInLinearGamma"]?.SetValue(1.0f);
				crtEffect.Parameters["shadowMask"]?.SetValue(3.0f);
				crtEffect.Parameters["brightboost"]?.SetValue(1.0f);
				crtEffect.Parameters["hardBloomScan"]?.SetValue(-1.5f);
				crtEffect.Parameters["hardBloomPix"]?.SetValue(-2.0f);
				crtEffect.Parameters["bloomAmount"]?.SetValue(0.15f);
				crtEffect.Parameters["shape"]?.SetValue(1.0f);
				crtEffect.Parameters["textureSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				crtEffect.Parameters["videoSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				crtEffect.Parameters["outputSize"].SetValue(new Vector2(SadConsole.Settings.Rendering.RenderRect.Width, SadConsole.Settings.Rendering.RenderRect.Height));


				RenderTarget2D combinedRender = new RenderTarget2D(Global.SharedSpriteBatch.GraphicsDevice,Global.RenderOutput.Width,Global.RenderOutput.Height);
				
				
				combineSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender);
				
				combineSpriteBatch.Begin();
				
				combineSpriteBatch.Draw(Global.RenderOutput, Global.RenderOutput.Bounds, Microsoft.Xna.Framework.Color.White);
				
				combineSpriteBatch.Draw(cursorTexture2D, new Vector2(SadConsole.Game.Instance.Mouse.ScreenPosition.X - cursorTexture2D.Width/2,SadConsole.Game.Instance.Mouse.ScreenPosition.Y - cursorTexture2D.Height/2), Microsoft.Xna.Framework.Color.White);
				
				combineSpriteBatch.End();
				combineSpriteBatch.GraphicsDevice.SetRenderTarget(null);

				Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

				crtEffect.CurrentTechnique.Passes[0].Apply();


				Global.SharedSpriteBatch.Draw(combinedRender,combinedRender.Bounds, Microsoft.Xna.Framework.Color.White);

				Global.SharedSpriteBatch.End();
			}
			finally
			{
				
			}

		}

	}




}