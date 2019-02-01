using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Splosion.Effects
{
    public class BloomEffect
    {
        public static implicit operator Effect(BloomEffect e)
        {
            return e._bloom;
        }

        private readonly Effect _bloom;
        private readonly Tween _tween;

        public BloomEffect(GraphicsDevice device)
        {
            _tween = new Tween(TimeSpan.FromSeconds(3), 1f, -1f, true);
            _bloom = new Effect(device, Splosion.LoadStream(@"Content\Effects\bloom.mgfxo"));
            _bloom.Parameters["BaseIntensity"].SetValue(.25f);
            _bloom.Parameters["BaseSaturation"].SetValue(.5f);
            _bloom.Parameters["BloomIntensity"].SetValue(1f);
            _bloom.Parameters["BloomSaturation"].SetValue(.75f);

        }

        public void Update(GameTime gameTime)
        {
            _tween.Update(gameTime.ElapsedGameTime);
           // _bloom.Parameters["Phase"].SetValue((float)Math.Sin(_tween * MathHelper.PiOver2));
        }
    }
}
