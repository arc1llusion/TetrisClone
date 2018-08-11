

using System;
using System.Collections.Generic;
using System.Linq;
using WindowsPhone_Tetris.GameScreens;
using WindowsPhone_Tetris.Input;
using WindowsPhone_Tetris.TetrisClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace WindowsPhone_Tetris
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Color background = Color.Black;

        ScreenManager screenManager;

        /// <summary>
        /// Initializes necessary data for the screen size on the mobile platform
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            PhoneApplicationService service = PhoneApplicationService.Current;
            service.Activated += new EventHandler<ActivatedEventArgs>(ServiceActivate);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //We need to call the ScreenManager code before the XNA parent Initialize. This is because Initialize calls LoadContent
            //Without doing this, ScreenManagers LoadContent will not get called because it is not be part of the Games components yet
            //And thus will never be called.
            screenManager = new ScreenManager(this);
            screenManager.AddGameScreen(new MainMenuScreen() { IsActive = true, IsVisible = true });

            this.Components.Add(new Input.InputHandler(this, (GestureType.Tap | GestureType.DoubleTap | GestureType.HorizontalDrag)));
            this.Components.Add(screenManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            base.Draw(gameTime);
        }

        /// <summary>
        /// When the app is resumed, this event delegate will be called.
        /// 
        /// Didn't have time to complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ServiceActivate(object sender, ActivatedEventArgs e)
        {
        //    IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
        //    IsolatedStorageFileStream isfs = null;

        //    if (savegameStorage.FileExists("TetrisResume"))
        //    {
        //        if (screenManager != null && screenManager.CurrentScreen != null)
        //        {
        //            GameScreen screen = screenManager.CurrentScreen;

        //            if (screen.GetType() == typeof(GameplayScreen))
        //            {
        //                isfs = savegameStorage.OpenFile("TetrisResume", System.IO.FileMode.Open);
        //                if (isfs != null)
        //                {
        //                    screenManager.CurrentScreen.OnActivated(isfs, sender, e);
        //                }
        //            }
        //        }
        //    }
        }

        /// <summary>
        /// Logic to handle the current game screens serialization to the user store
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(object sender, EventArgs args)
        {
            //IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
            //IsolatedStorageFileStream isfs = null;

            //if (screenManager != null && screenManager.CurrentScreen != null)
            //{
            //    GameScreen screen = screenManager.CurrentScreen;

            //    if (screen.GetType() == typeof(GameplayScreen))
            //    {
            //        isfs = savegameStorage.OpenFile("TetrisResume", System.IO.FileMode.Create);
            //        if (isfs != null)
            //        {
            //            screenManager.CurrentScreen.OnExiting(isfs, sender, args);
            //        }
            //    }
            //}

            //isfs.Close();

            base.OnExiting(sender, args);
        }
    }
}
