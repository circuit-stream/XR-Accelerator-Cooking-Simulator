using System;
using System.Collections.Generic;
using System.Linq;

namespace XRAccelerator.Gestures
{
    public abstract class GestureRecognizer<T> where T : Gesture<T>
    {
        public event Action<T> OnGestureStarted;

        protected readonly Dictionary<GestureKeyframeTracker, T> gestures = new Dictionary<GestureKeyframeTracker, T>();

        public abstract void AddGestureTracker(GestureKeyframeTracker gestureKeyframeTracker);

        public void RemoveGestureTracker(GestureKeyframeTracker gestureKeyframeTracker)
        {
            var gesture = gestures[gestureKeyframeTracker];
            gesture.Cancel();

            gestures.Remove(gestureKeyframeTracker);
        }
        
        public void Update()
        {
            foreach (var entry in gestures)
            {
                entry.Value.Update();
            }
        }
        
        protected void OnStart(T gesture)
        {
            OnGestureStarted?.Invoke(gesture);
        }
    }
}