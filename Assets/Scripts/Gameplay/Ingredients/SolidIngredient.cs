using System.Linq;
using UnityEngine;
using EzySlice;

namespace XRAccelerator.Gameplay
{
    public class SolidIngredient : IngredientGraphics
    {
        [SerializeField]
        private Material slicedSectionMaterial;

        public void ChangeMesh(Mesh newMesh, SolidIngredient originalIngredient)
        {
            var newSizePercentage =
                GetNewMeshSizePercentage(originalIngredient.GetComponent<MeshFilter>().mesh, newMesh);

            CurrentIngredients = originalIngredient.CurrentIngredients.Select(ingredientAmount =>
                new IngredientAmount
                {
                    Ingredient = ingredientAmount.Ingredient,
                    Amount = ingredientAmount.Amount * newSizePercentage
                }
            ).ToList();

            SlicedMeshHull.AddHullToGameObject(gameObject, originalIngredient.gameObject, newMesh, slicedSectionMaterial);
        }

        private float GetNewMeshSizePercentage(Mesh originalMesh, Mesh newMesh)
        {
            return newMesh.bounds.size.sqrMagnitude / originalMesh.bounds.size.sqrMagnitude;
        }
    }
}