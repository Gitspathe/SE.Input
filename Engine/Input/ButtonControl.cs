using SE.Input.ButtonControls;

namespace SE.Input
{
    public abstract class ButtonControl
    {
        /// <summary>State this frame.</summary>
        public abstract State CurrentState { get; protected set; }
        /// <summary>State last frame.</summary>
        public abstract State PreviousState { get; protected set; }

        public Players PlayerIndex { get; internal set; }

        /// <summary>If the button was pressed this frame.</summary>
        public abstract bool Pressed { get; }
        /// <summary>If the button was released this frame.</summary>
        public abstract bool Released { get; }
        /// <summary>If the button is down this frame.</summary>
        public abstract bool Down { get; }
        /// <summary>If the button is up this frame.</summary>
        public abstract bool Up { get; }

        /// <summary>
        /// Updates the button input.
        /// </summary>
        /// <param name="deltaTime">Time in seconds which has passed since last frame.</param>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Creates a deep copy of the ButtonControl.
        /// </summary>
        /// <returns>A deep copy.</returns>
        public abstract ButtonControl DeepCopy();
    }
}
