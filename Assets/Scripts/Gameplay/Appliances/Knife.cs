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
        private Collider bladeCollider;
        [SerializeField]
        private Collider gripCollider;

        private Dictionary<Collider, float> colliderCutCooldown;

        public void OnGrab()
        {
            bladeCollider.enabled = true;
            isApplianceEnabled = true;
        }

        public void OnReleaseGrab()
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
            solidIngredient.CurrentIngredients = new List<IngredientAmount>
            {
                new IngredientAmount
                {
                    Ingredient = recipeConfig.OutputIngredient,
                    Amount = solidIngredient.CurrentIngredients[0].Amount
                }
            };

            SolidIngredient newSlicedIngredient = Instantiate(recipeConfig.OutputIngredient.IngredientPrefab) as SolidIngredient;
            newSlicedIngredient.ChangeMesh(slicedHull.UpperHull, solidIngredient);
            solidIngredient.ChangeMesh(slicedHull.LowerHull, solidIngredient);

            colliderCutCooldown.Add(solidIngredient.GetComponent<Collider>(), cutCooldown);
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
                    colliderCutCooldown.Remove(pair.Key);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            bladeCollider.enabled = false;
            colliderCutCooldown = new Dictionary<Collider, float>();

            Physics.IgnoreCollision(gripCollider, bladeCollider);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Uncomment for slice plane debug
            return;

            var transformRef = transform;
            var plane = new Plane {trans_ref = transformRef};
            plane.Compute(transformRef.position, transformRef.up);
            plane.OnDebugDraw();
        }
#endif
    }
}