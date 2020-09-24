using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Services
{
    public class ContainerCollidersProvider
    {
        public readonly List<Collider> registeredColliders;

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

        public ContainerCollidersProvider()
        {
            registeredColliders = new List<Collider>();
        }
    }
}