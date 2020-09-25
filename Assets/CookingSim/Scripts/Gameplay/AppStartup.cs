using UnityEngine;
using XRAccelerator.Gestures;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class AppStartup : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to a GestureInteractor component")]
        private GestureInteractor gestureInteractor;

        [SerializeField]
        [Tooltip("Reference to the gameObject that holds locomotionProvider components")]
        private GameObject locomotionProviderHost;

        private void Awake()
        {
            ServiceLocator.RegisterService(new ConfigsProvider());

            var componentReferencesProvider = new ComponentReferencesProvider();
            componentReferencesProvider.RegisterLocomotionProviders(locomotionProviderHost);
            ServiceLocator.RegisterService(componentReferencesProvider);

            // TODO Arthur: gestureInteractor might need `DontDestroyOnLoad`
            ServiceLocator.RegisterService(gestureInteractor);

            Destroy(this);
        }

        // TODO Arthur: Unregister services?
    }
}