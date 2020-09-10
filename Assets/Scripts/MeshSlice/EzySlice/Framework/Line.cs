using UnityEngine;

namespace EzySlice
{
    public struct Line
    {
        private readonly Vector3 mPosA;
        private readonly Vector3 mPosB;

        public Line(Vector3 pta, Vector3 ptb)
        {
            mPosA = pta;
            mPosB = ptb;
        }

        public float dist => Vector3.Distance(mPosA, mPosB);

        public float distSq => (mPosA - mPosB).sqrMagnitude;

        public Vector3 positionA => mPosA;

        public Vector3 positionB => mPosB;
    }
}