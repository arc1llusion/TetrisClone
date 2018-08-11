

using System;
using System.Collections.Generic;
using System.Linq;
using WindowsPhone_Tetris.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace WindowsPhone_Tetris.GameScreens
{
    /// <summary>
    /// ScreenManager is a DrawableGameComponent which means XNA will call this classes methods every update and draw.
    /// ScreenManager is for adding game screens and managing their flow.
    /// </summary>
    public class ScreenManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields & Properties

        /// <summary>
        /// The list of GameScreens for this instance
        /// </summary>
        private List<GameScreen> gameScreens;

        /// <summary>
        /// The SpriteBatch object to draw with.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// The ContentManager object to load resources with
        /// </summary>
        private ContentManager content;

        /// <summary>
        /// Boolean value indicating the ScreenManager has been initialized.
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// The stack pointer of the current screen being shown
        /// </summary>
        private int index = 0;

        /// <summary>
        /// SpriteBatch property for accessing the ScreenManagers render component
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return this.spriteBatch; }
        }

        /// <summary>
        /// ContentManager property to encapsulate the ScreenManagers ContentManager instance
        /// </summary>
        public ContentManager Content
        {
            get { return this.content; }
        }

        public GameScreen CurrentScreen
        {
            get {
                if (index >= 0 && index < gameScreens.Count)
                    return this.gameScreens[index];
                return null;
            }
        }

        #endregion

        #region Constructors and XNA methods

        /// <summary>
        /// Initializes the list of GameScreens in the ScreenManager
        /// </summary>
        /// <param name="game">Game object being played</param>
        public ScreenManager(Game game)
            : base(game)
        {
            gameScreens = new List<GameScreen>();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //This is needed because if we add a game screen prior to here, the ScreenManager content is not loaded, and LoadContent
            //is only called once so we want to call it in AddGameScreen as well. However, in this scenario, calling it from
            //AddGameScreen will throw an error due to the content not being loaded. LoadContent in ScreenManager will then call
            //LoadContent for any screens that need it.
            isInitialized = true;
        }

        /// <summary>
        /// LoadContent will be called once per game (Unless used and added more than once) and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (GameScreen screen in gameScreens)
            {
                screen.LoadContent();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            foreach (GameScreen screen in gameScreens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            if (gameScreens.Count > 0 && index >= 0 && index < gameScreens.Count)
                gameScreens[index].Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            if (gameScreens.Count > 0 && index >= 0 && index < gameScreens.Count && gameScreens[index].ReceivedFirstUpdate)
                gameScreens[index].Draw(gameTime);
        }

        #endregion

        #region Screen Manager methods

        /// <summary>
        /// Adds a game screen to the ScreenManager instance, and assigns itself as the GameScreens manager. Finally, it calls LoadContent if
        /// ScreenManager has been initialized.
        /// </summary>
        /// <param name="screen"></param>
        public void AddGameScreen(GameScreen screen)
        {
            screen.ScreenManager = this;

            if(isInitialized)
                screen.LoadContent();

            gameScreens.Add(screen);
            Push();
        }

        /// <summary>
        /// Removes a GameScreen instance from the ScreenManager and calls the screens UnloadContent method.
        /// </summary>
        /// <param name="screen"></param>
        public void RemoveGameScreen(GameScreen screen)
        {
            if(isInitialized)
                screen.UnloadContent();

            gameScreens.Remove(screen);
            Pop();
        }

        /// <summary>
        /// Pushes the game screen to the next screen in the list
        /// </summary>
        public void Push()
        {
            index++;
            if (index >= gameScreens.Count)
                index--;

            //Flush is needed because otherwise input can carry over from gameplay screens to other screens
            InputHandler.Flush();
        }

        /// <summary>
        /// Pops the game screen off the stack and displays the one before it
        /// </summary>
        public void Pop()
        {
            index--;
            if (index < 0)
                index = 0;

            //Flush is needed because otherwise input can carry over from gameplay screens to other screens
            InputHandler.Flush();
        }

        #endregion
    }
}
