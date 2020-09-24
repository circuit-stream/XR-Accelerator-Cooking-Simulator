using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class LiquidContainer : MonoBehaviour
    {
        private static readonly int WobbleXShaderName = Shader.PropertyToID("_WobbleX");
        private static readonly int WobbleZShaderName = Shader.PropertyToID("_WobbleZ");
        private static readonly int FillAmountShaderName = Shader.PropertyToID("_FillAmount");

        public Action<float> Spilled;

        [Header("Container")]
        [SerializeField]
        [Tooltip("How many milliliters this container can hold")]
        private int containerVolume = 200;

        [Header("Liquid Wobble")]
        [SerializeField]
        [Tooltip("Wobble amplitude")]
        private float MaxWobble = 0.01f;
        [SerializeField]
        [Tooltip("Wobble oscillation frequency")]
        private float WobbleSpeed = 1f;
        [SerializeField]
        [Tooltip("How fast should the liquid stop wobbling")]
        private float WobbleRecovery = 1f;

        // Container Variables
        private const int pourPointsAmount = 10;
        private List<Transform> pourPoints;
        private List<Transform> basePoints;

        private float availableLocalHeight;
        private float extraLocalHeight;

        private float containerRadius;
        private float radiusLocalScale;

        private float containerVolumePerHeight;
        private float currentLiquidHeight;
        private float currentLiquidVolume;

        // Wobble Variables
        private Vector3 lastPos;
        private Vector3 lastRot;
        private float elapsedTime;
        private float wobbleAmountToAddX;
        private float wobbleAmountToAddZ;
        private float pulse;

        // References
        private Renderer _renderer;
        private Transform _transform;

        public Transform GetCurrentPourPoint()
        {
            return GetLowestPoint(pourPoints);
        }

        #region Container Logic

        public void AddLiquid(float volume)
        {
            currentLiquidVolume += volume;
            currentLiquidHeight += volume / containerVolumePerHeight;
        }

        private Transform GetLowestPoint(List<Transform> points)
        {
            Transform lowestPoint = null;
            float lowestHeight = Mathf.Infinity;

            foreach (var point in points)
            {
                if (point.position.y < lowestHeight)
                {
                    lowestHeight = point.position.y;
                    lowestPoint = point;
                }
            }

            return lowestPoint;
        }

        private Transform GetHighestPoint(List<Transform> points)
        {
            Transform highestPoint = null;
            float highestHeight = Mathf.NegativeInfinity;

            foreach (var point in points)
            {
                if (point.position.y > highestHeight)
                {
                    highestHeight = point.position.y;
                    highestPoint = point;
                }
            }

            return highestPoint;
        }

        private void Spill(float volumeHeightSpilled)
        {
            if (currentLiquidHeight <= 0)
            {
                return;
            }

            var previousHeight = currentLiquidHeight;
            currentLiquidHeight = Mathf.Max(0,currentLiquidHeight - volumeHeightSpilled);
            var volumeSpilled = (1 - (currentLiquidHeight / previousHeight)) * currentLiquidVolume;
            currentLiquidVolume -= volumeSpilled;

            Spilled?.Invoke(volumeSpilled);
        }

        private void TrySpill()
        {
            var highestPourPoint = GetHighestPoint(pourPoints);
            var pourPoint = GetCurrentPourPoint();
            var basePoint = GetLowestPoint(basePoints);

            var pourYPosition = pourPoint.position.y;
            availableLocalHeight = (pourYPosition - basePoint.position.y);
            extraLocalHeight = (highestPourPoint.position.y - pourYPosition) * 0.5f;
            containerVolumePerHeight = containerVolume / (Mathf.Abs(availableLocalHeight) + extraLocalHeight);

            if (currentLiquidHeight > availableLocalHeight)
            {
                Spill(currentLiquidHeight - availableLocalHeight);
            }
        }

        private void UpdateShaderFillAmount()
        {
            var shaderFill = currentLiquidHeight
                             - (availableLocalHeight * 0.5f) // offset from range [0, height] to [-height/2, height/2]
                             - extraLocalHeight; // remove the extra height that can't hold liquid when rotated

            if (currentLiquidHeight <= 0)
            {
                shaderFill = -500;
            }

            _renderer.material.SetFloat(FillAmountShaderName, shaderFill) ;
        }

        #endregion

        #region Wobble Logic

        private void Wobble()
        {
            // TODO Arthur: Prevent Wobble when empty

            // decrease wobble over elapsedTime
            wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (WobbleRecovery));
            wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (WobbleRecovery));

            // make a sine wave of the decreasing wobble
            elapsedTime += Time.deltaTime;
            var wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * elapsedTime);
            var wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * elapsedTime);

            // send it to the shader
            _renderer.material.SetFloat(WobbleXShaderName, wobbleAmountX);
            _renderer.material.SetFloat(WobbleZShaderName, wobbleAmountZ);

            // velocity
            var velocity = (lastPos - _transform.position) / Time.deltaTime;
            var angularVelocity = _transform.rotation.eulerAngles - lastRot;

            // add clamped velocity to wobble
            wobbleAmountToAddX += (velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble;
            wobbleAmountToAddX = Mathf.Clamp(wobbleAmountToAddX, -MaxWobble, MaxWobble);

            wobbleAmountToAddZ += (velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble;
            wobbleAmountToAddZ = Mathf.Clamp(wobbleAmountToAddZ, -MaxWobble, MaxWobble);

            // keep last position
            lastPos = _transform.position;
            lastRot = _transform.rotation.eulerAngles;
        }

        #endregion

        private void Update()
        {
            Wobble();

            TrySpill();
            UpdateShaderFillAmount();
        }

        private void CreatePoints(List<Transform> points, float verticalOffset, string groupName, float horizontalOffset = 0)
        {
            var radius = containerRadius / radiusLocalScale + horizontalOffset;

            Transform pointsParent = new GameObject(groupName).transform;
            pointsParent.parent = _transform;
            pointsParent.rotation = Quaternion.identity;
            pointsParent.localScale = Vector3.one;
            pointsParent.localPosition = new Vector3(0, verticalOffset, 0);

            for (int index = 0; index < pourPointsAmount; index++)
            {
                Transform newPoint = new GameObject($"Point{index}").transform;
                newPoint.parent = pointsParent;
                newPoint.rotation = Quaternion.identity;
                newPoint.localScale = Vector3.one;

                var x = radius * Mathf.Sin((2 * Mathf.PI * index) / pourPointsAmount);
                var z = radius * Mathf.Cos((2 * Mathf.PI * index) / pourPointsAmount);
                newPoint.localPosition = new Vector3(x, 0, z);

                points.Add(newPoint);
            }
        }

        private void InitializeConstantVariables()
        {
            // Container Variables
            Vector3 containerBounds = _renderer.bounds.size;
            availableLocalHeight = containerBounds.y;
            containerVolumePerHeight = containerVolume / (availableLocalHeight + extraLocalHeight);

            containerRadius = containerBounds.x * 0.5f;
            radiusLocalScale = _transform.localScale.x;

            pourPoints = new List<Transform>();
            CreatePoints(pourPoints, 1.05f, "PourPoints");

            basePoints = new List<Transform>();
            CreatePoints(basePoints, -1f, "BasePoints");

            // Wobble Variables
            pulse = 2 * Mathf.PI * WobbleSpeed;
        }

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _transform = transform;

            InitializeConstantVariables();
            UpdateShaderFillAmount();
        }
    }
}