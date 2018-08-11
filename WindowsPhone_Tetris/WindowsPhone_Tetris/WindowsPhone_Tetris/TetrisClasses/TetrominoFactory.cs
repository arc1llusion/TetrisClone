

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsPhone_Tetris.Utility;
using Microsoft.Xna.Framework;

namespace WindowsPhone_Tetris.TetrisClasses
{
    /// <summary>
    /// The TetrominoFactory is delegated to for handling work with Tetromino generation. It provides a random generation as well as 
    /// a generation by the Tetromino type. It stores the Tetromino rotation matrices in a static instead and calls on them when needed.
    /// </summary>
    public class TetrominoFactory
    {
        #region Private Fields

        /// <summary>
        /// The character array representing the tetromino piece names
        /// </summary>
        private static char[] tetrominoNames;

        /// <summary>
        /// A dictionary instance representing the tetromino name to rotation matrix mapping for faster access
        /// </summary>
        private static Dictionary<char, TetrominoData> tetrominoMapping;

        /// <summary>
        /// This is the bag that ensures a permutation of the 7 pieces is chosen. In this way if I decide to support multiplay
        /// in some way, each player can have their own bag while sharing the Random instance.
        /// </summary>
        private RandomBag bag;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initializes the tetromino mappings for generation
        /// </summary>
        public TetrominoFactory()
        {
            this.bag = new RandomBag(6);
            tetrominoNames = new char[] { 'o', 'i', 't', 'l', 'j', 's', 'z' };
            tetrominoMapping = new Dictionary<char, TetrominoData>();

            tetrominoMapping.Add('t', new TetrominoData(TRotationMatrix, JLSTZ_OffsetData, JLSTZ_StartingCenters));
            tetrominoMapping.Add('s', new TetrominoData(SRotationMatrix, JLSTZ_OffsetData, JLSTZ_StartingCenters));
            tetrominoMapping.Add('z', new TetrominoData(ZRotationMatrix, JLSTZ_OffsetData, JLSTZ_StartingCenters));
            tetrominoMapping.Add('o', new TetrominoData(ORotationMatrix, O_OffsetData, O_StartingCenters));
            tetrominoMapping.Add('i', new TetrominoData(IRotationMatrix, I_OffsetData, I_StartingCenters));
            tetrominoMapping.Add('l', new TetrominoData(LRotationMatrix, JLSTZ_OffsetData, JLSTZ_StartingCenters));
            tetrominoMapping.Add('j', new TetrominoData(JRotationMatrix, JLSTZ_OffsetData, JLSTZ_StartingCenters));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates a random tetromino and turns it
        /// </summary>
        /// <returns>The new Tetromino instance</returns>
        public Tetromino GenerateRandomTetromino()
        {
            int value = bag.GetRandomNumberForTetris();
            TetrominoData mapping = tetrominoMapping[tetrominoNames[value]];

            return new Tetromino(mapping.RotationMatrix, mapping.WallKickData, mapping.CenterTranslation, value);
        }

        /// <summary>
        /// Generates a tetromino by its name. Mostly for testing purposes. But if i continued, I could add logic to make it as painful for the 
        /// player as possible!
        /// </summary>
        /// <param name="c">The name of the tetromino to generate</param>
        /// <returns>A new tetromino instance</returns>
        public Tetromino GenerateTetromino(char c)
        {
            int value = Array.IndexOf<char>(tetrominoNames, c);
            TetrominoData mapping = tetrominoMapping[tetrominoNames[value]];

            return new Tetromino(mapping.RotationMatrix, mapping.WallKickData, mapping.CenterTranslation, value);
        }

        /// <summary>
        /// Flushes the factory of its state
        /// </summary>
        public void Flush()
        {
            bag.ResetBag();
        }

        #endregion

        #region Rotation Matrices

        /*
         * The rotation matrix focuses on a single rotation point. So for example in the T rotation matrix, the 0,0 point is always the
         * one marked with an 'R'
         * 
         *   [0]
         *    #
         *  # R #
         *   
         * The above format shows the # as normal Minos and the R as the rotation point. That is the first rotation matrix or
         * the first row in the TRotationmatrix Array. The rest would be as follows
         * 
         *   [1]   [2]    [3]    
         *    #            #
         *  # R   # R #    R #
         *    #     #      # 
         *  
         * The rest of the pieces follow a similar path.
         */

        /// <summary>
        /// Rotation Matrix for the T tetromino
        /// </summary>
        private static Point[,] TRotationMatrix = new Point[,] {
                    {new Point(0,0), new Point(-1,0), new Point(1, 0), new Point(0,-1)},
                    {new Point(0,0), new Point(0,1), new Point(0,-1), new Point(1,0)},
                    {new Point(0,0), new Point(1,0), new Point(-1, 0), new Point(0, 1)},
                    {new Point(0,0), new Point(0,-1), new Point(0,1), new Point(-1, 0)}
                };

        /// <summary>
        /// Rotation Matrix for the S tetromino
        /// </summary>
        private static Point[,] SRotationMatrix = {
                    {new Point(0,0), new Point(-1,0), new Point(0,-1), new Point(1,-1)},
                    {new Point(0,0), new Point(0,-1), new Point(1,0), new Point(1,1)},
                    {new Point(0,0), new Point(1,0), new Point(0,1), new Point(-1,1)},
                    {new Point(0,0), new Point(-1,0), new Point(-1,-1), new Point(0,1)}
                };

        /// <summary>
        /// Rotation Matrix for the Z tetromino
        /// </summary>
        private static Point[,] ZRotationMatrix = {
                    {new Point(0,0), new Point(1,0), new Point(0,-1), new Point(-1,-1)},
                    {new Point(0,0), new Point(0,1), new Point(1,0), new Point(1,-1)},
                    {new Point(0,0), new Point(-1,0), new Point(0,1), new Point(1,1)},
                    {new Point(0,0), new Point(0,-1), new Point(-1,0), new Point(-1,1)}
                };

        /// <summary>
        /// Rotation Matrix for the O tetromino
        /// </summary>
        private static Point[,] ORotationMatrix = {
                    {new Point(0,0), new Point(1,0), new Point(0,-1), new Point(1,-1)},
                    {new Point(0,0), new Point(1,0), new Point(0,-1), new Point(1,-1)},
                    {new Point(0,0), new Point(1,0), new Point(0,-1), new Point(1,-1)},
                    {new Point(0,0), new Point(1,0), new Point(0,-1), new Point(1,-1)}
                };

        /// <summary>
        /// Rotation Matrix for the I tetromino
        /// </summary>
        private static Point[,] IRotationMatrix = {
                    {new Point(0,0), new Point(-1,0), new Point(1,0), new Point(2,0)},
                    {new Point(1,0), new Point(1,-1), new Point(1,1), new Point(1,2)},
                    {new Point(0,0), new Point(-1,0), new Point(1,0), new Point(2,0)},
                    {new Point(0,0), new Point(0,-1), new Point(0,-2), new Point(0,1)}
                };

        /// <summary>
        /// Rotation Matrix for the L tetromino
        /// </summary>
        private static Point[,] LRotationMatrix = {
                    {new Point(0,0), new Point(1,0), new Point(-1,0), new Point(1,-1)},
                    {new Point(0,0), new Point(0,-1), new Point(0,1), new Point(1,1)},
                    {new Point(0,0), new Point(-1,0), new Point(1,0), new Point(-1,1)},
                    {new Point(0,0), new Point(0,1), new Point(0,-1), new Point(-1,-1)}
                };

        /// <summary>
        /// Rotation Matrix for the J tetromino
        /// </summary>
        private static Point[,] JRotationMatrix = {
                    {new Point(0,0), new Point(-1,0), new Point(1,0), new Point(-1,-1)},
                    {new Point(0,0), new Point(0,1), new Point(0,-1), new Point(1,-1)},
                    {new Point(0,0), new Point(1,0), new Point(-1,0), new Point(1,1)},
                    {new Point(0,0), new Point(0,-1), new Point(0,1), new Point(-1,1)}
                };

        #endregion

        #region Rotation Offset Data (For Wall Kicks)

        /* The offset data provides a means to translate wall kicks for various tetrominos */

        /// <summary>
        /// The J, L, S, T, and Z pieces can all be defined by these wall kick translations
        /// </summary>
        private static Dictionary<Tetromino.RotationState, Point[]> JLSTZ_OffsetData = new Dictionary<Tetromino.RotationState, Point[]> 
        {
            { Tetromino.RotationState.Kick_0R, new Point[] { new Point(-1, 0), new Point (-1, -1), new Point(0, 2), new Point(-1, 2) } },
            { Tetromino.RotationState.Kick_R0, new Point[] { new Point(1, 0), new Point (1, 1), new Point(0, -2), new Point(1, -2) } },

            { Tetromino.RotationState.Kick_R2, new Point[] { new Point(1, 0), new Point (1, -1), new Point(0, 2), new Point(1, -2) } },
            { Tetromino.RotationState.Kick_2R, new Point[] { new Point(-1, 0), new Point (-1, -1), new Point(0, 2), new Point(-1, 2) } },

            { Tetromino.RotationState.Kick_2L, new Point[] { new Point(1, 0), new Point (1, -1), new Point(0, 2), new Point(1, 2) } },
            { Tetromino.RotationState.Kick_L2, new Point[] { new Point(-1, 0), new Point (-1, 1), new Point(0, -2), new Point(-1, -2) } },

            { Tetromino.RotationState.Kick_L0, new Point[] { new Point(-1, 0), new Point (-1, 1), new Point(0, -2), new Point(-1, -2) } },
            { Tetromino.RotationState.Kick_0L, new Point[] { new Point(1, 0), new Point (1, -1), new Point(0, 2), new Point(1, 2) } }
        };

        /// <summary>
        /// The I piece is a special case and requires a different set of data
        /// </summary>
        private static Dictionary<Tetromino.RotationState, Point[]> I_OffsetData = new Dictionary<Tetromino.RotationState, Point[]> 
        {
            { Tetromino.RotationState.Kick_0R, new Point[] { new Point(-2, 0), new Point (1, 0), new Point(-2, 1), new Point(1, -2) } },
            { Tetromino.RotationState.Kick_R0, new Point[] { new Point(2, 0), new Point (-1, 0), new Point(2, -1), new Point(-1, 2) } },

            { Tetromino.RotationState.Kick_R2, new Point[] { new Point(-1, 0), new Point (2, 0), new Point(-1, -2), new Point(2, 1) } },
            { Tetromino.RotationState.Kick_2R, new Point[] { new Point(1, 0), new Point (-2, 0), new Point(1, 2), new Point(-2, -1) } },

            { Tetromino.RotationState.Kick_2L, new Point[] { new Point(2, 0), new Point (-1, 0), new Point(2, -1), new Point(-1, 2) } },
            { Tetromino.RotationState.Kick_L2, new Point[] { new Point(-2, 0), new Point (1, 0), new Point(-2, 1), new Point(1, -2) } },

            { Tetromino.RotationState.Kick_L0, new Point[] { new Point(1, 0), new Point (-2, 0), new Point(1, 2), new Point(-2, -1) } },
            { Tetromino.RotationState.Kick_0L, new Point[] { new Point(-1, 0), new Point (2, 0), new Point(-1, -2), new Point(2, 1) } }
        };

        /// <summary>
        /// The current implementation of rotation is not "true" rotation, and so the O offset is not needed in this case.
        /// 
        /// We still must define it so that the O piece is not causing problems for others.
        /// </summary>
        private static Dictionary<Tetromino.RotationState, Point[]> O_OffsetData = new Dictionary<Tetromino.RotationState, Point[]>();

        #endregion

        #region Tetromino Centers

        /* Finding the centers of the tetrominos in relation to the starting mino is difficult. For that reason, I've declared
         * the relative centers here and added it to the tetromino data */

        /// <summary>
        /// The relative center for the J, L, S, T, and Z tetrominos are all the same.
        /// </summary>
        static Point JLSTZ_StartingCenters = new Point(Globals.MinoWidth / 2, Globals.MinoHeight / 4);

        /// <summary>
        /// The relative center for the O tetromino
        /// </summary>
        static Point O_StartingCenters = new Point(Globals.MinoWidth, Globals.MinoHeight / 4);

        /// <summary>
        /// The relative center for the I tetromino
        /// </summary>
        static Point I_StartingCenters = new Point(Globals.MinoWidth, Globals.MinoHeight / 2);

        #endregion

        #region Tetromino Data struct

        /// <summary>
        /// Struct for convenience when generating the Tetromino
        /// </summary>
        private struct TetrominoData
        {
            /// <summary>
            /// Holds the rotation matrix for the tetromino
            /// </summary>
            public readonly Point[,] RotationMatrix;

            /// <summary>
            /// Holds the wall kick data for the tetromino
            /// </summary>
            public readonly Dictionary<Tetromino.RotationState, Point[]> WallKickData;

            /// <summary>
            /// The relative center translation between the tetromino center and the rotation mino
            /// </summary>
            public readonly Point CenterTranslation;

            /// <summary>
            /// Initializes the required components for the tetromino data
            /// </summary>
            /// <param name="rotationMatrix">Rotatino matrix for the tetromino</param>
            /// <param name="wallKickData">Wall kick data for the tetromino</param>
            /// <param name="centerTranslation">The center translation when trying to center the tetromino within an object</param>
            public TetrominoData(Point[,] rotationMatrix, Dictionary<Tetromino.RotationState, Point[]> wallKickData, Point centerTranslation)
            {
                this.RotationMatrix = rotationMatrix;
                this.WallKickData = wallKickData;
                this.CenterTranslation = centerTranslation;
            }
        }

        #endregion


    }
}
