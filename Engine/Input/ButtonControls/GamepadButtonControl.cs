using Microsoft.Xna.Framework.Input;
using SE.Core;

namespace SE.Input.ButtonControls
{
    public class GamepadButtonControl : ButtonControl
    {
        public const float _TRIGGER_THRESHOLD = 0.333f;

        public Players PlayerIndex { get; set; }

        public GamepadButtons GamepadButton { get; set; }

        ///<inheritdoc/>
        public override State CurrentState { get; protected set; }
        ///<inheritdoc/>
        public override State PreviousState { get; protected set; }

        ///<inheritdoc/>
        public override bool Pressed => PreviousState == State.Up && CurrentState == State.Down;
        ///<inheritdoc/>
        public override bool Released => PreviousState == State.Down && CurrentState == State.Up;
        ///<inheritdoc/>
        public override bool Down => CurrentState == State.Down;
        ///<inheritdoc/>
        public override bool Up => CurrentState == State.Up;

        public GamepadButtonControl(GamepadButtons gamepadButton)
        {
            GamepadButton = gamepadButton;
        }

        ///<inheritdoc/>
        public override void Update(float deltaTime)
        {
            GamePadState controlState = InputManager.GamePadStates[(int)PlayerIndex];
            if (!InputManager.GamePadCapabilities[(int)PlayerIndex].IsConnected) {
                CurrentState = State.None;
                return;
            }
            PreviousState = CurrentState;

            switch (GamepadButton) {
                case GamepadButtons.LTrigger:
                    CurrentState = controlState.Triggers.Left > _TRIGGER_THRESHOLD ? State.Down : State.Up;
                    break;
                case GamepadButtons.RTrigger:
                    CurrentState = controlState.Triggers.Right > _TRIGGER_THRESHOLD ? State.Down : State.Up;
                    break;
                default:
                    CurrentState = controlState.IsButtonDown((Buttons)GamepadButton) ? State.Down : State.Up;
                    break;
            }
        }

        public override ButtonControl DeepCopy()
            => new GamepadButtonControl(GamepadButton);
    }
}
