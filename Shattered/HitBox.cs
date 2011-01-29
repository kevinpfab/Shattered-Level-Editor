using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shattered
{
    /// <summary>
    /// Hitbox is a wrapper for RotatedRectangle with special functions.
    /// </summary>
    public class HitBox
    {
        // The RotatedRectangle of this HitBox
        public RotatedRectangle Rect;

        // Store the original values
        public int x;
        public int y;
        public int posX;
        public int posY;
        public int width;
        public int height;
        float rot;

        /// <summary>
        /// Creates a new HitBox.
        /// </summary>
        /// <param name="x">Relative X Position.</param>
        /// <param name="y">Relative Y Position.</param>
        /// <param name="posX">X Position.</param>
        /// <param name="posY">Y Position.</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="rot">Rotation</param>
        public HitBox(int x, int y, int posX, int posY, int width, int height, float rot)
        {
            this.x = x;
            this.y = y;
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.rot = rot;

            Rect = new RotatedRectangle(new Rectangle(posX, posY, width, height), rot);
        }

        /// <summary>
        /// Creates a new HitBox.
        /// </summary>
        /// <param name="posX">X Position.</param>
        /// <param name="posY">Y Position.</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="rot">Rotation</param>
        public HitBox(int posX, int posY, int width, int height, float rot)
        {
            this.posX = posX;
            this.posY = posY;
            this.width = width;
            this.height = height;
            this.rot = rot;

            Rect = new RotatedRectangle(new Rectangle(posX, posY, width, height), rot);
        }

        /// <summary>
        /// Shifts the rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Shift(int x, int y)
        {
            Rect.ChangePosition(x, y);
        }

        /// <summary>
        /// Shifts the HitBox, including its original position. Used for editor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AdjustPosition(int x, int y)
        {
            Shift(x, y);

            this.x += x;
            this.y += y;
        }

        /// <summary>
        /// Changes the Height of the HitBox.
        /// </summary>
        /// <param name="y"></param>
        public void ChangeHeight(int y)
        {
            Rect.CollisionRectangle.Height += y;
            height += y;
        }

        /// <summary>
        /// Changes the Width of the HitBox.
        /// </summary>
        /// <param name="y"></param>
        public void ChangeWidth(int x)
        {
            Rect.CollisionRectangle.Width += x;
            width += x;
        }

        /// <summary>
        /// Changes the Rotation of the HitBox;
        /// </summary>
        /// <param name="r"></param>
        public void ChangeRotation(float r)
        {
            Rect.Rotation += r;
            rot += r;
        }

        /// <summary>
        /// Check if this HitBox hits another HitBox.
        /// </summary>
        /// <param name="b">The Other Hitbox.</param>
        /// <returns></returns>
        public bool HitTest(HitBox b)
        {
            return Rect.Intersects(b.Rect);
        }

        /// <summary>
        /// Check if this HitBox hits another rectangle.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool HitTest(Rectangle r)
        {
            return Rect.Intersects(r);
        }

        public string XmlOutput()
        {
            string s = "\n\t\t\t\t<hitbox>";

            s += "\n\t\t\t\t\t<x>" + x.ToString() + "</x>";
            s += "\n\t\t\t\t\t<y>" + y.ToString() + "</y>";
            s += "\n\t\t\t\t\t<width>" + width.ToString() + "</width>";
            s += "\n\t\t\t\t\t<height>" + height.ToString() + "</height>";
            s += "\n\t\t\t\t\t<rotation>" + rot.ToString() + "</rotation>";

            s += "\n\t\t\t\t</hitbox>";

            return s;
        }
    }
}
