using UnityEngine;

namespace XRAccelerator
{
    public class FaucetHoseEndSetup : MonoBehaviour
    {
        [SerializeField]
        private Transform targetInitialPosition;
        
        [SerializeField]
        private Transform hoseEndIk;

        public void Start()
        {
            hoseEndIk.position = targetInitialPosition.position;
            Destroy(gameObject);
        }
    }
}