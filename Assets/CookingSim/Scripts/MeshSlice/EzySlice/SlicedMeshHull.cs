using UnityEngine;

namespace EzySlice
{
    /**
     * The final generated data structure from a slice operation. This provides easy access
     * to utility functions and the final Mesh data for each section of the HULL.
     */
    public sealed class SlicedMeshHull
    {
        public Mesh UpperHull => upperHull;
        public Mesh LowerHull => lowerHull;

        private readonly Mesh upperHull;
        private readonly Mesh lowerHull;

        public static void AddHullToGameObject(GameObject newObject, GameObject original, Mesh hull, Material crossSectionMat = null)
        {
            MeshFilter filter = newObject.GetComponent<MeshFilter>();
            filter.mesh = hull;

            SetNewGameObjectTransform(newObject.transform, original.transform);
            SetNewGameObjectMaterials(newObject, original, hull, crossSectionMat);
        }

        public SlicedMeshHull(Mesh upperHull, Mesh lowerHull)
        {
            this.upperHull = upperHull;
            this.lowerHull = lowerHull;
        }

        public GameObject CreateUpperHull(GameObject original, Material crossSectionMat = null)
        {
            return CreateGameObjectWithHull(original, upperHull, crossSectionMat, $"Sliced_{original.name}");
        }

        public GameObject CreateLowerHull(GameObject original, Material crossSectionMat = null)
        {
            return CreateGameObjectWithHull(original, lowerHull, crossSectionMat, $"Sliced_{original.name}");
        }

        private static GameObject CreateGameObjectWithHull(GameObject original, Mesh hull, Material crossSectionMat = null, string name = "SlicedHull")
        {
            if (hull == null)
            {
                return null;
            }

            GameObject newObject = CreateMeshGameObject(name, hull);
            AddHullToGameObject(newObject, original, hull, crossSectionMat);

            return newObject;
        }

        private static void SetNewGameObjectTransform(Transform newTransform, Transform originalTransform)
        {
            newTransform.position = originalTransform.position;
            newTransform.rotation = originalTransform.rotation;
            newTransform.localScale = originalTransform.localScale;
        }

        private static void SetNewGameObjectMaterials(GameObject newObject, GameObject originalObject, Mesh newMesh, Material crossSectionMat)
        {
            Material[] originalMaterials = originalObject.GetComponent<MeshRenderer>().sharedMaterials;
            Mesh originalMesh = originalObject.GetComponent<MeshFilter>().sharedMesh;
            Renderer newObjectRenderer = newObject.GetComponent<Renderer>();

            // nothing changed in the hierarchy, the cross section must have been batched
            // with the submeshes, return as is, no need for any changes
            if (originalMesh.subMeshCount == newMesh.subMeshCount)
            {
                // the the material information
                newObjectRenderer.sharedMaterials = originalMaterials;
                return;
            }

            // otherwise the cross section was added to the back of the submesh array because
            // it uses a different material. We need to take this into account
            Material[] newSharedMaterials = new Material[originalMaterials.Length + 1];

            // copy our material arrays across using native copy (should be faster than loop)
            System.Array.Copy(originalMaterials, newSharedMaterials, originalMaterials.Length);
            newSharedMaterials[originalMaterials.Length] = crossSectionMat;

            // the the material information
            newObject.GetComponent<Renderer>().sharedMaterials = newSharedMaterials;
        }

        /*
         * Helper function which will create a new GameObject with the desired mesh
         */
        private static GameObject CreateMeshGameObject(string name, Mesh hull)
        {
            GameObject newObject = new GameObject(name);
            newObject.AddComponent<MeshRenderer>();
            newObject.AddComponent<MeshFilter>();
            return newObject;
        }
    }
}