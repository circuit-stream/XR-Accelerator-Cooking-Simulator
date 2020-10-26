using UnityEngine;

// VRUI Tooltip Component
namespace XRAccelerator.VRUI
{
    [ExecuteAlways]
    public class VRUITooltip : MonoBehaviour
    {
        [SerializeField] 
        private Transform container;
        [SerializeField]
        private Transform anchor;
        [SerializeField] 
        private LineRenderer link;

        private void Start()
        {
            link.positionCount = 2;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            var points = new Vector3[link.positionCount];
            points[0] = container.position;
            points[1] = anchor.position;
            link.SetPositions(points);
        }
    }
}