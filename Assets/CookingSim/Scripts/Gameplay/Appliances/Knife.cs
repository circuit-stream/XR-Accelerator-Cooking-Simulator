using System.Collections;
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
        private const float cutCooldown = 1f;

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

        private Dictionary<Collider, float> colliderCutCooldown;

        public void OnGrab(XRBaseInteractor interactor)
        {
            bladeCollider.enabled = true;
            isApplianceEnabled = true;
        }

        public void OnReleaseGrab(XRBaseInteractor interactor)
        {
            bladeCollider.enabled = false;
            isApplianceEnabled = false;
        }

        private void Cut(SolidIngredient solidIngredient, RecipeConfig recipeConfig, Collision other)
        {
            // TODO: Check if collision normal and blade normal is too far apart
            var bladeTransform = other.contacts[0].thisCollider.transform;
            SlicedMeshHull slicedHull = MeshSlicerUtils.Slice(solidIngredient.gameObject, other.contacts[0].point, bladeTransform.up);
            if (slicedHull == null || slicedHull.UpperHull == null || slicedHull.LowerHull == null)
            {
                return;
            }

            // TODO Arthur: Check for minimum mesh size
            // TODO Arthur: Handle multiple ingredients
            // TODO Arthur: Detect chop gesture along the blade cut plane

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

            // For some reason I can't destroy it without XRGrabInteractable going bonkers
            solidIngredient.gameObject.SetActive(false);
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

            if (colliderCutCooldown.ContainsKey(other.collider))
            {
                return;
            }

            Cut(solidIngredient, recipe, other);
        }

        private void Update()
        {
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

            grabInteractable.onSelectEnter.AddListener(OnGrab);
            grabInteractable.onSelectExit.AddListener(OnReleaseGrab);

            bladeCollider.enabled = false;
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