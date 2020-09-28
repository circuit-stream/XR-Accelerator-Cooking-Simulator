using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gestures
{
    public class CircularGesture : Gesture<CircularGesture>
    {
        public const int AllowedJitter = 20;

        private struct CircularGestureKeyframe
        {
            // angle between vector.up and vector previousKeyframePos -> currentKeyframePos
            public float Angle;

            // Difference between previous keyframe angle and current keyframe angle
            public float DeltaAngle;

            // If the new keyframe rotated clockwise / counter-clockwise
            public int RotationDirection => DeltaAngle > 0 ? 1 : (DeltaAngle < 0 ? -1 : 0);

            public GestureKeyframe GestureKeyframe;
        }

        public float AngularVelocity => circularKeyframesDeltaAngleSum / circularKeyframes.Count;
        public bool IsMovingCircularly;

        private readonly LinkedList<CircularGestureKeyframe> circularKeyframes;

        private float circularKeyframesDeltaAngleSum;
        private int circularKeyframesRotationDirectionSum;

        public CircularGesture()
        {
            circularKeyframes = new LinkedList<CircularGestureKeyframe>();
        }

        protected override bool CanStart()
        {
            SetIsMovingCircularly();
            return IsMovingCircularly;
        }

        protected override bool UpdateGesture()
        {
            SetIsMovingCircularly();

            if (!IsMovingCircularly)
                Finish();

            return IsMovingCircularly;
        }

        protected override void OnAddedKeyframe(GestureKeyframe gestureKeyframe)
        {
            var newKeyframe = circularKeyframes.Count == 0 ? CreateEmptyKeyframe(gestureKeyframe) : CreateKeyframeFromPrevious(gestureKeyframe);
            circularKeyframes.AddLast(newKeyframe);

            circularKeyframesDeltaAngleSum += newKeyframe.DeltaAngle;
            circularKeyframesRotationDirectionSum += newKeyframe.RotationDirection;
        }

        protected override void OnRemovedKeyframe(GestureKeyframe gestureKeyframe)
        {
            var removedKeyframe = circularKeyframes.First.Value;
            Debug.Assert(removedKeyframe.GestureKeyframe == gestureKeyframe, "Removed circular keyframe doesn't match the gestureKeyframe");

            circularKeyframes.RemoveFirst();

            circularKeyframesDeltaAngleSum -= removedKeyframe.DeltaAngle;
            circularKeyframesRotationDirectionSum -= removedKeyframe.RotationDirection;
        }

        private CircularGestureKeyframe CreateKeyframeFromPrevious(GestureKeyframe gestureKeyframe)
        {
            var previousKeyframe = circularKeyframes.Last.Value;
            var direction = gestureKeyframe.HorizontalPlanePosition - previousKeyframe.GestureKeyframe.HorizontalPlanePosition;
            var angle = Vector2.SignedAngle(Vector2.up, direction) + 180;

            return new CircularGestureKeyframe()
            {
                Angle = angle,
                DeltaAngle = GetDeltaAngle(angle, previousKeyframe.Angle),
                GestureKeyframe = gestureKeyframe
            };
        }

        private CircularGestureKeyframe CreateEmptyKeyframe(GestureKeyframe gestureKeyframe)
        {
            return new CircularGestureKeyframe()
            {
                Angle = 0,
                DeltaAngle = 0,
                GestureKeyframe = gestureKeyframe
            };
        }

        private float GetDeltaAngle(float newAngle, float previousAngle)
        {
            if (previousAngle > 270 && newAngle < 90)
                newAngle += 360;

            else if (newAngle > 270 && previousAngle < 90)
                previousAngle += 360;

            return newAngle - previousAngle;
        }

        private void SetIsMovingCircularly()
        {
            IsMovingCircularly = Mathf.Abs(circularKeyframesRotationDirectionSum) >
                                 gestureKeyframeTracker.SampleSize - AllowedJitter;
        }
    }
}