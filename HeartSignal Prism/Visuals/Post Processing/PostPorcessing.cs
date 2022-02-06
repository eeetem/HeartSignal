using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

			
			emptyTexture = new Texture2D(Global.SharedSpriteBatch.GraphicsDevice, 1, 1);

			DrawOrder = 6;
			//default effect parameters

			EffectParams["hardScan"] = -8.0f;
			EffectParams["hardPix"] = -1f;
			EffectParams["warpX"] = 0.039f;
			EffectParams["warpY"] = 0.049f;
			EffectParams["maskDark"] = 1.5f;
			EffectParams["maskLight"] = 1.5f;
			EffectParams["scaleInLinearGamma"] = 1.0f;
			EffectParams["shadowMask"] = 3.0f;
			EffectParams["brightboost"] = 1f;
			EffectParams["hardBloomScan"] = -2.0f;
			EffectParams["hardBloomPix"] = -4.0f;
			EffectParams["bloomAmount"] = 1.0f;
			EffectParams["shape"] = 1.0f;

			EffectParams["noise"] = 0.5f;

			
			
			//loose conection
			EffectParams["clmagnitude"] = 9f;
			EffectParams["clalpha"] = 0.01f;
			EffectParams["clspeed"] = 2;


		}

		private readonly Texture2D[] cursorTextures;
		private readonly Texture2D emptyTexture;
		private static Texture2D overlayTexture;

		private readonly SpriteBatch combineSpriteBatch;

		private readonly Effect crtEffect;
		private readonly Effect connectionEffect;

		private static readonly Dictionary<string, float> EffectParams = new Dictionary<string, float>();

		private static float counter = 0;
		private Random rnd = new Random();

		private static RenderTarget2D combinedRender;
		//When we need to draw to the screen, it's done here.
		public static void SetOverlay(string base64)
		{
			if (base64.Length < 1)
			{
				overlayTexture = null;
				return;
			}

			byte[] imageBytes = Convert.FromBase64String(base64);
			using (MemoryStream ms = new MemoryStream(imageBytes))
			{
				overlayTexture = Texture2D.FromStream(Global.SharedSpriteBatch.GraphicsDevice,ms);
			}
			
			
		}

		float GetNoise()
		{
			float noiseAmount = ((float) rnd.NextDouble() - (float) rnd.NextDouble()) * EffectParams["noise"];
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

			counter += 1f * EffectParams["clspeed"];
			if (combinedRender == null)
			{ 
				RemakeRenderTarget(); 
				return;
			}

			//init effects
			ProcessTweens(gameTime);

			

			crtEffect.Parameters["hardScan"]?.SetValue(EffectParams["hardScan"] + GetNoise());
			crtEffect.Parameters["hardPix"]?.SetValue(EffectParams["hardPix"] + GetNoise());
			crtEffect.Parameters["warpX"]?.SetValue(EffectParams["warpX"] + GetNoise() * 0.001f);
			crtEffect.Parameters["warpY"]?.SetValue(EffectParams["warpY"] + GetNoise() * 0.001f);
			crtEffect.Parameters["maskDark"]?.SetValue(EffectParams["maskDark"] + GetNoise() * 0.1f);
			crtEffect.Parameters["maskLight"]?.SetValue(EffectParams["maskLight"] + GetNoise() * 0.1f);
			crtEffect.Parameters["scaleInLinearGamma"]?.SetValue(EffectParams["scaleInLinearGamma"]);
			crtEffect.Parameters["shadowMask"]?.SetValue(EffectParams["shadowMask"] + GetNoise() * 1f);
			crtEffect.Parameters["brightboost"]?.SetValue(EffectParams["brightboost"] + GetNoise() * 0.05f);
			crtEffect.Parameters["hardBloomScan"]?.SetValue(EffectParams["hardBloomScan"] + GetNoise() * 0.01f);
			crtEffect.Parameters["hardBloomPix"]?.SetValue(EffectParams["hardBloomPix"] + GetNoise() * 0.01f);
			crtEffect.Parameters["bloomAmount"]?.SetValue(EffectParams["bloomAmount"] + GetNoise() * 0.05f);
			crtEffect.Parameters["shape"]?.SetValue(EffectParams["shape"] + GetNoise() * 0.05f);
			crtEffect.Parameters["textureSize"]
				.SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
			crtEffect.Parameters["videoSize"]
				.SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
			crtEffect.Parameters["outputSize"].SetValue(new Vector2(SadConsole.Settings.Rendering.RenderRect.Width,
				SadConsole.Settings.Rendering.RenderRect.Height));
				
			
			
			connectionEffect.Parameters["textureSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
			connectionEffect.Parameters["videoSize"].SetValue(new Vector2(Global.RenderOutput.Width, Global.RenderOutput.Height));
			connectionEffect.Parameters["fps"].SetValue(counter);
			connectionEffect.Parameters["staticAlpha"].SetValue(EffectParams["clalpha"]);
			connectionEffect.Parameters["magnitude"].SetValue(EffectParams["clmagnitude"]);
			if (overlayTexture != null)
			{
				connectionEffect.Parameters["overlay"].SetValue(overlayTexture);
			}
			else
			{
				connectionEffect.Parameters["overlay"].SetValue(emptyTexture);
			}

			


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
			Tween t = new Tween(parameter,EffectParams[parameter],speed,target);
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
				EffectParams[parameter] = Utility.Lerp(startValue, endValue, counter);

			}

		}

	}
}