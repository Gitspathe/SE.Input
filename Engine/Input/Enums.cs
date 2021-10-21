using System;

namespace SE.Input
{
    public enum Players
    {
        One,
        Two,
        Three,
        Four
    }

    public enum Filter
    {
        Any,
        All
    }

    public enum MouseButtons
    {
        None,
        Left,
        Right
    }

    [Flags]
    public enum GamepadButtons
    {
        DPadUp = 1,
        DPadDown = 2,
        DPadLeft = 4,
        DPadRight = 8,
        Start = 16,
        Back = 32,
        LStick = 64,
        RStick = 128,
        LShoulder = 256,
        RShoulder = 512,
        Guide = 2048,
        A = 4096,
        B = 8192,
        X = 16384,
        Y = 32768,
        LTrigger = 8388608,
        RTrigger = 4194304,
        LThumbstickLeft = 2097152,
        RThumbstickUp = 16777216,
        RThumbstickDown = 33554432,
        RThumbstickRight = 67108864,
        RThumbstickLeft = 134217728,
        LThumbstickUp = 268435456,
        LThumbstickDown = 536870912,
        LThumbstickRight = 1073741824
    }

    public enum ThumbSticks
    {
        Left,
        Right
    }

    public enum ThumbSticksAxis
    {
        X,
        Y
    }
}
