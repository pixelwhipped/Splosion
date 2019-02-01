using System;
using Microsoft.Xna.Framework;
using Splosion.Audio;
using Splosion.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Splosion.States
{
    public class FreePlay : ExitableState
    {
        public void Tap(Vector2 t)
        {

        }

        public enum PlayState
        {
            READY, SET, GO, PLAY
        }
        public PlayState RSG;
        public Tween ActionTween;
        public FreePlay(Splosion game)
            : base(game)
        {
            RSG = PlayState.READY;
            ActionTween = new Tween(new TimeSpan(0, 0, 0, 1), 0, 1);
            ActionTween.AddEventListener(.5f,t =>
            {
                switch (RSG)
                {
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
                        RSG = PlayState.PLAY;
                        Selector.ShowRadius = true;
                        Selector.SelectorListeners.Add(game.AddFireWorks);
                        break;
                    case PlayState.PLAY:
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


            return base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            switch (RSG)
            {
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
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
