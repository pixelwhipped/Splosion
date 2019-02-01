using System;
using Microsoft.Xna.Framework;
using Splosion.Input;

namespace Splosion
{
    public class GameState<T> where T : Game,ParentInterface
    {
        public readonly T Game;
        public GameState<T> CurrentState;
        public TouchInput Touch;
        public MouseInput Mouse;
        public GameState(T game)
        {
            Game = game;
            CurrentState = this;
            Touch = new TouchInput();            
            Mouse = new MouseInput();
        }
        public virtual GameState<T> Update(GameTime gameTime)
        {
            return this;
        }

        public virtual void Draw(GameTime gameTime)
        {
            
        }

        public void ShowToast(string toast, string title = "Achievement", TimeSpan? time = null)
        {
            Game.ShowToast(toast, title, time);
        }
    }
}
