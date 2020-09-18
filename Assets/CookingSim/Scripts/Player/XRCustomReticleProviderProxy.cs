using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Player
{
    public class XRCustomReticleProviderProxy : MonoBehaviour, IXRCustomReticleProvider
    {
        public IXRCustomReticleProvider target;

        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            return target.AttachCustomReticle(reticleInstance);
        }

        public bool RemoveCustomReticle()
        {
            return target.RemoveCustomReticle();
        }
    }
}