using Microsoft.Xna.Framework.Input;
using SE.Core;
using System;

namespace SE.Input.AxisControls
{
    public class KeyAxisControl : AxisControl
    {
        private Keys KeyPositive { get; set; }
        private Keys KeyNegative { get; set; }

        public float? Sensitivity = 10f;
        public float? Gravity = 20f;

        private float state;

        ///<inheritdoc/>
        public override float State {
            get => Math.Abs(state) <= Deadzone ? 0f : state;
            set => state = value;
        }

        public override float Deadzone { get; set; }

        public override bool Reverse { get; set; }

        public KeyAxisControl(Keys keyPositive, Keys keyNegative, float? sensitivity, float? gravity, float deadzone, bool reverse)
        {
            KeyPositive = keyPositive;
            KeyNegative = keyNegative;
            Sensitivity = sensitivity;
            Gravity = gravity;
            Deadzone = deadzone;
            Reverse = reverse;
        }

        ///<inheritdoc/>
        public override void Update(float deltaTime)
        {
            float val = 0f;

            // Update temporary value according to the positive and/or negative keys being pressed.
            if (InputManager.NewKeys.Contains(KeyPositive)) {
                if (Sensitivity.HasValue) {
                    val += Sensitivity.Value;
                } else {
                    val += 42069.1337f;
                }
            }
            if (InputManager.NewKeys.Contains(KeyNegative)) {
                if (Sensitivity.HasValue) {
                    val -= Sensitivity.Value;
                } else {
                    val -= 42069.1337f;
                }
            }

            // Reverse the axis value if needed.
            if (Reverse) {
                val = -val;
            }

            // If the value is close to zero (the dead zone), make it zero. Apply gravity.
            if (Math.Abs(val) <= Deadzone) {
                val = 0;
                if (!Gravity.HasValue) {
                    State = 0f;
                } else if (Math.Abs(state) > Deadzone) {
                    if (State > 0) {
                        val = -Gravity.Value;
                    } else {
                        val = Gravity.Value;
                    }
                }
            }

            // Set the axis state.
            State = Extensions.Clamp(State + val * deltaTime, -1, 1);
        }

        public override AxisControl DeepCopy()
            => new KeyAxisControl(KeyPositive, KeyNegative, Sensitivity, Gravity, Deadzone, Reverse);
    }

}
