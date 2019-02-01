using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.UI.Popups;
using System;
using System.Threading.Tasks;
using Splosion.Audio;
using Splosion.Graphics;
using Splosion.ParticleSystem;
using System.IO;
using Splosion.ParticleSystem.EmmiterModifiers;
using Splosion.ParticleSystem.ParticleModifiers;
using Splosion.Input;
using Splosion.Effects;
using Splosion.States;

namespace Splosion
{
    public interface ParentInterface
    {
        void ShowToast(string toast, string title = "Achievement", TimeSpan? time = null);
    }

    public delegate void Setter<in TValue>(TValue value);

    public delegate TValue Getter<out TValue>();

    public delegate void Procedure<in TValue>(TValue value);

    public delegate void Operation<in TValue>(TValue a, TValue b);

    public delegate void SelectorEvent(Vector2 point, Color color, float size);

    public delegate void Routine();
    
    public class Splosion : Game, ParentInterface
    {
        public Tween Flash;
        public readonly Color _backGround;

        public Color LastFlash;
        public Color Background
        {
            get
            {
                return Color.Lerp(_backGround, Color.Lerp(new Color(0, 0, 65), LastFlash, (float)Math.Sin(Flash * Math.PI)), (float)Math.Sin(Flash * Math.PI));
            }            
        }

        public static Random Random = new Random();
        public GraphicsDeviceManager Graphics;
        public SpriteBatch SpriteBatch;

        public GameSettings GameSettings { get; protected set; }

        public float Width { get { return Graphics.GraphicsDevice.Viewport.Width; } }
        public float Height { get { return Graphics.GraphicsDevice.Viewport.Height; } }
        public Vector2 Center { get { return new Vector2(Width / 2f, Height / 2f); } }

        public int RushHighScore
        {
            get { return GameSettings.Data.HighScore; }
            set
            {
                GameSettings.Data.HighScore = value;
                GameSettings.Save();
            }
        }

        public int HighScore
        {
            get { return GameSettings.Data.HighScore; }
            set
            {
                GameSettings.Data.HighScore = value;
                GameSettings.Save();
            }
        }

        public int BestScore
        {
            get { return GameSettings.Data.BestScore; }
            set
            {
                GameSettings.Data.BestScore = value;
                GameSettings.Save();
            }
        }

        public float SceneHorizon
        {
            get
            {
                return (Height - (Height * .1f)) - (SceneScape.Height * SceneScapeScale);
            }
        }
        public Rectangle FireWorkArea
        {
            get
            {
                return new Rectangle(25, 25, (int)Width - 50, (int)SceneHorizon - 50);
            }
        }

        public Rectangle FountainArea
        {
            get
            {
                return new Rectangle(FireWorkArea.Left, FireWorkArea.Bottom, FireWorkArea.Width, 50);
            }
        }

        public Rectangle PlayArena
        {
            get
            {
                return new Rectangle(FireWorkArea.Left, FireWorkArea.Top, FireWorkArea.Right, FountainArea.Bottom);
            }
        }
        public AudioFx Audio { get; set; }
        
        public ParentInterface ParentInterface;
       // private Settings _settings;
        //public Settings Settings
        //{
        //    get { return _settings ?? (_settings = new Settings(this)); }
        //}

        public Texture2D SceneScape { get { return Textures.ScapesCity; } }
        public float SceneScapeScale { get { return Width / SceneScape.Width; } }
        public RenderTarget2D ParticleTarget;
        //public RenderTarget2D ParticleTargetFade;
        public RippleEffect Ripple;
        public BloomEffect Bloom;

        public List<IEmitterModifier> ParticleModifiers;
        public float ModifierCircRectSize = 50;
        public float ModifierConcentricSize = 100;
        public float ModifierEmissionRate = .6f;

        public ParticleEngine ParticleEngine;        

        public TouchInput Touch;
        public MouseInput Mouse;

        public GameState<Splosion> CurrentState;

        public bool ShownHint;
        public MainMenu MainMenuState;

        public bool IsPaused;

        public static Texture2D Pixel;
        public Splosion()
        {
            Graphics = new GraphicsDeviceManager(this);                       
            Content.RootDirectory = "Content";
            _backGround = new Color(0, 0, 32);
            LastFlash = _backGround;
            Flash = new Tween(new TimeSpan(0, 0, 0, 0, 500), 0, 1);
            IsMouseVisible = true;
        }

        public void AddFireWorks(Vector2 point, Color color, float size)
        {
            LastFlash = color;
            Audio.Play(Sounds.Explode);
            Flash.Reset();
            if (ParticleModifiers.Count == 0)
            {
                ParticleEngine.Add(new Emitter(point, TimeSpan.FromMilliseconds((int) (200f*(size*size))),
                    e => ParticleFactory.GenerateParticle(e, Textures.ParticleFlowerBurst,
                        Color.Lerp(color, Color.White, size), color), (int) Math.Max(3000f*size, 500),
                    (int) (250*size)));
            }
            else
            {
                var p = ParticleModifiers.ToArray(); 
                foreach (var t in p)
                {
                    if (t.GetType() == typeof(CircularPattern))
                    {
                        ((CircularPattern) t).Radius = ModifierCircRectSize*size;
                    }
                    else if (t.GetType() == typeof(RectanglePattern))
                    {
                        ((RectanglePattern)t).Size = new Vector2(ModifierCircRectSize*2f, ModifierCircRectSize*2f) * size;
                    }
                    else if (t.GetType() == typeof(RandomEmmisionRate))
                    {
                        ((RandomEmmisionRate)t).randomness = ModifierEmissionRate * size;
                    }
                    else if (t.GetType() == typeof(ConcentriclPattern))
                    {
                        ((ConcentriclPattern)t).Radius = ModifierConcentricSize * size;
                    }
                }
                ParticleEngine.Add(new Emitter(point, TimeSpan.FromMilliseconds((int)(200f * (size * size))),
                    e => ParticleFactory.GenerateParticle(e, Textures.ParticleFlowerBurst,
                        Color.Lerp(color, Color.White, size), color), (int)Math.Max(8000f * size, 500),
                    (int)(500f * size),p));
            }
        }

        protected override void Initialize()
        {            
            ApplicationViewChanged += (o, e) =>
            {

                IsPaused = ApplicationView.Value  != ApplicationViewState.FullScreenLandscape;
            };
            Audio = new AudioFx();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            GameSettings = new GameSettings(this);
            Textures.LoadContent(Content);            
            ParticleEngine = new ParticleEngine();
            ParticleModifiers = new List<IEmitterModifier>();
            Ripple = new RippleEffect(GraphicsDevice);
            Bloom = new BloomEffect(GraphicsDevice);
            ParticleTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
            MainMenuState = new MainMenu(this);
            CurrentState = MainMenuState;
            Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });

        }
              
        public static byte[] LoadStream(string path)
        {
            var s = TitleContainer.OpenStream(path);
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                return ms.ToArray();

            }
        }        

        protected override void Update(GameTime gameTime)
        {
            if (!IsPaused)
            {
                Ripple.Update(gameTime);
                Flash.Update(gameTime.ElapsedGameTime);
                ParticleEngine.Update(gameTime);
                if (CurrentState != null)
                    CurrentState = CurrentState.Update(gameTime);                
            }
            base.Update(gameTime);
        }
               
        protected override void Draw(GameTime gameTime)
        {           
            

            GraphicsDevice.SetRenderTarget(ParticleTarget);
            GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin();
                SpriteBatch.Draw(ParticleEngine);
            SpriteBatch.End();

            //Draw the City Scape
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Background);

            SpriteBatch.Begin();
            SpriteBatch.Draw(SceneScape, new Vector2(0, SceneHorizon), null, Color.Black, 0, Vector2.Zero, SceneScapeScale, SpriteEffects.None, 1);
            SpriteBatch.End();

            //Reflect the CityScape and Particle
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            try
            {
                ((Effect) Ripple).CurrentTechnique.Passes[0].Apply();
            }
            finally
            {
                SpriteBatch.Draw(SceneScape,
                    new Rectangle(0, (int) (Height - (Height*.1f)), (int) Width, (int) (Height*.125)), null, Color.Black,
                    0, Vector2.Zero, SpriteEffects.FlipVertically, 1);
                SpriteBatch.Draw(ParticleTarget,
                    new Rectangle(0, (int) (Height - (Height*.1f)), (int) Width, (int) (Height*.1)), null, Color.White,
                    0, Vector2.Zero, SpriteEffects.FlipVertically, 1);
            }
            SpriteBatch.End();

            //Add the Particles (aditive
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            try
            {
                ((Effect) Bloom).CurrentTechnique.Passes[0].Apply();
            }
            finally
            {
                SpriteBatch.Draw(ParticleTarget, new Rectangle(0, 0, (int) Width, (int) (Height)), Color.White);
                    //; - (Height * .1))), Color.White);                
            }
            SpriteBatch.End();
            //Add the Particles (aditive
            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            SpriteBatch.Draw(ParticleTarget, new Rectangle(0, 0, (int)Width, (int)(Height)), Color.White);//; - (Height * .1))), Color.White);                
            SpriteBatch.End();   


            if (CurrentState != null)
                CurrentState.Draw(gameTime);
            SpriteBatch.Begin();            
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        

        public async Task<IUICommand> ShowMessageAsync(string title, string message, Action onOk = null, Action onCancel = null)
        {

            var md = new MessageDialog(title, message);

            md.Commands.Add(new UICommand("Ok", ui => { if (onOk != null) onOk(); }));
            if (onCancel != null)
                md.Commands.Add(new UICommand("Cancel", ui => onCancel()));
            var c = await md.ShowAsync();
            return c;
        }

        public void ShowToast(string toast, string title = "Achievement", TimeSpan? time = null)
        {
            if (ParentInterface != null)
                ParentInterface.ShowToast(toast,title,time?? TimeSpan.FromSeconds(5));
        }

    }
}
