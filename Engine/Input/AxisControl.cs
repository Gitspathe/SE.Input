namespace SE.Input
{
    public abstract class AxisControl
    {
        public Players PlayerIndex { get; internal set; }
        /// <summary>Normalized float representing the state of the axis.</summary>
        public abstract float State { get; set; }
        /// <summary>When below this value, the State will return 0.
        ///          This is useful in preventing accidental inputs from gamepads.</summary>
        public abstract float Deadzone { get; set; }
        /// <summary>Whether or not to invert the axis.</summary>
        public abstract bool Reverse { get; set; }
        
        /// <summary>
        /// Updates the axis.
        /// </summary>
        /// <param name="deltaTime">Time in seconds which has passed since last frame.</param>
        public abstract void Update(float deltaTime);

        /// <summary>
        /// Creates a deep copy of the AxisControl.
        /// </summary>
        /// <returns>A deep copy.</returns>
        public abstract AxisControl DeepCopy();
    }
}
