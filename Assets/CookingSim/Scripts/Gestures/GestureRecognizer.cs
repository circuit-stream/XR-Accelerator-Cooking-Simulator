using System;
using System.Collections.Generic;

namespace XRAccelerator.Gestures
{
    public abstract class GestureRecognizer<T> where T : Gesture<T>, new()
    {
        public event Action<T> OnGestureStarted;

        protected readonly Dictionary<GestureKeyframeTracker, T> gestures = new Dictionary<GestureKeyframeTracker, T>();

        public void AddGestureTracker(GestureKeyframeTracker gestureKeyframeTracker)
        {
            var newGesture = new T();
            newGesture.SetGestureKeyframeTracker(gestureKeyframeTracker);
            newGesture.OnStart += OnStart;
            gestures.Add(gestureKeyframeTracker, newGesture);
        }

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