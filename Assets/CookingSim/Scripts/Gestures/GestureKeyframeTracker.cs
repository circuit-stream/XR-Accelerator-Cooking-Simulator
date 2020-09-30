using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gestures
{
    public class GestureKeyframeTracker
    {
        public event Action<GestureKeyframe> OnAddedKeyframe;
        public event Action<GestureKeyframe> OnRemovedKeyframe;

        public readonly Transform TrackedTransform;
        public readonly int SampleSize;
        private readonly float tickTime;
        private readonly LinkedList<GestureKeyframe> keyframes;

        private float currentTickTime;

        private void AddKeyframe()
        {
            var newKeyframe = new GestureKeyframe {TrackedPosition = TrackedTransform.position};

            keyframes.AddLast(newKeyframe);
            OnAddedKeyframe?.Invoke(newKeyframe);

            if (keyframes.Count > SampleSize)
            {
                var removedKeyframe = keyframes.First.Value;
                keyframes.RemoveFirst();
                OnRemovedKeyframe?.Invoke(removedKeyframe);
            }
        }

        public GestureKeyframeTracker(Transform transform, int sampleSize = 40, float tickTime = 0.05f)
        {
            keyframes = new LinkedList<GestureKeyframe>();

            TrackedTransform = transform;
            SampleSize = sampleSize;
            this.tickTime = tickTime;

            // This way the next update will immediately a addKeyframe
            // and gesture recognizers will have the time to register for the callbacks
            currentTickTime = tickTime;
        }

        public void Update()
        {
            currentTickTime += Time.deltaTime;
            if (currentTickTime < tickTime)
                return;

            currentTickTime = 0;
            AddKeyframe();
        }
    }
}