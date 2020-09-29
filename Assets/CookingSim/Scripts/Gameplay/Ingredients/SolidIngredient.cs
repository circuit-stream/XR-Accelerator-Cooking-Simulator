using System.Linq;
using UnityEngine;
using EzySlice;

namespace XRAccelerator.Gameplay
{
    public class SolidIngredient : IngredientGraphics
    {
        [SerializeField]
        [Tooltip("What the insides of this ingredient will look like")]
        private Material slicedSectionMaterial;

        [SerializeField]
        [Tooltip("The attach point of this interactable object")]
        private Transform attachPoint;

        public void ChangeMesh(Mesh newMesh, SolidIngredient originalIngredient)
        {
            var originalMesh = originalIngredient.GetComponent<MeshFilter>().mesh;
            var newSizePercentage = GetNewMeshSizePercentage(originalMesh, newMesh);

            CurrentIngredients = originalIngredient.CurrentIngredients.Select(ingredientAmount =>
                new IngredientAmount
                {
                    Ingredient = ingredientAmount.Ingredient,
                    Amount = ingredientAmount.Amount * newSizePercentage
                }
            ).ToList();

            SlicedMeshHull.AddHullToGameObject(gameObject, originalIngredient.gameObject, newMesh, slicedSectionMaterial);
            GetComponent<MeshCollider>().sharedMesh = newMesh;

            ChangeAttachPoint(originalMesh, newMesh);
        }

        private void ChangeAttachPoint(Mesh originalMesh, Mesh newMesh)
        {
            if (attachPoint == null)
            {
                return;
            }

            var newSize = newMesh.bounds.size;
            var originalSize = originalMesh.bounds.size;
            var factor = new Vector3(
                newSize.x / originalSize.x,
                newSize.y / originalSize.y,
                newSize.z / originalSize.z);

            var newPos = attachPoint.localPosition;
            newPos.Scale(factor);
            attachPoint.localPosition = newMesh.bounds.center + newPos;
        }

        private float GetNewMeshSizePercentage(Mesh originalMesh, Mesh newMesh)
        {
            return MeshUtils.VolumeOfMesh(newMesh) / MeshUtils.VolumeOfMesh(originalMesh);
        }
    }
}