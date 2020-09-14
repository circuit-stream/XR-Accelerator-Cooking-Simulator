using System;
using UnityEngine;

namespace XRAccelerator.Gestures
{
    public abstract class Gesture<T> where T : Gesture<T>, new()
    {
        public event Action<T> OnStart;
        public event Action<T> OnUpdated;
        public event Action<T> OnFinished;
        public event Action<T> OnCancel;

        public Transform TrackedTransform => gestureKeyframeTracker.TrackedTransform;

        protected GestureKeyframeTracker gestureKeyframeTracker;
        private bool isGestureActive;
        
        public void SetGestureKeyframeTracker(GestureKeyframeTracker keyframeTracker)
        {
            gestureKeyframeTracker = keyframeTracker;
            gestureKeyframeTracker.OnAddedKeyframe += OnAddedKeyframe;
            gestureKeyframeTracker.OnRemovedKeyframe += OnRemovedKeyframe;
        }
        
        public void Update()
        {
            if (!isGestureActive && CanStart())
            {
                Start();
                return;
            }

            if (isGestureActive && UpdateGesture())
            {
                OnUpdated?.Invoke(this as T);
            }
        }

        public void Cancel()
        {
            isGestureActive = false;
            OnCancel?.Invoke(this as T);
        }

        protected void Finish()
        {
            isGestureActive = false;
            OnFinished?.Invoke(this as T);
        }

        private void Start()
        {
            Debug.Assert(isGestureActive == false, "Starting already started gesture");
            
            isGestureActive = true;
            OnStart?.Invoke(this as T);
        }
        
        protected abstract bool CanStart();
        protected abstract bool UpdateGesture();
        protected abstract void OnAddedKeyframe(GestureKeyframe gestureKeyframe);
        protected abstract void OnRemovedKeyframe(GestureKeyframe gestureKeyframe);
    }
}