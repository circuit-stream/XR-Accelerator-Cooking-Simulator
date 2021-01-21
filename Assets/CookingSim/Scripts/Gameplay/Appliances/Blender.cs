using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Gameplay;

namespace CookingSim.Scripts.Gameplay.Appliances
{
    public class Blender : Container
    {
        // TODO Arthur Optional: blend time per recipe
        private const float blendTime = 5;
        private static readonly int BlendingHashName = Animator.StringToHash("Blending");

        [Header("Blender Specific")]
        [SerializeField]
        [Tooltip("The switch that will enable the faucet")]
        private RotarySwitch blenderSwitch;
        [SerializeField]
        [Tooltip("Reference to the lid socket")]
        private GrabInteractableSocket lidSocket;
        [SerializeField]
        [Tooltip("Reference to the glass socket")]
        private GrabInteractableSocket glassSocket;

        [SerializeField]
        [Tooltip("Reference to the blender animator")]
        private Animator animator;
        [SerializeField]
        [Tooltip("Reference to the audioSource component")]
        private AudioSource audioSource;

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
            if (isApplianceEnabled)
            {
                return;
            }

            if (!lidSocket.IsInteractableAttached || !glassSocket.IsInteractableAttached)
            {
                // TODO Arthur: Visual feedback
                return;
            }

            isApplianceEnabled = true;
            applianceEnabledTime = 0;

            // TODO Arthur Optional: Move these to a timeline
            liquidContainer.StartStirring();
            audioSource.Play();
            animator.SetBool(BlendingHashName, true);
        }

        private void DisableAppliance()
        {
            isApplianceEnabled = false;

            liquidContainer.StopStirring();
            audioSource.Stop();
            animator.SetBool(BlendingHashName, false);
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
            blenderSwitch.onSelectExited.AddListener(OnSwitchGrabRelease);
            glassSocket.OnAttach += OnGlassAttach;
            glassSocket.IgnoreCollisionWith(lidSocket.interactableColliders);
        }
    }
}