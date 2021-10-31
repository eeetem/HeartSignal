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
        }

        private byte[] baseImage;

        private int blurCounter;
        private int saturCounter;
        private int gammaCounter;

        private bool surfaceCreated = false;
        public void MakeSurfaceImage()
        {   if (Tagline=="") return;
           this.Clear();
           ICellSurface logo;
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
                    
                    ITexture texture = GameHost.Instance.GetTexture(outStream);
                  //  this.Resize(Program.Height * 2,Height,Program.Height * 2,Height,false);
                    logo = texture.ToSurface(TextureConvertMode.Foreground, Program.Height * 2, Program.Height, foregroundStyle: TextureConvertForegroundStyle.AsciiSymbol,cachedSurface: this.Surface);
                    Surface = logo;
   
    
                   
                }
            }
            				
            Position = new Point((Program.Width/2) - Program.Height , 0);
            miniDisplay.Position = new Point(Program.Width / 2 - 15, (Program.Height / 2) + 10);
            this.Print(Width/2 - Tagline.Length/2, (Program.Height/2)-7,Tagline);
            if (!surfaceCreated)
            {
                surfaceCreated = true;
                ReDraw();
            }
        }

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

        public void ReDraw()
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
        private int counter = 0;
        public override void Render(TimeSpan delta)
        {
            
            counter -= delta.Milliseconds;
            if (counter < 0)
            {
                counter = delta.Milliseconds*2;
     
               // Thread thread = new Thread(MakeSurfaceImage);
                //thread.Start();
                MakeSurfaceImage();
                IsDirty = true;
            }
            base.Render(delta);

        }
    }
}