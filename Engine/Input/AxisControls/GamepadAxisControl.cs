using System;
using SE.Core;

namespace SE.Input.AxisControls
{
    public class GamepadAxisControl : AxisControl
    {
        public ThumbSticks ThumbStick { get; set; }
        public ThumbSticksAxis ThumbStickAxis { get; set; }

        public override float Deadzone { get; set; }

        private float state;
        ///<inheritdoc/>
        public override float State {
            get => Math.Abs(state) <= Deadzone ? 0f : state;
            set => state = value;
        }

        public override bool Reverse { get; set; }

        public GamepadAxisControl(ThumbSticks thumbStick, ThumbSticksAxis axis, float deadzone, bool reverse)
        {
            ThumbStick = thumbStick;
            ThumbStickAxis = axis;
            Deadzone = deadzone;
            Reverse = reverse;
        }

        ///<inheritdoc/>
        public override void Update(float deltaTime)
        {
            float val;

            // Update temporary value according to the state of the thumb stick axis.
            switch (ThumbStick) {
                case ThumbSticks.Left:
                    val = ThumbStickAxis == ThumbSticksAxis.X
                        ? InputManager.GamePadStates[(int)PlayerIndex].ThumbSticks.Left.X 
                        : InputManager.GamePadStates[(int)PlayerIndex].ThumbSticks.Left.Y;
                    break;
                case ThumbSticks.Right:
                    val = ThumbStickAxis == ThumbSticksAxis.X
                        ? InputManager.GamePadStates[(int)PlayerIndex].ThumbSticks.Right.X 
                        : InputManager.GamePadStates[(int)PlayerIndex].ThumbSticks.Right.Y;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Reverse the axis value if needed.
            if (Reverse) {
                val = -val;
            }

            // If the value is close to zero (the dead zone), make it zero.
            if (Math.Abs(val) <= Deadzone) {
                val = 0;
            }

            // Set the axis state.
            State = Extensions.Clamp(val, -1, 1);
        }

        public override AxisControl DeepCopy() 
            => new GamepadAxisControl(ThumbStick, ThumbStickAxis, Deadzone, Reverse);
    }
}
