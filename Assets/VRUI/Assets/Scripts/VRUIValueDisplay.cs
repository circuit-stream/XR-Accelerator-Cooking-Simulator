using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XRAccelerator.VRUI
{
    public class VRUIValueDisplay : VRUITooltip
    {
        [SerializeField] private float maxValue;

        [SerializeField] private Text valueText;

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public void UpdateUIValue(float normalizedValue)
        {
            valueText.text = (Mathf.Ceil(normalizedValue * maxValue)).ToString();
        }
    }
}