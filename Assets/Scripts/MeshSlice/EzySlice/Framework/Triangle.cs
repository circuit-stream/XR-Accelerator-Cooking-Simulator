using UnityEngine;

namespace EzySlice
{
    /**
     * Represents a simple 3D Triangle structure with position
     * and UV map. The UV is required if the slicer needs
     * to recalculate the new UV position for texture mapping.
     */
    public struct Triangle
    {
        // the points which represent this triangle
        // these have to be set and are immutable. Cannot be
        // changed once set
        private readonly Vector3 m_pos_a;
        private readonly Vector3 m_pos_b;
        private readonly Vector3 m_pos_c;

        // the UV coordinates of this triangle
        // these are optional and may not be set
        private bool m_uv_set;
        private Vector2 m_uv_a;
        private Vector2 m_uv_b;
        private Vector2 m_uv_c;

        // the Normals of the Vertices
        // these are optional and may not be set
        private bool m_nor_set;
        private Vector3 m_nor_a;
        private Vector3 m_nor_b;
        private Vector3 m_nor_c;

        // the Tangents of the Vertices
        // these are optional and may not be set
        private bool m_tan_set;
        private Vector4 m_tan_a;
        private Vector4 m_tan_b;
        private Vector4 m_tan_c;

        public Triangle(Vector3 posa,
            Vector3 posb,
            Vector3 posc)
        {
            m_pos_a = posa;
            m_pos_b = posb;
            m_pos_c = posc;

            m_uv_set = false;
            m_uv_a = Vector2.zero;
            m_uv_b = Vector2.zero;
            m_uv_c = Vector2.zero;

            m_nor_set = false;
            m_nor_a = Vector3.zero;
            m_nor_b = Vector3.zero;
            m_nor_c = Vector3.zero;

            m_tan_set = false;
            m_tan_a = Vector4.zero;
            m_tan_b = Vector4.zero;
            m_tan_c = Vector4.zero;
        }

        public Vector3 positionA => m_pos_a;

        public Vector3 positionB => m_pos_b;

        public Vector3 positionC => m_pos_c;

        public bool hasUV => m_uv_set;

        public void SetUV(Vector2 uvA, Vector2 uvB, Vector2 uvC)
        {
            m_uv_a = uvA;
            m_uv_b = uvB;
            m_uv_c = uvC;

            m_uv_set = true;
        }

        public Vector2 uvA => m_uv_a;

        public Vector2 uvB => m_uv_b;

        public Vector2 uvC => m_uv_c;

        public bool hasNormal => m_nor_set;

        public void SetNormal(Vector3 norA, Vector3 norB, Vector3 norC)
        {
            m_nor_a = norA;
            m_nor_b = norB;
            m_nor_c = norC;

            m_nor_set = true;
        }

        public Vector3 normalA => m_nor_a;

        public Vector3 normalB => m_nor_b;

        public Vector3 normalC => m_nor_c;

        public bool hasTangent => m_tan_set;

        public void SetTangent(Vector4 tanA, Vector4 tanB, Vector4 tanC)
        {
            m_tan_a = tanA;
            m_tan_b = tanB;
            m_tan_c = tanC;

            m_tan_set = true;
        }

        public Vector4 tangentA => m_tan_a;

        public Vector4 tangentB => m_tan_b;

        public Vector4 tangentC => m_tan_c;

        /**
         * Compute and set the tangents of this triangle
         * Derived From https://answers.unity.com/questions/7789/calculating-tangents-vector4.html
         */
        public void ComputeTangents()
        {
            // computing tangents requires both UV and normals set
            if (!m_nor_set || !m_uv_set)
            {
                return;
            }

            Vector3 v1 = m_pos_a;
            Vector3 v2 = m_pos_b;
            Vector3 v3 = m_pos_c;

            Vector2 w1 = m_uv_a;
            Vector2 w2 = m_uv_b;
            Vector2 w3 = m_uv_c;

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            Vector3 n1 = m_nor_a;
            Vector3 nt1 = sdir;

            Vector3.OrthoNormalize(ref n1, ref nt1);
            Vector4 tanA = new Vector4(nt1.x, nt1.y, nt1.z,
                (Vector3.Dot(Vector3.Cross(n1, nt1), tdir) < 0.0f) ? -1.0f : 1.0f);

            Vector3 n2 = m_nor_b;
            Vector3 nt2 = sdir;

            Vector3.OrthoNormalize(ref n2, ref nt2);
            Vector4 tanB = new Vector4(nt2.x, nt2.y, nt2.z,
                (Vector3.Dot(Vector3.Cross(n2, nt2), tdir) < 0.0f) ? -1.0f : 1.0f);

            Vector3 n3 = m_nor_c;
            Vector3 nt3 = sdir;

            Vector3.OrthoNormalize(ref n3, ref nt3);
            Vector4 tanC = new Vector4(nt3.x, nt3.y, nt3.z,
                (Vector3.Dot(Vector3.Cross(n3, nt3), tdir) < 0.0f) ? -1.0f : 1.0f);

            // finally set the tangents of this object
            SetTangent(tanA, tanB, tanC);
        }

        /**
         * Calculate the Barycentric coordinate weight values u-v-w for Point p in respect to the provided
         * triangle. This is useful for computing new UV coordinates for arbitrary points.
         */
        public Vector3 Barycentric(Vector3 p)
        {
            Vector3 a = m_pos_a;
            Vector3 b = m_pos_b;
            Vector3 c = m_pos_c;

            Vector3 m = Vector3.Cross(b - a, c - a);

            float nu;
            float nv;
            float ood;

            float x = Mathf.Abs(m.x);
            float y = Mathf.Abs(m.y);
            float z = Mathf.Abs(m.z);

            // compute areas of plane with largest projections
            if (x >= y && x >= z)
            {
                // area of PBC in yz plane
                nu = Intersector.TriArea2D(p.y, p.z, b.y, b.z, c.y, c.z);
                // area of PCA in yz plane
                nv = Intersector.TriArea2D(p.y, p.z, c.y, c.z, a.y, a.z);
                // 1/2*area of ABC in yz plane
                ood = 1.0f / m.x;
            }
            else if (y >= x && y >= z)
            {
                // project in xz plane
                nu = Intersector.TriArea2D(p.x, p.z, b.x, b.z, c.x, c.z);
                nv = Intersector.TriArea2D(p.x, p.z, c.x, c.z, a.x, a.z);
                ood = 1.0f / -m.y;
            }
            else
            {
                // project in xy plane
                nu = Intersector.TriArea2D(p.x, p.y, b.x, b.y, c.x, c.y);
                nv = Intersector.TriArea2D(p.x, p.y, c.x, c.y, a.x, a.y);
                ood = 1.0f / m.z;
            }

            float u = nu * ood;
            float v = nv * ood;
            float w = 1.0f - u - v;

            return new Vector3(u, v, w);
        }

        /**
         * Generate a set of new UV coordinates for the provided point pt in respect to Triangle.
         * 
         * Uses weight values for the computation, so this triangle must have UV's set to return
         * the correct results. Otherwise Vector2.zero will be returned. check via hasUV().
         */
        public Vector2 GenerateUV(Vector3 pt)
        {
            // if not set, result will be zero, quick exit
            if (!m_uv_set)
            {
                return Vector2.zero;
            }

            Vector3 weights = Barycentric(pt);

            return (weights.x * m_uv_a) + (weights.y * m_uv_b) + (weights.z * m_uv_c);
        }

        /**
         * Generates a set of new Normal coordinates for the provided point pt in respect to Triangle.
         * 
         * Uses weight values for the computation, so this triangle must have Normal's set to return
         * the correct results. Otherwise Vector3.zero will be returned. check via hasNormal().
         */
        public Vector3 GenerateNormal(Vector3 pt)
        {
            // if not set, result will be zero, quick exit
            if (!m_nor_set)
            {
                return Vector3.zero;
            }

            Vector3 weights = Barycentric(pt);

            return (weights.x * m_nor_a) + (weights.y * m_nor_b) + (weights.z * m_nor_c);
        }

        /**
         * Generates a set of new Tangent coordinates for the provided point pt in respect to Triangle.
         * 
         * Uses weight values for the computation, so this triangle must have Tangent's set to return
         * the correct results. Otherwise Vector4.zero will be returned. check via hasTangent().
         */
        public Vector4 GenerateTangent(Vector3 pt)
        {
            // if not set, result will be zero, quick exit
            if (!m_nor_set)
            {
                return Vector4.zero;
            }

            Vector3 weights = Barycentric(pt);

            return (weights.x * m_tan_a) + (weights.y * m_tan_b) + (weights.z * m_tan_c);
        }

        /**
         * Helper function to split this triangle by the provided plane and store
         * the results inside the IntersectionResult structure.
         * Returns true on success or false otherwise
         */
        public bool Split(Plane pl, IntersectionResult result)
        {
            Intersector.Intersect(pl, this, result);

            return result.IsValid;
        }

        /**
         * Check the triangle winding order, if it's Clock Wise or Counter Clock Wise 
         */
        public bool IsCW()
        {
            return SignedSquare(m_pos_a, m_pos_b, m_pos_c) >= float.Epsilon;
        }

        /**
         * Returns the Signed square of a given triangle, useful for checking the
         * winding order
         */
        public static float SignedSquare(Vector3 a, Vector3 b, Vector3 c)
        {
            return (a.x * (b.y * c.z - b.z * c.y) -
                    a.y * (b.x * c.z - b.z * c.x) +
                    a.z * (b.x * c.y - b.y * c.x));
        }

#if UNITY_EDITOR
        public void OnDebugDraw()
        {
            OnDebugDraw(Color.white);
        }

        public void OnDebugDraw(Color drawColor)
        {
            Color prevColor = Gizmos.color;

            Gizmos.color = drawColor;

            Gizmos.DrawLine(positionA, positionB);
            Gizmos.DrawLine(positionB, positionC);
            Gizmos.DrawLine(positionC, positionA);

            Gizmos.color = prevColor;
        }
#endif
    }
}