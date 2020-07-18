using System;
using System.Collections.Generic;
using static SE.Core.InputManager;

namespace SE.Input
{
    public class Controller
    {
        public Players PlayerIndex { get; internal set; }

        private Dictionary<string, ButtonInputSet> buttons = new Dictionary<string, ButtonInputSet>();
        private Dictionary<string, AxisInputSet> axis = new Dictionary<string, AxisInputSet>();

        /// <summary>
        /// Updates the controller.
        /// </summary>
        /// <param name="deltaTime">Time in seconds which has passed since the last frame.</param>
        internal void Update(float deltaTime)
        {
            // Reset the update state of all inputs.
            foreach (KeyValuePair<string, ButtonInputSet> buttonInput in buttons)
                buttonInput.Value.NewFrame();
            foreach (KeyValuePair<string, AxisInputSet> axisInput in axis)
                axisInput.Value.NewFrame();

            // Update the inputs.
            foreach (KeyValuePair<string, ButtonInputSet> buttonInput in buttons)
                buttonInput.Value.Update(deltaTime);
            foreach (KeyValuePair<string, AxisInputSet> axisInput in axis)
                axisInput.Value.Update(deltaTime);
        }

        /// <summary>
        /// Registers an axis input to the controller.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="theAxis">Axis input.</param>
        /// <param name="replace">Whether or not to replace an existing input with the same inputName.</param>
        /// <returns>An AxisInputSet created from the provided AxisInput.</returns>
        public AxisInputSet AddAxisInput(string inputName, AxisInput theAxis, bool replace = false)
        {
            AxisInputSet set;
            if (axis.ContainsKey(inputName)) {
                if (replace) {
                    axis.Remove(inputName);
                    set = new AxisInputSet(theAxis);
                    axis.Add(inputName, set);
                } else {
                    set = axis[inputName];
                    set.Inputs.Add(theAxis);
                }
            } else {
                set = new AxisInputSet(theAxis);
                axis.Add(inputName, set);
            }
            set.PlayerIndex = PlayerIndex;
            set.Key = inputName;
            return set;
        }

        /// <summary>
        /// Registers multiple axis inputs to the controller, under a shared ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <param name="axis">One or more axis to be added. All axis inputs will be assigned to the same ID.</param>
        /// <returns>An AxisInputSet created from the provided AxisInputs.</returns>
        public AxisInputSet AddAxisInput(string inputName, bool replace = false, params AxisInput[] axis)
        {
            AxisInputSet set = null;
            if (replace)
                this.axis.Remove(inputName);
            for (int i = 0; i < axis.Length; i++)
                set = AddAxisInput(inputName, axis[i]);
            if (set == null) 
                return null;

            set.Key = inputName;
            set.PlayerIndex = PlayerIndex;
            return set;
        }

        /// <summary>
        /// Registers multiple axis inputs to the controller, under a shared ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="axis">One or more axis to be added. All axis inputs will be assigned to the same ID.</param>
        /// <returns>An AxisInputSet created from the provided AxisInputs.</returns>
        public AxisInputSet AddAxisInput(string inputName, params AxisInput[] axis)
            => AddAxisInput(inputName, false, axis);

        /// <summary>
        /// Removes an AxisInputSet from the controller.
        /// </summary>
        /// <param name="input">AxisInputSet to remove.</param>
        /// <returns>True if the AxisInputSet was found and removed.</returns>
        public bool RemoveAxisInput(AxisInputSet input)
        {
            foreach (KeyValuePair<string, AxisInputSet> btn in axis) {
                if (btn.Value == input) {
                    buttons.Remove(btn.Key);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes an AxisInputSet from the controller.
        /// </summary>
        /// <param name="inputName">ID of the AxisInputSet to remove.</param>
        /// <returns>True if the AxisInputSet was found and removed.</returns>
        public bool RemoveAxisInput(string inputName)
            => axis.Remove(inputName);

        public bool RemoveAxisInput(string inputName, AxisInput theAxis)
        {
            if (!buttons.ContainsKey(inputName))
                return false;

            AxisInputSet axisInput = axis[inputName];
            axisInput.Inputs.Remove(theAxis);
            if (axis.Count == 0) {
                buttons.Remove(inputName);
            }
            return true;
        }

        /// <summary>
        /// Registers a button input to the controller.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="button">ButtonInput to add.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInput.</returns>
        public ButtonInputSet AddButtonInput(string inputName, ButtonInput button, bool replace = false)
        {
            ButtonInputSet set;
            if (buttons.ContainsKey(inputName)) {
                if (replace) {
                    buttons.Remove(inputName);
                    set = new ButtonInputSet(button);
                    buttons.Add(inputName, set);
                } else {
                    set = buttons[inputName];
                    set.Inputs.Add(button);
                }
            } else {
                set = new ButtonInputSet(button);
                buttons.Add(inputName, set);
            }
            set.Key = inputName;
            set.PlayerIndex = PlayerIndex;
            return set;
        }

        /// <summary>
        /// Registers multiple button inputs to the controller, under the same ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <param name="buttons">One or more buttons to add.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInputs.</returns>
        public ButtonInputSet AddButtonInput(string inputName, bool replace = false, params ButtonInput[] buttons)
        {
            ButtonInputSet set = null;
            if (replace)
                this.buttons.Remove(inputName);
            for (int i = 0; i < buttons.Length; i++)
                set = AddButtonInput(inputName, buttons[i]);
            if (set == null)
                return null;

            set.Key = inputName;
            set.PlayerIndex = PlayerIndex;
            return set;
        }

        /// <summary>
        /// Registers multiple button inputs to the controller, under the same ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="buttons">One or more buttons to add.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInputs.</returns>
        public ButtonInputSet AddButtonInput(string inputName, params ButtonInput[] buttons)
            => AddButtonInput(inputName, false, buttons);

        /// <summary>
        /// Removes a ButtonInputSet from the controller.
        /// </summary>
        /// <param name="input">ButtonInputSet to remove.</param>
        /// <returns>True if the ButtonInputSet was found and removed.</returns>
        public bool RemoveButtonInput(ButtonInputSet input)
        {
            if (input == null)
                return false;

            foreach (KeyValuePair<string, ButtonInputSet> btn in buttons) {
                if (btn.Value == input) {
                    buttons.Remove(btn.Key);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a ButtonInputSet from the controller.
        /// </summary>
        /// <param name="inputName">ID of the button input to remove.</param>
        /// <returns>True if the ButtonInputSet was found and removed.</returns>
        public bool RemoveButtonInput(string inputName)
            => buttons.Remove(inputName);

        public bool RemoveButtonInput(string inputName, ButtonInput button)
        {
            if (!buttons.ContainsKey(inputName))
                return false;

            ButtonInputSet buttonInput = buttons[inputName];
            buttonInput.Inputs.Remove(button);
            if (buttonInput.Inputs.Count == 0) {
                buttons.Remove(inputName);
            }
            return true;
        }

        /// <summary>
        /// Check if a button was just pressed.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button was pressed this frame.</returns>
        public bool ButtonPressed(string key)
            => buttons.ContainsKey(key) && buttons[key].Pressed;

        /// <summary>
        /// Checks the pressed state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided were just pressed, according to the provided filter.</returns>
        public bool ButtonPressed(Filter filter = Filter.Any, List<string> buttonInputs = null)
        {
            if (buttonInputs == null) {
                buttonCache.Clear();
                foreach (KeyValuePair<string, ButtonInputSet> btn in buttons) {
                    buttonCache.Add(btn.Key);
                }
                buttonInputs = buttonCache;
            }
            switch (filter) {
                case Filter.Any:
                    for (int i = 0; i < buttonInputs.Count; i++) {
                        string key = buttonInputs[i];
                        if (!buttons.ContainsKey(key))
                            continue;
                        if (buttons[key].Pressed)
                            return true;
                    }
                    return false;
                case Filter.All:
                    for (int i = 0; i < buttonInputs.Count; i++) {
                        string key = buttonInputs[i];
                        if (!buttons.ContainsKey(key) || !buttons[key].Pressed)
                            return false;
                    }
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        /// <summary>
        /// Check if a button was just released.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button was released this frame.</returns>
        public bool ButtonReleased(string key)
            => buttons.ContainsKey(key) && buttons[key].Released;

        /// <summary>
        /// Checks the released state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided were just released, according to the provided filter.</returns>
        public bool ButtonReleased(Filter filter = Filter.Any, List<string> buttonInputs = null)
        {
            if (buttonInputs == null) {
                buttonCache.Clear();
                foreach (KeyValuePair<string, ButtonInputSet> btn in buttons) {
                    buttonCache.Add(btn.Key);
                }
                buttonInputs = buttonCache;
            }
            switch (filter) {
                case Filter.Any:
                    for (int i = 0; i < buttonInputs.Count; i++) {
                        string key = buttonInputs[i];
                        if (!buttons.ContainsKey(key))
                            continue;
                        if (buttons[key].Released)
                            return true;
                    }
                    return false;
                case Filter.All:
                    for (int i = 0; i < buttonInputs.Count; i++) {
                        string key = buttonInputs[i];
                        if (!buttons.ContainsKey(key) || !buttons[key].Released)
                            return false;
                    }
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        /// <summary>
        /// Check if a button is down.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button is down frame.</returns>
        public bool ButtonDown(string key)
            => buttons.ContainsKey(key) && buttons[key].Down;

        /// <summary>
        /// Checks the down state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided are down, according to the provided filter.</returns>
        public bool ButtonDown(Filter filter = Filter.Any, List<string> buttonInputs = null)
        {
            if (buttonInputs == null) {
                buttonCache.Clear();
                foreach (KeyValuePair<string, ButtonInputSet> btn in buttons) {
                    buttonCache.Add(btn.Key);
                }
                buttonInputs = buttonCache;
            }
            switch (filter) {
                case Filter.Any:
                    for (int i = 0; i < buttonInputs.Count; i++) {
                        string key = buttonInputs[i];
                        if (!buttons.ContainsKey(key))
                            continue;
                        if (buttons[key].Down)
                            return true;
                    }
                    return false;
                case Filter.All:
                    for (int i = 0; i < buttonInputs.Count; i++) {
                        string key = buttonInputs[i];
                        if (!buttons.ContainsKey(key) || !buttons[key].Down)
                            return false;
                    }
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }

        /// <summary>
        /// Check if a button is up.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button is up this frame.</returns>
        public bool ButtonUp(string key)
            => !ButtonDown(key);

        /// <summary>
        /// Checks the up state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided are up, according to the provided filter.</returns>
        public bool ButtonUp(Filter filter = Filter.Any, List<string> buttonInputs = null)
            => !ButtonDown(filter, buttonInputs);

        /// <summary>
        /// Checks the state of an axis input.
        /// </summary>
        /// <param name="key">ID of the axis to check.</param>
        /// <returns>Normalized float, from 0 to 1, representing the axis state.</returns>
        public float AxisState(string key)
            => !axis.ContainsKey(key) ? 0f : axis[key].AxisState;

        internal Controller(Players playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}
