using System;
using Microsoft.Xna.Framework.Input;
using SE.Input.AxisControls;

namespace SE.Input
{
    public class AxisInput
    {
        public AxisControl Axis { get; }
        public ButtonInputSet Modifier { get; set; }

        internal Players PlayerIndex {
            set {
                Axis.PlayerIndex = value;
                if(Modifier != null)
                    Modifier.PlayerIndex = value;
            }
        }

        public float State {
            get {
                if (Modifier == null)
                    return Axis.State;
                if (!Modifier.Down)
                    return 0;

                return Axis.State;
            }
        }

        internal void Update(float deltaTime)
        {
            if (Modifier == null) {
                Axis.Update(deltaTime);
            } else {
                Modifier.NewFrame();
                Modifier.Update(deltaTime);
                Axis.Update(deltaTime);
                if (!Modifier.Down)
                    Axis.State = 0f;
            }
        }

        internal AxisInput(AxisControl axis)
        {
            Axis = axis ?? throw new ArgumentNullException(nameof(axis));
        }

        internal AxisInput(AxisControl axis, ButtonInputSet modifier)
        {
            Axis = axis ?? throw new ArgumentNullException(nameof(axis));
            if (modifier != null)
                Modifier = modifier;
        }

        public static AxisInput FromThumbStick(ThumbSticks thumbStick, ThumbSticksAxis thumbSticksAxis, float deadzone = 0.05f, bool reverse = false) 
            => new AxisInput(new GamepadAxisControl(thumbStick, thumbSticksAxis, deadzone, reverse));

        public static AxisInput FromThumbStick(ThumbSticks thumbStick, ThumbSticksAxis thumbSticksAxis, ButtonInputSet modifier, float deadzone = 0.05f, bool reverse = false) 
            => new AxisInput(new GamepadAxisControl(thumbStick, thumbSticksAxis, deadzone, reverse), modifier);

        public static AxisInput FromThumbStick(ThumbSticks thumbStick, ThumbSticksAxis thumbSticksAxis, ButtonInput modifier, float deadzone = 0.05f, bool reverse = false)
            => new AxisInput(new GamepadAxisControl(thumbStick, thumbSticksAxis, deadzone, reverse), new ButtonInputSet(modifier));

        public static AxisInput FromKeyboard(Keys positiveKey, Keys negativeKey, float? sensitivity = null, float? gravity = null, float deadzone = 0.05f, bool reverse = false) 
            => new AxisInput(new KeyAxisControl(positiveKey, negativeKey, sensitivity, gravity, deadzone, reverse));

        public static AxisInput FromKeyboard(Keys positiveKey, Keys negativeKey, ButtonInputSet modifier, float? sensitivity = null, float? gravity = null, float deadzone = 0.05f, bool reverse = false) 
            => new AxisInput(new KeyAxisControl(positiveKey, negativeKey, sensitivity, gravity, deadzone, reverse), modifier);

        public static AxisInput FromKeyboard(Keys positiveKey, Keys negativeKey, ButtonInput modifier, float? sensitivity = null, float? gravity = null, float deadzone = 0.05f, bool reverse = false)
            => new AxisInput(new KeyAxisControl(positiveKey, negativeKey, sensitivity, gravity, deadzone, reverse), new ButtonInputSet(modifier));

        public AxisInput DeepCopy() 
            => new AxisInput(Axis.DeepCopy());
    }
}
