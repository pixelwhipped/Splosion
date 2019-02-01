using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Splosion.Effects
{
    public class RippleEffect
    {
        public static implicit operator Effect(RippleEffect e)
        {
            return e._ripple;
        }

        private readonly Effect _ripple;
        private readonly Tween _tween;

        public RippleEffect(GraphicsDevice device)
        {
            
            _tween = new Tween(TimeSpan.FromSeconds(3), 1f, -1f, true);
            _ripple = new Effect(device, Splosion.LoadStream(@"Content\Effects\ripple.mgfxo"));
            _ripple.Parameters["wave"].SetValue(1.75f);
            _ripple.Parameters["distortion"].SetValue(0f);
            _ripple.Parameters["centerCoord"].SetValue(new Vector2(0.5f, 0.5f));
//            _ripple.Parameters["Phase"].SetValue(.5f);

        }

        public void Update(GameTime gameTime)
        {
            _tween.Update(gameTime.ElapsedGameTime);
            _ripple.Parameters["distortion"].SetValue((float)Math.Sin(_tween * MathHelper.PiOver2));
        }
    }
}
