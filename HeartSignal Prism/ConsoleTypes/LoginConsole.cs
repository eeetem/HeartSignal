using SadConsole;
using System;
using System.Drawing;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;
using System.IO;
using ImageProcessor;
using Color = SadRogue.Primitives.Color;
using Point = SadRogue.Primitives.Point;
using System.Threading;

namespace HeartSignal
{
    public class LoginConsole : SadConsole.UI.ControlsConsole
    {
        public LoginConsole(int width, int height) : base(width, height)
        {

            // Disable the cursor since our keyboard handler will do the work.
            Cursor.IsEnabled = false;
            Cursor.IsVisible = false;
            Surface.DefaultForeground = new Color(20, 20, 20);
            Cursor.DisableWordBreak = true;

            Cursor.UseStringParser = true;
           
            miniDisplay = new Console(30,15);
            
           // miniDisplay.SadComponents.Add(new AnimatedBorderComponent());
            Children.Add(miniDisplay);
            baseImage = File.ReadAllBytes("lobby.png");
            //MakeSurfaceImage();
            Random rnd = new Random();
           blurCounter = rnd.Next(0,50);
           saturCounter = rnd.Next(0,40);
            gammaCounter = rnd.Next(0,200);
            using (MemoryStream stream = new MemoryStream(baseImage))
            {
                texture = GameHost.Instance.GetTexture(stream);
            }
        }

        private byte[] baseImage;

        private int blurCounter;
        private int saturCounter;
        private int gammaCounter;

        private bool surfaceCreated = false;
        private Color[] PixelCache;//since you cant load pixels on a non UI thread - surface generation works the following way: 1.Main thread calls for surface generation 2.Main thread loads Pixels of OLD texture 3. new texture is generated 4. OLD cached pixels are turned into a surface. so image gets generated and printed into surface in 1 go however the surface print is from previous cycle
        ITexture texture;
        public void MakeSurfaceImage()
        {   if (Tagline=="") return;
            var watch = System.Diagnostics.Stopwatch.StartNew();
           //this.Clear();
         
           
          
            //using ITexture sadImage = GameHost.Instance.OpenStream("lobby.png");
            Random rnd = new Random();
            using (MemoryStream inStream = new MemoryStream(baseImage))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData:true))
                    {
                        blurCounter++;
                        saturCounter++;
                        gammaCounter++;
                        if (blurCounter > 50) blurCounter = 0;
                        if (gammaCounter > 40) gammaCounter = 0;
                        if (saturCounter > 200) saturCounter = 0;
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            //  .Rotate(rnd.Next(-1,1))
                            .Saturation(gammaCounter < 100 ? gammaCounter : 200-gammaCounter)
                            .GaussianSharpen(rnd.Next(0,10))
                            .GaussianBlur(blurCounter < 25 ? blurCounter : 50-blurCounter)
                             .Gamma(gammaCounter < 20 ? gammaCounter : 40-gammaCounter)
                            .Resize(new Size(Program.Height*2,Program.Height*2))//it's faster to do all effects on a lowres image and then upscale it
                            .Save(outStream);
                    }
                    
                    texture = GameHost.Instance.GetTexture(outStream);
                  //  this.Resize(Program.Height * 2,Height,Program.Height * 2,Height,false);
                  
    
                   
                }
            }
            

            var surfaceHeight = Program.Height;
            var surfaceWidth = Program.Height * 2;
          // this chunk of code is taken straight out of ToSurface() in sadconsole - however since the GetPixel() can't be dont outside of main thread i had to reuse their fucntion but with getpixel done on main thread beforehand
            if (surfaceWidth <= 0 || surfaceHeight <= 0 || surfaceWidth != texture.Width )
                return;

            this.Clear();
            ICellSurface surface = this.Surface;

            

            int fontSizeX = texture.Width / surfaceWidth;
            int fontSizeY = texture.Height / surfaceHeight;

            global::System.Threading.Tasks.Parallel.For(0, texture.Height / fontSizeY, (h) =>
            //for (int h = 0; h < imageHeight / fontSizeY; h++)
            {
                int startY = h * fontSizeY;
                //System.Threading.Tasks.Parallel.For(0, imageWidth / fontSizeX, (w) =>
                for (int w = 0; w < texture.Width / fontSizeX; w++)
                {
                    int startX = w * fontSizeX;

                    float allR = 0;
                    float allG = 0;
                    float allB = 0;

                    for (int y = 0; y < fontSizeY; y++)
                    {
                        for (int x = 0; x < fontSizeX; x++)
                        {
                            int cY = y + startY;
                            int cX = x + startX;

                            int index = cY * texture.Width + cX;
                            if (PixelCache.Length <= index) return;//window got resized - texutre and size mismatch
                            Color color = PixelCache[index];
                           
                          
                           
                            allR += color.R;
                            allG += color.G;
                            allB += color.B;
                        }
                    }

                    byte sr = (byte)(allR / (fontSizeX * fontSizeY));
                    byte sg = (byte)(allG / (fontSizeX * fontSizeY));
                    byte sb = (byte)(allB / (fontSizeX * fontSizeY));

                    var newColor = new SadRogue.Primitives.Color(sr, sg, sb);

                        float sbri = newColor.GetBrightness() * 255;

                        if (sbri > 230)
                            surface.SetGlyph(w, h, '#', newColor);
                        else if (sbri > 207)
                            surface.SetGlyph(w, h, '&', newColor);
                        else if (sbri > 184)
                            surface.SetGlyph(w, h, '$', newColor);
                        else if (sbri > 161)
                            surface.SetGlyph(w, h, 'X', newColor);
                        else if (sbri > 138)
                            surface.SetGlyph(w, h, 'x', newColor);
                        else if (sbri > 115)
                            surface.SetGlyph(w, h, '=', newColor);
                        else if (sbri > 92)
                            surface.SetGlyph(w, h, '+', newColor);
                        else if (sbri > 69)
                            surface.SetGlyph(w, h, ';', newColor);
                        else if (sbri > 46)
                            surface.SetGlyph(w, h, ':', newColor);
                        else if (sbri > 23)
                            surface.SetGlyph(w, h, '.', newColor);
                    
                }
            }
            );

            Surface = surface;

            Position = new Point((Program.Width/2) - Program.Height , 0);
            miniDisplay.Position = new Point((Width / 2) - miniDisplay.Width/2, (Program.Height / 2) + 6);
            this.Print(Width/2 - Tagline.Length/2, (Program.Height/2)-7,Tagline);
            if (!surfaceCreated)
            {
                surfaceCreated = true;
                MakeControlls();
            }
             watch.Stop();
            SurfaceGenerationTime = watch.ElapsedMilliseconds;

        }
        float SurfaceGenerationTime = 100;

        public Console miniDisplay;


        private string tagline = "";
        public string Tagline
        {
            get => tagline;


            set
            {
                File.WriteAllText("tagline.txt",value);
                tagline = value;
            }
        }

        
        //idealy this should be part of the multithreaded MakeSurface image - however you cannot run ToSurface on a non Main thread due to monogame quirk that will hopefully be fixed at some point;

        public void MakeControlls()
        {
          //  this.Clear();
          if(!surfaceCreated) return;
          
          this.Controls.Clear();



            
            var input = new TextBox(26)
            {
                Text = "login",
                
                Position = new Point(Width/2 -13 , (Program.Height/2)-4)
            };

            void Handler(object sender, ControlBase.ControlMouseState args)
            {
                input.Text = "";
                input.MouseButtonClicked -= Handler;
            }

            input.MouseButtonClicked += Handler;
            
            Controls.Add(input);

            var password = new TextBox(26)
            {
                Text = "password",
                Mask = '*',
                Position = new Point(Width/2 -13 ,(Program.Height/2))
            };

            void Handler2(object sender, ControlBase.ControlMouseState args)
            {
                password.Text = "";
                password.MouseButtonClicked -= Handler2;
            }

            password.MouseButtonClicked += Handler2;
            
            Controls.Add(password);
            
            
           
            var button = new Button(11)
            {
                Text = "Be Born",
                Position = new Point(Width/2 -5 ,(Program.Height/2)+2),
            };
            button.MouseButtonClicked += (s, a) => input.FocusLost();
            button.MouseButtonClicked += (s, a) => password.FocusLost();
            button.MouseButtonClicked += (s,a) => Program.SendNetworkMessage("connect " + input.Text + " " + password.Text);
            Controls.Add(button);
        }

        delegate void VoidDelegate();
        private float counter = 0;
        public static Thread ImageDrawThread;
        public override void Render(TimeSpan delta)
        {
            
            counter -= delta.Milliseconds;
            if (counter < 0)
            {

                PixelCache = texture.GetPixels();
                ImageDrawThread = new Thread(MakeSurfaceImage);
                ImageDrawThread.Start();
                counter = SurfaceGenerationTime*2;//dynamic animation speed to not melt bad CPUs
                IsDirty = true;
            }
            //DrawImage();
            base.Render(delta);

        }
    }
}