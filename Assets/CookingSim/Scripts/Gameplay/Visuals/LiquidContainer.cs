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
        private float uprightContainerLocalHeight;
        private float currentContainerMaxLocalHeight;
        private float currentContainerExtraLocalHeight;
        private float containerRadius;
        private float containerVolumePerHeight;

        private float currentLiquidHeight;

        private const int pourPointsAmount = 10;
        private List<Transform> pourPoints;

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

        public float CurrentLiquidVolume => currentLiquidHeight * containerVolumePerHeight;

        public Transform GetCurrentPourPoint()
        {
            Transform lowestPoint = null;
            float lowestHeight = Mathf.Infinity;

            foreach (var point in pourPoints)
            {
                if (point.position.y < lowestHeight)
                {
                    lowestHeight = point.position.y;
                    lowestPoint = point;
                }
            }

            return lowestPoint;
        }

        #region Container Logic

        public void AddLiquid(float volume)
        {
            currentLiquidHeight += volume / containerVolumePerHeight;
        }

        private void Spill(float volumeHeightSpilled)
        {
            currentLiquidHeight -= volumeHeightSpilled;
            Spilled?.Invoke(volumeHeightSpilled * containerVolumePerHeight);
        }

        private void TrySpill()
        {
            // TODO Arthur: Consider wobbling

            var eulerAngles = _transform.eulerAngles;
            var maxAngle = Mathf.Max(GetSignedAngle(eulerAngles.x), GetSignedAngle(eulerAngles.z));

            currentContainerMaxLocalHeight = Mathf.Cos(Mathf.Deg2Rad * maxAngle) * uprightContainerLocalHeight;
            currentContainerExtraLocalHeight = Mathf.Sin(Mathf.Deg2Rad * maxAngle) * containerRadius;

            if (currentLiquidHeight > currentContainerMaxLocalHeight)
            {
                Spill(currentLiquidHeight - currentContainerMaxLocalHeight);
            }
        }

        private float GetSignedAngle(float angle)
        {
            return Mathf.Abs(angle > 180 ? angle - 360 : angle);
        }

        private void UpdateShaderFillAmount()
        {
            var shaderFill = currentLiquidHeight
                             - (currentContainerMaxLocalHeight * 0.5f) // offset from range [0, height] to [-height/2, height/2]
                             - currentContainerExtraLocalHeight; // remove the extra height that can't hold liquid when rotated
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

        private void CreatePourPoints()
        {
            pourPoints = new List<Transform>();
            var horizontalScale = _transform.localScale.x;
            var radius = containerRadius / horizontalScale + 0.05f;

            Transform pourPointsParent = new GameObject("PourPoints").transform;
            pourPointsParent.parent = _transform;
            pourPointsParent.rotation = Quaternion.identity;
            pourPointsParent.localScale = Vector3.one;
            pourPointsParent.localPosition = new Vector3(0, 1.05f, 0);

            for (int index = 0; index < pourPointsAmount; index++)
            {
                Transform newObject = new GameObject($"PourPoint{index}").transform;
                newObject.parent = pourPointsParent;
                newObject.rotation = Quaternion.identity;
                newObject.localScale = Vector3.one;

                var x = radius * Mathf.Sin((2 * Mathf.PI * index) / pourPointsAmount);
                var z = radius * Mathf.Cos((2 * Mathf.PI * index) / pourPointsAmount);
                newObject.localPosition = new Vector3(x, 0, z);

                pourPoints.Add(newObject);
            }
        }

        private void InitializeConstantVariables()
        {
            // Container Variables
            Vector3 containerBounds = _renderer.bounds.size;
            uprightContainerLocalHeight = containerBounds.y;
            currentContainerMaxLocalHeight = uprightContainerLocalHeight;
            containerRadius = containerBounds.x * 0.5f;

            containerVolumePerHeight = containerVolume / uprightContainerLocalHeight;

            CreatePourPoints();

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