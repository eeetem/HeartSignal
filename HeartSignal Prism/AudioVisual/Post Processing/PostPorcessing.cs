﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SadConsole.Host;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Point = SadRogue.Primitives.Point;
using SadRogue.Primitives;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadConsole.Input;

namespace HeartSignal
{
	public class PostPorcessing : DrawableGameComponent
	{



		public PostPorcessing() : base(SadConsole.Game.Instance.MonoGameInstance)
		{
			


			crtEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("CRT");
			connectionEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("lc");
			colorEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("colorshader");
			distortEffect = SadConsole.Game.Instance.MonoGameInstance.Content.Load<Effect>("distort");
			SadConsole.Game.Instance.MonoGameInstance.IsMouseVisible = false; //hide default mouse
			int countx;
			int county;
			cursorTextures = Utility.SplitTexture(SadConsole.Game.Instance.MonoGameInstance.Content.Load<Texture2D>("eye"),40,40, out countx, out county);

			
			combinedSpriteBatch = new SpriteBatch(Global.SharedSpriteBatch.GraphicsDevice);

			
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
			EffectParams["clmagnitude"] = 5f;
			EffectParams["clalpha"] = 0.05f;
			EffectParams["clspeed"] = 1f;
			EffectParams["overlayalpha"] = 0.5f;
			
			
			
			//colorshader
			EffectParams["minR"] = 0f;
			EffectParams["minG"] = 0f;
			EffectParams["minB"] = 0f;
			EffectParams["maxR"] = 1f;
			EffectParams["maxG"] = 1f;
			EffectParams["maxB"] = 1f;
			EffectParams["tintR"] = 1f;
			EffectParams["tintG"] = 1f;
			EffectParams["tintB"] = 1f;
			
			
			//distort
			EffectParams["dxspeed"] = 0f;
			EffectParams["dxamplitude"] = 0f;
			EffectParams["dxfrequency"] = 0f;
			
			EffectParams["dyspeed"] = 0f;
			EffectParams["dyamplitude"] = 0f;
			EffectParams["dyfrequency"] = 0f;

		}

		private readonly Texture2D[] cursorTextures;
		private readonly Texture2D emptyTexture;
		private static Texture2D overlayTexture;

		private readonly SpriteBatch combinedSpriteBatch;

		private readonly Effect crtEffect;
		private readonly Effect connectionEffect;
		private readonly Effect colorEffect;
		private readonly Effect distortEffect;

		private static readonly Dictionary<string, float> EffectParams = new Dictionary<string, float>();

		private static float clcounter = 0;
		private static float dxcounter = 0;
		private static float dycounter = 0;
		private Random rnd = new Random();

		private static RenderTarget2D combinedRender;
		private static RenderTarget2D combinedRender2;
		//When we need to draw to the screen, it's done here.


		public static bool OverlayChangeInProgress = false;
		public static void SetOverlay(string link)
		{
			while (OverlayChangeInProgress)
			{

				System.Threading.Thread.Sleep(100);

			}

			OverlayChangeInProgress = true;
			if (link.Length < 1)
			{
				overlayTexture = null;
				return;
			}

			overlayTexture = Utility.GetImageOrDownload(link);
			OverlayChangeInProgress = false;

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
				combinedRender = new RenderTarget2D(Global.SharedSpriteBatch.GraphicsDevice, Global.RenderOutput.Width, Global.RenderOutput.Height);
				combinedRender2?.Dispose();
				combinedRender2 = new RenderTarget2D(Global.SharedSpriteBatch.GraphicsDevice, Global.RenderOutput.Width, Global.RenderOutput.Height);
			}
		}

		private float mouseInactivtyCounter = 0;
		private Point oldMousePos;
		public override void Draw(GameTime gameTime)
		{

			clcounter += (float)gameTime.ElapsedGameTime.Milliseconds/1000 * EffectParams["clspeed"];
			dxcounter += (float)gameTime.ElapsedGameTime.Milliseconds/1000 * EffectParams["dxspeed"];
			dycounter += (float)gameTime.ElapsedGameTime.Milliseconds/1000 * EffectParams["dyspeed"];
			
			if (combinedRender == null || combinedRender2 == null)//shitcode
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
			connectionEffect.Parameters["fps"].SetValue(clcounter);
			connectionEffect.Parameters["staticAlpha"].SetValue(EffectParams["clalpha"] + GetNoise() * 0.01f);
			connectionEffect.Parameters["magnitude"].SetValue(EffectParams["clmagnitude"] + GetNoise() * 1f);
			connectionEffect.Parameters["overlayalpha"].SetValue(EffectParams["overlayalpha"] + GetNoise() * 0.05f);
			if (overlayTexture != null)
			{
				connectionEffect.Parameters["overlay"].SetValue(overlayTexture);
			}
			else
			{
				connectionEffect.Parameters["overlay"].SetValue(emptyTexture);
			}

			colorEffect.Parameters["max"].SetValue(new Vector4(EffectParams["maxR"],EffectParams["maxG"],EffectParams["maxB"],1));
			colorEffect.Parameters["min"].SetValue(new Vector4(EffectParams["minR"],EffectParams["minG"],EffectParams["minB"],1));
			colorEffect.Parameters["tint"].SetValue(new Vector4(EffectParams["tintR"],EffectParams["tintG"],EffectParams["tintB"],1));
			

			distortEffect.Parameters["xfps"].SetValue(dxcounter);
			distortEffect.Parameters["yfps"].SetValue(dycounter);
			distortEffect.Parameters["xamplitude"].SetValue(EffectParams["dxamplitude"]);
			distortEffect.Parameters["yamplitude"].SetValue(EffectParams["dyamplitude"]);
			distortEffect.Parameters["xfrequency"].SetValue(EffectParams["dxfrequency"]);
			distortEffect.Parameters["yfrequency"].SetValue(EffectParams["dyfrequency"]);
			

			if(oldMousePos ==SadConsole.Game.Instance.Mouse.ScreenPosition){
				
				mouseInactivtyCounter += gameTime.ElapsedGameTime.Milliseconds;
				
			}
			else
			{
				mouseInactivtyCounter = 0;
			}

			oldMousePos = SadConsole.Game.Instance.Mouse.ScreenPosition;

			Texture2D cursorTexture2D;
			if (SadConsole.Game.Instance.Mouse.LeftButtonDown)
			{
				cursorTexture2D = cursorTextures[3];
				mouseInactivtyCounter = 0;
			}
			else if (mouseInactivtyCounter > 2000)
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

			Color targetcolor = Color.Green;

			if (SadConsole.Game.Instance.Keyboard.IsKeyDown(Keys.LeftShift))
			{
				targetcolor = Color.Red;
			}
			else if (SadConsole.Game.Instance.Keyboard.IsKeyDown(Keys.LeftControl))
			{
				targetcolor = Color.Blue;
			}

			Color[] tcolor=new Color[cursorTexture2D.Width*cursorTexture2D.Height];
			cursorTexture2D.GetData<Color>(tcolor);

			for (int i = 0;  i < tcolor.Length; i++)
			{
				if (tcolor[i].A > 1)
				{
					tcolor[i] = targetcolor;
				}
			}
	
			cursorTexture2D.SetData<Color>(tcolor);

			float fade = Math.Clamp((3000 - (mouseInactivtyCounter-2000)) / 3000 * 1, 0, 1);


			combinedSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender);
		combinedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);

		combinedSpriteBatch.Draw(Global.RenderOutput, Global.RenderOutput.Bounds, Color.White);

		combinedSpriteBatch.Draw(cursorTexture2D, new Vector2(SadConsole.Game.Instance.Mouse.ScreenPosition.X - cursorTexture2D.Width / 2, SadConsole.Game.Instance.Mouse.ScreenPosition.Y - cursorTexture2D.Height / 2), new Microsoft.Xna.Framework.Color(fade,fade,fade,fade));
		
		combinedSpriteBatch.End();
		

	





	combinedSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender2);
	combinedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone,distortEffect);
	combinedSpriteBatch.Draw(combinedRender, combinedRender.Bounds, Color.White);
	combinedSpriteBatch.End();
	
	combinedSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender);
	combinedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone,colorEffect);
	combinedSpriteBatch.Draw(combinedRender2, combinedRender2.Bounds, Color.White);
	combinedSpriteBatch.End();
		
	combinedSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender2);
	combinedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone,connectionEffect);
	combinedSpriteBatch.Draw(combinedRender, combinedRender.Bounds, Color.White);
	combinedSpriteBatch.End();	
		
	
	combinedSpriteBatch.GraphicsDevice.SetRenderTarget(combinedRender);
	combinedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone,crtEffect);
	combinedSpriteBatch.Draw(combinedRender2, combinedRender2.Bounds, Color.White);
	combinedSpriteBatch.End();
		



		combinedRender.GraphicsDevice.SetRenderTarget(null);

		
		Global.SharedSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullNone);
		Global.SharedSpriteBatch.Draw(combinedRender, combinedRender.Bounds, Color.White);
		Global.SharedSpriteBatch.End();
			
	

	

		}


		
		private static List<EventWaitHandle> threadQueue = new List<EventWaitHandle>();
		private static Dictionary<string, List<EventWaitHandle>> awaitingthreadQueue = new();
		
		
		private static readonly object syncObj = new object();
		public static void AddTween(string parameter,float target, float speed, bool wipeQueue = false)
		{

			lock (syncObj)
			{
				if (!awaitingthreadQueue.ContainsKey(parameter))
				{
					awaitingthreadQueue[parameter] = new List<EventWaitHandle>(); //create queue for each parameter
				}

				if (wipeQueue)
				{
					awaitingthreadQueue[parameter] = new List<EventWaitHandle>();
					int index = tweens.FindIndex(x => x.parameter == parameter);
					if (index > -1)
					{
						tweens.RemoveAt(index);
					}

				}
			}

			if (tweens.FindIndex(x => x.parameter == parameter) != -1) //queue up if parameter is being currently tweened
			{

				var eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
				awaitingthreadQueue[parameter].Add(eventWaitHandle);
				//System.Console.WriteLine("stoped by awaiting for: "+parameter);
				eventWaitHandle.WaitOne();
				eventWaitHandle.Close();
			}
			lock (syncObj)
			{
				//	System.Console.WriteLine("passed and set: "+parameter);
				Tween t = new Tween(parameter,EffectParams[parameter],speed,target);
				tweens.Add(t);

			}


		}

		//might be better to make this a dict and only allow 1 tween per parameter
		private static List<Tween> tweens = new List<Tween>();
		private void ProcessTweens(GameTime gameTime)
		{
			lock (syncObj)
			{
				foreach (Tween t in tweens.ToList())
				{
					if (t.counter > 1)
					{
						tweens.Remove(t);
						if (awaitingthreadQueue[t.parameter].Count > 0)
						{
							EventWaitHandle nextThread = awaitingthreadQueue[t.parameter][^1];

							nextThread.Set();
							awaitingthreadQueue[t.parameter].RemoveAt(awaitingthreadQueue[t.parameter].Count - 1);

						}

						continue;
					}

					t.Lerp(gameTime);
				}
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