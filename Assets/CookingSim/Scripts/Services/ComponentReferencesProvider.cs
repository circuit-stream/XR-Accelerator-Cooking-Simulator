using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Services
{
    public class ComponentReferencesProvider
    {
        public readonly List<Collider> registeredColliders;
        public readonly List<LocomotionProvider> registeredLocomotionProviders;

        public void RegisterContainerCollider(Collider collider)
        {
            Debug.Assert(collider.gameObject.layer == LayerMask.NameToLayer("Container"),
                "Registering container collider that is not on Container layer", collider.gameObject);
            Debug.Assert(collider.isTrigger, "Registering container collider that is not a trigger", collider.gameObject);

            registeredColliders.Add(collider);
        }

        public void RemoveContainerCollider(Collider collider)
        {
            Debug.Assert(registeredColliders.Contains(collider), "Trying to unregister unregistered collider", collider.gameObject);

            registeredColliders.Remove(collider);
        }

        public void RegisterLocomotionProviders(GameObject locomotionProviderHost)
        {
            foreach (var locomotionProvider in locomotionProviderHost.GetComponents<LocomotionProvider>())
            {
                registeredLocomotionProviders.Add(locomotionProvider);
            }
        }

        public ComponentReferencesProvider()
        {
            registeredColliders = new List<Collider>();
            registeredLocomotionProviders = new List<LocomotionProvider>();
        }
    }
}