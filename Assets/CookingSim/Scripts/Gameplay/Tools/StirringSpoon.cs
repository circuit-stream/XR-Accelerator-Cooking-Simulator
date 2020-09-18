using UnityEngine;
using XRAccelerator.Gestures;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class StirringSpoon : MonoBehaviour
    {
        public bool IsStirring => circularGesture != null && circularGesture.IsMovingCircularly;

        private GestureInteractor GestureInteractor => ServiceLocator.GetService<GestureInteractor>();
        private CircularGesture circularGesture;

        // TODO Arthur: Remove when we have visual feedback
        [SerializeField]
        private MeshRenderer meshRenderer;

        private void Update()
        {
            meshRenderer.material.color = IsStirring ? Color.red : Color.white;
        }

        public void OnGrab()
        {
            GestureInteractor.TrackGestures(transform);
        }

        public void OnRelease()
        {
            GestureInteractor.StopTrackingGestures(transform);
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
            GestureInteractor.circularGestureRecognizer.OnGestureStarted += OnGestureStart;
        }
    }
}