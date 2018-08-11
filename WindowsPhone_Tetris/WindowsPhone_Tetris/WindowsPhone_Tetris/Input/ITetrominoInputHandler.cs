

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsPhone_Tetris.Input
{
    /// <summary>
    /// The possible actions the player can initiate.
    /// </summary>
    public enum TetrominoAction {
        /// <summary>
        /// The default action of nothing. Meaning, the player is not taking any action
        /// </summary>
        None,

        /// <summary>
        /// The player is initiating a tetromino action to move left
        /// </summary>
        Left,

        /// <summary>
        /// The player is initiating a tetromino action to move right
        /// </summary>
        Right,

        /// <summary>
        /// The player is initiating a tetromino action to move down one
        /// </summary>
        SoftDrop,

        /// <summary>
        /// The player is initiating a tetromino action to hard drop to the surface of the field
        /// </summary>
        HardDrop,

        /// <summary>
        /// The player is initiating a tetromino action to rotate the tetromino
        /// </summary>
        Rotate,

        /// <summary>
        /// The player has initiated a hold action on the tetromino
        /// </summary>
        Hold
        
    }

    /// <summary>
    /// A common interface structure for tetris input. This allows the player handler and a demo handler
    /// </summary>
    interface ITetrominoInputHandler
    {
        /// <summary>
        /// The method that determines which action is being taken
        /// </summary>
        /// <returns>The actions being returned from the tetris handler</returns>
        TetrominoAction GetTetrominoActions(GameTime gameTime);
    }
}
