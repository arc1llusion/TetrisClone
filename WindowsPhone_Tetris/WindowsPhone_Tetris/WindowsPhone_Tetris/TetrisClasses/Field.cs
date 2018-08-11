

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
    /// The play field for tetris. This handles the Tetris logic dedicating to line clearing and drawing minos
    /// </summary>
    public class Field
    {
        #region Private Fields & Properties

        /// <summary>
        /// The grid representing the play field in Tetris
        /// </summary>
        private List<int[]> fieldLines;

        /// <summary>
        /// the point on the screen that the field should start drawing.
        /// </summary>
        private readonly Point fieldTopLeft;

        /// <summary>
        /// The alpha factor of a tetromino as a ghost piece
        /// </summary>
        private const float GhostTetrominoAlphaFactor = 0.25f;

        /// <summary>
        /// The height of each column on the field. This will eventually be used for the deterministic weighted AI
        /// </summary>
        private int[] surfaceHeights;

        private int width;
        /// <summary>
        /// The width (In blocks) of the playfield
        /// </summary>
        public int Width
        {
            get { return this.width; }
        }

        private int height;
        /// <summary>
        /// The height (In blocks) of the playfield
        /// </summary>
        public int Height
        {
            get { return this.height; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the required parameters for the field to be used for updating and drawing
        /// </summary>
        /// <param name="fieldTopLeft">Where the top left of the field start start drawing</param>
        /// <param name="width">The width (in blocks) of the play field</param>
        /// <param name="height">The height (in blocks) of the play field</param>
        public Field(Point fieldTopLeft, int width, int height)
        {
            this.width = width;
            this.height = height + 2; //To account for the skyline
            this.fieldTopLeft = fieldTopLeft;
            this.surfaceHeights = new int[width];
            fieldLines = new List<int[]>();
            InitializeInitialField();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Call this method to clear the full lines of the field and replace them with new ones
        /// </summary>
        /// <returns></returns>
        public List<int[]> ClearLines()
        {
            List<int[]> fullLines = new List<int[]>();

            for (int i = 0; i < height; i++)
            {
                if (IsLineFull(i))
                {
                    fullLines.Add(fieldLines[i]);
                }
            }

            for (int i = 0; i < fullLines.Count; i++)
            {
                fieldLines.Remove(fullLines[i]);
                AddNewLine();
            }

            if (fullLines.Count > 0)
                UpdateSurfaceHeights(0 - fullLines.Count);

            return fullLines;
        }

        /// <summary>
        /// Checks a row of the playfield to see if it is full. That is, there are no empty blocks
        /// </summary>
        /// <param name="row">The row of the playfield to check</param>
        /// <returns>True if the row is full, false otherwise</returns>
        public bool IsLineFull(int row)
        {
            bool isLineFull = true;

            for (int i = 0; i < width; i++)
            {
                isLineFull &= fieldLines[row][i] != -1;
            }

            return isLineFull;
        }

        /// <summary>
        /// Returns a boolean value indicating whether or not the point (In blocks) is within the field
        /// </summary>
        /// <param name="p">Point to check its bounds within the field</param>
        /// <returns>True if the point is in the field, false otherwise</returns>
        public bool IsInBounds(Point p)
        {
            bool isInBounds = true;

            if (p.X < 0 || p.Y < 0 || p.X >= width || p.Y >= height)
                isInBounds =  false;

            return isInBounds;
        }

        /// <summary>
        /// Inserts the Tetromino to the field, and indicates success if it could or couldn't
        /// </summary>
        /// <param name="tetromino">The tetromino to insert</param>
        /// <param name="point">The point (in blocks) on the playfield to insert the tetromino at</param>
        /// <returns>True if the tetromino can be inserted, false otherwise</returns>
        public bool InsertTetrominoAt(Tetromino tetromino, Point point)
        {
            if (IsTetrominoInsertableAt(tetromino, point))
            {
                SetMinos(tetromino, point);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if a tetromino can be inserted to the field.
        /// </summary>
        /// <param name="tetromino">The tetromino to check its insertion worthiness</param>
        /// <param name="x">The position, in blocks, on the horizontal axis within the play area</param>
        /// <param name="y">The position, in blocks, on the vertical axis within the play area</param>
        /// <returns></returns>
        public bool IsTetrominoInsertableAt(Tetromino tetromino, int x, int y)
        {
            return IsTetrominoInsertableAt(tetromino, new Point(x, y));
        }

        /// <summary>
        /// Checks to see if a tetromino can be inserted to the field.
        /// </summary>
        /// <param name="tetromino">The tetromino to check its insertion worthiness</param>
        /// <param name="insertPosition">The position (in blocks) on the field to check the Tetrominos insertion worthiness</param>
        /// <returns>True if the tetromino can be inserted, false otherwise</returns>
        public bool IsTetrominoInsertableAt(Tetromino tetromino, Point insertPosition)
        {
            bool insertable = true;

            for (int i = 0; i < 4; i++)
            {
                Point minoPosition = tetromino[i];
                Point translatedMino = new Point(insertPosition.X + minoPosition.X, insertPosition.Y + minoPosition.Y);
                insertable &= (IsInBounds(translatedMino) && fieldLines[translatedMino.Y][translatedMino.X] == -1);
            }

            return insertable;
        }

        /// <summary>
        /// Resets the fields to a new, blank field.
        /// </summary>
        public void ResetField()
        {
            surfaceHeights = new int[width];
            fieldLines.Clear();
            InitializeInitialField();
        }

        /// <summary>
        /// Checks the game over conditions necessary during normal gameplay.
        /// 
        /// As of this version, multiplayer is not supported, so the Top Out condition where garbage blocks are forcefully inserted is not needed
        /// The block out condition is managed by the Tetris engine
        /// </summary>
        /// <returns>True if game over condition has occurred, false otherwise</returns>
        public bool IsGameOver()
        {
            return IsLockOut();
        }

        /// <summary>
        /// Returns a copy of the current surface heights
        /// </summary>
        /// <returns>Integer array representing the columns in the field</returns>
        public int[] GetSurfaceHeights()
        {
            return (int[])this.surfaceHeights.Clone();
        }

        /// <summary>
        /// Gets the field data as an array, so to prevent copying
        /// </summary>
        /// <returns></returns>
        public int[][] GetFieldData()
        {
            return fieldLines.ToArray();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds a new line to the top of the field
        /// </summary>
        private void AddNewLine()
        {
            int[] newLine = new int[width];
            for (int i = 0; i < width; i++)
            {
                newLine[i] = -1;
            }

            fieldLines.Insert(0, newLine);
        }

        /// <summary>
        /// Sets the individual minos of a tetromino to the field. At this point, the Tetromino object is no longer needed.
        /// </summary>
        /// <param name="tetromino">The tetromino being inserted into the field</param>
        /// <param name="insertPosition">The position for the tetromino to be inserted</param>
        private void SetMinos(Tetromino tetromino, Point insertPosition)
        {
            for (int i = 0; i < 4; i++)
            {
                Point minoPosition = tetromino[i];

                Point translatedMino = new Point(insertPosition.X + minoPosition.X, insertPosition.Y + minoPosition.Y);
                fieldLines[translatedMino.Y][translatedMino.X] = tetromino.TetrominoType;

                surfaceHeights[translatedMino.X] = Math.Max(height - translatedMino.Y, surfaceHeights[translatedMino.X]);
            }
        }

        /// <summary>
        /// Checks a line within the field matrix to see if it contains any minos at all.
        /// 
        /// This is useful when checking for Lock Out game over conditions
        /// </summary>
        /// <param name="row">The row in the matrix to scan</param>
        /// <returns>True if any mino is inserted in that row, false otherwise</returns>
        private bool ContainsMinos(int row)
        {
            for (int i = 0; i < width; i++)
            {
                if (fieldLines[row][i] != -1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes the initial empty field.
        /// </summary>
        private void InitializeInitialField()
        {
            for (int i = 0; i < height; i++)
            {
                AddNewLine();
            }
        }

        /// <summary>
        /// Method to determine whether or not the lock out condition has occurred.
        /// 
        /// Lock out is when any tetromino is solidified above the visible matrix and in the skyline which are the top two rows.
        /// </summary>
        /// <returns>True if Lock Out condition has occurred, false otherwise</returns>
        private bool IsLockOut()
        {
            return ContainsMinos(0) || ContainsMinos(1);
        }

        /// <summary>
        /// Adds or a removes the number of specified height blocks from a surface height
        /// </summary>
        /// <param name="number"></param>
        private void UpdateSurfaceHeights(int number)
        {
            for (int i = 0; i < width; i++)
            {
                surfaceHeights[i] += number;
            }
        }

        #endregion

        #region Draw Field Methods

        /// <summary>
        /// A draw method that performs drawing of the entire field and its contained blocks
        /// </summary>
        /// <param name="batch">The sprite batch instance to draw with</param>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            Viewport view = batch.GraphicsDevice.Viewport;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int mino = fieldLines[i][j];
                    if (mino != -1)
                    {
                        this.DrawMino(view, batch, new Point(j, i), 1.0f, mino);
                    }
                }
            }
        }

        /// <summary>
        /// A draw method what draws a specific tetromino as if it were in the grid. This is done to reuse code for drawing the individual
        /// Tetromino blocks. This is for a tetromino that is not yet inserted.
        /// </summary>
        /// <param name="batch">The sprite batch instance to draw with</param>
        /// <param name="position">The position (in blocks) of the point on the field to do its initial draw</param>
        /// <param name="tetromino">The tetromino to mock draw in the field</param>
        public void DrawTetromino(SpriteBatch batch, Point position, Tetromino tetromino)
        {
            this.DrawTetromino(batch, position, tetromino, false);
        }

        /// <summary>
        /// A draw method what draws a specific tetromino as if it were in the grid. This is done to reuse code for drawing the individual
        /// Tetromino blocks. This is for a tetromino that is not yet inserted.
        /// </summary>
        /// <param name="batch">The sprite batch instance to draw with</param>
        /// <param name="position">The position (in blocks) of the point on the field to do its initial draw</param>
        /// <param name="tetromino">The tetromino to mock draw in the field</param>
        /// <param name="isGhost">Boolean indicating whether or not to draw the tetromino as a ghost</param>
        public void DrawTetromino(SpriteBatch batch, Point position, Tetromino tetromino, bool isGhost)
        {
            float colorTransparency = isGhost ? GhostTetrominoAlphaFactor : 1.0f;

            for (int i = 0; i < 4; i++)
            {
                Point mino = tetromino[i];
                Point translatedMino = new Point(position.X + mino.X, position.Y + mino.Y);

                this.DrawMino(batch.GraphicsDevice.Viewport, batch, translatedMino, colorTransparency, tetromino.TetrominoType);
            }
        }

        #region For Debugging Rotations in Draw method by coloring rotation point

        /*
         *      //Color backTo = color; //For debugging rotations

                if (i == 0)
                    color = Color.White;
                else
                    color = backTo;
         */

        #endregion

        /// <summary>
        /// The draw method that actually dows the drawing part.
        /// </summary>
        /// <param name="view">The viewport of the current screen.</param>
        /// <param name="batch">The sprite batch instance to draw with</param>
        /// <param name="position">The position (in blocks) of the point on the field to do its initial draw</param>
        /// <param name="transparency">The transparancy value to alter the color values</param>
        /// <param name="value">Value is the value type of the mino which is from the tetromino. This maintains its color when separated</param>
        private void DrawMino(Viewport view, SpriteBatch batch, Point position, float transparency, int value)
        {
            batch.Draw(Globals.TetrominoTexture,
                new Rectangle(fieldTopLeft.X + (position.X * Globals.MinoWidth), fieldTopLeft.Y + (position.Y * Globals.MinoHeight), Globals.MinoWidth, Globals.MinoHeight),
                new Rectangle(Globals.MinoTextureWidth * value, 0, Globals.MinoTextureWidth, Globals.MinoTextureHeight),
                Color.White * transparency);
        }

        #endregion
    }
}
