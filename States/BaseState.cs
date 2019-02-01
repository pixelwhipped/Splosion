using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Splosion.Graphics;
using Splosion.Input;

namespace Splosion.States
{
    public class BaseState : GameState<Splosion>
    {
        public float Width { get { return Game.Width; } }
        public float Height { get { return Game.Height; } }
        public Vector2 Center { get { return Game.Center; } }
        public float SceneHorizon
        {
            get
            {
                return Game.SceneHorizon;
            }
        }
        public Rectangle FireWorkArea
        {
            get
            {
                return Game.FireWorkArea;
            }
        }

        public Rectangle FountainArea
        {
            get
            {
                return Game.FountainArea;
            }
        }

        public Rectangle PlayArena
        {
            get
            {
                return Game.PlayArena;
            }
        }

        public Selector Selector;

        public SpriteBatch SpriteBatch { get { return Game.SpriteBatch; } }
        public BaseState(Splosion game) : base(game)
        {
            Mouse = new MouseInput();
            Touch = new TouchInput();
            Selector = new Selector(game, Textures.Selector) {ShowRadius = false};
        }

        public override GameState<Splosion> Update(GameTime gameTime)
        {
            Mouse.Update(gameTime);
            Touch.Update(gameTime);
            Selector.Update(gameTime);
            base.Update(gameTime);
            return this;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            Selector.Draw(SpriteBatch);            
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
