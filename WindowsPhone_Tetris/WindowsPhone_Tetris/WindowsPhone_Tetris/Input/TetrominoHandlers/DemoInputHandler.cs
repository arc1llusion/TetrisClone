

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsPhone_Tetris.GameScreens;
using WindowsPhone_Tetris.Utility;
using Microsoft.Xna.Framework;

namespace WindowsPhone_Tetris.Input.TetrominoHandlers
{
    /// <summary>
    /// The class responsible for determining the demo input actions
    /// </summary>
    public class DemoInputHandler : ITetrominoInputHandler
    {
        /// <summary>
        /// The player inputs game screen instance for checking touch collisions with field data
        /// </summary>
        private GameplayScreen gameplayScreen;

        /// <summary>
        /// The time before another action can be placed
        /// </summary>
        private const int ActionInput = 300;

        /// <summary>
        /// The elapsed time since the last action
        /// </summary>
        private int actionInputDelta = ActionInput;

        /* For AI when I get time
        private const float HeightWeight = -0.03f;

        private const float ClearWeight = 8.0f;

        private const float HoleWeight = -7.5f;

        private const float BlockadeWeight = -3.5f;

        */

        TetrominoAction[] actions;

        /// <summary>
        /// Sets up the demo input handler
        /// </summary>
        /// <param name="screen">The screen the input handler is part of</param>
        public DemoInputHandler(GameplayScreen screen)
        {
            //Compact framework does not include Enum.GetValues() which would make this easier...
            actions = new TetrominoAction[] { TetrominoAction.None, TetrominoAction.Left, TetrominoAction.Right, TetrominoAction.SoftDrop, TetrominoAction.HardDrop, TetrominoAction.Rotate, TetrominoAction.Hold };

            gameplayScreen = screen;
            //gameplayScreen.Field
        }

        /// <summary>
        /// The method that determines which action is being taken
        /// </summary>
        /// <returns>The actions being returned from the tetris handler</returns>
        public TetrominoAction GetTetrominoActions(GameTime gameTime)
        {
            TetrominoAction action = TetrominoAction.None;
            actionInputDelta -= gameTime.ElapsedGameTime.Milliseconds;

            if (actionInputDelta <= 0)
            {
                actionInputDelta = ActionInput;
                gameplayScreen.ResetInput();

                int value = RandomGenerator.Instance.Next(0, actions.Length);
                action = actions[value];                
            }

            return action;
        }
    }
}
