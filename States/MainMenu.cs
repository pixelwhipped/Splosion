using System;
using Windows.UI.Notifications;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splosion.Audio;
using Splosion.Graphics;

namespace Splosion.States
{
    public class MainMenu : BaseState
    {
        public TimeSpan NextExplosion;
        public Tween PlayTween;
        public Tween FreePlayTween;
        public Tween RushTween;
        public Tween ContinueTween;
        public Tween FadeTween;
        public bool Choice;
        public bool FreePlay;
        public bool Play;
        public bool Rush;
        public bool Continue;

        public void Tap(Vector2 t)
        {
            if (Choice) return;
            
            var pRect =
                new Rectangle((int)((Width - (Textures.TextPlay.Width * PlayTween.Value)) / 2), 300, (int)(Textures.TextPlay.Width * PlayTween.Value), (int)(Textures.TextPlay.Height * PlayTween.Value));

            var fpRect =
                new Rectangle((int)((Width - (Textures.TextFreePlay.Width * FreePlayTween.Value)) / 2), 372, (int)(Textures.TextFreePlay.Width * FreePlayTween.Value), (int)(Textures.TextFreePlay.Height * FreePlayTween.Value));

            var rRect =
            new Rectangle((int)((Width - (Textures.Continue.Width * ContinueTween.Value)) / 2), 444, (int)(Textures.Continue.Width * RushTween.Value), (int)(Textures.Continue.Height * ContinueTween.Value));


            var cRect =
              new Rectangle((int)((Width - (Textures.Continue.Width * ContinueTween.Value)) / 2), 516, (int)(Textures.Continue.Width * ContinueTween.Value), (int)(Textures.Continue.Height * ContinueTween.Value));


      
            if (fpRect.Contains(new Point((int) t.X, (int) t.Y)))
            {
                Choice = true;
                Game.Audio.Play(Sounds.Menu);
                FreePlay = true;
                FadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 1f, 0f);
            }

            if (rRect.Contains(new Point((int)t.X, (int)t.Y)))
            {
                Choice = true;
                Game.Audio.Play(Sounds.Menu);
                Rush = true;
                FadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 1f, 0f);
            }

            else if (pRect.Contains(new Point((int) t.X, (int) t.Y)))
            {
                Choice = true;
                Game.Audio.Play(Sounds.Menu);
                Play = true;
                FadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 1f, 0f);
            }
            else if (cRect.Contains(new Point((int)t.X, (int)t.Y)) && Game.GameSettings.Data.Level != 0)
            {
                Choice = true;
                Game.Audio.Play(Sounds.Menu);
                Continue = true;
                Game.ShownHint = true;
                FadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 1f, 0f);
            }            
        }
        public MainMenu(Splosion game) : base(game)
        {
            NextExplosion = new TimeSpan(0, 0, 0, 1);
            PlayTween = new Tween(new TimeSpan(0, 0, 0, 2), 1f, 1.1f, true);
            FreePlayTween = new Tween(new TimeSpan(0, 0, 0, 2), 1.1f, 1f, true);
            RushTween = new Tween(new TimeSpan(0, 0, 0, 2), 1f, 1.1f, true);
            ContinueTween = new Tween(new TimeSpan(0, 0, 0, 2), 1.1f, 1f, true);
            FadeTween = new Tween(new TimeSpan(0, 0, 0, 2), 0f, 1f);
            Selector.ShowRadius = false;            
            Touch.TapListeners.Add(Tap);
            //Touch.MoveListeners.Add(Tap);
            Mouse.LeftClickListeners.Add(Tap); 
        }

        public override GameState<Splosion> Update(GameTime gameTime)
        {
            var pRect =
                new Rectangle((int)((Width - (Textures.TextPlay.Width * PlayTween.Value)) / 2), 300, (int)(Textures.TextPlay.Width * PlayTween.Value), (int)(Textures.TextPlay.Height * PlayTween.Value));

            var fpRect =
                new Rectangle((int)((Width - (Textures.TextFreePlay.Width * FreePlayTween.Value)) / 2), 372, (int)(Textures.TextFreePlay.Width * FreePlayTween.Value), (int)(Textures.TextFreePlay.Height * FreePlayTween.Value));

            var rRect =
            new Rectangle((int)((Width - (Textures.Continue.Width * ContinueTween.Value)) / 2), 444, (int)(Textures.Continue.Width * RushTween.Value), (int)(Textures.Continue.Height * ContinueTween.Value));


            var cRect =
              new Rectangle((int)((Width - (Textures.Continue.Width * ContinueTween.Value)) / 2), 516, (int)(Textures.Continue.Width * ContinueTween.Value), (int)(Textures.Continue.Height * ContinueTween.Value));



            if(!fpRect.Contains(new Point((int)Mouse.Location.X,(int)Mouse.Location.Y)))
                FreePlayTween.Update(gameTime.ElapsedGameTime);
            if (!pRect.Contains(new Point((int)Mouse.Location.X, (int)Mouse.Location.Y)))
                PlayTween.Update(gameTime.ElapsedGameTime);
            if (!rRect.Contains(new Point((int)Mouse.Location.X, (int)Mouse.Location.Y)))
                RushTween.Update(gameTime.ElapsedGameTime);
            if (!cRect.Contains(new Point((int)Mouse.Location.X, (int)Mouse.Location.Y)))
                ContinueTween.Update(gameTime.ElapsedGameTime);
            FadeTween.Update(gameTime.ElapsedGameTime);

            if (Choice)
            {
                if (FadeTween.IsComplete && FreePlay)
                {
                    return new FreePlay(Game);
                }
                if (FadeTween.IsComplete && Rush)
                {
                    return new Rush(Game);
                }
                if (FadeTween.IsComplete && Play)
                {
                    return new Play(Game);
                }
                if (FadeTween.IsComplete && Continue)
                {
                    return new Play(Game, Game.GameSettings.Data.GameScore,Game.GameSettings.Data.Level);
                }
            }
            else
            {
                NextExplosion -= gameTime.ElapsedGameTime;

                if (NextExplosion <= TimeSpan.Zero)
                {
                    NextExplosion = new TimeSpan(0, 0, 0, (int) (Splosion.Random.NextFloat()*2),
                        50 + (int) (Splosion.Random.NextFloat()*150));                        
                    var c = Color.Red;
                    c = Color.Lerp(c, Splosion.Random.NextFloat() > .5f ? Color.Green : Color.Blue, Splosion.Random.NextFloat());
                    Game.AddFireWorks(FireWorkArea.RandomPoint(), c, Splosion.Random.NextFloat());
                }
            }
            base.Update(gameTime);
            return this;
        }

        public override void Draw(GameTime gameTime)
        {//Draw Menu and Interfave
            SpriteBatch.Begin();
            SpriteBatch.Draw(Textures.LogoLarge, new Vector2((Width- Textures.LogoLarge.Width) / 2, 100), Color.White * FadeTween.Value);
            SpriteBatch.Draw(Textures.TextPlay,
                new Vector2((Width - (Textures.TextPlay.Width * PlayTween.Value)) / 2, 300), null, Color.White * FadeTween.Value, 0f, Vector2.Zero, PlayTween.Value, SpriteEffects.None, 1f);
            SpriteBatch.Draw(Textures.TextFreePlay,
                new Vector2((Width - (Textures.TextFreePlay.Width * FreePlayTween.Value)) / 2, 372), null, Color.White * FadeTween.Value, 0f, Vector2.Zero, FreePlayTween.Value, SpriteEffects.None, 1f);
            SpriteBatch.Draw(Textures.TextRush,
               new Vector2((Width - (Textures.TextRush.Width * RushTween.Value)) / 2, 444), null, Color.White * FadeTween.Value, 0f, Vector2.Zero, RushTween.Value, SpriteEffects.None, 1f);
     
            if(Game.GameSettings.Data.Level !=0)
                SpriteBatch.Draw(Textures.Continue,
                    new Vector2((Width - (Textures.Continue.Width * ContinueTween.Value)) / 2, 516), null, Color.White * FadeTween.Value, 0f, Vector2.Zero, ContinueTween.Value, SpriteEffects.None, 1f);
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
