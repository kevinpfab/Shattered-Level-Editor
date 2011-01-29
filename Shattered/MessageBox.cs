using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shattered
{
    public class MessageBox
    {
        // The text we are displaying in the message box
        public string Text;

        // The font we will use to draw the string
        public SpriteFont Font;
        
        // The font color we will use to draw the string
        public Color FontColor;

        // The color we will use as the background
        public Color BackgroundColor;

        // The texture we will use to display a background
        public Texture2D Pixel;

        // The Rectangle we will draw (size)
        public Rectangle Rect;

        // The starting position of the text
        public Vector2 Pos;

        // Are we displaying the box?
        public bool IsVisible;

        /// <summary>
        /// Creates a new MessageBox.
        /// </summary>
        /// <param name="f"></param>
        public MessageBox(SpriteFont f, Texture2D t)
        {
            Font = f;
            Pixel = t;

            Initialize();
        }

        public void Initialize()
        {
            Rect = new Rectangle(200, 600, 1280 - 400, 100);
            Pos = new Vector2(1280 / 2, 650);

            FontColor = Color.Black;

            IsVisible = false;

            BackgroundColor = Color.Gray;
        }

        /// <summary>
        /// Changes the message box text.
        /// </summary>
        /// <param name="s"></param>
        public void SetText(string s)
        {
            Text = s;
        }

        /// <summary>
        /// Changes the font to what
        /// </summary>
        /// <param name="f"></param>
        public void ChangeFont(SpriteFont f)
        {
            Font = f;
        }

        public void Open()
        {
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
        }

        /// <summary>
        /// Draws the MessageBox onscreen;
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(Pixel, Rect, BackgroundColor);
            spriteBatch.DrawString(Font, Text, Pos, FontColor, 0, Font.MeasureString(Text) / 2, 1.0f, SpriteEffects.None, 0);
        }
    }
}
