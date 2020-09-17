using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace XRAccelerator.Player
{
    // Heavily inspired by InputHelpers class from XRToolkit.
    public static class InputDeviceUtils
    {
        private enum ButtonReadType
        {
            None = 0,
            Binary,
            Axis1D,
            Axis2DUp,
            Axis2DDown,
            Axis2DLeft,
            Axis2DRight
        }

        private readonly struct ButtonFeatureInfo
        {
            public ButtonFeatureInfo(string name, ButtonReadType type)
            {
                Name = name;
                Type = type;
            }

            public readonly string Name;
            public readonly ButtonReadType Type;
        }

        static Dictionary<InputHelpers.Button, ButtonFeatureInfo> buttonFeatureInfos =
            new Dictionary<InputHelpers.Button, ButtonFeatureInfo>
            {
                {InputHelpers.Button.None, new ButtonFeatureInfo("", ButtonReadType.None)},
                {InputHelpers.Button.MenuButton, new ButtonFeatureInfo("MenuButton", ButtonReadType.Binary)},
                {InputHelpers.Button.Trigger, new ButtonFeatureInfo("Trigger", ButtonReadType.Axis1D)},
                {InputHelpers.Button.Grip, new ButtonFeatureInfo("Grip", ButtonReadType.Axis1D)},
                {InputHelpers.Button.TriggerPressed, new ButtonFeatureInfo("TriggerPressed", ButtonReadType.Binary)},
                {InputHelpers.Button.GripPressed, new ButtonFeatureInfo("GripPressed", ButtonReadType.Binary)},
                {InputHelpers.Button.PrimaryButton, new ButtonFeatureInfo("PrimaryButton", ButtonReadType.Binary)},
                {InputHelpers.Button.PrimaryTouch, new ButtonFeatureInfo("PrimaryTouch", ButtonReadType.Binary)},
                {InputHelpers.Button.SecondaryButton, new ButtonFeatureInfo("SecondaryButton", ButtonReadType.Binary)},
                {InputHelpers.Button.SecondaryTouch, new ButtonFeatureInfo("SecondaryTouch", ButtonReadType.Binary)},
                {InputHelpers.Button.Primary2DAxisTouch, new ButtonFeatureInfo("Primary2DAxisTouch", ButtonReadType.Binary)},
                {InputHelpers.Button.Primary2DAxisClick, new ButtonFeatureInfo("Primary2DAxisClick", ButtonReadType.Binary)},
                {InputHelpers.Button.Secondary2DAxisTouch, new ButtonFeatureInfo("Secondary2DAxisTouch", ButtonReadType.Binary)},
                {InputHelpers.Button.Secondary2DAxisClick, new ButtonFeatureInfo("Secondary2DAxisClick", ButtonReadType.Binary)},
                {InputHelpers.Button.PrimaryAxis2DUp , new ButtonFeatureInfo("Primary2DAxis", ButtonReadType.Axis2DUp)},
                {InputHelpers.Button.PrimaryAxis2DDown , new ButtonFeatureInfo("Primary2DAxis", ButtonReadType.Axis2DDown)},
                {InputHelpers.Button.PrimaryAxis2DLeft , new ButtonFeatureInfo("Primary2DAxis", ButtonReadType.Axis2DLeft)},
                {InputHelpers.Button.PrimaryAxis2DRight , new ButtonFeatureInfo("Primary2DAxis", ButtonReadType.Axis2DRight)},
                {InputHelpers.Button.SecondaryAxis2DUp , new ButtonFeatureInfo("Secondary2DAxis", ButtonReadType.Axis2DUp)},
                {InputHelpers.Button.SecondaryAxis2DDown , new ButtonFeatureInfo("Secondary2DAxis", ButtonReadType.Axis2DDown)},
                {InputHelpers.Button.SecondaryAxis2DLeft , new ButtonFeatureInfo("Secondary2DAxis", ButtonReadType.Axis2DLeft)},
                {InputHelpers.Button.SecondaryAxis2DRight , new ButtonFeatureInfo("Secondary2DAxis", ButtonReadType.Axis2DRight)},
        };

        public static bool GetPressValue(InputDevice device, InputHelpers.Button button, out float pressValue)
        {
            if (!buttonFeatureInfos.ContainsKey(button))
            {
                throw new ArgumentException(
                    $"[InputDeviceUtils.GetPressValue] The value of <button> '{button}' is not supported.");
            }

            pressValue = 0;

            if (!device.isValid)
            {
                return false;
            }

            ButtonFeatureInfo info = buttonFeatureInfos[button];
            switch (info.Type)
            {
                case ButtonReadType.Binary:
                {
                    if (device.TryGetFeatureValue(new InputFeatureUsage<bool>(info.Name), out bool value))
                    {
                        pressValue = value ? 1 : 0;
                        return true;
                    }

                    break;
                }
                case ButtonReadType.Axis1D:
                {
                    if (device.TryGetFeatureValue(new InputFeatureUsage<float>(info.Name), out pressValue))
                    {
                        return true;
                    }

                    break;
                }

                case ButtonReadType.Axis2DUp:
                {
                    if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(info.Name), out Vector2 value))
                    {
                        pressValue = value.y;
                        return true;
                    }

                    break;
                }
                case ButtonReadType.Axis2DDown:
                {
                    if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(info.Name), out Vector2 value))
                    {
                        pressValue = value.y;
                        return true;
                    }

                    break;
                }
                case ButtonReadType.Axis2DLeft:
                {
                    if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(info.Name), out Vector2 value))
                    {
                        pressValue = value.x;
                        return true;
                    }

                    break;
                }
                case ButtonReadType.Axis2DRight:
                {
                    if (device.TryGetFeatureValue(new InputFeatureUsage<Vector2>(info.Name), out Vector2 value))
                    {
                        pressValue = value.x;
                        return true;
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentException(
                        $"[InputDeviceUtils.GetPressValue] The value of <button> '{button}' has no pressValue.");
                }
            }

            return false;
        }
    }
}