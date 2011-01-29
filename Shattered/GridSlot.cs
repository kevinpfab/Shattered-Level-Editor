using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shattered
{
    /// <summary>
    /// A GridSlot represents one of the GRID_WIDTH by GRID_HEIGHT slots that are locked to the Grid. They contain one to zero GridPieces.
    /// </summary>
    public class GridSlot
    {
        // The position of the slot in the grid
        public int GridPositionX;
        public int GridPositionY;

        // The Position of this Grid Slot on the screen
        public Vector2 Position;

        // The GridPiece currently in this GridSlot.
        public GridPiece gPiece;

        // The Rectangle representing this GridSlot.
        public Rectangle Rect;

        // Make a new GridSlot
        public GridSlot(Vector2 Position, Vector2 GridPosition)
        {
            this.GridPositionX = (int)GridPosition.X;
            this.GridPositionY = (int)GridPosition.Y;
            this.Position = Position;

            Initialize();
        }

        /// <summary>
        /// Resets all variables for initial use.
        /// </summary>
        public void Initialize()
        {
            gPiece = null;

            this.Rect = new Rectangle((int)Position.X, (int)Position.Y, Level.GRID_WIDTH, Level.GRID_HEIGHT);
        }
        
        /// <summary>
        /// Moves the GridSlot on the screen
        /// </summary>
        /// <param name="x">The distance in the x direction.</param>
        /// <param name="y">The distance in the y direction.</param>
        public void Move(float x, float y)
        {
            Position.X += x;
            Position.Y += y;

            Rect.X += (int)x;
            Rect.Y += (int)y;
        }

        /// <summary>
        /// Puts a GridPiece into this GridSlot.
        /// </summary>
        /// <param name="piece">The GridPiece we are putting in the slot.</param>
        public void PutInPiece(GridPiece piece)
        {
            // Give each an identifier of eachother.
            this.gPiece = piece;
            this.gPiece.gSlot = this;
        }

        /// <summary>
        /// Removes the GridPiece from this GridSlot.
        /// </summary>
        public void RemovePiece()
        {
            this.gPiece = null;
        }
    }
}
