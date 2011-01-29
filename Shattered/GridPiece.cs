using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shattered
{
    /// <summary>
    /// The piece of the grid with art, animation, and hitboxes.
    /// 
    /// GridPieces are contained in Tiles, placed in GridSlots, and part of the overall Grid array.
    /// </summary>
    public class GridPiece
    {
        // The GridSlot this piece currently occupies.
        public GridSlot gSlot;

        // The List of HitBoxes that are contained in this GridPiece
        public List<HitBox> HitBoxes;

        /// <summary>
        /// Creates a new GridPiece with default values.
        /// </summary>
        public GridPiece()
        {
            // Initialize the GridPiece
            Initialize();
        }

        /// <summary>
        /// Sets all the initial values to the GridPiece.
        /// </summary>
        public void Initialize()
        {
            HitBoxes = new List<HitBox>();
        }

        /// <summary>
        /// Adds a HitBox to this GridPiece.
        /// </summary>
        /// <param name="b"></param>
        public void AddHitBox(HitBox b)
        {
            HitBoxes.Add(b);
        }

        /// <summary>
        /// Removes a HitBox from this GridPiece.
        /// </summary>
        /// <param name="b"></param>
        public void RemoveHitBox(HitBox b)
        {
            HitBoxes.Remove(b);
        }

        /// <summary>
        /// Moves the HitBoxes a specific number of pixels.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveHitBoxes(int x, int y)
        {
            foreach (HitBox b in HitBoxes)
            {
                b.Shift(x, y);
            }
        }

        /// <summary>
        /// Draws everything in this GridPiece. Temporary draw method.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        /// <param name="t"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Texture2D t)
        {
            foreach (HitBox b in HitBoxes)
            {
                Rectangle positionAdjusted = new Rectangle(b.Rect.X + (b.Rect.Width / 2), b.Rect.Y + (b.Rect.Height / 2), b.Rect.Width, b.Rect.Height);
                spriteBatch.Draw(t, positionAdjusted, new Rectangle(0, 0, 2, 6), Color.Black, b.Rect.Rotation, new Vector2(1, 3), SpriteEffects.None, 0);
            }
        }

        /// <summary>
        /// The XML representation of the GridPiece.
        /// </summary>
        /// <returns></returns>
        public string XmlOutput()
        {
            string s = "\n\t\t\t<gridpiece>";

            s += "\n\t\t\t\t<x>" + gSlot.GridPositionX.ToString() + "</x>";
            s += "\n\t\t\t\t<y>" + gSlot.GridPositionY.ToString() + "</y>";

            foreach (HitBox b in HitBoxes)
            {
                s += b.XmlOutput();
            }

            s += "\n\t\t\t</gridpiece>";

            return s;
        }

    }
}
