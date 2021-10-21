using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SE.Input;
using SE.Utility;
using System;

namespace SE.Core
{
    /// <summary>
    /// Handles input.
    /// </summary>
    public static class InputManager
    {
        internal static Keys[] OldKeys;
        internal static Keys[] NewKeys;

        internal static GamePadCapabilities[] GamePadCapabilities = new GamePadCapabilities[4];
        internal static GamePadState[] GamePadStates = new GamePadState[4];

        private static Controller[] controllers = new Controller[4];
        private static QuickList<Controller> controllerAlloc = new QuickList<Controller>();

        internal static QuickList<string> buttonCache = new QuickList<string>();
        internal static QuickList<char> oldPressedChars = new QuickList<char>();

        internal static int oldScrollValue;
        internal static int scrollValue;

        internal static KeyboardState oldKeyboardState;
        internal static MouseState oldMouseState;

        private static Game game;

        internal const float _CAPABILITY_CHECK_TIMER = 1.0f;
        internal static float curCapabilityTimer = _CAPABILITY_CHECK_TIMER;

        /// <summary>Current keyboard state.</summary>
        public static KeyboardState KeyboardState { get; private set; }

        /// <summary>Current mouse state.</summary>
        public static MouseState MouseState { get; private set; }

        /// <summary>True if either the left or right mouse button was clicked this frame.</summary>
        public static bool MouseClicked { get; private set; }

        /// <summary>True if the left mouse button was clicked this frame.</summary>
        public static bool LeftMouseClicked { get; private set; }

        /// <summary>True if the right mouse button was clicked this frame.</summary>
        public static bool RightMouseClicked { get; private set; }

        /// <summary>True if either the left or right mouse button is down this frame.</summary>
        public static bool MouseDown { get; private set; }

        /// <summary>True if the left mouse button is down this frame.</summary>
        public static bool LeftMouseDown { get; private set; }

        /// <summary>True if the right mouse button is down this frame.</summary>
        public static bool RightMouseDown { get; private set; }

        /// <summary>True if either the left or right mouse button was released this frame.</summary>
        public static bool MouseReleased { get; private set; }

        /// <summary>True if the left mouse button was released this frame.</summary>
        public static bool LeftMouseReleased { get; private set; }

        /// <summary>True if the right mouse button was released this frame.</summary>
        public static bool RightMouseReleased { get; private set; }

        /// <summary>State of the mouse scroll wheel. Is -1 if the wheel was scrolled backwards, 1 if scrolled forward, or 0 if there was no change.</summary>
        public static int MouseScrollValue { get; private set; }

        /// <summary>List of KeyCodes which were pressed this frame.</summary>
        public static QuickList<Keys> PressedKeys { get; } = new QuickList<Keys>();

        /// <summary>List of characters which were pressed this frame.</summary>
        public static QuickList<char> PressedChars { get; } = new QuickList<char>();

        /// <summary>True if the cursor moved since last frame.</summary>
        public static bool MouseMoved => oldMouseState.Position != MouseState.Position;

        /// <summary>Determines whether or not to update the input system.</summary>
        public static bool Enabled { get; set; } = true;

        /// <summary>If true, the input system will be updated even when the game window isn't in focus.</summary>
        public static bool UpdateWhenNotFocused { get; set; } = false;

        /// <summary>True if the input manager is in a state in which it will be updated each frame.</summary>
        public static bool IsActive => Enabled && (UpdateWhenNotFocused || (!UpdateWhenNotFocused && game.IsActive));

        private static readonly Players[] All = { Players.One, Players.Two, Players.Three, Players.Four };

        /// <summary>
        /// Initializes the input system.
        /// </summary>
        /// <param name="game">Game instance the input system shall bind to.</param>
        /// <param name="handleWindowInput">Whether or not to handle text input.</param>
        public static void Initialize(Game game, bool handleWindowInput = true)
        {
            InputManager.game = game;
            if (handleWindowInput) {
                game.Window.TextInput += TextInput;
            }

            for (int i = 0; i < controllers.Length; i++) {
                controllers[i] = new Controller((Players)i);
            }
        }

        /// <summary>
        /// Updates the input system. Should be called once per frame.
        /// </summary>
        /// <param name="deltaTime">Time in seconds which has passed since the last frame.</param>
        public static void Update(float deltaTime)
        {
            // TODO: Multiple gamepad support (local multiplayer).
            // Check for capabilities each second. Reduces GC allocations.
            if (curCapabilityTimer >= _CAPABILITY_CHECK_TIMER) {
                for (int i = 0; i < GamePadCapabilities.Length; i++) {
                    GamePadCapabilities[i] = GamePad.GetCapabilities(i);
                }
                curCapabilityTimer = 0.0f;
            } else {
                curCapabilityTimer += deltaTime;
            }

            for (int i = 0; i < GamePadCapabilities.Length; i++) {
                GamePadStates[i] = GamePadCapabilities[i].IsConnected
                    ? GamePad.GetState(i)
                    : default;
            }

            // Clear pressed keys.
            PressedKeys.Clear();
            PressedChars.Clear();

            // Update inputs.
            if (IsActive) {
                UpdateInputs(deltaTime);
            }

            // Clean-up for next frame.
            oldScrollValue = scrollValue;
            oldPressedChars.Clear();
        }

        public static Controller GetController(Players playerIndex)
            => controllers[(int)playerIndex];
        public static Controller GetController(int playerIndex)
            => controllers[playerIndex];

        private static void UpdateInputs(float deltaTime)
        {
            // Set old states.
            oldScrollValue = scrollValue;
            oldMouseState = MouseState;
            oldKeyboardState = KeyboardState;

            // Get current keyboard and mouse states.
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            PressedChars.AddRange(oldPressedChars);

            // Check key state capacities and move the key states into arrays.
            OldKeys = oldKeyboardState.GetPressedKeys();
            NewKeys = KeyboardState.GetPressedKeys();
            for (int i = 0; i < NewKeys.Length; i++) {
                if (!OldKeys.Contains(NewKeys[i])) {
                    PressedKeys.Add(NewKeys[i]);
                }
            }

            // Set mouse properties.
            LeftMouseDown = MouseState.LeftButton == ButtonState.Pressed;
            RightMouseDown = MouseState.RightButton == ButtonState.Pressed;
            MouseDown = LeftMouseDown || RightMouseDown;

            LeftMouseClicked = oldMouseState.LeftButton == ButtonState.Released && LeftMouseDown;
            RightMouseClicked = oldMouseState.RightButton == ButtonState.Released && RightMouseDown;
            MouseClicked = LeftMouseClicked || RightMouseClicked;

            LeftMouseReleased = oldMouseState.LeftButton == ButtonState.Pressed && !LeftMouseDown;
            RightMouseReleased = oldMouseState.RightButton == ButtonState.Pressed && !RightMouseDown;
            MouseReleased = LeftMouseReleased || RightMouseReleased;

            // Set the scroll wheel state.
            scrollValue = MouseState.ScrollWheelValue;
            if (scrollValue > oldScrollValue) {
                MouseScrollValue = 1;
            } else if (scrollValue < oldScrollValue) {
                MouseScrollValue = -1;
            } else {
                MouseScrollValue = 0;
            }

            for (int i = 0; i < controllers.Length; i++) {
                controllers[i].Update(deltaTime);
            }
        }

        private static void CheckKeysCapacities(int oldKeysCount, int newKeysCount)
        {
            if (OldKeys == null || OldKeys.Length < oldKeysCount) {
                OldKeys = OldKeys == null
                    ? new Keys[64]
                    : new Keys[OldKeys.Length * 2];
            }
            if (NewKeys == null || NewKeys.Length < newKeysCount) {
                NewKeys = NewKeys == null
                    ? new Keys[64]
                    : new Keys[NewKeys.Length * 2];
            }
        }

        private static bool KeysSpanContains(ReadOnlySpan<Keys> span, Keys key)
        {
            for (int i = 0; i < span.Length; i++) {
                if (span[i] == key)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Registers an axis input to the input manager.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="theAxis">Axis input.</param>
        /// <param name="players">Array of players to add the axis input to.</param>
        /// <param name="replace">Whether or not to replace an existing input with the same inputName.</param>
        /// <returns>An AxisInputSet created from the provided AxisInput.</returns>
        public static void AddAxisInput(string inputName, AxisInput theAxis, Players[] players, bool replace = false)
        {
            if (players == null)
                players = All;
            for (int i = 0; i < players.Length; i++)
                controllers[(int)players[i]].AddAxisInput(inputName, theAxis.DeepCopy(), replace);
        }
        /// <summary>
        /// Registers an axis input to the input manager.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="theAxis">Axis input.</param>
        /// <param name="replace">Whether or not to replace an existing input with the same inputName.</param>
        /// <returns>An AxisInputSet created from the provided AxisInput.</returns>
        public static void AddAxisInput(string inputName, AxisInput theAxis, bool replace)
            => AddAxisInput(inputName, theAxis, null, replace);
        /// <summary>
        /// Registers an axis input to the input manager.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="theAxis">Axis input.</param>
        /// <returns>An AxisInputSet created from the provided AxisInput.</returns>
        public static void AddAxisInput(string inputName, AxisInput theAxis)
            => AddAxisInput(inputName, theAxis, null, false);

        /// <summary>
        /// Registers multiple axis inputs to the input system, under a shared ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <param name="players">Array of players to add the axis inputs to.</param>
        /// <param name="axis">One or more axis to be added. All axis inputs will be assigned to the same ID.</param>
        /// <returns>An AxisInputSet created from the provided AxisInputs.</returns>
        public static void AddAxisInputs(string inputName, bool replace, Players[] players, params AxisInput[] axis)
        {
            if (players == null)
                players = All;

            for (int i = 0; i < players.Length; i++) {
                AxisInput[] copy = new AxisInput[axis.Length];
                for (int y = 0; y < axis.Length; y++) {
                    copy[y] = axis[y].DeepCopy();
                }

                controllers[(int)players[i]].AddAxisInput(inputName, replace, copy);
            }
        }
        /// <summary>
        /// Registers multiple axis inputs to the input system, under a shared ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <param name="axis">One or more axis to be added. All axis inputs will be assigned to the same ID.</param>
        /// <returns>An AxisInputSet created from the provided AxisInputs.</returns>
        public static void AddAxisInputs(string inputName, bool replace, params AxisInput[] axis)
            => AddAxisInputs(inputName, replace, null, axis);

        /// <summary>
        /// Registers multiple axis inputs to the input system, under a shared ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="players">Array of players to add the axis input to.</param>
        /// <param name="axis">One or more axis to be added. All axis inputs will be assigned to the same ID.</param>
        /// <returns>An AxisInputSet created from the provided AxisInputs.</returns>
        public static void AddAxisInputs(string inputName, Players[] players, params AxisInput[] axis)
            => AddAxisInputs(inputName, false, players, axis);
        /// <summary>
        /// Registers multiple axis inputs to the input system, under a shared ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="axis">One or more axis to be added. All axis inputs will be assigned to the same ID.</param>
        /// <returns>An AxisInputSet created from the provided AxisInputs.</returns>
        public static void AddAxisInputs(string inputName, params AxisInput[] axis)
            => AddAxisInputs(inputName, null, axis);

        /// <summary>
        /// Removes an AxisInputSet from the input manager.
        /// </summary>
        /// <param name="inputName">ID of the AxisInputSet to remove.</param>
        /// <param name="players">Array of players to remove the axis input from. Defaults to all players.</param>
        /// <returns>True if the AxisInputSet was found and removed.</returns>
        public static bool RemoveAxisInput(string inputName, Players[] players = null)
        {
            if (players == null)
                players = All;

            bool found = false;
            for (int i = 0; i < players.Length; i++) {
                if (controllers[(int)players[i]].RemoveAxisInput(inputName)) {
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Registers a button input to the input manager.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="button">ButtonInput to add.</param>
        /// <param name="players">Array of players to add the button input to.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInput.</returns>
        public static void AddButtonInput(string inputName, ButtonInput button, Players[] players, bool replace)
        {
            if (players == null)
                players = All;
            for (int i = 0; i < players.Length; i++)
                controllers[(int)players[i]].AddButtonInput(inputName, button.DeepCopy(), replace);
        }

        /// <summary>
        /// Registers a button input to the input manager.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="button">ButtonInput to add.</param>
        /// <param name="players">Array of players to add the button input to.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInput.</returns>
        public static void AddButtonInput(string inputName, ButtonInput button, Players[] players)
            => AddButtonInput(inputName, button, players, true);

        /// <summary>
        /// Registers a button input to the input manager.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="button">ButtonInput to add.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInput.</returns>
        public static void AddButtonInput(string inputName, ButtonInput button)
            => AddButtonInput(inputName, button, null, true);

        /// <summary>
        /// Registers multiple button inputs to the input manager, under the same ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <param name="players">Array of players to add the button inputs to.</param>
        /// <param name="buttons">One or more buttons to add.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInputs.</returns>
        public static void AddButtonInputs(string inputName, Players[] players, bool replace, params ButtonInput[] buttons)
        {
            if (players == null)
                players = All;

            for (int i = 0; i < players.Length; i++) {
                ButtonInput[] copy = new ButtonInput[buttons.Length];
                for (int y = 0; y < buttons.Length; y++) {
                    copy[y] = buttons[y].DeepCopy();
                }

                controllers[(int)players[i]].AddButtonInput(inputName, replace, copy);
            }
        }
        /// <summary>
        /// Registers multiple button inputs to the input manager, under the same ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="replace">Whether or not to replace existing inputs with the same inputName.</param>
        /// <param name="buttons">One or more buttons to add.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInputs.</returns>
        public static void AddButtonInputs(string inputName, bool replace, params ButtonInput[] buttons)
            => AddButtonInputs(inputName, All, replace, buttons);

        /// <summary>
        /// Registers multiple button inputs to the input manager, under the same ID.
        /// </summary>
        /// <param name="inputName">Name used for identification.</param>
        /// <param name="buttons">One or more buttons to add.</param>
        /// <returns>A ButtonInputSet created from the provided ButtonInputs.</returns>
        public static void AddButtonInputs(string inputName, params ButtonInput[] buttons)
            => AddButtonInputs(inputName, All, false, buttons);

        /// <summary>
        /// Removes a ButtonInputSet from the input manager.
        /// </summary>
        /// <param name="inputName">ID of the button input to remove.</param>
        /// <param name="players">Array of players to remove the button input from. Defaults to all players.</param>
        /// <returns>True if the ButtonInputSet was found and removed.</returns>
        public static bool RemoveButtonInput(string inputName, Players[] players = null)
        {
            if (players == null)
                players = All;

            bool found = false;
            for (int i = 0; i < players.Length; i++) {
                if (controllers[(int)players[i]].RemoveButtonInput(inputName)) {
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Check if a button was just pressed.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button was pressed this frame.</returns>
        public static bool ButtonPressed(Players player, string key)
            => controllers[(int)player].ButtonPressed(key);
        /// <summary>
        /// Check if a button was just pressed by player one.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button was pressed this frame.</returns>
        public static bool ButtonPressed(string key)
            => controllers[0].ButtonPressed(key);

        /// <summary>
        /// Checks the pressed state of multiple buttons.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided were just pressed, according to the provided filter.</returns>
        public static bool ButtonPressed(Players player, Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => controllers[(int)player].ButtonPressed(filter, buttonInputs);
        /// <summary>
        /// Checks the pressed state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided were just pressed, according to the provided filter.</returns>
        public static bool ButtonPressed(Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => controllers[0].ButtonPressed(filter, buttonInputs);

        /// <summary>
        /// Check if a button was just released.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button was released this frame.</returns>
        public static bool ButtonReleased(Players player, string key)
            => controllers[(int)player].ButtonReleased(key);
        /// <summary>
        /// Check if a button was just released.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button was released this frame.</returns>
        public static bool ButtonReleased(string key)
            => controllers[0].ButtonReleased(key);

        /// <summary>
        /// Checks the released state of multiple buttons.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided were just released, according to the provided filter.</returns>
        public static bool ButtonReleased(Players player, Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => controllers[(int)player].ButtonReleased(filter, buttonInputs);
        /// <summary>
        /// Checks the released state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided were just released, according to the provided filter.</returns>
        public static bool ButtonReleased(Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => controllers[0].ButtonReleased(filter, buttonInputs);

        /// <summary>
        /// Check if a button is down.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button is down frame.</returns>
        public static bool ButtonDown(Players player, string key)
            => controllers[(int)player].ButtonDown(key);
        /// <summary>
        /// Check if a button is down.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button is down frame.</returns>
        public static bool ButtonDown(string key)
            => controllers[0].ButtonDown(key);

        /// <summary>
        /// Checks the down state of multiple buttons.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided are down, according to the provided filter.</returns>
        public static bool ButtonDown(Players player, Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => controllers[(int)player].ButtonDown(filter, buttonInputs);
        /// <summary>
        /// Checks the down state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided are down, according to the provided filter.</returns>
        public static bool ButtonDown(Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => controllers[0].ButtonDown(filter, buttonInputs);

        /// <summary>
        /// Check if a button is up.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button is up this frame.</returns>
        public static bool ButtonUp(Players player, string key)
            => !ButtonDown(player, key);
        /// <summary>
        /// Check if a button is up.
        /// </summary>
        /// <param name="key">ID of the ButtonInputSet to check.</param>
        /// <returns>True if the button is up this frame.</returns>
        public static bool ButtonUp(string key)
            => !ButtonDown(key);

        /// <summary>
        /// Checks the up state of multiple buttons.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided are up, according to the provided filter.</returns>
        public static bool ButtonUp(Players player, Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => !ButtonDown(player, filter, buttonInputs);
        /// <summary>
        /// Checks the up state of multiple buttons.
        /// </summary>
        /// <param name="filter">Filter used to determine the return behaviour.</param>
        /// <param name="buttonInputs">IDs of buttons to check.</param>
        /// <returns>True if the inputs provided are up, according to the provided filter.</returns>
        public static bool ButtonUp(Filter filter = Filter.Any, QuickList<string> buttonInputs = null)
            => !ButtonDown(filter, buttonInputs);

        /// <summary>
        /// Checks the state of an axis input.
        /// </summary>
        /// <param name="player">Which player to check.</param>
        /// <param name="key">ID of the axis to check.</param>
        /// <returns>Normalized float, from 0 to 1, representing the axis state.</returns>
        public static float AxisState(Players player, string key)
            => controllers[(int)player].AxisState(key);
        /// <summary>
        /// Checks the state of an axis input.
        /// </summary>
        /// <param name="key">ID of the axis to check.</param>
        /// <returns>Normalized float, from 0 to 1, representing the axis state.</returns>
        public static float AxisState(string key)
            => controllers[0].AxisState(key);

        /// <summary>
        /// Checks if a specific key was pressed this frame.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key was pressed.</returns>
        public static bool KeyCodePressed(Keys key)
            => PressedKeys.Contains(key);

        /// <summary>
        /// Checks if a specific key was released this frame.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key was released.</returns>
        public static bool KeyCodeReleased(Keys key)
            => OldKeys.Contains(key) && !NewKeys.Contains(key);

        /// <summary>
        /// Checks if a specific key is currently down.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key is down.</returns>
        public static bool KeyCodeDown(Keys key)
            => NewKeys.Contains(key);

        /// <summary>
        /// Checks if a specific key is currently up.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if the key is up.</returns>
        public static bool KeyCodeUp(Keys key)
            => !NewKeys.Contains(key);

        internal static void TextInput(object sender, TextInputEventArgs e)
        {
            if (char.IsControl(e.Character))
                return;

            oldPressedChars.Add(e.Character);
        }
    }
}