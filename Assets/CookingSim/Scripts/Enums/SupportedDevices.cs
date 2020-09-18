using System.ComponentModel;

namespace XRAccelerator.Enums
{
    public enum SupportedVRControllers
    {
        [Description("Valve Index")]
        Knuckles,
        [Description("Oculus")]
        OculusTouch,
        [Description("HTC Vive")]
        ViveWand,
        [Description("Unsupported")]
        Unsupported
    }
}