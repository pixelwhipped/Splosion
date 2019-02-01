using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace Splosion.Input
{
    public class TouchInput
    {        
        public List<Procedure<Vector2>> TapListeners;
        public List<Procedure<Vector2>> MoveListeners;
        public List<Operation<Vector2>> DraggingListeners;
        public List<Operation<Vector2>> DraggedListeners;

        public Vector2 DragFrom = Vector2.Zero;
        public Vector2 Location = Vector2.Zero;
        public TouchInput()
        {            
            TapListeners = new List<Procedure<Vector2>>();
            MoveListeners = new List<Procedure<Vector2>>();
            DraggingListeners = new List<Operation<Vector2>>();
            DraggedListeners = new List<Operation<Vector2>>();   
        }

        public void Update(GameTime gameTime)
        {

            Vector2 delta;   
            var currentTouchState = TouchPanel.GetState();


            var first = true;
            var dremove = new List<Operation<Vector2>>();
            //If no touches the notify existing drag listeners
            if (currentTouchState.Count == 0)
            {
                if (DragFrom != Vector2.Zero && DragFrom != Location)
                {
                    foreach (var listener in DraggedListeners)
                    {
                        try
                        {
                            delta = DragFrom - Location;
                            //Only Notify if changed
                            if (Math.Abs(delta.X) > 20 || Math.Abs(delta.Y) > 20)
                                listener(DragFrom, Location);
                        }
                        catch
                        {
                            dremove.Add(listener);
                        }
                    }
                    DraggedListeners.RemoveAll(dremove.Contains);
                    dremove.Clear();
                }
                Location = Vector2.Zero;
                DragFrom = Vector2.Zero;
            }

            foreach (var touch in currentTouchState)
            {
                TouchLocation prevLoc;
                if (first)
                {
                    if ((touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved) && DragFrom == Vector2.Zero)
                    {
                        DragFrom = touch.Position;
                    }

                    delta = touch.Position - Location;
                    Location = touch.Position;
                    //Only Notify if changed
                    if (Math.Abs(delta.X) > 4 || Math.Abs(delta.Y) > 4)
                    {

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
                        dremove.Clear();
                    }
                    first = false;
                }
                var remove = new List<Procedure<Vector2>>();

                if (touch.State != TouchLocationState.Pressed)
                {
                    foreach (var tapListener in TapListeners)
                    {
                        try
                        {
                            //var t = new Vector2(touch.Position.X, touch.Position.Y);
                            //delta = Location - t;
                            //if ((Math.Abs(delta.X) > 4 || Math.Abs(delta.Y) > 4) || DragFrom == Location)
                            tapListener(new Vector2(touch.Position.X, touch.Position.Y));
                        }
                        catch
                        {
                            remove.Add(tapListener);
                        }
                    }
                    TapListeners.RemoveAll(remove.Contains);
                }
                remove.Clear();

                if (!touch.TryGetPreviousLocation(out prevLoc))
                    continue;
                

                if (prevLoc.State == TouchLocationState.Moved)
                {
                    delta = touch.Position - prevLoc.Position;
                    if (Math.Abs(delta.X) > 2 || Math.Abs(delta.Y) > 2)
                    {
                        foreach (var moveListener in MoveListeners)
                        {
                            try
                            {
                                moveListener(new Vector2(touch.Position.X, touch.Position.Y));
                            }
                            catch
                            {
                                remove.Add(moveListener);
                            }
                        }
                        MoveListeners.RemoveAll(remove.Contains);
                    }
                }
                remove.Clear();
                // if (touch.State != TouchLocationState.Released) continue;

                //Notify Tap Listeners on Release
                
            }
        }

    }
}
