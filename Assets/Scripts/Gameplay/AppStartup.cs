using System;
using UnityEngine;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class AppStartup : MonoBehaviour
    {
        // TODO Arthur: Make this script run before any other
        private void Awake()
        {
            // TODO Arthur: Instantiate services + any other setup required
            ServiceLocator.RegisterService(new ConfigsProvider());
        }
    }
}