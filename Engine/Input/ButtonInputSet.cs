using System;
using System.Collections.Generic;

namespace SE.Input
{
    public class ButtonInputSet
    {
        public List<ButtonInput> Inputs { get; }
        public string Key { get; internal set; }

        internal Players PlayerIndex {
            set {
                foreach (ButtonInput input in Inputs)
                    input.PlayerIndex = value;
            }
        }

        internal bool Updated; // Prevent excess updates.

        internal void Update(float deltaTime)
        {
            if (Updated)
                return;
            for (int i = 0; i < Inputs.Count; i++)
                Inputs[i].Update(deltaTime);

            Updated = true;
        }

        internal void NewFrame()
        {
            Updated = false;
        }

        public bool Pressed {
            get {
                for (int i = 0; i < Inputs.Count; i++)
                    if (Inputs[i].Pressed)
                        return true;

                return false;
            }
        }

        public bool Released {
            get {
                for (int i = 0; i < Inputs.Count; i++)
                    if (Inputs[i].Released)
                        return true;

                return false;
            }
        }

        public bool Down {
            get {
                for (int i = 0; i < Inputs.Count; i++)
                    if (Inputs[i].Down)
                        return true;

                return false;
            }
        }

        public bool Up => !Down;

        public ButtonInputSet(List<ButtonInput> inputs)
        {
            if (inputs == null)
                throw new ArgumentNullException(nameof(inputs));
            if (inputs.Count < 1)
                throw new InvalidOperationException("Tried to initialize a ButtonInputSet with an empty list of inputs.");

            Inputs = inputs;
        }

        public ButtonInputSet(ButtonInput input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            Inputs = new List<ButtonInput> { input };
        }

        public ButtonInputSet DeepCopy()
        {
            List<ButtonInput> copyInputs = new List<ButtonInput>();
            foreach (ButtonInput existing in Inputs)
                copyInputs.Add(existing.DeepCopy());

            ButtonInputSet copy = new ButtonInputSet(copyInputs);
            return copy;
        }
    }
}
