using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Splosion.Input
{
    public class MouseInput
    {
        public List<Procedure<Vector2>> LeftClickListeners;
        public List<Procedure<Vector2>> RightClickListeners;
        public List<Procedure<Vector2>> MiddleClickListeners;
        public List<Operation<Vector2>> DraggingListeners;
        public List<Operation<Vector2>> DraggedListeners;

        public Vector2 DragFrom = Vector2.Zero;

        public List<Procedure<Vector2>> MoveListeners;

        public MouseState State;

        public Vector2 Location
        {
            get
            {
                return new Vector2(State.X, State.Y);
            }
        }

        public MouseInput()
        {
            State = Mouse.GetState();
            LeftClickListeners = new List<Procedure<Vector2>>();
            RightClickListeners = new List<Procedure<Vector2>>();
            MiddleClickListeners = new List<Procedure<Vector2>>();
            MoveListeners = new List<Procedure<Vector2>>();
            DraggingListeners = new List<Operation<Vector2>>();
            DraggedListeners = new List<Operation<Vector2>>();
        }
        public void Update(GameTime gameTime)
        {
            
            var currentState = Mouse.GetState();
            

            var remove = new List<Procedure<Vector2>>();
            var previous = new Vector2(State.X, State.Y);
            var current = new Vector2(currentState.X, currentState.Y);

            var dremove = new List<Operation<Vector2>>();
            if (currentState.LeftButton == ButtonState.Pressed && State.LeftButton == ButtonState.Released)
            {
                DragFrom = current;
            }
            if (currentState.LeftButton == ButtonState.Released && State.LeftButton == ButtonState.Pressed)
            {
                foreach (var listener in DraggedListeners)
                {
                    try
                    {
                        listener(DragFrom, Location);
                    }
                    catch
                    {
                        dremove.Add(listener);
                    }
                }
                DraggedListeners.RemoveAll(dremove.Contains);
                dremove.Clear();
                DragFrom = Vector2.Zero;
            }

            #region Move
            if (previous != current)
            {
                var delta = current - previous;
                if (Math.Abs(delta.X) < 4 || Math.Abs(delta.Y) < 4)
                {
                    foreach (var moveListener in MoveListeners)
                    {
                        try
                        {
                            moveListener(current);
                        }
                        catch
                        {
                            remove.Add(moveListener);
                        }
                    }
                    MoveListeners.RemoveAll(remove.Contains);
                    foreach (var listener in DraggingListeners)
                    {
                        try
                        {
                            listener(DragFrom, Location);
                        }
                        catch
                        {
                            dremove.Add(listener);
                        }
                    }
                    DraggingListeners.RemoveAll(dremove.Contains);
                    
                }
            }
            dremove.Clear();
            remove.Clear();
            #endregion

            #region Left Click

            if (State.LeftButton == ButtonState.Released && currentState.LeftButton == ButtonState.Pressed)
            {
                foreach (var listener in LeftClickListeners)
                {
                    try
                    {
                        listener(current);
                    }
                    catch
                    {
                        remove.Add(listener);
                    }
                }
                LeftClickListeners.RemoveAll(remove.Contains);
                remove.Clear();
            }
            #endregion

            #region Right Click

            if (State.RightButton == ButtonState.Released && currentState.RightButton == ButtonState.Pressed)
            {
                foreach (var listener in RightClickListeners)
                {
                    try
                    {
                        listener(current);
                    }
                    catch
                    {
                        remove.Add(listener);
                    }
                }
                RightClickListeners.RemoveAll(remove.Contains);
                remove.Clear();
            }
            #endregion

            #region Middle Click

            if (State.MiddleButton == ButtonState.Released && currentState.MiddleButton == ButtonState.Pressed)
            {
                foreach (var listener in MiddleClickListeners)
                {
                    try
                    {
                        listener(current);
                    }
                    catch
                    {
                        remove.Add(listener);
                    }
                }
                MiddleClickListeners.RemoveAll(remove.Contains);
                remove.Clear();
            }
            #endregion

            State = currentState;
        }        
    }
}
