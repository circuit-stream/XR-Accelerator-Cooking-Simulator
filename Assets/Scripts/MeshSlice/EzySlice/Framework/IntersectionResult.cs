using UnityEngine;

namespace EzySlice
{
    /**
     * A Basic Structure which contains intersection information
     * for Plane -> Triangle Intersection Tests
     * TO-DO -> This structure can be optimized to hold less data
     * via an optional indices array. Could lead for a faster
     * intersection test as well.
     */
    public sealed class IntersectionResult
    {
        // general tag to check if this structure is valid
        private bool isSuccess;

        // our intersection points/triangles
        private readonly Triangle[] upperHull;
        private readonly Triangle[] lowerHull;
        private readonly Vector3[] intersectionPt;

        // our counters. We use raw arrays for performance reasons
        private int upperHullCount;
        private int lowerHullCount;
        private int intersectionPtCount;

        public IntersectionResult()
        {
            isSuccess = false;

            upperHull = new Triangle[2];
            lowerHull = new Triangle[2];
            intersectionPt = new Vector3[2];

            upperHullCount = 0;
            lowerHullCount = 0;
            intersectionPtCount = 0;
        }

        public Triangle[] UpperHull => upperHull;

        public Triangle[] LowerHull => lowerHull;

        public Vector3[] IntersectionPoints => intersectionPt;

        public int UpperHullCount => upperHullCount;

        public int LowerHullCount => lowerHullCount;

        public int IntersectionPointCount => intersectionPtCount;

        public bool IsValid => isSuccess;

        /**
         * Used by the intersector, adds a new triangle to the
         * upper hull section
         */
        public IntersectionResult AddUpperHull(Triangle tri)
        {
            upperHull[upperHullCount++] = tri;

            isSuccess = true;

            return this;
        }

        /**
         * Used by the intersector, adds a new triangle to the
         * lower gull section
         */
        public IntersectionResult AddLowerHull(Triangle tri)
        {
            lowerHull[lowerHullCount++] = tri;

            isSuccess = true;

            return this;
        }

        /**
         * Used by the intersector, adds a new intersection point
         * which is shared by both upper->lower hulls
         */
        public void AddIntersectionPoint(Vector3 pt)
        {
            intersectionPt[intersectionPtCount++] = pt;
        }

        /**
         * Clear the current state of this object 
         */
        public void Clear()
        {
            isSuccess = false;
            upperHullCount = 0;
            lowerHullCount = 0;
            intersectionPtCount = 0;
        }

        /**
         * Editor only DEBUG functionality. This should not be compiled in the final
         * Version.
         */
        public void OnDebugDraw()
        {
            OnDebugDraw(Color.white);
        }

        public void OnDebugDraw(Color drawColor)
        {
#if UNITY_EDITOR
            if (!IsValid)
            {
                return;
            }

            Color prevColor = Gizmos.color;

            Gizmos.color = drawColor;

            // draw the intersection points
            for (int i = 0; i < IntersectionPointCount; i++)
            {
                Gizmos.DrawSphere(IntersectionPoints[i], 0.1f);
            }

            // draw the upper hull in RED
            for (int i = 0; i < UpperHullCount; i++)
            {
                UpperHull[i].OnDebugDraw(Color.red);
            }

            // draw the lower hull in BLUE
            for (int i = 0; i < LowerHullCount; i++)
            {
                LowerHull[i].OnDebugDraw(Color.blue);
            }

            Gizmos.color = prevColor;
#endif
        }
    }
}