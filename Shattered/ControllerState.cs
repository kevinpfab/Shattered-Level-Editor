using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Shattered
{
    /// <summary>
    /// Allows us easy access to the states of our controllers.
    /// </summary>
    public class ControllerState
    {
        // The controller info we want to store
        public static KeyboardState CurrentKeyboardState;
        public static KeyboardState LastKeyboardState;
        public static MouseState CurrentMouseState;
        public static MouseState LastMouseState;

        /// <summary>
        /// Keeps the controller information on hand and updated.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        /// <summary>
        /// Is a Key currently pressed down?
        /// </summary>
        /// <param name="k">The Key.</param>
        /// <returns></returns>
        public static bool Down(Keys k)
        {
            return CurrentKeyboardState.IsKeyDown(k);
        }

        /// <summary>
        /// Is a Key currently not being pressed?
        /// </summary>
        /// <param name="k">The Key.</param>
        /// <returns></returns>
        public static bool Up(Keys k)
        {
            return CurrentKeyboardState.IsKeyUp(k);
        }

        /// <summary>
        /// Is a Key pressed down that wasn't being pressed last frame?
        /// </summary>
        /// <param name="k">The Key.</param>
        /// <returns></returns>
        public static bool Pushed(Keys k)
        {
            return CurrentKeyboardState.IsKeyDown(k) && LastKeyboardState.IsKeyUp(k);
        }

        /// <summary>
        /// Is the left mouse button pressed down and not pressed the previous frame?
        /// </summary>
        /// <returns></returns>
        public static bool MouseLeftClicked()
        {
            return CurrentMouseState.LeftButton.Equals(ButtonState.Pressed) && LastMouseState.LeftButton.Equals(ButtonState.Released);
        }

        /// <summary>
        /// The x position of the mouse relative to the top left-hand corner.
        /// </summary>
        /// <returns></returns>
        public static int MouseX()
        {
            return CurrentMouseState.X;
        }

        /// <summary>
        /// The y position of the mouse relative to the top left-hand corner.
        /// </summary>
        /// <returns></returns>
        public static int MouseY()
        {
            return CurrentMouseState.Y;
        }
    }
}
