using System;
using System.Collections.Generic;

namespace SE.Input
{
    public class AxisInputSet
    {
        public List<AxisInput> Inputs { get; }
        public string Key { get; internal set; }

        internal Players PlayerIndex {
            set {
                foreach (AxisInput input in Inputs)
                    input.PlayerIndex = value;
            }
        }

        internal bool Updated; // Prevent excess updates.

        internal void Update(float deltaTime)
        {
            if(Updated)
                return;
            for (int i = 0; i < Inputs.Count; i++)
                Inputs[i].Update(deltaTime);
            
            Updated = true;
        }

        internal void NewFrame()
        {
            Updated = false;
        }

        public float AxisState {
            get {
                for (int i = 0; i < Inputs.Count; i++)
                    if (Math.Abs(Inputs[i].State) > 0.001f)
                        return Inputs[i].State;
                
                return 0f;
            }
        }

        public AxisInputSet(List<AxisInput> inputs)
        {
            if (inputs == null)
                throw new ArgumentNullException(nameof(inputs));
            if(inputs.Count < 1)
                throw new InvalidOperationException("Tried to initialize a AxisInputSet with an empty list of inputs.");

            Inputs = inputs;
        }

        public AxisInputSet(AxisInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            Inputs = new List<AxisInput> { input };
        }

        public AxisInputSet DeepCopy()
        {
            List<AxisInput> copyInputs = new List<AxisInput>();
            foreach (AxisInput existing in Inputs)
                copyInputs.Add(existing.DeepCopy());

            AxisInputSet copy = new AxisInputSet(copyInputs);
            return copy;
        }
    }

}
