using System;
using UnityEngine;
using XRAccelerator.Gestures;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class AppStartup : MonoBehaviour
    {
        [SerializeField]
        private GestureInteractor gestureInteractor;

        private void Awake()
        {
            ServiceLocator.RegisterService(new ConfigsProvider());
            ServiceLocator.RegisterService(new ContainerCollidersProvider());

            // TODO Arthur: gestureInteractor might need `DontDestroyOnLoad`
            ServiceLocator.RegisterService(gestureInteractor);

            Destroy(this);
        }

        // TODO Arthur: Unregister services?
    }
}