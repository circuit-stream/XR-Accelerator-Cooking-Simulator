using UnityEngine;

namespace EzySlice
{

    /**
     * Quick Internal structure which checks where the point lays on the
     * Plane. UP = Upwards from the Normal, DOWN = Downwards from the Normal
     * ON = Point lays straight on the plane
     */
    public enum SideOfPlane
    {
        UP,
        DOWN,
        ON
    }

    /**
     * Represents a simple 3D Plane structure with a position
     * and direction which extends infinitely in its axis. This provides
     * an optimal structure for collision tests for the slicing framework.
     */
    public struct Plane
    {
        private Vector3 m_normal;
        private float m_dist;

#if UNITY_EDITOR
        // this is for editor debugging only! do NOT try to access this
        // variable at runtime, we will be stripping it out for final
        // builds
        public Transform trans_ref;
#endif

        public Plane(Vector3 pos, Vector3 norm)
        {
            m_normal = norm;
            m_dist = Vector3.Dot(norm, pos);

            // this is for editor debugging only!
#if UNITY_EDITOR
            trans_ref = null;
#endif
        }

        public Plane(Vector3 norm, float dot)
        {
            m_normal = norm;
            m_dist = dot;

#if UNITY_EDITOR
            trans_ref = null;
#endif
        }

        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            m_normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            m_dist = -Vector3.Dot(m_normal, a);

#if UNITY_EDITOR
            trans_ref = null;
#endif
        }

        public void Compute(Vector3 pos, Vector3 norm)
        {
            m_normal = norm;
            m_dist = Vector3.Dot(norm, pos);
        }

        public void Compute(Transform trans)
        {
            Compute(trans.position, trans.up);

#if UNITY_EDITOR
            trans_ref = trans;
#endif
        }

        public void Compute(GameObject obj)
        {
            Compute(obj.transform);
        }

        public Vector3 normal => m_normal;

        public float dist => m_dist;

        /**
         * Checks which side of the plane the point lays on.
         */
        public SideOfPlane SideOf(Vector3 pt)
        {
            float result = Vector3.Dot(m_normal, pt) - m_dist;

            if (result > Intersector.Epsilon)
            {
                return SideOfPlane.UP;
            }

            if (result < -Intersector.Epsilon)
            {
                return SideOfPlane.DOWN;
            }

            return SideOfPlane.ON;
        }

#if UNITY_EDITOR
        public void OnDebugDraw()
        {
            if (trans_ref == null)
            {
                return;
            }

            Color prevColor = Gizmos.color;
            Matrix4x4 prevMatrix = Gizmos.matrix;

            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(trans_ref.position, trans_ref.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 0.0f, 1.0f));

            Gizmos.color = prevColor;
            Gizmos.matrix = prevMatrix;
        }
#endif
    }
}
