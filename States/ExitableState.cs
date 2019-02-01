using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Splosion.Audio;
using Splosion.Graphics;

namespace Splosion.States
{
    public class ExitableState:BaseState
    {
        public Tween FadeTween;
        public Tween ExitTween;
        public bool Exit;

        public void CheckExit(Vector2 t)
        {
            if (Exit) return;
            var rect =
                new Rectangle((int)(Game.GraphicsDevice.Viewport.Width - (Textures.TextExit.Width * ExitTween.Value)), (int)(Game.GraphicsDevice.Viewport.Height - ((Textures.TextExit.Height * ExitTween.Value)))-30, (int)(Textures.TextExit.Width * ExitTween.Value), (int)(Textures.TextExit.Height * ExitTween.Value));

            if (!rect.Contains(new Point((int) t.X, (int) t.Y))) return;
            Game.Audio.Play(Sounds.Menu);
            Exit = true;
            FadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 1f, 0f);
        }
        public ExitableState(Splosion game) : base(game)
        {
            FadeTween = new Tween(new TimeSpan(0, 0, 0, 2), 0f, 1f);
            ExitTween = new Tween(new TimeSpan(0, 0, 0, 2), 1f, 1.1f, true);
            Mouse.LeftClickListeners.Add(CheckExit);
            //Touch.MoveListeners.Add(CheckExit);
            Touch.TapListeners.Add(CheckExit);
        }

        public override GameState<Splosion> Update(GameTime gameTime)
        {
            ExitTween.Update(gameTime.ElapsedGameTime);
            FadeTween.Update(gameTime.ElapsedGameTime);
            if (Keyboard.GetState().GetPressedKeys().Contains(Keys.Escape))
            {
                Game.Audio.Play(Sounds.Menu);
                Exit = true;
                FadeTween = new Tween(new TimeSpan(0, 0, 0, 1), 1f, 0f);
            }
            if (Exit && FadeTween.IsComplete)
            {
                return new MainMenu(Game);
            }
            base.Update(gameTime);
            return this;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();            
            SpriteBatch.Draw(Textures.TextExit,
                new Vector2((Game.GraphicsDevice.Viewport.Width - (Textures.TextExit.Width * ExitTween.Value)), (Game.GraphicsDevice.Viewport.Height - ((Textures.TextExit.Height * ExitTween.Value)+30))), null, Color.White * FadeTween.Value, 0f, Vector2.Zero, ExitTween.Value, SpriteEffects.None, 1f);
//#if ShowBounds
            //var rect =
           //    new Rectangle((int)(Game.GraphicsDevice.Viewport.Width - (Textures.TextExit.Width * ExitTween.Value)), (int)(Game.GraphicsDevice.Viewport.Height - ((Textures.TextExit.Height * ExitTween.Value))) - 30, (int)(Textures.TextExit.Width * ExitTween.Value), (int)(Textures.TextExit.Height * ExitTween.Value));
           // Utilities.DrawRectangle(SpriteBatch, rect);
//#endif
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

