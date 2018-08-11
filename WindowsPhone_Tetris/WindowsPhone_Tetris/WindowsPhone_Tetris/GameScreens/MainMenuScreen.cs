

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsPhone_Tetris.Audio;
using WindowsPhone_Tetris.Controls;
using WindowsPhone_Tetris.Input;
using WindowsPhone_Tetris.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace WindowsPhone_Tetris.GameScreens
{
    /// <summary>
    /// The main menu screen for the Tetris game
    /// </summary>
    public class MainMenuScreen : GameScreen
    {
        #region Fields

        /// <summary>
        /// The texture for the tetris logo
        /// </summary>
        private Texture2D tetrisLogo;

        /// <summary>
        /// The sprite batch to be used for rendering the games objects
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The text flasher for the text in the main menu on how to start
        /// </summary>
        private TextFlasher startFlasher;

        /// <summary>
        /// How long to wait before playing a demo
        /// </summary>
        private const int DemoWait = 10000;

        /// <summary>
        /// The elapsed time in the demo wait time
        /// </summary>
        private int demoDelta = DemoWait;

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor for the main menu
        /// </summary>
        public MainMenuScreen()
        {
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

            spriteBatch = ScreenManager.SpriteBatch;

            tetrisLogo = ScreenManager.Content.Load<Texture2D>(@"MainMenuTextures\Tetris-logo");
            Globals.TetrisFont = ScreenManager.Content.Load<SpriteFont>(@"Fonts\TetrisFont");

            //TODO: Read from XML
            AudioHelper.AddSoundEffect("LockDown", ScreenManager.Content.Load<SoundEffect>(@"Audio\TetrisPlace"));
            AudioHelper.AddSoundEffect("Tetris!", ScreenManager.Content.Load<SoundEffect>(@"Audio\TetrisUnlocked"));
            AudioHelper.AddSoundEffect("LineClear", ScreenManager.Content.Load<SoundEffect>(@"Audio\TetrisLineClear"));
            AudioHelper.AddSoundEffect("LevelUp", ScreenManager.Content.Load<SoundEffect>(@"Audio\TetrisLevelUp"));

            AudioHelper.AddSong("TetrisClassic", ScreenManager.Content.Load<Song>(@"Audio\TetrisClassic"));
            AudioHelper.AddSong("TetrisAcapella", ScreenManager.Content.Load<Song>(@"Audio\TetrisAcapella"));
            AudioHelper.AddSong("TetrisMetal", ScreenManager.Content.Load<Song>(@"Audio\TetrisMetal"));
            AudioHelper.PlaySong("TetrisMetal", true);

            Viewport port = this.ScreenManager.GraphicsDevice.Viewport;

            Vector2 fontMetrics = Globals.TetrisFont.MeasureString("Touch Anywhere to Begin");
            Vector2 startTextPosition = new Vector2((port.Width / 2) - (fontMetrics.X / 2), ((port.Height / 2) ) - (fontMetrics.Y / 2) + 50);

            startFlasher = new TextFlasher(Globals.TetrisFont, "Touch Anywhere to Begin", startTextPosition, 1.0f, Color.Orange, 1000);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            startFlasher.Update(gameTime);

            if (InputHandler.WasTouchInputPressed())
            {
                ScreenManager.AddGameScreen(new GameplayScreen(false));
            }

            demoDelta -= gameTime.ElapsedGameTime.Milliseconds;
            if (demoDelta <= 0)
            {
                demoDelta = DemoWait;
                ScreenManager.AddGameScreen(new GameplayScreen(true));
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();

            spriteBatch.Draw(tetrisLogo, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - tetrisLogo.Width / 2, 10), Color.White);
            startFlasher.Draw(spriteBatch);

            spriteBatch.End();
        }

        #endregion
    }
}
