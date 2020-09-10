using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gestures
{
    public class GestureInteractor : MonoBehaviour
    {
        public CircularGestureRecognizer circularGestureRecognizer;

        private Dictionary<Transform, GestureKeyframeTracker> trackedTransforms;

        public void TrackGestures(Transform transform)
        {
            var tracker = new GestureKeyframeTracker(transform);
            trackedTransforms.Add(transform, tracker);
            
            circularGestureRecognizer.AddGestureTracker(tracker);
        }
        
        public void StopTrackingGestures(Transform transform)
        {
            var tracker = trackedTransforms[transform];
            trackedTransforms.Remove(transform);
            
            circularGestureRecognizer.RemoveGestureTracker(tracker);
        }

        private void Update()
        {
            foreach (var trackedTransform in trackedTransforms)
            {
                trackedTransform.Value.Update();
            }
            
            circularGestureRecognizer.Update();
        }

        private void Awake()
        {
            circularGestureRecognizer = new CircularGestureRecognizer();
            trackedTransforms = new Dictionary<Transform, GestureKeyframeTracker>();
        }
    }
}