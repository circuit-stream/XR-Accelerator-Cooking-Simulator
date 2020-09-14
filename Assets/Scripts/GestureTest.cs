using System;
using UnityEngine;
using XRAccelerator.Gestures;

namespace XRAccelerator
{
    public class GestureTest : MonoBehaviour
    {
        [SerializeField]
        private float targetAngularVelocity = 20;
        [SerializeField]
        private float allowedAngularVelocityDelta = 5;

        [SerializeField]
        private GestureInteractor gestureInteractor;
        [SerializeField]
        private MeshRenderer meshRenderer;
        
        private CircularGesture circularGesture;

        public void OnGrab()
        {
            gestureInteractor.TrackGestures(transform);
        }

        public void OnRelease()
        {
            gestureInteractor.StopTrackingGestures(transform);
        }
        
        public bool IsMovingAtTargetVelocity()
        {
            if (circularGesture == null)
            {
                return false;
            }
            
            var absAngularVelocity = Mathf.Abs(circularGesture.AngularVelocity);
            var absTargetDifference = Mathf.Abs(absAngularVelocity - targetAngularVelocity);

            return circularGesture.IsMovingCircularly && absTargetDifference < allowedAngularVelocityDelta;
        }

        private void Update()
        {
            meshRenderer.material.color =
                (circularGesture == null || !circularGesture.IsMovingCircularly)
                    ? Color.white
                    : (IsMovingAtTargetVelocity() ? Color.red : Color.yellow);
        }
        
        private void OnGestureStart(CircularGesture gesture)
        {
            if (gesture.TrackedTransform != transform)
                return;
            
            Debug.Assert(circularGesture == null, "CircularGesture was already active!");

            circularGesture = gesture;
            gesture.OnCancel += OnGestureFinished;
            gesture.OnFinished += OnGestureFinished;
        }

        private void OnGestureFinished(CircularGesture gesture)
        {
            Debug.Assert(gesture == circularGesture, "Received OnGestureFinished from different gesture");
            
            gesture.OnFinished -= OnGestureFinished;
            gesture.OnCancel -= OnGestureFinished;

            circularGesture = null;
        }

        private void Start()
        {
            gestureInteractor.circularGestureRecognizer.OnGestureStarted += OnGestureStart;
        }
    }
}