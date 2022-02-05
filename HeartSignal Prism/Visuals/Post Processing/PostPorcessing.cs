using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SadConsole.Host;
using SadRogue.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = SadRogue.Primitives.Color;
using Console = SadConsole.Console;
using Point = SadRogue.Primitives.Point;


namespace HeartSignal
{
	public class PostPorcessing : DrawableGameComponent
	{



		public PostPorcessing() : base(SadConsole.Game.Instance.MonoGameInstance)
		{
			


			crtEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("CRT");
			connectionEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("lc");
			SadConsole.Game.Instance.MonoGameInstance.IsMouseVisible = false; //hide default mouse
			int countx;
			int county;
			cursorTextures = Utility.SplitTexture(SadConsole.Game.Instance.MonoGameInstance.Content.Load<Texture2D>("eye"),40,40, out countx, out county);

			combineSpriteBatch = new SpriteBatch(Global.SharedSpriteBatch.GraphicsDevice);
			

			DrawOrder = 6;
			//default effect parameters

			effectParams["hardScan"] = -8.0f;
			effectParams["hardPix"] = -1f;
			effectParams["warpX"] = 0.039f;
			effectParams["warpY"] = 0.049f;
			effectParams["maskDark"] = 1.5f;
			effectParams["maskLight"] = 1.5f;
			effectParams["scaleInLinearGamma"] = 1.0f;
			effectParams["shadowMask"] = 3.0f;
			effectParams["brightboost"] = 1f;
			effectParams["hardBloomScan"] = -2.0f;
			effectParams["hardBloomPix"] = -4.0f;
			effectParams["bloomAmount"] = 1.0f;
			effectParams["shape"] = 1.0f;

			effectParams["noise"] = 0.5f;

			
			
			//loost conection
			effectParams["clmagnitude"] = 6f;
			

		}

		private Texture2D[] cursorTextures;

		private SpriteBatch combineSpriteBatch;

		private Effect crtEffect;
		private Effect connectionEffect;

		private static Dictionary<string, float> effectParams = new Dictionary<string, float>();

		private static float counter = 0;
		private Random rnd = new Random();

		private static RenderTarget2D combinedRender;
		//When we need to draw to the screen, it's done here.


		float GetNoise()
		{
			float noiseAmount = ((float) rnd.NextDouble() - (float) rnd.NextDouble()) * effectParams["noise"];
			return noiseAmount;
		}

		public static void RemakeRenderTarget()
		{
			if (Global.RenderOutput != null)
			{
				combinedRender?.Dispose();
				combinedRender = new RenderTarget2D(Global.SharedSpriteBatch.GraphicsDevice,
					Global.RenderOutput.Width, Global.RenderOutput.Height);
			}
		}

		private float mouseInactivtyCounter = 0;
		private Point oldMousePos;
		public override void Draw(GameTime gameTime)
		{
			
				counter += 1f;
				if (combinedRender == null)
				{
					RemakeRenderTarget();
					return;
				}

				//init effects
				ProcessTweens(gameTime);


				crtEffect.Parameters["hardScan"]?.SetValue(effectParams["hardScan"] + GetNoise());
				crtEffect.Parameters["hardPix"]?.SetValue(effectParams["hardPix"] + GetNoise());
				crtEffect.Parameters["warpX"]?.SetValue(effectParams["warpX"] + GetNoise() * 0.001f);
				crtEffect.Parameters["warpY"]?.SetValue(effectParams["warpY"] + GetNoise() * 0.001f);
				crtEffect.Parameters["maskDark"]?.SetValue(effectParams["maskDark"] + GetNoise() * 0.1f);
				crtEffect.Parameters["maskLight"]?.SetValue(effectParams["maskLight"] + GetNoise() * 0.1f);
				crtEffect.Parameters["scaleInLinearGamma"]?.SetValue(effectParams["scaleInLinearGamma"]);
				crtEffect.Parameters["shadowMask"]?.SetValue(effectParams["shadowMask"] + GetNoise() * 1f);
				crtEffect.Parameters["brightboost"]?.SetValue(effectParams["brightboost"] + GetNoise() * 0.05f);
				crtEffect.Parameters["hardBloomScan"]?.SetValue(effectParams["hardBloomScan"] + GetNoise() * 0.01f);
				crtEffect.Parameters["hardBloomPix"]?.SetValue(effectParams["hardBloomPix"] + GetNoise() * 0.01f);
				crtEffect.Parameters["bloomAmount"]?.SetValue(effectParams["bloomAmount"] + GetNoise() * 0.05f);
				crtEffect.Parameters["shape"]?.SetValue(effectParams["shape"] + GetNoise() * 0.05f);
				crtEffect.Parameters["textureSize"]
					.SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				crtEffect.Parameters["videoSize"]
					.SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				crtEffect.Parameters["outputSize"].SetValue(new Vector2(SadConsole.Settings.Rendering.RenderRect.Width,
					SadConsole.Settings.Rendering.RenderRect.Height));
				
			//	connectionEffect.Parameters["textureSize"]
			//		.SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				connectionEffect.Parameters["videoSize"]
					.SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
				connectionEffect.Parameters["fps"].SetValue(counter);
				connectionEffect.Parameters["magnitude"].SetValue(effectParams["clmagnitude"]);

				if(oldMousePos ==SadConsole.Game.Instance.Mouse.ScreenPosition){
				
					mouseInactivtyCounter += gameTime.ElapsedGameTime.Milliseconds;
				
				}
				else
				{
					mouseInactivtyCounter = 0;
				}

				oldMousePos = SadConsole.Game.Instance.Mouse.ScreenPosition;

				Texture2D cursorTexture2D;
				if (mouseInactivtyCounter > 2000)
				{
					cursorTexture2D = cursorTextures[2];
				}else if (mouseInactivtyCounter > 1000)
				{
					cursorTexture2D = cursorTextures[1];
				}
				else
				{
				
					cursorTexture2D = cursorTextures[0];
		
				}


				float fade = Math.Clamp((3000 - (mouseInactivtyCounter-2000)) / 3000 * 1, 0, 1);		

				
				
				combineSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender);

				combineSpriteBatch.Begin(blendState: BlendState.AlphaBlend);

				combineSpriteBatch.Draw(Global.RenderOutput, Global.RenderOutput.Bounds,
					Microsoft.Xna.Framework.Color.White);

				combineSpriteBatch.Draw(cursorTexture2D,
					new Vector2(SadConsole.Game.Instance.Mouse.ScreenPosition.X - cursorTexture2D.Width / 2,
						SadConsole.Game.Instance.Mouse.ScreenPosition.Y - cursorTexture2D.Height / 2),
					new Microsoft.Xna.Framework.Color(fade,fade,fade,fade));
//)
				combineSpriteBatch.End();
				//combineSpriteBatch.GraphicsDevice.SetRenderTarget(null);
				//combineSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender);

				combineSpriteBatch.Begin(SpriteSortMode.Immediate);

				
				connectionEffect.CurrentTechnique.Passes[0].Apply();
				combineSpriteBatch.Draw(combinedRender, combinedRender.Bounds,
					Microsoft.Xna.Framework.Color.White);
				combineSpriteBatch.End();
				combineSpriteBatch.GraphicsDevice.SetRenderTarget(null);


				
				
				
				Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate);

				crtEffect.CurrentTechnique.Passes[0].Apply();
				

				Global.SharedSpriteBatch.Draw(combinedRender, combinedRender.Bounds,
					Microsoft.Xna.Framework.Color.White);

				Global.SharedSpriteBatch.End();
	

		}

		public static void AddTween(string parameter,float target, float speed)
		{
			Tween t = new Tween(parameter,effectParams[parameter],speed,target);
			tweens.Add(t);
			

		}

		//might be better to make this a dict and only allow 1 tween per parameter
		private static List<Tween> tweens = new List<Tween>();
		private void ProcessTweens(GameTime gameTime)
		{
			foreach (Tween t in tweens.ToList())
			{
				if (t.counter > 1)
				{
					tweens.Remove(t);
					continue;
				}
				t.Lerp(gameTime);
			}
		}

		public class Tween
		{
			public Tween(string parameter, float start, float speed, float endValue)
			{
				this.parameter = parameter;
				this.speed = speed;
				this.endValue = endValue;
				this.startValue = start;
				
			}

			public float counter = 0;
			public string parameter;
			public float startValue;
			public float endValue;
			public float speed;
	

			public void Lerp(GameTime gameTime)
			{
				
				counter += ((float)gameTime.ElapsedGameTime.Milliseconds / 10000) * speed;
				effectParams[parameter] = Utility.Lerp(startValue, endValue, counter);

			}

		}

	}
}