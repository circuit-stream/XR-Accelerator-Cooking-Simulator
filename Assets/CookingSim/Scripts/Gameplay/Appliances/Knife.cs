using System.Collections.Generic;
using System.Linq;
using EzySlice;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Configs;
using Plane = EzySlice.Plane;

namespace XRAccelerator.Gameplay
{
    public class Knife : Appliance
    {
        private const float cutCooldown = 0.5f;
        private const float bladeCooldown = 0.3f;

        [Header("Prefab References")]
        [SerializeField]
        [Tooltip("Reference to the blade collider")]
        private Collider bladeCollider;

        [SerializeField]
        [Tooltip("Reference to the grip collider")]
        private Collider gripCollider;

        [SerializeField]
        [Tooltip("XR GrabInteractable component reference")]
        private XRGrabInteractable grabInteractable;

        [SerializeField]
        [Tooltip("Reference to the audioSouce component for cutting sound feedback")]
        private AudioSource audioSource;

        private Dictionary<Collider, float> colliderCutCooldown;
        private float currentBladeCooldown;

        public void OnGrab(XRBaseInteractor interactor)
        {
            isApplianceEnabled = true;
        }

        public void OnReleaseGrab(XRBaseInteractor interactor)
        {
            isApplianceEnabled = false;
        }

        private void Cut(SolidIngredient solidIngredient, RecipeConfig recipeConfig, Collision other)
        {
            var bladeTransform = other.contacts[0].thisCollider.transform;
            SlicedMeshHull slicedHull = MeshSlicerUtils.Slice(solidIngredient.gameObject, other.contacts[0].point, bladeTransform.up);
            if (slicedHull == null || slicedHull.UpperHull == null || slicedHull.LowerHull == null)
            {
                return;
            }

            currentBladeCooldown = bladeCooldown;
            bladeCollider.enabled = false;


            // TODO Arthur: Check for minimum mesh size
            // TODO Arthur: Handle multiple ingredients
            // TODO Arthur: Detect chop gesture along the blade cut plane
            // TODO Arthur: Check if collision normal and blade normal is too far apart

            solidIngredient.CurrentIngredients = new List<IngredientAmount>
            {
                new IngredientAmount
                {
                    Ingredient = recipeConfig.OutputIngredient,
                    Amount = solidIngredient.CurrentIngredients[0].Amount
                }
            };

            CreateSlicedIngredient(recipeConfig.OutputIngredient.IngredientPrefab, slicedHull.UpperHull, solidIngredient);
            CreateSlicedIngredient(recipeConfig.OutputIngredient.IngredientPrefab, slicedHull.LowerHull, solidIngredient);
            KillIngredient(solidIngredient);

            audioSource.Play();
        }

        private void KillIngredient(SolidIngredient solidIngredient)
        {
            // [XRToolkitWorkaround] XRDirectInteractor for some reason is keeping a reference to the XRGrabInteractable
            // this way we can kill the object and prevent missing references from the colliders access.
            solidIngredient.GetComponent<XRGrabInteractable>().colliders.Clear();
            Destroy(solidIngredient.gameObject);
        }

        private void CreateSlicedIngredient(IngredientGraphics prefab, Mesh newMesh, SolidIngredient originalIngredient)
        {
            SolidIngredient newSlicedIngredient = Instantiate(prefab) as SolidIngredient;
            newSlicedIngredient.ChangeMesh(newMesh, originalIngredient);
            colliderCutCooldown.Add(newSlicedIngredient.GetComponent<Collider>(), cutCooldown);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isApplianceEnabled)
            {
                return;
            }

            if (colliderCutCooldown.ContainsKey(other.collider) || currentBladeCooldown > 0)
            {
                return;
            }

            var solidIngredient = other.gameObject.GetComponent<SolidIngredient>();
            if (solidIngredient == null)
            {
                return;
            }

            RecipeConfig recipe = GetRecipeForIngredients(solidIngredient.CurrentIngredients);
            if (recipe == null)
            {
                return;
            }

            Cut(solidIngredient, recipe, other);
        }

        private void Update()
        {
            currentBladeCooldown -= Time.deltaTime;
            if (currentBladeCooldown < 0)
            {
                bladeCollider.enabled = true;
            }

            foreach (var pair in colliderCutCooldown.ToList())
            {
                colliderCutCooldown[pair.Key] = pair.Value - Time.deltaTime;

                if (colliderCutCooldown[pair.Key] <= 0)
                {
                    colliderCutCooldown.Remove(pair.Key);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            grabInteractable.onSelectEntered.AddListener(OnGrab);
            grabInteractable.onSelectExited.AddListener(OnReleaseGrab);

            colliderCutCooldown = new Dictionary<Collider, float>();

            Physics.IgnoreCollision(gripCollider, bladeCollider);
        }

#if UNITY_EDITOR
        // Uncomment for slice plane debug
        private void OnDrawGizmosSelected()
        {
            var transformRef = transform;
            var plane = new Plane {trans_ref = transformRef};
            plane.Compute(transformRef.position, transformRef.up);
            plane.OnDebugDraw();
        }
#endif
    }
}