

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsPhone_Tetris.Audio;
using WindowsPhone_Tetris.Controls;
using WindowsPhone_Tetris.Input;
using WindowsPhone_Tetris.Input.TetrominoHandlers;
using WindowsPhone_Tetris.TetrisClasses;
using WindowsPhone_Tetris.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace WindowsPhone_Tetris.GameScreens
{
    /// <summary>
    /// Where the primary Tetris logic is done
    /// </summary>
    public class GameplayScreen : GameScreen
    {
        #region Fields

        #region Tetris Specific Fields

        /// <summary>
        /// The object for the current tetromino that is controlled by the player
        /// </summary>
        private Tetromino currentTetromino;

        /// <summary>
        /// The tetromino that is currently being held
        /// </summary>
        private Tetromino holdTetromino;

        /// <summary>
        /// The hold Queue for the next tetrominos that will be in use
        /// </summary>
        private Queue<Tetromino> nextQueue;

        /// <summary>
        /// Boolean value indicating whether or not a hold was activated during the current round. A hold can only be used once
        /// When a piece drops it can be used again
        /// </summary>
        private bool usedHold;

        /// <summary>
        /// Tetromino factory handles generating the Tetrominos and supplying them with their rotation matrices
        /// </summary>
        private TetrominoFactory factory;

        /// <summary>
        /// The field is the play area for the player and where the tetrominos are confined
        /// </summary>
        private Field field;

        /// <summary>
        /// The level time step is the number of milliseconds that pass for before the tetromino is affected by gravity
        /// </summary>
        private int fallSpeed;

        /// <summary>
        /// the fall speed - elapsed time. When this reaches 0, it resets to current the next fall speed after moving the piece
        /// </summary>
        private int fallSpeedDelta;

        /// <summary>
        /// Array containing the fall speeds for each level
        /// </summary>
        private readonly int[] FallSpeeds;

        /// <summary>
        /// Integer array that contains the number of lines required to advance to the next level
        /// </summary>
        private readonly int[] LevelGoals;

        /// <summary>
        /// Integer that maintains the current level.
        /// </summary>
        private int level = 0;

        /// <summary>
        /// Contains the number of required lines - lines cleared for this level.
        /// </summary>
        private int linesClearedDelta = 0;

        /// <summary>
        /// Integer that maintains the players score.
        /// </summary>
        private int score = 0;

        #endregion

        #region Timing Related

        /// <summary>
        /// The "Lock Down" is when a tetromino comes into contact with some surface. The timer begins counting down before actually solidifying
        /// </summary>
        private const int lockDownTime = 500;

        /// <summary>
        /// The timer is what will hold the lockDownTime - elapsed time. When this reaches 0, the tetromino will lock.
        /// </summary>
        private int lockDownTimer = lockDownTime;

        #endregion

        #region Positioning, Input, and Demo related

        /// <summary>
        /// The cursor position describes the point for the rotation point of a tetromino and is where the player currently is
        /// </summary>
        private Point cursorPosition;

        /// <summary>
        /// The starting point of each Tetromino when initialized
        /// </summary>
        private readonly Point CursorStart = new Point(5, 1);

        /// <summary>
        /// The player input handler for tetris, whether that be the player or demo
        /// </summary>
        private ITetrominoInputHandler tetrisInput;

        /// <summary>
        /// Indicates whether the required touch input was activated at any update loop.
        /// 
        /// The reason this is needed is because if we simply "count" if anything happened, then when a "Drop" command is pressed
        /// during the "LockDown" state, it resets the timer and state instead of dropping. So we want to ignore this command.
        /// </summary>
        private bool inputFromUpdate = false;

        /// <summary>
        /// Boolean for detecting and resetting button clicks to show the arrow click feedback
        /// </summary>
        private bool[] buttonClicks = new bool[4];

        /// <summary>
        /// The text flasher for demo mode
        /// </summary>
        private TextFlasher demoFlasher;

        #endregion

        #region XNA Related

        /// <summary>
        /// The sprite batch to be used for rendering the games objects
        /// </summary>
        private SpriteBatch spriteBatch;

        #endregion

        #region Gameplay States

        /// <summary>
        /// The gameplay state coordinates what to do on an update.
        /// </summary>
        private enum GameplayState
        {
            /// <summary>
            /// The initialization of a player controlled game
            /// </summary>
            PlayerStart,

            /// <summary>
            /// The initialization of a demo controlled game
            /// </summary>
            DemoStart,

            /// <summary>
            /// Calls the TetrominoFactory object to supply the player and queue with a new tetromino
            /// </summary>
            InitializeTetromino,

            /// <summary>
            /// This occurs when a Tetromino is currently in play by the player and applies gravity to the piece
            /// </summary>
            TetrominoFalling,

            /// <summary>
            /// This state keeps track of the lock down timer when a Tetromino can no longer move.
            /// </summary>
            LockDown,

            /// <summary>
            /// The state that handles clearing the lines, tallying score, and resetting the state cycle
            /// </summary>
            LinesClearing,

            /// <summary>
            /// This occurs when a Tetromino is Initialized and cannot be placed. It will reset the game and score
            /// </summary>
            GameOver,

            /// <summary>
            /// When a player pauses, no other functions continue
            /// </summary>
            Paused
        };

        /// <summary>
        /// The current gameplay state of Tetris
        /// </summary>
        private GameplayState currentGameplayState;

        #endregion

        #region Collision Bounds

        /// <summary>
        /// The left button collision bounds
        /// </summary>
        public readonly Rectangle LeftButton = new Rectangle(-5, 676, 115, 105);

        /// <summary>
        /// The middle button collision bounds
        /// </summary>
        public readonly Rectangle MiddleButton = new Rectangle(110, 676, 115, 105);

        /// <summary>
        /// The right button collision bounds
        /// </summary>
        public readonly Rectangle RightButton = new Rectangle(225, 676, 115, 105);

        /// <summary>
        /// The rotate button collision bounds
        /// </summary>
        public readonly Rectangle RotateButton = new Rectangle(340, 676, 130, 105);

        /// <summary>
        /// The next box collision bounds
        /// </summary>
        public Rectangle NextBox = new Rectangle(332, 114, 68, 68);

        /// <summary>
        /// The hold box collision bounds
        /// </summary>
        public Rectangle HoldBox = new Rectangle(332, 184, 68, 68);

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the field, TetrominoFactory, queues and gameplay states needed for handling the gameplay logic
        /// </summary>
        public GameplayScreen(bool isDemo)
            : base()
        {
            field = new Field(new Point(0, -(Globals.MinoHeight * 2)), Globals.FieldWidth, Globals.FieldHeight);
            factory = new TetrominoFactory();
            nextQueue = new Queue<Tetromino>();
            currentGameplayState = isDemo ? GameplayState.DemoStart : GameplayState.PlayerStart;

            fallSpeed = 0;
            fallSpeedDelta = fallSpeed;

            LevelGoals = new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75 };
            FallSpeeds = new int[] { 1200, 1100, 1000, 900, 800, 700, 600, 500, 400, 300, 200, 100, 50, 25, 10 };

            linesClearedDelta = LevelGoals[level];
            fallSpeed = FallSpeeds[level];

            nextQueue.Enqueue(factory.GenerateRandomTetromino());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the button feedback
        /// </summary>
        public void ResetInput()
        {
            buttonClicks = new bool[4];
        }

        /// <summary>
        /// The field present in this gameplay screen
        /// </summary>
        public Field Field
        {
            get
            {
                return this.field;
            }
        }

        #endregion

        #region XNA Overridden Methods

        /// <summary>
        /// LoadContent will be called once per game (Unless used and added more than once) and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = ScreenManager.SpriteBatch; //Keep a local instance so we're not always calling a method
            Globals.TetrominoTexture = ScreenManager.Content.Load<Texture2D>(@"GameplayTextures\tetriminos");
            Globals.FieldTexture = ScreenManager.Content.Load<Texture2D>(@"GameplayTextures\Panda Attack");
            Globals.ArrowsTexture = ScreenManager.Content.Load<Texture2D>(@"GameplayTextures\arrows_2");

            Vector2 fontMetrics = Globals.TetrisFont.MeasureString("Demo Mode") * 2;
            Vector2 demoPosition = new Vector2((field.Width * Globals.MinoWidth / 2) - (fontMetrics.X / 2), ((field.Height - 2) * Globals.MinoHeight / 2) - (fontMetrics.Y / 2));

            Vector2 subHeadingMetrics = Globals.TetrisFont.MeasureString("Touch Anywhere");
            Vector2 subDemoPosition = new Vector2((field.Width * Globals.MinoWidth / 2) - (subHeadingMetrics.X / 2), demoPosition.Y + fontMetrics.Y + 10);

            demoFlasher = new TextFlasher(Globals.TetrisFont, 
                new String[] { "Demo Mode", "Touch Anywhere" },
                    new Vector2[] { demoPosition, subDemoPosition }, 
                    new float[] { 2.0f, 1.0f }, 
                    new Color[] { Color.Orange, Color.Orange },
                    1000);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (currentGameplayState)
            {
                case GameplayState.DemoStart:
                    demoFlasher.IsActive = true;
                    tetrisInput = new DemoInputHandler(this);
                    currentGameplayState = GameplayState.InitializeTetromino;
                    SetupGame();
                    break;
                case GameplayState.PlayerStart:
                    demoFlasher.IsActive = false;
                    tetrisInput = new PlayerInputHandler(this);
                    currentGameplayState = GameplayState.InitializeTetromino;
                    SetupGame();
                    break;
                case GameplayState.InitializeTetromino:
                    InitiateNewTetromino();
                    break;
                case GameplayState.TetrominoFalling:
                    HandleButtonClicks(gameTime);
                    HandleTetrominoFallingState(gameTime);
                    break;
                case GameplayState.LockDown:
                    HandleButtonClicks(gameTime);
                    HandleLockDownState(gameTime);
                    break;
                case GameplayState.LinesClearing:
                    HandleLinesClearingState(gameTime);
                    break;
                case GameplayState.GameOver:
                    HandleGameOverState(gameTime);
                    break;
                case GameplayState.Paused:
                    break;
            }

            demoFlasher.Update(gameTime);

            score = (int)MathHelper.Clamp(score, 0, 9999999);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (currentTetromino != null)
            {
                spriteBatch.Begin();

                this.DrawButtons();
                field.Draw(spriteBatch, gameTime); //draws the field
                field.DrawTetromino(spriteBatch, cursorPosition, currentTetromino); //draws the tetromino
                field.DrawTetromino(spriteBatch, GetDropTetrominoPosition(), currentTetromino, true); //draws the tetromino ghost

                this.DrawSideTetromino(nextQueue.Peek(), NextBox);
                this.DrawSideTetromino(holdTetromino, HoldBox);

                demoFlasher.Draw(spriteBatch);

                spriteBatch.End();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates a new Tetromino. It returns a boolean value indicating whether or not it was successful
        /// 
        /// Method also checks for Block Out game over condition
        /// </summary>
        /// <returns>True if it can be generated, false otherwise</returns>
        private void InitiateNewTetromino()
        {
            currentTetromino = nextQueue.Dequeue();
            nextQueue.Enqueue(factory.GenerateRandomTetromino());
            cursorPosition = CursorStart;
            fallSpeedDelta = fallSpeed;
            currentGameplayState = GameplayState.TetrominoFalling;

            if (!field.IsTetrominoInsertableAt(currentTetromino, cursorPosition) || field.IsGameOver())
            {
                currentGameplayState = GameplayState.GameOver;
            }
        }

        /// <summary>
        /// Handles the detection of whether or not the player clicked the movement buttons
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void HandleButtonClicks(GameTime gameTime)
        {
            TouchCollection touch = InputHandler.GetCurrentTouchLocationCollection();
            inputFromUpdate = false;

            switch (tetrisInput.GetTetrominoActions(gameTime))
            {
                case TetrominoAction.Left:
                    MoveTetromino(-1, 0);
                    inputFromUpdate = true;
                    buttonClicks[0] = true;
                    break;
                case TetrominoAction.SoftDrop:
                    MoveTetromino(0, 1);
                    buttonClicks[1] = true;
                    break;
                case TetrominoAction.Right:
                    MoveTetromino(1, 0);
                    inputFromUpdate = true;
                    buttonClicks[2] = true;
                    break;
                case TetrominoAction.HardDrop:
                    DropTetromino();
                    break;
                case TetrominoAction.Rotate:
                    this.RotateTetrominoClockwise();
                    inputFromUpdate = true;
                    buttonClicks[3] = true;
                    break;
                case TetrominoAction.Hold:
                    this.HoldTetromino();
                    usedHold = true;
                    break;
            }

            if (demoFlasher.IsActive && InputHandler.WasTouchInputReleased())
            {
                PopGameplayScreen();
            }
        }

        /// <summary>
        /// Handles the gravity logic for the current tetromino and automatic insertion if it can no longer move down
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void HandleTetrominoFallingState(GameTime gameTime)
        {
            fallSpeedDelta -= gameTime.ElapsedGameTime.Milliseconds;
            if (fallSpeedDelta <= 0)
            {
                fallSpeedDelta = fallSpeed;

                Point newCursorPos = cursorPosition;
                newCursorPos.Y += 1;

                if (field.IsTetrominoInsertableAt(currentTetromino, newCursorPos))
                {
                    cursorPosition = newCursorPos;
                }
                else
                {
                    currentGameplayState = GameplayState.LockDown;
                }
            }
        }

        /// <summary>
        /// Handles the logic necessary for the Tetromino delay. This allows a player to do rotations and movements even if it is in a position
        /// to be placed
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void HandleLockDownState(GameTime gameTime)
        {
            lockDownTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (lockDownTimer <= 0) //If the timer expires, the tetromino is placed and we proceed to line clearing as if DropTetromino was called
            {
                score += (3 + ((level + 1) * 3));
                lockDownTimer = lockDownTime;
                field.InsertTetrominoAt(currentTetromino, cursorPosition);
                currentGameplayState = GameplayState.LinesClearing;

                AudioHelper.PlaySound("LockDown");
            }

            Point down = new Point(cursorPosition.X, cursorPosition.Y + 1);
            if (inputFromUpdate || field.IsTetrominoInsertableAt(currentTetromino, down)) //If input was received, we reset the timer and continue to let the tetromino fall
            {
                lockDownTimer = lockDownTime;
                fallSpeedDelta = fallSpeed;
                currentGameplayState = GameplayState.TetrominoFalling;
            }
        }

        /// <summary>
        /// Handles the logic for clearing the lines and updating the score and level.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void HandleLinesClearingState(GameTime gameTime)
        {
            List<int[]> clearedLines = field.ClearLines();
            int linesClearedCount = clearedLines.Count;

            if (linesClearedCount > 0)
            {
                if (linesClearedCount == 4)
                {
                    linesClearedDelta -= 8;
                    AudioHelper.PlaySound("Tetris!");
                }
                else
                {
                    linesClearedDelta -= linesClearedCount;
                    AudioHelper.PlaySound("LineClear");
                }
            }

            if (linesClearedDelta <= 0)
                LevelUp();

            score += linesClearedCount * (level + 1) * 100;

            usedHold = false;

            currentGameplayState = GameplayState.InitializeTetromino;
        }

        /// <summary>
        /// Logic to handle the GameOver state in the Tetris Engine
        /// </summary>
        private void HandleGameOverState(GameTime gameTime)
        {
            PopGameplayScreen();
        }

        /// <summary>
        /// Removes this screen from the screen manager and goes back to the main menu
        /// </summary>
        private void PopGameplayScreen()
        {
            this.IsVisible = false;
            ScreenManager.RemoveGameScreen(this);
        }

        private void SetupGame()
        {
            score = level = 0;
            linesClearedDelta = LevelGoals[level];
            fallSpeed = FallSpeeds[level];

            factory.Flush();
            field.ResetField();

            nextQueue.Clear();
            nextQueue.Enqueue(factory.GenerateRandomTetromino()); //Otherwise we run a risk for repeats in the beginning of a level

            holdTetromino = null;
            usedHold = false;
        }

        /// <summary>
        /// Method to handle what happens on level up
        /// </summary>
        private void LevelUp()
        {
            if (level < 14)
            {
                fallSpeed = FallSpeeds[level];
                linesClearedDelta = LevelGoals[++level];
                AudioHelper.PlaySound("LevelUp");
            }
        }

        /// <summary>
        /// Moves the tetromino relative to the current position
        /// </summary>
        /// <param name="x">Movement on the horizontal axis</param>
        /// <param name="y">Movement on the vertical axis</param>
        private void MoveTetromino(int x, int y)
        {
            Point newCursorPos = cursorPosition;
            newCursorPos.X += x;
            newCursorPos.Y += y;

            if (field.IsTetrominoInsertableAt(currentTetromino, newCursorPos))
            {
                cursorPosition = newCursorPos;
            }
        }

        /// <summary>
        /// Forcefully drops the tetromino to its current lowest position. No delay factor will be considered here.
        /// </summary>
        private void DropTetromino()
        {
            Point position = GetDropTetrominoPosition();
            field.InsertTetrominoAt(currentTetromino, new Point(position.X, position.Y));
            score += position.Y - cursorPosition.Y;
            currentGameplayState = GameplayState.LinesClearing;

            AudioHelper.PlaySound("LockDown");
        }

        /// <summary>
        /// Gets the drop point of a tetromino. This can be used to get the ghost piece position or to actually drop the tetromino.
        /// </summary>
        /// <returns></returns>
        private Point GetDropTetrominoPosition()
        {
            int y = cursorPosition.Y;

            while (field.IsTetrominoInsertableAt(currentTetromino, cursorPosition.X, y + 1)) { y++; }

            return new Point(cursorPosition.X, y);
        }

        /// <summary>
        /// This method is for rotating the current Tetromino clockwise. The difference between this method and the method
        /// in the Tetromino class is that it accounts for wall kicks.
        /// </summary>
        private void RotateTetrominoClockwise()
        {
            currentTetromino.RotateClockwise();

            if (!field.IsTetrominoInsertableAt(currentTetromino, cursorPosition))
            {
                Point position;
                if (IterateWallKicks(out position))
                {
                    cursorPosition = position;
                }
                else
                {
                    currentTetromino.RotateCounterClockwise();
                }
            }
        }

        /// <summary>
        /// This method is for rotating the current Tetromino counter clockwise. The difference between this method and the method
        /// in the Tetromino class is that it accounts for wall kicks.
        /// </summary>
        private void RotateTetrominoCounterClockwise()
        {
            currentTetromino.RotateCounterClockwise();

            if (!field.IsTetrominoInsertableAt(currentTetromino, cursorPosition))
            {
                Point position;
                if (IterateWallKicks(out position))
                {
                    cursorPosition = position;
                }
                else
                {
                    currentTetromino.RotateClockwise();
                }
            }
        }

        /// <summary>
        /// Iterates through the wall kick attempts in order to try and find a valid place to rotate the tetromino.
        /// </summary>
        private bool IterateWallKicks(out Point position)
        {
            Point[] wallData = currentTetromino.WallKickData[currentTetromino.CurrentRotationState];
            bool success = false;

            position = new Point(0, 0);

            for (int i = 0; i < wallData.Length && !success; i++)
            {
                Point currentWallKick = wallData[i];
                position = cursorPosition;

                position = new Point(cursorPosition.X + currentWallKick.X, cursorPosition.Y + currentWallKick.Y);

                success = field.IsTetrominoInsertableAt(currentTetromino, position);
            }

            return success;
        }

        private void HoldTetromino()
        {
            if (!usedHold)
            {
                Tetromino temp = holdTetromino;
                holdTetromino = currentTetromino;
                holdTetromino.ResetRotation();

                if (temp != null)
                {
                    currentTetromino = temp;
                    cursorPosition = CursorStart;
                }
                else
                {
                    currentGameplayState = GameplayState.InitializeTetromino;
                }

                usedHold = true;
            }
        }

        /// <summary>
        /// Handles the drawing of the movement buttons
        /// </summary>
        private void DrawButtons()
        {
            Texture2D arrows = Globals.ArrowsTexture;

            SpriteFont font = Globals.TetrisFont;
            spriteBatch.Draw(Globals.FieldTexture, new Vector2(0, 0), Color.White);

            spriteBatch.DrawString(font, (this.level + 1).ToString("D2"), new Vector2(430, 250), Color.White);
            spriteBatch.DrawString(font, this.score.ToString("D7"), new Vector2(340, 45), Color.Black);
            spriteBatch.DrawString(font, this.linesClearedDelta.ToString("D2"), new Vector2(430, 290), Color.White);

            int x = -5;
            int y = 676;

            int width = 0;

            for (int i = 0; i < 4; i++)
            {
                if (buttonClicks[i])
                    if (i == 3)
                        spriteBatch.Draw(arrows, new Vector2(x - 1, y - 8), new Rectangle(width, 178, 130, 105), Color.White);
                    else
                        spriteBatch.Draw(arrows, new Vector2(x - 1, y - 8), new Rectangle(width, 178, 115, 105), Color.White);
                else
                    spriteBatch.Draw(arrows, new Vector2(x, y), new Rectangle(width, 0, 115, 105), Color.White);

                x += 115;
                width += 115;
            }
        }

        /// <summary>
        /// Draws the tetromino in either the next or hold box on the right side of the game screen
        /// </summary>
        private void DrawSideTetromino(Tetromino tetromino, Rectangle box)
        {
            if (tetromino != null)
            {
                //We divide by two because we're showing a smaller tetromino in the box
                Point centerTranslation = new Point(tetromino.CenterTranslation.X / 2, tetromino.CenterTranslation.Y / 2);

                int x = box.Left + (box.Width / 2) - centerTranslation.X;
                int y = box.Top + (box.Height / 2) - centerTranslation.Y;
                int width = Globals.MinoWidth / 2;
                int height = Globals.MinoHeight / 2;

                for (int i = 0; i < 4; i++)
                {
                    Point mino = tetromino[i];

                    spriteBatch.Draw(Globals.TetrominoTexture,
                        new Rectangle(x + ((width) * mino.X), y + ((height) * mino.Y), width, height),
                        new Rectangle(Globals.MinoTextureWidth * tetromino.TetrominoType, 0, Globals.MinoTextureWidth, Globals.MinoTextureHeight), Color.White);
                }
            }
        }

        #endregion
    }
}
