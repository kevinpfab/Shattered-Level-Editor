using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shattered
{
    public class Level
    {
        #region Instance Variables

        // The content manager for loading the level
        private ContentManager Content;

        // The Player in our level
        public Player Player;

        // Is our Level currently active?
        public bool IsActive;

        #region Grid

        // The point of origin of the grid, to keep track of its location.
        public Vector2 GridOrigin;

        // Grid Sizes
        public int GET_GRID_WIDTH
        {
            get
            {
                return GRID_WIDTH;
            }
        }
        public const int GRID_WIDTH = 256;
        public int GET_GRID_HEIGHT
        {
            get
            {
                return GRID_HEIGHT;
            }
        }
        public const int GRID_HEIGHT = 180;
        public const int HORIZONTAL_GRID_SPACES = 20;
        public const int VERTICAL_GRID_SPACES = 8;
        public const int GRID_DEFAULT_X = 0;
        public const int GRID_DEFAULT_Y = 4;
        public const float GRID_ZOOMED_IN = 1.0f;
        public const float GRID_ZOOMED_OUT = 0.25f;
        public float GRID_SCALE; // For zooming in and out on the grid.

        // Grid Array
        public GridSlot[,] Grid;

        // The type of movement in the grid
        public enum GridType
        {
            Move,
            Slide,
            Shift
        }
        public GridType gType;

        #endregion

        // The group of Tiles on the Grid
        public List<Tile> Tiles;

        // The Tile in the Grid we selected
        public Tile selectedTile;

        // The Tile in the Grid we are hovering over
        public Tile hoverTile;

        // Tile Sliding variables
        public bool tileSliding;
        public Vector2 slideDirection;
        public float slideInterval;
        public const float MAX_SLIDE_INTERVAL = 25.0f;

        #endregion

        #region Testing

        public Texture2D BlankPixel;
        public Texture2D WhiteScreen;

        #endregion

        /// <summary>
        /// Creates a new level.
        /// </summary>
        /// <param name="content">The content manager for loading.</param>
        public Level(ContentManager content)
        {
            this.Content = content;

            CreateGrid(GridType.Slide);

            Initialize();

            // Use Threads for this later
            LoadContent();

            CreatePlayer(Vector2.Zero);
        }

        /// <summary>
        /// Gets all the variables in the level ready for play.
        /// </summary>
        public void Initialize()
        {
            GRID_SCALE = 1.0f;

            Tiles = new List<Tile>();
            selectedTile = null;

            ActivateLevel();

            #region Testing
            /*
            // Add a Tile to the Grid
            GridPiece gp1 = new GridPiece();
            GridPiece gp2 = new GridPiece();
            GridPiece gp3 = new GridPiece();

            Grid[1, 6].PutInPiece(gp1);
            Grid[1, 7].PutInPiece(gp2);
            Grid[0, 7].PutInPiece(gp3);

            gp3.AddHitBox(new HitBox((int)(gp3.gSlot.Position.X + 50), (int)(gp3.gSlot.Position.Y + 100), 200, 50, 0));

            Tile t = new Tile(this);

            t.Add(gp1);
            t.Add(gp2);
            t.Add(gp3);

            Tiles.Add(t);

            // Add a second tile to test further
            GridPiece gp4 = new GridPiece();
            GridPiece gp5 = new GridPiece();

            Grid[3, 3].PutInPiece(gp4);
            Grid[3, 2].PutInPiece(gp5);

            Tile t2 = new Tile(this);

            t2.Add(gp4);
            t2.Add(gp5);

            Tiles.Add(t2);

            // Add a third Tile to test even further
            GridPiece gp6 = new GridPiece();
            GridPiece gp7 = new GridPiece();
            GridPiece gp8 = new GridPiece();
            GridPiece gp9 = new GridPiece();

            Grid[12, 4].PutInPiece(gp6);
            Grid[12, 5].PutInPiece(gp7);
            Grid[12, 6].PutInPiece(gp8);
            Grid[12, 7].PutInPiece(gp9);

            Tile t3 = new Tile(this);

            t3.Add(gp6);
            t3.Add(gp7);
            t3.Add(gp8);
            t3.Add(gp9);

            Tiles.Add(t3);
             * */
            #endregion

            // LoadLevel testing
            LevelLoader loader = new LevelLoader("1-1.xml", Content, this);
            loader.LoadLevel();

            SetGridStart(GRID_DEFAULT_X, GRID_DEFAULT_Y);
        }

        /// <summary>
        /// Custom LoadContent method for loading the level components.
        /// </summary>
        public void LoadContent()
        {
            // Testing
            BlankPixel = Content.Load<Texture2D>("Grid/WhitePixel");
            WhiteScreen = Content.Load<Texture2D>("Grid/WhiteScreen");
        }

        /// <summary>
        /// Creates the grid for use by the level. Makes a new GridSlot for each spot in the grid.
        /// </summary>
        public void CreateGrid(GridType gridType)
        {
            GridOrigin = Vector2.Zero;
            Grid = new GridSlot[HORIZONTAL_GRID_SPACES, VERTICAL_GRID_SPACES];

            for (int i = 0; i < HORIZONTAL_GRID_SPACES; i++)
            {
                for (int j = 0; j < VERTICAL_GRID_SPACES; j++)
                {
                    Vector2 SlotPosition = new Vector2(i * GRID_WIDTH, j * GRID_HEIGHT);
                    Vector2 PositionInGrid = new Vector2(i, j);
                    Grid[i,j] = new GridSlot(SlotPosition, PositionInGrid);
                }
            }

            this.gType = gridType;
        }

        /// <summary>
        /// Creates a new player for this level.
        /// </summary>
        /// <param name="pos">The position of the player.</param>
        public void CreatePlayer(Vector2 pos)
        {
            Player = new Player(this);
            Player.Position = pos;
        }

        /// <summary>
        /// Starts the Grid Camera in a different position than (0, 0)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetGridStart(int x, int y)
        {
            MoveElements(-(x * GRID_WIDTH), -(y * GRID_HEIGHT));
        }

        /// <summary>
        /// Updates the level, and all its components.
        /// </summary>
        /// <param name="gameTime">The current GameTime.</param>
        public void Update(GameTime gameTime)
        {
            // If we are not active, we cannot update
            if (!IsActive)
                return;

            /*
            #region Testing

            // If we have a Tile selected, we can move
            if (selectedTile != null)
            {
                #region Shift
                if (gType.Equals(GridType.Shift))
                {
                    // Move our tile on the grid
                    if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Right) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Right))
                    {
                        selectedTile.Shift(1, 0);
                    }

                    if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Left) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Left))
                    {
                        selectedTile.Shift(-1, 0);
                    }

                    if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Up) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Up))
                    {
                        selectedTile.Shift(0, -1);
                    }

                    if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Down) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Down))
                    {
                        selectedTile.Shift(0, 1);
                    }
                }
                #endregion

                #region Slide
                if (gType.Equals(GridType.Slide))
                {
                    if (!tileSliding)
                    {
                        // Slides
                        if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Right) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Right))
                        {
                            tileSliding = true;
                            slideDirection = new Vector2(1, 0);
                        }

                        if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Left) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Left))
                        {
                            tileSliding = true;
                            slideDirection = new Vector2(-1, 0);
                        }

                        if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Up) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Up))
                        {
                            tileSliding = true;
                            slideDirection = new Vector2(0, -1);
                        }

                        if (ShatteredGame.Global.CurrentKeyboardState.IsKeyDown(Keys.Down) && !ShatteredGame.Global.LastKeyboardState.IsKeyDown(Keys.Down))
                        {
                            tileSliding = true;
                            slideDirection = new Vector2(0, 1);
                        }
                    }
                    else
                    {
                        // Slide

                        // Are we ready to shift again?
                        if (slideInterval > MAX_SLIDE_INTERVAL)
                        {
                            // Try shifting, if we fail the sliding is done
                            if (!selectedTile.Shift((int)slideDirection.X, (int)slideDirection.Y))
                            {
                                tileSliding = false;
                            }
                            else
                            {
                                // We have shifted, reset the slide interval
                                slideInterval = 0;
                            }
                        }
                        else
                        {
                            slideInterval += gameTime.ElapsedGameTime.Milliseconds;
                        }
                    }
                }
                #endregion
            }

            #region Select a Different Tile
            // We can't select a new Tile if we are in motion
            if (!tileSliding)
            {
                // Try selecting a Tile
                // Determine the GridSlot we are currently hovering over
                int x = (int)((ShatteredGame.Global.CurrentMouseState.X) / (GRID_WIDTH * GRID_SCALE));
                int y = (int)((ShatteredGame.Global.CurrentMouseState.Y) / (GRID_HEIGHT * GRID_SCALE));

                if (x >= 0 && x < HORIZONTAL_GRID_SPACES && y >= 0 && y < VERTICAL_GRID_SPACES)
                {
                    GridSlot gs = Grid[x, y];

                    if (gs.gPiece != null)
                    {
                        // Find the Tile this GridPiece belongs to
                        foreach (Tile t in Tiles)
                        {
                            if (t.GridPieces.Contains(gs.gPiece))
                            {
                                if (ShatteredGame.Global.CurrentMouseState.LeftButton.Equals(ButtonState.Pressed) && ShatteredGame.Global.LastMouseState.LeftButton.Equals(ButtonState.Released))
                                {
                                    selectedTile = t;
                                }
                                else
                                {
                                    hoverTile = t;
                                }
                                break;
                            }
                        }
                    }
                    else if (ShatteredGame.Global.CurrentMouseState.LeftButton.Equals(ButtonState.Pressed))
                    {
                        // The Left Mouse Button is down, and we aren't over a GridPiece, so we are clearing the selectedTile
                        selectedTile = null;
                    }
                    else
                    {
                        // We are not hovering over a GridPiece anymore
                        hoverTile = null;
                    }
                }
            }
            #endregion

            #endregion
             */

            // Updates our Player
            Player.Update(gameTime);
        }

        /// <summary>
        /// Do the updates we need with the Editor active.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateEditor(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draws the entire Level onscreen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch we use to draw our 2d textures.</param>
        /// <param name="gameTime">The current GameTime.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draws the GridPieces on the field.
            foreach (Tile t in Tiles)
            {
                t.Draw(spriteBatch, gameTime);
            }

            // Draw the Player
            Player.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Activate the level.
        /// </summary>
        public void ActivateLevel()
        {
            IsActive = true;
        }

        /// <summary>
        /// Deactivate the level.
        /// </summary>
        public void DeactivateLevel()
        {
            IsActive = false;
        }


        /// <summary>
        /// Moves all the elements in the level. Movement is kept in integer increments. (No less than 1 pixel in a given direction)
        /// </summary>
        /// <param name="x">The distance in the x direction.</param>
        /// <param name="y">The distance in the y direction.</param>
        public void MoveElements(int x, int y)
        {
            // Moves the origin of the grid
            GridOrigin.X += x;
            GridOrigin.Y += y;

            // Moves all the GridSlots in the grid
            MoveGrid(x, y);

            // Moves the Player
            if(Player != null)
                Player.Move(x, y);
        }

        /// <summary>
        /// Moves every GridSlot in our Grid.
        /// </summary>
        /// <param name="x">The distance in the x direction.</param>
        /// <param name="y">The distance in the y direction.</param>
        public void MoveGrid(int x, int y)
        {
            foreach (GridSlot g in Grid)
            {
                g.Move(x, y);
            }

            foreach (Tile t in Tiles)
            {
                t.Move(x, y);
            }
        }

        /// <summary>
        /// Returns the XML in string format for the level we are using. MUST RESET LEVEL BEFORE CALLING FOR CORRECT XML!
        /// </summary>
        /// <returns>XML representation of the level.</returns>
        public string XmlOutput()
        {
            string s = "<level>\n\t<tiles>";

            foreach (Tile t in Tiles)
            {
                s += t.XmlOutput();
            }

            s += "\n\t</tiles>\n</level>";

            return s;
        }
    }
}
