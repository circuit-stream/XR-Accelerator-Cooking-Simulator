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
        public Rigidbody MasterRigidbody;
        public XRGrabInteractable GrabInteractable;

        private const float cutCooldown = 1f;

        private Collider bladeCollider;

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

        private void Cut(SolidIngredient solidIngredient, RecipeConfig recipeConfig)
        {
            SlicedMeshHull slicedHull = MeshSlicerUtils.Slice(solidIngredient.gameObject, transform.position, transform.up);

            if (slicedHull == null || slicedHull.UpperHull == null || slicedHull.LowerHull == null)
            {
                return;
            }

            SetDebug2(solidIngredient.gameObject);

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

        private void OnTriggerEnter(Collider other)
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

            if (colliderCutCooldown.ContainsKey(other))
            {
                return;
            }

            Cut(solidIngredient, recipe);
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

            bladeCollider = GetComponent<Collider>();
            bladeCollider.enabled = false;

            colliderCutCooldown = new Dictionary<Collider, float>();

#if UNITY_EDITOR
            SetDebug();
#endif
        }

#if UNITY_EDITOR
        private Plane plane;

        private void SetDebug()
        {
            plane = new Plane {trans_ref = transform};
            plane.Compute(transform.position, transform.forward);
            // plane.Compute(transform);
        }

        private void SetDebug2(GameObject obj)
        {
            Vector3 refUp = obj.transform.InverseTransformDirection(transform.up);
            Vector3 refPt = obj.transform.InverseTransformPoint(transform.position);

            plane = new Plane();
            plane.Compute(refPt, refUp);
            plane.trans_ref = transform;
            // plane.Compute(transform);

            MasterRigidbody.useGravity = false;
            MasterRigidbody.velocity = Vector3.zero;
            MasterRigidbody.angularVelocity = Vector3.zero;

            Destroy(GrabInteractable);
        }

        private void OnDrawGizmos()
        {
            if (plane.normal != Vector3.zero)
            {
                plane = new Plane {trans_ref = transform};
                plane.Compute(transform.position, transform.up);
                plane.OnDebugDraw();
            }
        }
#endif
    }
}