using System.Linq;
using UnityEngine;
using EzySlice;

namespace XRAccelerator.Gameplay
{
    public class SolidIngredient : IngredientGraphics
    {
        [SerializeField]
        private Material slicedSectionMaterial;

        public void ChangeMesh(Mesh newMesh, float originalMeshVolume, SolidIngredient originalIngredient)
        {
            var newSizePercentage =
                GetNewMeshSizePercentage(originalMeshVolume, newMesh);

            CurrentIngredients = originalIngredient.CurrentIngredients.Select(ingredientAmount =>
                new IngredientAmount
                {
                    Ingredient = ingredientAmount.Ingredient,
                    Amount = ingredientAmount.Amount * newSizePercentage
                }
            ).ToList();

            SlicedMeshHull.AddHullToGameObject(gameObject, originalIngredient.gameObject, newMesh, slicedSectionMaterial);
        }

        private float GetNewMeshSizePercentage(float originalMeshVolume, Mesh newMesh)
        {
            return MeshUtils.VolumeOfMesh(newMesh) / originalMeshVolume;
        }
    }
}