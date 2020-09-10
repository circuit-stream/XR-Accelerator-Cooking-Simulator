using UnityEngine;

namespace XRAccelerator.Gestures
{
    public class GestureKeyframe
    {
        public Vector3 TrackedPosition;
        
        public Vector2 HorizontalPlanePosition => new Vector2(TrackedPosition.x, TrackedPosition.z);
    }
}