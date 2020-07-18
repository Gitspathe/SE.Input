using System;
using Microsoft.Xna.Framework.Input;
using SE.Input.ButtonControls;

namespace SE.Input
{
    public class ButtonInput
    {
        public ButtonControl Button { get; }
        public ButtonInputSet Modifier { get; set; }

        internal Players PlayerIndex {
            set {
                Button.PlayerIndex = value;
                if (Modifier != null)
                    Modifier.PlayerIndex = value;
            }
        }

        internal void Update(float deltaTime)
        {
            Modifier?.NewFrame();
            Modifier?.Update(deltaTime);
            Button.Update(deltaTime);
        }

        public bool Pressed {
            get {
                if (Modifier == null)
                    return Button.Pressed;

                return Modifier.Down && Button.Pressed;
            }
        }

        public bool Released {
            get {
                if (Modifier == null)
                    return Button.Released;

                return Modifier.Down && Button.Released;
            }
        }

        public bool Down {
            get {
                if (Modifier == null)
                    return Button.Down;

                return Modifier.Down && Button.Down;
            }
        }

        public bool Up {
            get {
                if (Modifier == null)
                    return Button.Up;

                return Modifier.Up || Button.Up;
            }
        }

        internal ButtonInput(ButtonControl button)
        {
            Button = button ?? throw new ArgumentNullException(nameof(button));
        }

        internal ButtonInput(ButtonControl button, ButtonInputSet modifier)
        {
            Button = button ?? throw new ArgumentNullException(nameof(button));
            if (modifier != null)
                Modifier = modifier;
        }

        public static ButtonInput FromKeyboard(Keys key) 
            => new ButtonInput(new KeyButtonControl(key), null);

        public static ButtonInput FromKeyboard(Keys key, ButtonInputSet modifier) 
            => new ButtonInput(new KeyButtonControl(key), modifier);

        public static ButtonInput FromKeyboard(Keys key, ButtonInput modifier)
            => new ButtonInput(new KeyButtonControl(key), new ButtonInputSet(modifier));

        public static ButtonInput FromGamepad(GamepadButtons button) 
            => new ButtonInput(new GamepadButtonControl(button), null);

        public static ButtonInput FromGamepad(GamepadButtons button, ButtonInputSet modifier) 
            => new ButtonInput(new GamepadButtonControl(button), modifier);

        public static ButtonInput FromGamepad(GamepadButtons button, ButtonInput modifier)
            => new ButtonInput(new GamepadButtonControl(button), new ButtonInputSet(modifier));

        public ButtonInput DeepCopy()
            => new ButtonInput(Button.DeepCopy(), Modifier.DeepCopy());
    }
}
