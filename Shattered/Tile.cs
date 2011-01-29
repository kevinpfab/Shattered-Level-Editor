using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shattered
{
    /// <summary>
    /// The Tile class is a wrapper for a group of GridPieces. It handles movement and collision detection for each GridPiece.
    /// </summary>
    public class Tile
    {
        // The List of GridPieces in this tile
        public List<GridPiece> GridPieces;

        // The reference to our level, containing the Grid array
        public Level Level;

        // Make a new tile
        public Tile(Level l)
        {
            this.Level = l;

            // Initialize the variables for our Tile
            Initialize();
        }

        /// <summary>
        /// Gets this tile ready for use.
        /// </summary>
        public void Initialize()
        {
            GridPieces = new List<GridPiece>();
        }

        /// <summary>
        /// Adds a new GridPiece to our overall Tile.
        /// </summary>
        /// <param name="g">The GridPiece we are adding to our Tile.</param>
        public void Add(GridPiece g)
        {
            GridPieces.Add(g);
        }

        /// <summary>
        /// Attempts to shift the GridPieces in this tile in the specified direction. Returns the success of the shifting.
        /// </summary>
        /// <param name="dir">The direction we want to shift</param>
        public bool Shift(int xDir, int yDir)
        {
            // Perform the checks
            foreach (GridPiece gp in GridPieces)
            {
                int newX = gp.gSlot.GridPositionX + xDir;
                int newY = gp.gSlot.GridPositionY + yDir;

                // We are trying to shift out of bounds.
                if(newX >= Level.HORIZONTAL_GRID_SPACES || newY >= Level.VERTICAL_GRID_SPACES || newX < 0 || newY < 0)
                    return false;

                GridSlot gs = Level.Grid[newX, newY];

                // If the GridPiece in the GridSlot we are looking at does not belong to our Tile, we can't shift.
                if (gs.gPiece != null && !GridPieces.Contains(gs.gPiece))
                    return false;
            }

            #region Save a state of the old grid
            GridPiece[,] OldGrid = new GridPiece[Level.HORIZONTAL_GRID_SPACES, Level.VERTICAL_GRID_SPACES];

            for (int i = 0; i < Level.HORIZONTAL_GRID_SPACES; i++)
            {
                for (int j = 0; j < Level.VERTICAL_GRID_SPACES; j++)
                {
                    OldGrid[i, j] = Level.Grid[i, j].gPiece;
                }
            }
            #endregion

            // Loops through each piece on the board, and shifts it
            for (int i = 0; i < Level.HORIZONTAL_GRID_SPACES; i++)
            {
                for (int j = 0; j < Level.VERTICAL_GRID_SPACES; j++)
                {
                    // If this GridSlot has a GridPiece that belongs to this tile, perform actions on it.
                    if (GridPieces.Contains(OldGrid[i, j]))
                    {
                        // Shift the GridPiece to the next GridSlot in the specified direction
                        Level.Grid[i + xDir, j + yDir].PutInPiece(OldGrid[i, j]);

                        // Changes the position of the objects in the piece
                        int xMove = (i * Level.GRID_WIDTH) - ((i + xDir) * Level.GRID_WIDTH);
                        int yMove = (j * Level.GRID_HEIGHT) - ((j + yDir) * Level.GRID_HEIGHT);
                        OldGrid[i, j].MoveHitBoxes(xMove, yMove);

                        // Remove this GridPiece from the GridSlot before it moved
                        if (Level.Grid[i, j].gPiece.Equals(OldGrid[i, j]))
                        {
                            Level.Grid[i, j].RemovePiece();
                        }
                    }
                }
            }

            // If we have reached this point, we have successfully moved the piece.
            return true;
        }

        /// <summary>
        /// Moves all the elements in the Tile the specified number of pixels.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(int x, int y)
        {
            foreach (GridPiece g in GridPieces)
            {
                g.MoveHitBoxes(x, y);
            }
        }

        /// <summary>
        /// Draws the Tile onscreen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (GridPiece g in GridPieces)
            {
                g.Draw(spriteBatch, gameTime, Level.WhiteScreen);
            }
        }

        /// <summary>
        /// Returns the XML representation of this tile.
        /// </summary>
        /// <returns></returns>
        public string XmlOutput()
        {
            string s = "\n\t\t<tile>";

            foreach (GridPiece g in GridPieces)
            {
                s += g.XmlOutput();
            }

            s += "\n\t\t</tile>";

            return s;
        }
    }
}
