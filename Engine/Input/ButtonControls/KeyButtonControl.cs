using Microsoft.Xna.Framework.Input;
using SE.Core;

namespace SE.Input.ButtonControls
{
    public class KeyButtonControl : ButtonControl
    {
        public Keys KeyboardKey { get; set; }

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

        public KeyButtonControl(Keys key)
        {
            KeyboardKey = key;
        }

        ///<inheritdoc/>
        public override void Update(float deltaTime)
        {
            PreviousState = CurrentState;
            CurrentState = InputManager.NewKeys.Contains(KeyboardKey) ? State.Down : State.Up;
        }

        public override ButtonControl DeepCopy() 
            => new KeyButtonControl(KeyboardKey);
    }
}
