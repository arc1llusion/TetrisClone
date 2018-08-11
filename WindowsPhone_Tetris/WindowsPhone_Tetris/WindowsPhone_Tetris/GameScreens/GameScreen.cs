
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;

namespace WindowsPhone_Tetris.GameScreens
{
    /// <summary>
    /// GameScreen class creates a framework for all screens in the game.
    /// </summary>
    public abstract class GameScreen
    {
        #region Private Fields

        /// <summary>
        /// ScreenManager object that the game screen is part of
        /// </summary>
        private ScreenManager screenManager;

        /// <summary>
        /// Boolean value indicating the screen is active and ready for updates
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Boolean value indicating that the screen is visible and should be drawn
        /// </summary>
        private bool isVisible;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that sets up some default variables for game screens
        /// </summary>
        public GameScreen()
        {
            this.isActive = true;
            this.isVisible = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// ScreenManager object that the game screen is part of
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return this.screenManager; }
            set { this.screenManager = value; }
        }

        /// <summary>
        /// Boolean value indicating the screen is active and ready for updates
        /// </summary>
        public bool IsActive
        {
            get { return this.isActive; }
            set { this.isActive = value; }
        }

        /// <summary>
        /// Boolean value indicating that the screen is visible and should be drawn
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <summary>
        /// A condition that gets set when update is first called. With screen managing sometimes the update is called after draw
        /// </summary>
        public bool ReceivedFirstUpdate
        {
            get;
            private set;
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Load Content is called when the screen is first loaded. It is only called once
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// If for some reason there are some unmanaged resources used by the screen, it will be handled here.
        /// </summary>
        public virtual void UnloadContent() { }

        /// <summary>
        /// Update is called every frame and is used to update logic within a game screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime) { ReceivedFirstUpdate = true; }

        /// <summary>
        /// Draw is called every frame after update and is used to render objects ob the screen. SpriteBatch object should be obtained from
        /// the ScreenManager
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Draw(GameTime gameTime) { }

        public virtual void OnActivated(IsolatedStorageFileStream stream, object sender, Microsoft.Phone.Shell.ActivatedEventArgs e) { }

        public virtual void OnExiting(IsolatedStorageFileStream stream, object sender, System.EventArgs e) { }

        #endregion
    }
}
