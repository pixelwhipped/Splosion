using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splosion.Audio;
using Splosion.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Splosion.States
{
    public class RushFireWork
    {
        public Vector2 Location;
        public Color Color;
        public float Size;        

        public RushFireWork(Vector2 location, Color color, float size)
        {
            Location = location;
            Color = color;
            Size = size;            
        }
        public float Compare(RushFireWork f)
        {
            var d = Vector2.Distance(Location, f.Location);
            d = 1f - Math.Min(d, 200f) / 200f; //0-1
            var rd = 1f - Math.Abs(f.Color.R - Color.R) / 255f;
            var gd = 1f - Math.Abs(f.Color.G - Color.G) / 255f;
            var bd = 1f - Math.Abs(f.Color.B - Color.B) / 255f;
            var ca = (rd + gd + bd) / 3f;
            var sd = 1f - Math.Abs(Size - f.Size);
            return (d * .5f) + (sd * .2f) + (ca * .3f);
        }
    }

    public class TimeParticle
    {
        public float Time;
        public Tween Fade;
        public Vector2 Location;
    }
    public class Rush : ExitableState
    {

        public List<MiniScore> ScoreParticles;
        public List<TimeParticle> TimeParticles; 

        public int Score;
        public int GameScore;

        public TimeSpan CurrentTTL;
        public TimeSpan StartTTL;
        public TimeSpan TTL;
        public TimeSpan MaxTTL;
        public TimeSpan? FireTime = null;

        
        public RushFireWork CurrentFireWork;

        public Tween CurrectSelectorTween;

        public bool FirstRun;
        public int level;
        public int Multiplier;
        public Tween MultiplierTween;
        public void Tap(Vector2 t)
        {

        }

        public enum PlayState
        {
            KEEP_UP,READY, SET, GO, ADD, PLAY
        }
        public PlayState RSG;
        public Tween ActionTween;
        public Rush(Splosion game)
            : base(game)
        {
            FirstRun = true;
            ScoreParticles = new List<MiniScore>();
            TimeParticles = new List<TimeParticle>();
            MultiplierTween = new Tween(new TimeSpan(0, 0, 0, 4), 1, 0);
            MultiplierTween.Finish();
            CurrectSelectorTween = new Tween(new TimeSpan(0,0,0,1),1,0);
            CurrectSelectorTween.Finish();
            StartTTL = new TimeSpan(0, 0, 0, 4);
            CurrentTTL = new TimeSpan(0, 0, 0, 4);
            MaxTTL = new TimeSpan(0, 0, 0, 10);
            TTL = StartTTL;
            RSG = PlayState.KEEP_UP;
            ActionTween = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            ActionTween.AddEventListener(.5f,t =>
            {
                switch (RSG)
                {
                    case PlayState.KEEP_UP:
                        Game.Audio.Play(Sounds.Menu);
                        break;
                    case PlayState.READY:
                        Game.Audio.Play(Sounds.Menu);
                        break;
                    case PlayState.SET:
                        Game.Audio.Play(Sounds.Menu);
                        break;
                    case PlayState.GO:
                        Game.Audio.Play(Sounds.Menu);
                        break;
                }
            });
            
            
            
            ActionTween.CompletionListeners.Add(t =>
            {
                switch (RSG)
                {
                    case PlayState.KEEP_UP:
                        Selector.ShowRadius = false;
                        Selector.SelectorListeners.Clear();
                        RSG = PlayState.READY;
                        ActionTween.Reset();
                        break;
                    case PlayState.READY:                        
                        Selector.ShowRadius = false;
                        Selector.SelectorListeners.Clear();
                        RSG = PlayState.SET;
                        ActionTween.Reset();
                        break;
                    case PlayState.SET:
                        RSG = PlayState.GO;
                        ActionTween.Reset();
                        break;
                    case PlayState.GO:                        
                        RSG = PlayState.ADD;
                        break;
                }
            });
            Selector.SelectorListeners.Add(game.AddFireWorks);
            Mouse.LeftClickListeners.Add(Tap);
            Touch.TapListeners.Add(Tap);
        }

        public override GameState<Splosion> Update(GameTime gameTime)
        {
            ActionTween.Update(gameTime.ElapsedGameTime);
            CurrectSelectorTween.Update(gameTime.ElapsedGameTime);
            MultiplierTween.Update(gameTime.ElapsedGameTime);
            if (MultiplierTween.IsComplete)
            {
                Multiplier = 0;
            }
            switch (RSG)
            {
                case PlayState.KEEP_UP:
                    level = 0;
                    FirstRun = true;
                    FireTime = TimeSpan.Zero;
                    
                    break;
                case PlayState.READY:
                    break;
                case PlayState.SET:
                    break;
                case PlayState.GO:
                    
                    break;
                case PlayState.ADD:
                    TTL -= gameTime.ElapsedGameTime;
                    if (FirstRun)
                    {
                        FirstRun = false;
                        FireTime = TimeSpan.Zero;
                        Selector.SelectorListeners.Clear();
                    }
                    if (FireTime == null)
                    {
                        FireTime = new TimeSpan(0, 0, 0, 0,500)+new TimeSpan(TimeSpan.TicksPerMillisecond*(level*level));
                        if (FireTime > new TimeSpan(0, 0, 0, 2, 750))
                            FireTime = new TimeSpan(0, 0, 0, 2, 750);
                        level++;
                        Selector.SelectorListeners.Clear();
                    }
                    else
                    {
                        FireTime -= gameTime.ElapsedGameTime;
                        if (FireTime <= TimeSpan.Zero)
                        {
                            Selector.ShowRadius = true;
                            //Selector.SelectorListeners.Add(game.AddFireWorks)
                            Selector.SelectorListeners.Add(AddFireWorks);
                            CreateMap();
                            RSG = PlayState.PLAY;
                            FireTime = null;
                        }
                    }                                                  
                    break;
                case PlayState.PLAY:
                    
                    TTL -= gameTime.ElapsedGameTime;
                    if (TTL < TimeSpan.Zero)
                    {
                        GameScore = 0;
                        Score = 0;
                        CurrentTTL = StartTTL;
                        TTL = CurrentTTL;
                        FirstRun = true;
                        level = 0;
                        RSG = PlayState.KEEP_UP;
                        
                    }
                    if (Score > Game.RushHighScore)
                    {
                        Game.RushHighScore = Score;
                    }

                    break;
            }
            foreach (var t in ScoreParticles)
            {
                t.Fade.Update(gameTime.ElapsedGameTime);
                t.Location = new Vector2(t.Location.X,
                    t.Location.Y - 1);
            }
            ScoreParticles.RemoveAll(p => p.Fade.IsComplete);
            foreach (var t in TimeParticles)
            {
                t.Fade.Update(gameTime.ElapsedGameTime);
                t.Location = new Vector2(t.Location.X,
                    t.Location.Y + 2);
            }
            TimeParticles.RemoveAll(p => p.Fade.IsComplete);

            return base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            switch (RSG)
            {
                case PlayState.KEEP_UP:
                    SpriteBatch.Draw(Textures.TextKeepUp,
                        new Vector2(Center.X - Textures.TextKeepUp.Center().X, Center.Y - Textures.TextKeepUp.Center().Y),
                        Color.White * (float)Math.Sin(ActionTween * Math.PI));
                    break;
                case PlayState.READY:
                    SpriteBatch.Draw(Textures.TextReady,
                        new Vector2(Center.X - Textures.TextReady.Center().X, Center.Y - Textures.TextReady.Center().Y),
                        Color.White * (float)Math.Sin(ActionTween*Math.PI));
                    break;
                case PlayState.SET:
                    SpriteBatch.Draw(Textures.TextSet,
                        new Vector2(Center.X - Textures.TextSet.Center().X, Center.Y - Textures.TextSet.Center().Y),
                        Color.White * (float)Math.Sin(ActionTween * Math.PI));
                    break;
                case PlayState.GO:
                    SpriteBatch.Draw(Textures.TextGo,
                        new Vector2(Center.X - Textures.TextGo.Center().X, Center.Y - Textures.TextGo.Center().Y),
                        Color.White * (float)Math.Sin(ActionTween * Math.PI));
                    break;
                case PlayState.PLAY:
                    
                    break;
            }


            
            DrawProgress();

            if (Multiplier == 0)
            {
                SpriteBatch.Draw(Textures.X1,new Vector2(10,(int)Height - 83),Color.White*MultiplierTween );
            }
            if (Multiplier == 1)
            {
                SpriteBatch.Draw(Textures.X2, new Vector2(10, (int)Height - 83), Color.White * MultiplierTween);
            }
            if (Multiplier == 2)
            {
                SpriteBatch.Draw(Textures.X3, new Vector2(10, (int)Height - 83), Color.White * MultiplierTween);
            }
            if (Multiplier == 3)
            {
                SpriteBatch.Draw(Textures.X4, new Vector2(10, (int)Height - 83), Color.White * MultiplierTween);
            }
            if (CurrentFireWork!=null)
                SpriteBatch.Draw(Textures.Selector, CurrentFireWork.Location -Textures.Selector.Center(), null, CurrentFireWork.Color * CurrectSelectorTween, 0, Vector2.Zero, CurrentFireWork.Size, SpriteEffects.None, 1);    


            foreach (var s in ScoreParticles)
            {
                var t = MiniScore.GetTextureFor(s.Score);
                SpriteBatch.Draw(t, new Vector2(s.Location.X - (t.Width / 2f), s.Location.Y), Color.White * s.Fade);
            }

            foreach (var s in TimeParticles)
            {
                Utilities.DrawNumbersPlus(SpriteBatch, s.Time, s.Location, Color.White*s.Fade);
                //var t = MiniScore.GetTextureFor(s.Score);
                //SpriteBatch.Draw(t, new Vector2(s.Location.X - (t.Width / 2f), s.Location.Y), Color.White * s.Fade);
            }
            DrawScore();

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        private void AddFireWorks(Vector2 point, Color color, float size)
        {
           
            Game.AddFireWorks(point, color, size);
            
            var f = new RushFireWork(point, color, size);
            var s = f.Compare(CurrentFireWork);
            var ms = MiniScore.GetMiniScoreFor(s);
            int sc = MiniScore.GetScoreFor(ms);
            ScoreParticles.Add(new MiniScore
            {
                Score = ms,
                Location = point,
                Fade = new Tween(new TimeSpan(0, 0, 0, 1), 1, 0)
            });
            if (sc == MiniScore.GetScoreFor(MiniScores.PERFECT))
            {

                Multiplier ++;
                if (Multiplier > 3) Multiplier = 3;
                MultiplierTween.Reset();
            }
            Score += sc*(Multiplier +1);

            var pTime = new TimeSpan(MiniScore.GetTimeFor(MiniScore.GetMiniScoreFor(s)).Ticks*(Multiplier + 1));
            if (pTime > TimeSpan.Zero)
            {
                TimeParticles.Add(new TimeParticle
                {
                    Time = float.Parse(pTime.Seconds + ((pTime.Milliseconds>0)?"." + pTime.Milliseconds:"")),
                    Location = point,
                    Fade = new Tween(new TimeSpan(0, 0, 0, 6), 1, 0)
                });
            }
            TTL += pTime;
            if (TTL > CurrentTTL)
            {

                if (TTL > MaxTTL)
                {
                    TTL = MaxTTL;
                    CurrentTTL = TTL;
                }
                else
                {
                    CurrentTTL = TTL;
                }
            }
            RSG = PlayState.ADD;
        }

        public void CreateMap()
        {
            CurrectSelectorTween.Reset();
            CurrentFireWork = new RushFireWork(FireWorkArea.RandomPoint(), CreateColor(Splosion.Random.NextFloat()),
                Math.Max(.80f + (Splosion.Random.NextFloat()*.2f), 1f));
            Game.AddFireWorks(CurrentFireWork.Location, CurrentFireWork.Color, CurrentFireWork.Size);
        }
        public Color CreateColor(float v)
        {
            var c = Color.Red;
            c = Color.Lerp(c, v > .5f ? Color.Green : Color.Blue, Splosion.Random.NextFloat());
            return c;
        }
        public void DrawScore()
        {
            var v = new Vector2(10, 10);
            SpriteBatch.Draw(Textures.Score, v, Color.White * .6f * FadeTween);
            v = new Vector2(v.X, v.Y + Textures.Score.Height);
            Utilities.DrawNumbers(SpriteBatch, Score, v, Color.White * .6f * FadeTween);
  
            v = new Vector2(Width - Textures.HighScore.Width, 10);
            SpriteBatch.Draw(Textures.HighScore, v, Color.White * .6f * FadeTween);
            var w = Utilities.GetNumWidth(Game.RushHighScore);
            v = new Vector2(Width - w, v.Y + Textures.HighScore.Height);
            Utilities.DrawNumbers(SpriteBatch, Game.RushHighScore, v, Color.White * .6f * FadeTween);

        }
        public void DrawProgress()
        {
            var rect = new Rectangle(10, (int)Height - 28, (int)Width - 20, 10);
            SpriteBatch.Draw(Textures.Bar, rect, Color.Gray * ExitTween * FadeTween);
            var rectc = new Rectangle(rect.Left, rect.Top, (int)(rect.Width * (((float)TTL.Ticks / (float)CurrentTTL.Ticks))), rect.Height);
            SpriteBatch.Draw(Textures.Bar, rectc, Color.Orange * FadeTween);            
        }
    }
}
