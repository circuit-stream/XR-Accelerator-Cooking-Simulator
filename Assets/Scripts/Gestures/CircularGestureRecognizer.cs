using System;
using UnityEngine;

namespace XRAccelerator.Gestures
{
    public class CircularGestureRecognizer : GestureRecognizer<CircularGesture>
    {
        public override void AddGestureTracker(GestureKeyframeTracker gestureKeyframeTracker)
        {
            var newGesture = new CircularGesture(gestureKeyframeTracker);
            newGesture.OnStart += OnStart;
            
            gestures.Add(gestureKeyframeTracker, newGesture);
        }
    }
}