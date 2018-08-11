
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsPhone_Tetris.Utility
{
    /// <summary>
    /// Class for dealing with global variables in the game environment.
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// The play field width in blocks
        /// </summary>
        public const int FieldWidth = 10;

        /// <summary>
        /// The play field height in blocks
        /// </summary>
        public const int FieldHeight = 20;

        /// <summary>
        /// The mino width in game. XNA will scale as needed
        /// </summary>
        public const int MinoWidth = 32;

        /// <summary>
        /// The mino height in game. XNA will scale as needed
        /// </summary>
        public const int MinoHeight = 32;

        /// <summary>
        /// The texture mino width
        /// </summary>
        public const int MinoTextureWidth = 32;

        /// <summary>
        /// The texture mino height
        /// </summary>
        public const int MinoTextureHeight = 32;

        /// <summary>
        /// The calculated scale based on the in game size and the original texture size.
        /// </summary>
        public const float MinoScale = (float)MinoWidth / 32.0f;

        /// <summary>
        /// Rectangle used as a source rectangle for the Mino texture
        /// </summary>
        public static readonly Rectangle MinoTextureRect = new Rectangle(0, 0, 32, 32);

        /// <summary>
        /// The texture used for the Tetromino pieces. For a small game this is okay, but for larger games wouldn't make sense
        /// </summary>
        public static Texture2D TetrominoTexture
        {
            get;
            set;
        }

        /// <summary>
        /// The texture for the background field.
        /// </summary>
        public static Texture2D FieldTexture
        {
            get;
            set;
        }

        /// <summary>
        /// The arrows texture for the button presses in the game play screen
        /// </summary>
        public static Texture2D ArrowsTexture
        {
            get;
            set;
        }

        /// <summary>
        /// The font used to display text
        /// </summary>
        public static SpriteFont TetrisFont
        {
            get;
            set;
        }
    }
}
