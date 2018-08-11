
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace WindowsPhone_Tetris.Input
{
    /// <summary>
    /// The input handler class for input throughout the game. 
    /// </summary>
    public class InputHandler : Microsoft.Xna.Framework.GameComponent
    {
        #region Keyboard Field Region

        private static KeyboardState keyboardState;
        private static KeyboardState lastKeyboardState;

        #endregion

        #region Mouse Region

        private static MouseState mouseState;
        private static MouseState lastMouseState;

        #endregion

        #region Touch Region

        private static TouchCollection touchState;
        private static TouchCollection lastTouchState;

        private static GestureType gestureTypes;
        private static List<GestureSample> gestureSamples;

        #endregion

        #region Keyboard Property Region

        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }

        #endregion

        #region Mouse Property Region

        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }

        #endregion

        #region Touch Property Region

        public static TouchCollection TouchState
        {
            get { return touchState; }
        }

        public static TouchCollection LastTouchState
        {
            get { return lastTouchState; }
        }

        public static List<GestureSample> GestureSamples
        {
            get { return gestureSamples; }
        }

        #endregion

        #region Constructor Region

        public InputHandler(Game1 game, GestureType enabledGestures)
            : base(game)
        {
            keyboardState = Keyboard.GetState();

            mouseState = Mouse.GetState();

            touchState = TouchPanel.GetState();

            TouchPanel.EnabledGestures = enabledGestures;

            gestureSamples = new List<GestureSample>();
        }

        #endregion

        #region XNA methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            lastTouchState = touchState;
            touchState = TouchPanel.GetState();

            ReadGestures();

            base.Update(gameTime);
        }

        #endregion

        #region General Method Region

        public static void Flush()
        {
            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;

            touchState = new TouchCollection();
            lastTouchState = touchState;
        }

        #endregion

        #region Keyboard Region

        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) &&
                lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        #endregion

        #region Mouse Region

        public static bool MouseButtonDown(ButtonState mouseButton)
        {
            return mouseButton == ButtonState.Pressed;
        }

        public static bool MouseButtonPressed(ButtonState previous, ButtonState mouseButton)
        {
            return mouseButton == ButtonState.Released && previous == ButtonState.Pressed;
        }

        public static bool MouseButtonReleased(ButtonState previous, ButtonState mouseButton)
        {
            return mouseButton == ButtonState.Pressed && previous == ButtonState.Released;
        }

        public static Vector2 CurrentMousePosition
        {
            get { return new Vector2(mouseState.X, mouseState.Y); }
        }

        #endregion

        #region Touch Region

        public static TouchCollection GetCurrentTouchLocationCollection()
        {
            return touchState;
        }

        public static bool WasTouchInputReceived()
        {
            return touchState.Count != 0;
        }

        public static bool WasTouchInputReleased()
        {
            foreach (TouchLocation tl in InputHandler.GetCurrentTouchLocationCollection())
            {
                if (tl.State == TouchLocationState.Released)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool WasTouchInputPressed()
        {
            foreach (TouchLocation tl in InputHandler.GetCurrentTouchLocationCollection())
            {
                if (tl.State == TouchLocationState.Pressed)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method is for reading the gestures present within a certain update. Reading Gestures each time would not work if
        /// there are multiple calls within a single update. So we bitwise OR them to hold their values for this update, and clear
        /// on the next.
        /// </summary>
        private void ReadGestures()
        {
            gestureTypes = GestureType.None;
            gestureSamples.Clear();

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample sample = TouchPanel.ReadGesture();

                gestureSamples.Add(sample);
                gestureTypes |= sample.GestureType;
            }
        }

        #endregion
    }

}
