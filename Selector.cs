using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splosion.Input;

namespace Splosion
{
    public class Selector
    {
        public readonly Splosion Game;
        private readonly Texture2D _selector;
        private readonly MouseInput _mouse;
        private readonly TouchInput _touch;
        public Vector2 Center = Vector2.Zero;

        public bool ShowRadius;

        public List<SelectorEvent> SelectorListeners;

        public Selector(Splosion game, Texture2D selector)
        {
            Game = game;
            _selector = selector;
            _mouse = new MouseInput();
            _mouse.DraggedListeners.Add(PostEvent);
            _touch = new TouchInput();
            _touch.DraggedListeners.Add(PostEvent);
            SelectorListeners = new List<SelectorEvent>();
        }

        private void PostEvent(Vector2 DragFrom, Vector2 Location)
        {
            if (!Game.PlayArena.Contains(new Point((int) DragFrom.X, (int) DragFrom.Y))) return;
            var remove = new List<SelectorEvent>();
            var c = Color.Red;

            var s = .33f + Math.Min(Math.Abs(DragFrom.X - Location.X), 200f) / 300f;
            c = Color.Lerp(c, DragFrom.Y < Location.Y ? Color.Green : Color.Blue, Math.Min(Math.Abs(DragFrom.Y - Location.Y), 100f) / 100f);
            // var p = DragFrom - new Vector2((_selector.Width / 2f) * s, (_selector.Height / 2f) * s);

            foreach (var listener in SelectorListeners)
            {
                try
                {
                    listener(DragFrom,c,s);
                }
                catch
                {
                    remove.Add(listener);
                }
            }
            SelectorListeners.RemoveAll(remove.Contains);
            remove.Clear();
            //spriteBatch.Draw(_selector, p, null, c, 0, Vector2.Zero, s, SpriteEffects.None, 1); 
        }

        public void Update(GameTime gametime)
        {
            _mouse.Update(gametime);
            _touch.Update(gametime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!ShowRadius) return;
            if ((!Game.PlayArena.Contains(new Point((int) _mouse.DragFrom.X, (int) _mouse.DragFrom.Y))) &&
                !Game.PlayArena.Contains(new Point((int) _touch.DragFrom.X, (int) _touch.DragFrom.Y))) return;
            if (_mouse.DragFrom != Vector2.Zero)
            {
                DrawSelector(spriteBatch, _mouse.DragFrom, _mouse.Location);
            }
            if (_touch.DragFrom != Vector2.Zero)
            {
                DrawSelector(spriteBatch, _touch.DragFrom, _touch.Location);
            }
        }
        public void DrawSelector(SpriteBatch spriteBatch, Vector2 DragFrom, Vector2 Location)
        {
            var c = Color.Red;
            var s = .33f + Math.Min(Math.Abs(DragFrom.X - Location.X), 200f) / 300f;
            c = Color.Lerp(c, DragFrom.Y < Location.Y ? Color.Green : Color.Blue, Math.Min(Math.Abs(DragFrom.Y - Location.Y), 100f) / 100f);
            var p = DragFrom - new Vector2((_selector.Width / 2f) * s, (_selector.Height / 2f) * s);
            spriteBatch.Draw(_selector, p, null, c, 0, Vector2.Zero, s, SpriteEffects.None, 1);            
        }
    }
}
