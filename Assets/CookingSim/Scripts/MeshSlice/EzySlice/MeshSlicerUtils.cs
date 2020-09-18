using UnityEngine;

namespace EzySlice
{
    /**
     * Define sugar methods for easy access to slicer functionality
     */
    public static class MeshSlicerUtils
    {
        /**
         * SlicedHull Return functions and appropriate overrides!
         */
        public static SlicedMeshHull Slice(GameObject obj, Plane pl, Material crossSectionMaterial = null)
        {
            return Slice(obj, pl, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);
        }

        public static SlicedMeshHull Slice(GameObject obj, Vector3 position, Vector3 direction,
            Material crossSectionMaterial = null)
        {
            return Slice(obj, position, direction, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f), crossSectionMaterial);
        }

        public static SlicedMeshHull Slice(GameObject obj, Vector3 position, Vector3 direction,
            TextureRegion textureRegion, Material crossSectionMaterial = null)
        {
            Plane cuttingPlane = new Plane();

            Vector3 refUp = obj.transform.InverseTransformDirection(direction);
            Vector3 refPt = obj.transform.InverseTransformPoint(position);

            cuttingPlane.Compute(refPt, refUp);

            return Slice(obj, cuttingPlane, textureRegion, crossSectionMaterial);
        }

        public static SlicedMeshHull Slice(GameObject obj, Plane pl, TextureRegion textureRegion,
            Material crossSectionMaterial = null)
        {
            return MeshSlicer.Slice(obj, pl, textureRegion, crossSectionMaterial);
        }

        /**
         * These functions (and overrides) will return the final indtaniated GameObjects types
         */
        public static GameObject[] SliceInstantiate(GameObject obj, Plane pl)
        {
            return SliceInstantiate(obj, pl, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f));
        }

        public static GameObject[] SliceInstantiate(GameObject obj, Vector3 position, Vector3 direction)
        {
            return SliceInstantiate(obj, position, direction, null);
        }

        public static GameObject[] SliceInstantiate(GameObject obj, Vector3 position, Vector3 direction,
            Material crossSectionMat)
        {
            return SliceInstantiate(obj, position, direction, new TextureRegion(0.0f, 0.0f, 1.0f, 1.0f),
                crossSectionMat);
        }

        public static GameObject[] SliceInstantiate(GameObject obj, Vector3 position, Vector3 direction,
            TextureRegion cuttingRegion, Material crossSectionMaterial = null)
        {
            Plane cuttingPlane = new Plane();

            Vector3 refUp = obj.transform.InverseTransformDirection(direction);
            Vector3 refPt = obj.transform.InverseTransformPoint(position);

            cuttingPlane.Compute(refPt, refUp);

            return SliceInstantiate(obj, cuttingPlane, cuttingRegion, crossSectionMaterial);
        }

        public static GameObject[] SliceInstantiate(GameObject obj, Plane pl, TextureRegion cuttingRegion,
            Material crossSectionMaterial = null)
        {
            SlicedMeshHull slice = MeshSlicer.Slice(obj, pl, cuttingRegion, crossSectionMaterial);

            if (slice == null)
            {
                return null;
            }

            GameObject upperHull = slice.CreateUpperHull(obj, crossSectionMaterial);
            GameObject lowerHull = slice.CreateLowerHull(obj, crossSectionMaterial);

            if (upperHull != null && lowerHull != null)
            {
                return new[] {upperHull, lowerHull};
            }

            // otherwise return only the upper hull
            if (upperHull != null)
            {
                return new[] {upperHull};
            }

            // otherwise return only the lower hull
            if (lowerHull != null)
            {
                return new[] {lowerHull};
            }

            // nothing to return, so return nothing!
            return null;
        }
    }
}