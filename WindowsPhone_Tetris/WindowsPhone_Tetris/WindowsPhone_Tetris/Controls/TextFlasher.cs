

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsPhone_Tetris.Controls
{
    /// <summary>
    /// Small class for displaying text on the screen in a flashing manner. Supports multiple messages of varying positions and sizes
    /// </summary>
    public class TextFlasher
    {
        /// <summary>
        /// The amount of time to elapse before simulating a flash effect for the demo text
        /// </summary>
        private readonly int FlashTime;

        /// <summary>
        /// The delta for the demo flash time. When this reaches 0, the demo text will flash.
        /// </summary>
        private int flashDelta;

        /// <summary>
        /// Indicates whether or not the demo flash has toggled
        /// </summary>
        private bool isTextActive;

        /// <summary>
        /// The positions of the text to flash
        /// </summary>
        private Vector2[] positions;

        /// <summary>
        /// The font to use for all text
        /// </summary>
        private SpriteFont font;

        /// <summary>
        /// The scale of the fonts for each text item
        /// </summary>
        private float[] fontScales;

        /// <summary>
        /// The text to actually display
        /// </summary>
        private String[] text;

        /// <summary>
        /// The Colors for each item
        /// </summary>
        private Color[] colors;

        private bool isActive;
        /// <summary>
        /// Boolean value idnicating whether or not the flashing text is active. If true it will count down time and flash, false it will not render
        /// </summary>
        public bool IsActive
        {
            get { return this.isActive; }
            set { if (value) flashDelta = FlashTime; this.isActive = value; }
        }

        /// <summary>
        /// The constructor for a single message to be displayed
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="text">The text to display</param>
        /// <param name="position">The position for the text</param>
        /// <param name="fontScale">The scale of the font size</param>
        /// <param name="color">The color of the text</param>
        /// <param name="flashTime">The time to wait before flashing and alternating visibility</param>
        public TextFlasher(SpriteFont font, String text, Vector2 position, float fontScale, Color color, int flashTime) :
            this(font, new String[] { text }, new Vector2[] { position }, new float[] { fontScale }, new Color[] { color }, flashTime)
        {
        }

        /// <summary>
        /// The constructor for multiple messages to be displayed
        /// </summary>
        /// <param name="font">The fonts to use</param>
        /// <param name="text">The text to display</param>
        /// <param name="position">The positions for the text</param>
        /// <param name="fontScale">The scales of the font size</param>
        /// <param name="colors">The colors of the text</param>
        /// <param name="flashTime">The time to wait before flashing and alternating visibility</param>
        public TextFlasher(SpriteFont font, String[] text, Vector2[] position, float[] fontScale, Color[] colors, int flashTime)
        {
            this.FlashTime = flashTime;
            flashDelta = flashTime;

            isTextActive = true;
            isActive = true;

            this.positions = position;
            this.font = font;
            this.fontScales = fontScale;
            this.text = text;
            this.colors = colors;

            if (text.Length != position.Length || text.Length != fontScale.Length)
                throw new ArgumentException("Array lengths must match");
        }


        /// <summary>
        /// Counts down time before alternating the texts flash state
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                flashDelta -= gameTime.ElapsedGameTime.Milliseconds;
                if (flashDelta <= 0)
                {
                    flashDelta = FlashTime;
                    isTextActive = !isTextActive;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="batch">The sprite batch being used by the game</param>
        public void Draw(SpriteBatch batch)
        {
            if (isActive && isTextActive)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    batch.DrawString(font, text[i], positions[i], colors[i], 0.0f, Vector2.Zero, fontScales[i], SpriteEffects.None, 0.0f);
                }
            }
        }
    }
}
