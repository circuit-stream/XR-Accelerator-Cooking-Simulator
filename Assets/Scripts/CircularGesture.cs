using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator
{
    public class CircularGesture : MonoBehaviour
    {
        private struct GestureKeyframe
        {
            public Vector2 position;
            public float angle;
            public float deltaAngle;
            public float deltaPos;

            public int ComboDirection => deltaAngle > 0 ? 1 : (deltaAngle < 0 ? -1 : 0);
        }

        private float Velocity => sampleTraveledDistance / sampleSize;
        private float AngularVelocity => sampleAngularDistance / sampleSize;

        [SerializeField] private int allowedJitter = 10;
        [SerializeField] private int sampleSize = 50;
        [SerializeField] private float tickTime = 0.05f;

        [SerializeField] private float targetAngularVelocity = 20;
        [SerializeField] private float allowedAngularVelocityDelta = 5;

        private float currentTickTime;
        private LinkedList<GestureKeyframe> sampleKeyframes;

        private float sampleTraveledDistance;
        private float sampleAngularDistance;
        private int currentComboDirections;

        private Vector2 HorizontalPosition => new Vector2(transform.position.x, transform.position.z);

        public bool IsMovingCircularly()
        {
            return Mathf.Abs(currentComboDirections) > sampleSize - allowedJitter;
        }

        public bool IsMovingAtTargetVelocity()
        {
            var absAngularVelocity = Mathf.Abs(AngularVelocity);

            return IsMovingCircularly() && absAngularVelocity > targetAngularVelocity - allowedAngularVelocityDelta &&
                   absAngularVelocity < targetAngularVelocity + allowedAngularVelocityDelta;
        }

        private void AddKeyframe()
        {
            var previousKeyframe = sampleKeyframes.Last.Value;
            var currentPosition = HorizontalPosition;
            var direction = currentPosition - previousKeyframe.position;
            var angle = Vector2.SignedAngle(Vector2.up, direction) + 180;

            var newKeyframe = new GestureKeyframe
            {
                position = currentPosition,
                deltaPos = direction.magnitude,
                angle = angle,
                deltaAngle = GetDeltaAngle(angle, previousKeyframe.angle)
            };

            sampleKeyframes.AddLast(newKeyframe);

            sampleTraveledDistance += newKeyframe.deltaPos;
            sampleAngularDistance += newKeyframe.deltaAngle;
            currentComboDirections += newKeyframe.ComboDirection;

            if (sampleKeyframes.Count > sampleSize)
            {
                var lastKeyframe = sampleKeyframes.First.Value;
                sampleKeyframes.RemoveFirst();

                sampleTraveledDistance -= lastKeyframe.deltaPos;
                sampleAngularDistance -= lastKeyframe.deltaAngle;
                currentComboDirections -= lastKeyframe.ComboDirection;
            }
        }

        private float GetDeltaAngle(float newAngle, float previousAngle)
        {
            if (previousAngle > 270 && newAngle < 90)
                newAngle += 360;

            else if (newAngle > 270 && previousAngle < 90)
                previousAngle += 360;

            return newAngle - previousAngle;
        }

        private void Awake()
        {
            sampleKeyframes = new LinkedList<GestureKeyframe>();
            sampleKeyframes.AddLast(new GestureKeyframe {position = HorizontalPosition});
        }

        private void Update()
        {
            currentTickTime += Time.deltaTime;
            if (currentTickTime < tickTime)
                return;

            currentTickTime = 0;
            AddKeyframe();

            LogDebug();
        }

        private bool logginDebug = true;

        private void LogDebug()
        {
            if (Input.GetKey(KeyCode.Space))
                logginDebug = !logginDebug;

            var last = sampleKeyframes.Last.Value;

            if (logginDebug)
                Debug.Log("Position: " + last.position +
                          "Angular Velocity: " + AngularVelocity +
                          " , dentaAngle: " + last.deltaAngle +
                          " , angle: " + last.angle +
                          " , combo: " + currentComboDirections);
        }
    }
}
