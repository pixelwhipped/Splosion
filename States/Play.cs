using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splosion.Audio;
using Splosion.Graphics;

namespace Splosion.States
{
    public class FireWork
    {
        public Vector2 Location;
        public Color Color;
        public float Size;
        public float Time;

        public FireWork(Vector2 location, Color color, float size, float time)
        {
            Location = location;
            Color = color;
            Size = size;
            Time = time;
        }
        public float Compare(FireWork f)
        {
            var t = 1f - Math.Abs(f.Time - Time);
            var d = Vector2.Distance(Location, f.Location);
            d = 1f-Math.Min(d, 200f)/200f; //0-1
            var rd = 1f - Math.Abs(f.Color.R - Color.R)/255f;
            var gd = 1f - Math.Abs(f.Color.G - Color.G) / 255f;
            var bd = 1f - Math.Abs(f.Color.B - Color.B) / 255f;
            var ca = (rd + gd + bd)/3f;
            var sd = 1f- Math.Abs(Size - f.Size);
            return (d*.5f) + (sd*.1f) + (t*.3f) + (ca*.1f);
        }
    }
    public class Map
    {
        public TimeSpan Timer;
        public List<FireWork> Events;
        public Splosion Game;
        public float Complete { get { return MapTween.Value; } }
        public Tween MapTween;
        public bool HasEvents { get { return MapTween.EventListeners.Count != 0; } }
        public Map(Splosion game,List<FireWork> events, TimeSpan timer)
        {
            
            Game = game;
            Events = events;
            Timer = timer;
            MapTween = new Tween(timer,0,1);            
        }

        public void AddEvents()
        {
            foreach (var f in Events)
            {
                var f1 = f;
                MapTween.AddEventListener(f.Time, p => Game.AddFireWorks(f1.Location, f1.Color, f1.Size));
            }
        }

        public void RemoveEvents()
        {
            MapTween.EventListeners.Clear();
        }

        public void Update(GameTime gameTime)
        {
            MapTween.Update(gameTime.ElapsedGameTime);
        }
    }

    public enum MiniScores
    {
        PERFECT,
        AWESOME,
        GREAT,
        GOOD,
        OK,
        NOGOOD,
        SAD,
        BOO,
    }
    public class MiniScore
    {
        public MiniScores Score;
        public Vector2 Location;
        public Tween Fade;

        public static MiniScores GetMiniScoreFor(float f)
        {
            if (f < .5f)
            {
                return MiniScores.BOO;
            }

            if (f < .55f)
            {
                return MiniScores.SAD;
            }
            if (f < .6f)
            {
                return MiniScores.NOGOOD;
            }
            if (f < .65f)
            {
                return MiniScores.OK;
            }
            if (f < .7f)
            {
                return MiniScores.GOOD;
            }
            if (f < .85f)
            {
                return MiniScores.GREAT;
            }            
            return f < .9f ? MiniScores.AWESOME : MiniScores.PERFECT;
        }
        public static int GetScoreFor(MiniScores s)
        {
            switch (s)
            {
                case MiniScores.PERFECT:
                    return 50;
                case MiniScores.AWESOME:
                    return 30;
                case MiniScores.GREAT:
                    return 20;
                case MiniScores.GOOD:
                    return 10;
                case MiniScores.OK:
                    return 5;
                case MiniScores.NOGOOD:
                    return 3;
                case MiniScores.SAD:
                    return 2;
                case MiniScores.BOO:
                    return 1;
            }
            return 0;
        }

        public static TimeSpan GetTimeFor(MiniScores s)
        {
            switch (s)
            {
                case MiniScores.PERFECT:
                    return new TimeSpan(0,0,0,2,250);
                case MiniScores.AWESOME:
                    return new TimeSpan(0, 0, 0, 1,500);
                case MiniScores.GREAT:
                    return new TimeSpan(0, 0, 0, 1);
                case MiniScores.GOOD:
                    return new TimeSpan(0, 0, 0, 0,500);
                case MiniScores.OK:
                    return new TimeSpan(0, 0, 0, 0,250);
                case MiniScores.NOGOOD:
                    return new TimeSpan(0, 0, 0, 0,50);
                case MiniScores.SAD:
                    return new TimeSpan(0, 0, 0, 0, 0);
                case MiniScores.BOO:
                    return TimeSpan.Zero;
            }
            return TimeSpan.Zero;
        }

        public static Texture2D GetTextureFor(MiniScores s)
        {
            switch (s)
            {
                case MiniScores.PERFECT:
                    return Textures.TextPerfect;
                case MiniScores.AWESOME:
                    return Textures.TextAwesome;
                case MiniScores.GREAT:
                    return Textures.TextGreat;
                case MiniScores.GOOD:
                    return Textures.TextGood;
                case MiniScores.OK:
                    return Textures.TextOK;
                case MiniScores.NOGOOD:
                    return Textures.TextNoGood;
                case MiniScores.SAD:
                    return Textures.TextSad;
                default:
                    return Textures.TextBoo;

            }
        }
    }

    public class Play : ExitableState
    {
        
        public List<MiniScore> ScoreParticles; 
        public enum PlayState
        {
            SHOW_HAND,
            HAND_LEFT,
            HAND_RIGHT,
            HAND_UP,
            HAND_DOWN,
            HIDE_HAND,
            WATCH,
            WAIT1,
            VIEW,
            READY,
            SET,
            GO,
            PLAY,
            REVIEW
        }

        public int Score;
        public int GameScore;
        public int Level;

        public Map CurrentMap;
        public List<FireWork> RecordEvents;
       // public Tween CurrentRun;

        public PlayState RSG;
        public Tween ActionTween;
        public Tween HandFade;
        public Tween HandMoveH;
        public Tween HandMoveV;
        public Tween SelectorFader;

        public Tween SaveFadeTween;
        public Tween SaveTween;
        public Tween SavedTween;

        public void CheckSave(Vector2 t)
        {
            if (!SavedTween.IsComplete) return;
            var rect =
                new Rectangle(0, (int)(Game.GraphicsDevice.Viewport.Height - ((Textures.Save.Height * SaveTween.Value)))- 30, (int)(Textures.Save.Width * SaveTween.Value), (int)(Textures.Save.Height * SaveTween.Value));

            if (!rect.Contains(new Point((int) t.X, (int) t.Y))) return;
            Game.GameSettings.Data.Level = Level;
            Game.GameSettings.Data.GameScore = GameScore;
                
            SavedTween.Reset();
            Game.GameSettings.Save();
        }

        public Play(Splosion game, int gameScore = 0, int level = 0)
            : base(game)
        {
            GameScore = gameScore;
            Level = level;
            ScoreParticles = new List<MiniScore>();           
            RecordEvents = new List<FireWork>();
            HandFade = new Tween(new TimeSpan(0, 0, 0, 1), 0, 0);
            HandMoveH = new Tween(new TimeSpan(0, 0, 0, 1), Game.Center.X, Game.Center.X);
            HandMoveV = new Tween(new TimeSpan(0, 0, 0, 1), Game.Center.Y, Game.Center.Y);
            SelectorFader = new Tween(new TimeSpan(0, 0, 0, 3), 1, 0);
            SaveFadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            SaveTween = new Tween(new TimeSpan(0, 0, 0, 2), 1f, 1.1f, true);
            SavedTween = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            SavedTween.Finish();

            Mouse.LeftClickListeners.Add(CheckSave);
           // Touch.MoveListeners.Add(CheckSave);
            Touch.TapListeners.Add(CheckSave);

            CreateMap();
            if (Game.ShownHint)
            {
                RSG = PlayState.WATCH;
            }
            else
            {
                Game.ShownHint = true;
                RSG = PlayState.SHOW_HAND;
            }
            ActionTween = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            ActionTween.AddEventListener(.5f, t =>
            {
                switch (RSG)
                {
                    case PlayState.WATCH:
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
                    case PlayState.SHOW_HAND:
                        Selector.ShowRadius = false;
                        Selector.SelectorListeners.Clear();
                        HandFade = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
                        RSG = PlayState.HAND_RIGHT;
                        ActionTween.Reset();
                        break;
                    case PlayState.HAND_RIGHT:
                        Selector.ShowRadius = false;
                        HandMoveH = new Tween(new TimeSpan(0, 0, 0, 1), Game.Center.X, Game.Center.X + 100);
                        RSG = PlayState.HAND_LEFT;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 1));
                        break;
                    case PlayState.HAND_LEFT:
                        Selector.ShowRadius = false;
                        HandMoveH = new Tween(new TimeSpan(0, 0, 0, 2), Game.Center.X + 100, Game.Center.X - 200);
                        RSG = PlayState.HAND_UP;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 2));
                        break;
                    case PlayState.HAND_UP:
                        Selector.ShowRadius = false;
                        HandMoveV = new Tween(new TimeSpan(0, 0, 0, 1), Game.Center.Y, Game.Center.Y + 100);
                        RSG = PlayState.HAND_DOWN;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 1));
                        break;
                    case PlayState.HAND_DOWN:
                        Selector.ShowRadius = false;
                        HandMoveV = new Tween(new TimeSpan(0, 0, 0, 2), Game.Center.Y + 100, Game.Center.Y - 200);
                        RSG = PlayState.HIDE_HAND;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 2));
                        break;
                    case PlayState.HIDE_HAND:
                        Selector.ShowRadius = false;
                        Game.AddFireWorks(new Vector2(Center.X - 52, Center.Y - Textures.Hand.Height), Color.Blue, 1);
                        HandFade = new Tween(new TimeSpan(0, 0, 0, 1), 1, 0);
                        RSG = PlayState.WATCH;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 1));
                        break;
                    case PlayState.WATCH:
                        Score = 0;
                        Selector.ShowRadius = false;                                                
                        Selector.SelectorListeners.Clear();
                        RSG = PlayState.WAIT1;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 2));
                        break;
                    case PlayState.WAIT1:
                        Selector.ShowRadius = false;
                        RSG = PlayState.VIEW;
                        CurrentMap.AddEvents();
                        SelectorFader.Reset();
                        ActionTween.Reset(CurrentMap.Timer);
                        break;
                    case PlayState.VIEW:
                        Selector.ShowRadius = false;
                        RSG = PlayState.READY;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 1));
                        break;
                    case PlayState.READY:
                        Selector.ShowRadius = false;
                        RSG = PlayState.SET;
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 1));
                        break;
                    case PlayState.SET:
                        Selector.ShowRadius = false;                        
                        CurrentMap.RemoveEvents();
                        RSG = PlayState.GO;
                        ActionTween.Reset();
                        break;
                    case PlayState.GO:
                        RSG = PlayState.PLAY;
                        Selector.ShowRadius = true;
                        Selector.SelectorListeners.Add(AddFireWorks);
                        ActionTween.Reset(CurrentMap.Timer);
                        CurrentMap.MapTween.Reset();
                        RecordEvents.Clear();
                        ScoreParticles.Clear();
                        break;
                    case PlayState.PLAY:
                        Selector.ShowRadius = false;   
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 3));
                        RSG = PlayState.REVIEW;
                        break;
                    case PlayState.REVIEW:
                        Selector.ShowRadius = false;   
                        var numOfFW = Math.Max(Math.Min(Level, MaxFireworksLevelTopOut), MinFireworks);
                        numOfFW = (numOfFW / MaxFireworksLevelTopOut) * MaxFireworks;
                        numOfFW = Math.Max(numOfFW, MinFireworks);
                        var rscore = (int) ((numOfFW*MiniScore.GetScoreFor(MiniScores.GOOD)));
                        if (Score >= rscore)
                        {
                            GameScore += Score;
                            Level++;
                        }
                        if (Score > Game.BestScore)
                        {
                            Game.BestScore = Score;
                        }
                        if (GameScore > Game.HighScore)
                        {
                            Game.HighScore = GameScore;
                        }
                        CreateMap();
                        ActionTween.Reset(new TimeSpan(0, 0, 0, 1));
                        RSG = PlayState.WATCH;
                        break;
                }
            });
        }

        private void AddFireWorks(Vector2 point, Color color, float size)
        {
            if (CurrentMap.Events.Count <= RecordEvents.Count) return;
            Game.AddFireWorks(point, color, size);
            var f = new FireWork(point, color, size, CurrentMap.Complete);
            var s = f.Compare(CurrentMap.Events[RecordEvents.Count]);
            ScoreParticles.Add(new MiniScore
            {
                Score = MiniScore.GetMiniScoreFor(s),
                Location = point,
                Fade = new Tween(new TimeSpan(0, 0, 0, 1), 1, 0)
            });
                
            RecordEvents.Add(f);
                
            Score += MiniScore.GetScoreFor(MiniScore.GetMiniScoreFor(s));
        }

        public override GameState<Splosion> Update(GameTime gameTime)
        {
            if (RSG == PlayState.VIEW)
                CurrentMap.Update(gameTime);
            if (RSG == PlayState.READY || RSG == PlayState.SET || RSG == PlayState.GO)
                SelectorFader.Update(gameTime.ElapsedGameTime);
            if (RSG == PlayState.PLAY)
            {
                CurrentMap.Update(gameTime);
            }
            if(Level > 0)
                SaveFadeTween.Update(gameTime.ElapsedGameTime);
            SaveTween.Update(gameTime.ElapsedGameTime);
            SavedTween.Update(gameTime.ElapsedGameTime);

            //CurrentRun.Update(gameTime.ElapsedGameTime);
            ActionTween.Update(gameTime.ElapsedGameTime);
            HandFade.Update(gameTime.ElapsedGameTime);
            HandMoveH.Update(gameTime.ElapsedGameTime);
            HandMoveV.Update(gameTime.ElapsedGameTime);
            foreach (var t in ScoreParticles)
            {
                t.Fade.Update(gameTime.ElapsedGameTime);
                t.Location = new Vector2(t.Location.X,
                    t.Location.Y - 1);
            }
            return base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
//Draw Menu and Interfave
            SpriteBatch.Begin();
            SpriteBatch.Draw(Textures.Hand, new Vector2(HandMoveH - 52, HandMoveV - Textures.Hand.Height),
                Color.White*HandFade);
            switch (RSG)
            {
                case PlayState.HAND_DOWN:
                case PlayState.HAND_UP:
                case PlayState.HAND_LEFT:
                case PlayState.HAND_RIGHT:
                case PlayState.HIDE_HAND:
                case PlayState.SHOW_HAND:
                    if (HandFade.IsComplete)
                        Selector.DrawSelector(SpriteBatch, new Vector2(Center.X - 52, Center.Y - Textures.Hand.Height),
                            new Vector2(HandMoveH, HandMoveV - Textures.Hand.Height));
                    break;
                case PlayState.WATCH:
                    SpriteBatch.Draw(Textures.TextWatch,
                        new Vector2(Center.X - Textures.TextWatch.Center().X, Center.Y - Textures.TextWatch.Center().Y),
                        Color.White * (float)Math.Sin(ActionTween * Math.PI));
                    break;
                case PlayState.READY:
                    SpriteBatch.Draw(Textures.TextReady,
                        new Vector2(Center.X - Textures.TextReady.Center().X, Center.Y - Textures.TextReady.Center().Y),
                        Color.White * (float)Math.Sin(ActionTween * Math.PI));
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

            SpriteBatch.Draw(Textures.Saved,
                        new Vector2(Center.X - Textures.Saved.Center().X, Center.Y - Textures.Saved.Center().Y),
                        Color.White * (float)Math.Sin(SavedTween * Math.PI));
            DrawProgress();
            foreach (var s in ScoreParticles)
            {
                var t = MiniScore.GetTextureFor(s.Score);
                SpriteBatch.Draw(t, new Vector2(s.Location.X - (t.Width/2f),s.Location.Y), Color.White*s.Fade);
            }
            ScoreParticles.RemoveAll(p => p.Fade.IsComplete);
            DrawScore();

            SpriteBatch.Draw(Textures.Save,
               new Vector2(0, (Game.GraphicsDevice.Viewport.Height - ((Textures.Save.Height * SaveTween.Value) + 30))), null, Color.White * SaveFadeTween.Value * FadeTween.Value, 0f, Vector2.Zero, SaveTween.Value, SpriteEffects.None, 1f);

          //  var rect =
          //     new Rectangle(0, (int)(Game.GraphicsDevice.Viewport.Height - ((Textures.Save.Height * SaveTween.Value))) - 30, (int)(Textures.Save.Width * SaveTween.Value), (int)(Textures.Save.Height * SaveTween.Value));
          //  Utilities.DrawRectangle(SpriteBatch, rect);
            //SpriteBatch.DrawString(Fonts.ArialSmall, Score + " Score" + ScoreParticles.Count + " Record" + RecordEvents.Count + "  Map " + CurrentMap.Events.Count, new Vector2(10, 10), Color.White);
            SpriteBatch.End();
            base.Draw(gameTime);
        }

        public void DrawScore()
        {
            var v = new Vector2(10, 10);
            SpriteBatch.Draw(Textures.BestScore, v, Color.White * .6f * FadeTween);
            v = new Vector2(v.X, v.Y + Textures.BestScore.Height);
            Utilities.DrawNumbers(SpriteBatch, Game.BestScore, v, Color.White * .6f * FadeTween);
            v = new Vector2(v.X, v.Y + 42);
            SpriteBatch.Draw(Textures.Score, v, Color.White * .6f * FadeTween);
            v = new Vector2(v.X, v.Y + Textures.Score.Height);
            Utilities.DrawNumbers(SpriteBatch, Score, v, Color.White * .6f * FadeTween);
            v = new Vector2(v.X, v.Y + 42);
            SpriteBatch.Draw(Textures.Required, v, Color.White * .6f * FadeTween);
            v = new Vector2(v.X, v.Y + Textures.Required.Height);
            var numOfFW = Math.Max(Math.Min(Level, MaxFireworksLevelTopOut), MinFireworks);
            numOfFW = (numOfFW / MaxFireworksLevelTopOut) * MaxFireworks;
            numOfFW = Math.Max(numOfFW, MinFireworks);
            var rscore = (int) ((numOfFW*MiniScore.GetScoreFor(MiniScores.GOOD)));
            Utilities.DrawNumbers(SpriteBatch, rscore, v, Color.White * .6f * FadeTween);
            v = new Vector2(Width-Textures.HighScore.Width, 10);
            SpriteBatch.Draw(Textures.HighScore, v, Color.White * .6f * FadeTween);
            var w = Utilities.GetNumWidth(Game.HighScore);
            v = new Vector2(Width - w, v.Y + Textures.HighScore.Height);
            Utilities.DrawNumbers(SpriteBatch, Game.HighScore, v, Color.White * .6f * FadeTween);

            v = new Vector2(Width - Textures.Score.Width, v.Y + 42);
            SpriteBatch.Draw(Textures.Score, v, Color.White * .6f * FadeTween);
            w = Utilities.GetNumWidth(GameScore);
            v = new Vector2(Width - w, v.Y + Textures.Score.Height);
            Utilities.DrawNumbers(SpriteBatch, GameScore, v, Color.White * .6f * FadeTween);

            v = new Vector2(Width - Textures.Level.Width, v.Y + 42);
            SpriteBatch.Draw(Textures.Level, v, Color.White * .6f * FadeTween);
            w = Utilities.GetNumWidth(Level);
            v = new Vector2(Width - w, v.Y + Textures.Level.Height);
            Utilities.DrawNumbers(SpriteBatch, Level, v, Color.White * .6f * FadeTween);
        }
        public void DrawProgress()
        {
            var rect = new Rectangle(10, (int)Height - 28, (int)Width - 20, 10);
            SpriteBatch.Draw(Textures.Bar, rect, Color.Gray * ExitTween * FadeTween);
            var rectc = new Rectangle(rect.Left, rect.Top, (int)(rect.Width * CurrentMap.Complete), rect.Height);
            SpriteBatch.Draw(Textures.Bar, rectc, Color.Orange * FadeTween);
            foreach (var e in CurrentMap.Events)
            {
                SpriteBatch.Draw(Textures.Tick, new Vector2((rect.Width*e.Time) + 10, rect.Top - 4),
                    (e.Time > CurrentMap.Complete ? Color.Gray : Color.Orange) * FadeTween);
            }
            if (!CurrentMap.HasEvents) return;
            foreach (var f in CurrentMap.Events)
            {
                if (!(f.Time < CurrentMap.Complete)) continue;
                var p = f.Location - new Vector2((Textures.Selector.Width / 2f) * f.Size, (Textures.Selector.Height / 2f) * f.Size);
                SpriteBatch.Draw(Textures.Selector, p, null, f.Color * SelectorFader * FadeTween, 0, Vector2.Zero, f.Size, SpriteEffects.None, 1);
            }
        }

        public float MinFireworks = 3;
        public float MaxFireworks = 12;
        public float MaxFireworksLevelTopOut = 30;
        public float MaxFWRelaseTime = 5f;
        public float MinFWRelaseTime = .7f;
        public float MinReleaseLevelTopOut = 30;
        public float SizeDeviationTopOut = 25;
        public float SizeMaxDeviation = 0.2f;
        public float ColorDeviationTopOut = 35;

        public void CreateMap()
        {
            
            //Determin number of fireworks 3-12 reach that at level 20+
            var numOfFW = Math.Max(Math.Min(Level, MaxFireworksLevelTopOut), MinFireworks);
            numOfFW = (numOfFW/MaxFireworksLevelTopOut)*MaxFireworks;
            numOfFW = Math.Max(numOfFW, MinFireworks);

            //Determin Time
            var time = (TimeSpan.TicksPerSecond * MinFWRelaseTime + (
                (MaxFWRelaseTime - MinFWRelaseTime) * (1f - (Level / MinReleaseLevelTopOut))));
            var ttime = (time * numOfFW) + (TimeSpan.TicksPerSecond);
            var ctime = (float)TimeSpan.TicksPerSecond ;
           
            //create first color
            var cv = Splosion.Random.NextFloat();
            
            var cd = Math.Max(Level, ColorDeviationTopOut) / ColorDeviationTopOut;

            //create first size;
            var s = Math.Max(.80f + (Splosion.Random.NextFloat()*.2f),1f);
            var sd = (Math.Max(Level, SizeDeviationTopOut) / SizeDeviationTopOut) * SizeMaxDeviation;

            var events = new List<FireWork>();
            for (var i = 0; i < (int) numOfFW; i++)
            {
                var loc = FireWorkArea.RandomPoint();

                var tme = ctime / ttime;                
                s = Splosion.Random.NextFloat() > .5f ? Math.Max(s - (Splosion.Random.NextFloat() * sd),.33f) : Math.Min(s - (Splosion.Random.NextFloat() * sd), 1f);

                cv = Splosion.Random.NextFloat() > .5f ? Math.Max(cv + (Splosion.Random.NextFloat() * cd), 1f) : Math.Min(cv - (Splosion.Random.NextFloat() * cd), .2f);
                var c = CreateColor(cv);
                events.Add(new FireWork(loc, c, s, tme));
                ctime += time;
            }
            CurrentMap = new Map(Game,events, new TimeSpan((long)ttime));
        }

        public Color CreateColor(float v)
        {
            var c = Color.Red;
            c = Color.Lerp(c, v > .5f ? Color.Green : Color.Blue, Splosion.Random.NextFloat());
            return c;
        }
    }
}
