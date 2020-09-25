using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Gameplay;

namespace CookingSim.Scripts.Gameplay.Appliances
{
    public class Blender : Container
    {
        // TODO Arthur Optional: blend time per recipe
        private const float blendTime = 5;

        [Header("Blender Specific")]
        [SerializeField]
        [Tooltip("The switch that will enable the faucet")]
        private RotarySwitch blenderSwitch;

        [SerializeField]
        [Tooltip("Reference to the blender animator")]
        private Animator animator;

        [SerializeField]
        [Tooltip("Reference to the lid socket")]
        private GrabInteractableSocket lidSocket;
        [SerializeField]

        [Tooltip("Reference to the glass socket")]
        private GrabInteractableSocket glassSocket;

        private float applianceEnabledTime;

        protected override void ExecuteRecipe()
        {
            base.ExecuteRecipe();

            applianceEnabledTime = 0;
        }

        private void Update()
        {
            if (!isApplianceEnabled)
            {
                return;
            }

            applianceEnabledTime += Time.deltaTime;

            if (applianceEnabledTime > blendTime)
            {
                ExecuteRecipe();
            }
        }

        private void EnableAppliance()
        {
            if (!lidSocket.IsInteractableAttached || !glassSocket.IsInteractableAttached)
            {
                // TODO Arthur: Visual feedback
                return;
            }

            isApplianceEnabled = true;
            applianceEnabledTime = 0;

            // TODO Arthur: animator / timer
        }

        private void DisableAppliance()
        {
            isApplianceEnabled = false;
        }

        private void OnSwitchStateChange(int index)
        {
            if (index > 0)
            {
                // TODO Arthur: Handle blender speeds
                EnableAppliance();
            }
            else
            {
                DisableAppliance();
            }
        }

        private void OnSwitchGrabRelease(XRBaseInteractor interactor)
        {
            if (!isApplianceEnabled && blenderSwitch.CurrentStateIndex > 0)
            {
                blenderSwitch.JumpToIndex(0);
            }
        }

        private void OnGlassAttach()
        {
            ForceIngredientsStayInContainer();
            StartCoroutine(DisableForceIngredientsStayInContainer());
        }

        protected override void Awake()
        {
            base.Awake();

            blenderSwitch.StateChanged += OnSwitchStateChange;
            blenderSwitch.onSelectEnter.AddListener(OnSwitchGrabRelease);
            glassSocket.OnAttach += OnGlassAttach;
        }
    }
}