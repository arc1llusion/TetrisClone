

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsPhone_Tetris.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace WindowsPhone_Tetris.Input.TetrominoHandlers
{
    /// <summary>
    /// The player input handler. This class determines the player actions by checking collisions with the field data
    /// </summary>
    public class PlayerInputHandler : ITetrominoInputHandler
    {
        /// <summary>
        /// The player inputs game screen instance for checking touch collisions with field data
        /// </summary>
        private GameplayScreen gameplayScreen;

        /// <summary>
        /// The amount of time that elapses before input is registered. This is to keep it from going haywire.
        /// </summary>
        private const int InputStep = 100;

        /// <summary>
        /// The delta time for input
        /// </summary>
        private int inputDelta = InputStep;

        /// <summary>
        /// Constructor for setting up the player handler
        /// </summary>
        /// <param name="gameplayScreen">The game play screen this handler is part of</param>
        public PlayerInputHandler(GameplayScreen gameplayScreen)
        {
            this.gameplayScreen = gameplayScreen;
        }

        /// <summary>
        /// The method that determines which action is being taken
        /// </summary>
        /// <returns>The actions being returned from the tetris handler</returns>
        public TetrominoAction GetTetrominoActions(GameTime gameTime)
        {
            TetrominoAction action = TetrominoAction.None;
            inputDelta -= gameTime.ElapsedGameTime.Milliseconds;
            if (inputDelta <= 0)
            {
                inputDelta = InputStep;
                gameplayScreen.ResetInput();

                foreach (TouchLocation tl in InputHandler.GetCurrentTouchLocationCollection())
                {
                    Rectangle rect = new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 1, 1);
                    if (tl.State == TouchLocationState.Pressed || tl.State == TouchLocationState.Moved)
                    {

                        if (gameplayScreen.LeftButton.Intersects(rect))
                        {
                            action = TetrominoAction.Left;
                        }
                        else if (gameplayScreen.MiddleButton.Intersects(rect))
                        {
                            action = TetrominoAction.HardDrop;
                        }
                        else if (gameplayScreen.RightButton.Intersects(rect))
                        {
                            action = TetrominoAction.Right;
                        }
                        else if (gameplayScreen.RotateButton.Intersects(rect))
                        {
                            action = TetrominoAction.Rotate;
                        }
                    }
                }
            }


            foreach (TouchLocation tl in InputHandler.GetCurrentTouchLocationCollection())
            {
                Rectangle rect = new Rectangle((int)tl.Position.X, (int)tl.Position.Y, 1, 1);
                if (tl.State == TouchLocationState.Pressed)
                {
                    if (gameplayScreen.HoldBox.Intersects(rect))
                    {
                        action = TetrominoAction.Hold;
                    }
                }
            }

            return action;
        }
    }
}
