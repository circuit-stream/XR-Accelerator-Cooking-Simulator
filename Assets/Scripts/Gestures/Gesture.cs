using System;
using UnityEngine;

namespace XRAccelerator.Gestures
{
    public abstract class Gesture<T> where T : Gesture<T>
    {
        public event Action<T> OnStart;
        public event Action<T> OnUpdated;
        public event Action<T> OnFinished;
        public event Action<T> OnCancel;

        public Transform TrackedTransform => gestureKeyframeTracker.TrackedTransform;

        protected readonly GestureKeyframeTracker gestureKeyframeTracker;
        protected bool IsGestureActive;
        
        public Gesture(GestureKeyframeTracker gestureKeyframeTracker)
        {
            this.gestureKeyframeTracker = gestureKeyframeTracker;

            gestureKeyframeTracker.OnAddedKeyframe += OnAddedKeyframe;
            gestureKeyframeTracker.OnRemovedKeyframe += OnRemovedKeyframe;
        }
        
        public void Update()
        {
            if (!IsGestureActive && CanStart())
            {
                Start();
                return;
            }

            if (IsGestureActive && UpdateGesture())
            {
                OnUpdated?.Invoke(this as T);
            }
        }

        public virtual void Cancel()
        {
            IsGestureActive = false;
            OnCancel?.Invoke(this as T);
        }

        protected virtual void Finish()
        {
            IsGestureActive = false;
            OnFinished?.Invoke(this as T);
        }
        
        protected virtual void Start()
        {
            Debug.Assert(IsGestureActive == false, "Starting already started gesture");
            
            IsGestureActive = true;
            OnStart?.Invoke(this as T);
        }
        
        protected abstract bool CanStart();
        protected abstract bool UpdateGesture();
        protected abstract void OnAddedKeyframe(GestureKeyframe gestureKeyframe);
        protected abstract void OnRemovedKeyframe(GestureKeyframe gestureKeyframe);
    }
}