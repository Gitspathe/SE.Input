using SE.Core;

namespace SE.Input.ButtonControls
{
    public class MouseButtonControl : ButtonControl
    {
        public MouseButtons MouseButton { get; set; }

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

        public MouseButtonControl(MouseButtons mouseButton)
        {
            MouseButton = mouseButton;
        }

        ///<inheritdoc/>
        public override void Update(float deltaTime)
        {
            PreviousState = CurrentState;

            switch (MouseButton) {
                case MouseButtons.Left:
                    CurrentState = InputManager.LeftMouseDown ? State.Down : State.Up;
                    break;
                case MouseButtons.Right:
                    CurrentState = InputManager.RightMouseDown ? State.Down : State.Up;
                    break;
            }
        }

        public override ButtonControl DeepCopy()
            => new MouseButtonControl(MouseButton);
    }
}
