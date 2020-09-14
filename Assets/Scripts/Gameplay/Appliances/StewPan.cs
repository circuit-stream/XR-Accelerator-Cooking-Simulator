using System;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class StewPan : Container
    {
        private float applianceEnabledTime;

        private void EnableAppliance()
        {
            isApplianceEnabled = true;
            applianceEnabledTime = 0;
        }

        private void DisableAppliance()
        {
            isApplianceEnabled = false;
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            
            // TODO Arthur: If Fire call EnableAppliance
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            
            // TODO Arthur: If Fire call DisableAppliance
        }

        private void Update()
        {
            if (!isApplianceEnabled)
                return;
            
            // TODO Arthur: After currentRecipeTime execute recipe
            
            throw new NotImplementedException();
        }
    }
}