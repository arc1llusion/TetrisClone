

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsPhone_Tetris.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsPhone_Tetris.TetrisClasses
{

    /// <summary>
    /// The tetromino represents all possible tetrominos by its rotation matrix and also handles its rotation
    /// </summary>
    public class Tetromino
    {
        #region Enum

        /// <summary>
        /// Wall Kick Orientation for the Tetromino
        /// </summary>
        public enum RotationState
        {
            /// <summary>
            /// The rotation state from the default rotation to the clockwise rotation
            /// </summary>
            Kick_0R, 
            
            /// <summary>
            /// The rotation state from the right clockwise rotation state to the default rotation state
            /// </summary>
            Kick_R0, 
            
            /// <summary>
            /// The clockwise rotation state to the state of rotation that is 2 rotations from the default state. That is, either two clock wise rotations or two counter clock wise rotations
            /// </summary>
            Kick_R2, 
            
            /// <summary>
            /// The 2 rotation state to the clockwise rotations tate
            /// </summary>
            Kick_2R, 
            
            /// <summary>
            /// The 2 rotation state to the counter clockwise rotation state
            /// </summary>
            Kick_2L, 
            
            /// <summary>
            /// The counter clockwise rotation state to the 2 rotation state
            /// </summary>
            Kick_L2, 
            
            /// <summary>
            /// The counter clockwise state to the default rotation state
            /// </summary>
            Kick_L0, 
            
            /// <summary>
            /// The default rotation state moving to the counter clockwise rotation state. That is, one counter clockwise from the default took place
            /// </summary>
            Kick_0L
        }

        #endregion

        #region Private Fields & Properties

        /// <summary>
        /// The rotation matrix that represents the indivial minos of the tetromino.
        /// <seealso cref="TetrominoFactory"/>
        /// </summary>
        private Point[,] rotationMatrix;

        /// <summary>
        /// The current rotation represented by the current matrix
        /// </summary>
        private int currentRotation;

        private Tetromino.RotationState currentRotationState;
        /// <summary>
        /// The wall kick key when performing a rotation
        /// </summary>
        public Tetromino.RotationState CurrentRotationState
        {
            get { return this.currentRotationState; }
        }

        private Dictionary<Tetromino.RotationState, Point[]> wallKickData;
        /// <summary>
        /// The wall kick data for this tetromino
        /// </summary>
        public Dictionary<Tetromino.RotationState, Point[]> WallKickData
        {
            get { return wallKickData; }
        }

        private Point centerTranslation;
        /// <summary>
        /// The relative center translation between the tetromino center and the rotation mino
        /// </summary>
        public Point CenterTranslation
        {
            get { return centerTranslation; }
        }

        private int tetrominoType;
        /// <summary>
        /// The type of tetromino. This will be used to determine its color for drawing.
        /// </summary>
        public int TetrominoType
        {
            get { return this.tetrominoType; }
        }

        /// <summary>
        /// Indexer used to get a Mino within the current rotation matrix
        /// </summary>
        /// <param name="i">The index of the mino within the current rotation matrix</param>
        /// <returns>A point relative to the rotation point</returns>
        public Point this[int i]
        {
            get { return rotationMatrix[currentRotation, i]; }
        }

        //TODO: Make this genuinely static instead of global
        /// <summary>
        /// The texture used to draw the minos
        /// </summary>
        private static Texture2D blockTexture;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the rotation matrix and type as well as defaults the starting rotation
        /// </summary>
        /// <param name="rotationMatrix">The rotation matrix of the tetromino</param>
        /// <param name="type">The type of the tetromino</param>
        /// <param name="wallKickData">The sequence data for checking wall kicks</param>
        /// <param name="centerTranslation">The relative center translation between the tetromino center and the rotation mino</param>
        public Tetromino(Point[,] rotationMatrix, Dictionary<Tetromino.RotationState, Point[]> wallKickData, Point centerTranslation, int type)
        {
            this.rotationMatrix = rotationMatrix;
            this.wallKickData = wallKickData;
            this.currentRotation = 0;
            this.tetrominoType = type;
            this.centerTranslation = centerTranslation;

            blockTexture = Globals.TetrominoTexture;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Rotates the tetromino Clockwise
        /// </summary>
        public void RotateClockwise()
        {
            switch (currentRotation)
            {
                case 0:
                    this.currentRotationState = Tetromino.RotationState.Kick_0R;
                    break;
                case 1:
                    this.currentRotationState = Tetromino.RotationState.Kick_R2;
                    break;
                case 2:
                    this.currentRotationState = Tetromino.RotationState.Kick_2L;
                    break;
                case 3:
                    this.currentRotationState = Tetromino.RotationState.Kick_L0;
                    break;
            }

            currentRotation = ++currentRotation % rotationMatrix.GetLength(0);
        }

        /// <summary>
        /// Rotates the tetromino Counter clockwise
        /// </summary>
        public void RotateCounterClockwise()
        {
            switch (currentRotation)
            {
                case 0:
                    this.currentRotationState = Tetromino.RotationState.Kick_0L;
                    break;
                case 1:
                    this.currentRotationState = Tetromino.RotationState.Kick_L2;
                    break;
                case 2:
                    this.currentRotationState = Tetromino.RotationState.Kick_2R;
                    break;
                case 3:
                    this.currentRotationState = Tetromino.RotationState.Kick_R0;
                    break;
            }

            //C# does not have a "mathematically correct" version of the modulo operation, and is instead a remainder operation
            //That takes the sign of the dividend.
            if (--currentRotation == -1)
                currentRotation = 3;
        }

        /// <summary>
        /// Resets the tetromino rotation to its default state
        /// </summary>
        public void ResetRotation()
        {
            currentRotation = 0;
        }

        /// <summary>
        /// Gets the Mino relative to the rotation point in the current rotation matrix
        /// </summary>
        /// <param name="mino">The position of the mino in the matrix to retrieve</param>
        /// <returns>The point matrix of the requested mino</returns>
        public Point GetMinoAtPosition(int mino)
        {
            return rotationMatrix[currentRotation, mino];
        }

        #endregion
    }
}
