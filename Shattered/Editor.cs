using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Shattered
{
    public class Editor
    {
        // Is the Editor Active?
        public bool IsActive;

        // The SpriteFont used for the overlay
        public SpriteFont DefaultFont;

        // The Level we are currently editing
        public Level l;

        // The ContentManager we will use
        public ContentManager Content;

        #region Selected Elements
        public GridPiece SelectedGridPiece;
        public HitBox SelectedHitBox;
        public Tile SelectedTile;
        #endregion

        #region Overlay
        public Vector2 OverlayPosition;
        public string OverlayText;
        #endregion

        // The MessageBox for our editor
        public MessageBox Message;

        // The scroll speed of our editor
        public int ScrollSpeed;

        public enum HitboxState
        {
            None,
            Placing,
            Placed
        }
        public HitboxState NewHitboxState;

        public Editor(ContentManager Content, Level lev)
        {
            this.Content = Content;
            this.l = lev;

            Initialize();
        }

        /// <summary>
        /// Initializes all the variables in the Editor.
        /// </summary>
        public void Initialize()
        {
            IsActive = false;

            DefaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");

            OverlayPosition = new Vector2(25, 25);
            OverlayText = "Press e to toggle editor\n" +
                            "Click a GridPiece to select that GridPiece\n" +
                            "Clicking a HitBox will also select its GridPiece\n" +
                            "Click while holding n for a new HitBox\n" +
                            "-said HitBox will be added to the GridPiece in the GridSlot you started it in.\n" +
                            "";

            ScrollSpeed = 10;

            Message = new MessageBox(DefaultFont, l.BlankPixel);

            NewHitboxState = HitboxState.None;
        }

        /// <summary>
        /// Updates the Editor.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // Check for Activity Change
            if (ControllerState.Pushed(Keys.Insert))
                if (IsActive)
                    IsActive = false;
                else
                    IsActive = true;

            // Don't use the editor if it isn't active
            if (!IsActive)
                return;

            // Move the Grid Around
            if (ControllerState.Down(Keys.Right))
            {
                l.MoveElements(ScrollSpeed, 0);
            }
            else if (ControllerState.Down(Keys.Left))
            {
                l.MoveElements(-ScrollSpeed, 0);
            }

            if (ControllerState.Down(Keys.Up))
            {
                l.MoveElements(0, -ScrollSpeed);
            }
            else if (ControllerState.Down(Keys.Down))
            {
                l.MoveElements(0, ScrollSpeed);
            }

            // WASDR expansion and rotation
            if (SelectedHitBox != null)
            {
                if (ControllerState.Down(Keys.W))
                {
                    SelectedHitBox.ChangeHeight(-1);
                }
                else if (ControllerState.Down(Keys.S))
                {
                    SelectedHitBox.ChangeHeight(1);
                }

                if (ControllerState.Down(Keys.D))
                {
                    SelectedHitBox.ChangeWidth(1);
                }
                else if (ControllerState.Down(Keys.A))
                {
                    SelectedHitBox.ChangeWidth(-1);
                }

                if (ControllerState.Down(Keys.E))
                {
                    SelectedHitBox.ChangeRotation(-MathHelper.Pi / 100);
                }
                else if (ControllerState.Down(Keys.R))
                {
                    SelectedHitBox.ChangeRotation(MathHelper.Pi / 100);
                }
            }

            // Check if we want to make a new HitBox
            if (ControllerState.Pushed(Keys.N))
            {
                if (NewHitboxState.Equals(HitboxState.None))
                {
                    Message.Open();
                    Message.SetText("Please click the GridSlot you would like to create the HitBox in.");

                    NewHitboxState = HitboxState.Placing;
                }
                else if (NewHitboxState.Equals(HitboxState.Placed))
                {
                    // We chose make a new Tile
                    Tile nt = new Tile(l);
                    nt.GridPieces.Add(SelectedGridPiece);
                    l.Tiles.Add(nt);

                    NewHitboxState = HitboxState.None;
                    Message.Close();
                }
            }

            // Check for a Mouse Click somewhere
            if (ControllerState.MouseLeftClicked())
            {
                int x = (int)(MathHelper.Clamp(ControllerState.MouseX(), 0, 1280));
                int y = (int)(MathHelper.Clamp(ControllerState.MouseY(), 0, 720));

                Rectangle tempRect = new Rectangle(x, y, 1, 1);

                // Determine the GridSlot we clicked in
                int gridX = (int)MathHelper.Clamp(((x - l.GridOrigin.X) / Level.GRID_WIDTH), 0, Level.HORIZONTAL_GRID_SPACES - 1);
                int gridY = (int)MathHelper.Clamp(((y - l.GridOrigin.Y) / Level.GRID_HEIGHT), 0, Level.VERTICAL_GRID_SPACES - 1);

                GridPiece gp = l.Grid[gridX, gridY].gPiece;

                SelectedGridPiece = gp;

                // Check if we were trying to place a NewHitBox
                if (NewHitboxState.Equals(HitboxState.Placing))
                {
                    if (gp == null)
                    {
                        // Create a new GridPiece because there was not one
                        GridPiece NewPiece = new GridPiece();
                        l.Grid[gridX, gridY].PutInPiece(NewPiece);
                        gp = NewPiece;
                        SelectedGridPiece = gp;

                        // Add a new HitBox to that GridPiece
                        HitBox nhb = new HitBox(50, 50, (int)(gp.gSlot.Position.X + 50), (int)(gp.gSlot.Position.Y + 50), 150, 25, 0);
                        gp.AddHitBox(nhb);

                        // Change the Message
                        Message.SetText("Now please select the tile you want to add this GridPiece to. Or press N for a new Tile");

                        // Change the State
                        NewHitboxState = HitboxState.Placed;
                    }
                    else // We already have a GridPiece and therefore a Tile to put the new object in
                    {
                        // Add a new HitBox to that GridPiece
                        HitBox nhb = new HitBox(50, 50, (int)(gp.gSlot.Position.X + 50), (int)(gp.gSlot.Position.Y + 50), 150, 25, 0);
                        gp.AddHitBox(nhb);

                        Message.Close();
                        NewHitboxState = HitboxState.None;
                    }

                }

                // Don't go further if we are still trying to place a NewHitBox
                if (!NewHitboxState.Equals(HitboxState.None))
                    return;

                // Check if we hit a HitBox with our click
                bool hit = false;

                // Loop through each HitBox to check for intersections
                if (gp != null)
                {
                    foreach (HitBox b in gp.HitBoxes)
                    {
                        if (b.HitTest(tempRect))
                        {
                            hit = true;
                            SelectedHitBox = b;
                            break;
                        }
                    }
                }

                // If there was no hit, clear the selection
                if (!hit)
                {
                    SelectedHitBox = null;
                }
                else
                {
                    // Check the Tile we are selecting
                    foreach (Tile t in l.Tiles)
                    {
                        if (t.GridPieces.Contains(gp))
                        {
                            SelectedTile = t;
                            break;
                        }
                    }
                }
            }

            // Temp testing (working logic for moving objects)
            if (ControllerState.CurrentMouseState.LeftButton.Equals(ButtonState.Pressed))
            {
                if (SelectedHitBox != null)
                {
                    SelectedHitBox.AdjustPosition(ControllerState.CurrentMouseState.X - ControllerState.LastMouseState.X, ControllerState.CurrentMouseState.Y - ControllerState.LastMouseState.Y);

                    // If we are moving out of our GridPiece, stay in that GridPiece
                    HitBox nhb = new HitBox(ControllerState.MouseX(), ControllerState.MouseY(), 1, 1, 0);
                    if (!SelectedHitBox.HitTest(SelectedGridPiece.gSlot.Rect) || !nhb.HitTest(SelectedGridPiece.gSlot.Rect))
                    {
                        SelectedHitBox.AdjustPosition(-(ControllerState.CurrentMouseState.X - ControllerState.LastMouseState.X), -(ControllerState.CurrentMouseState.Y - ControllerState.LastMouseState.Y));
                    }
                }
            }

            // HitBox Deletion
            if (ControllerState.Pushed(Keys.Delete))
            {
                // Delete the selected HitBox
                if (SelectedHitBox != null)
                {
                    // Find where the HitBox is located
                    foreach (GridSlot gs in l.Grid)
                    {
                        if (gs.gPiece != null && gs.gPiece.HitBoxes.Contains(SelectedHitBox))
                        {
                            gs.gPiece.RemoveHitBox(SelectedHitBox);
                            SelectedHitBox = null;
                            break;
                        }
                    }
                }
            }

            // Check for XML Output Request
            if (ControllerState.Pushed(Keys.PageUp))
            {
                System.Console.Write(l.XmlOutput());
            }

        }

        /// <summary>
        /// Draw the Editor.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsActive)
                return;

            DrawGrid(spriteBatch, gameTime);

            DrawOverlay(spriteBatch, gameTime);

            if (SelectedHitBox != null)
            {
                Rectangle positionAdjusted = new Rectangle(SelectedHitBox.Rect.X + (SelectedHitBox.Rect.Width / 2), SelectedHitBox.Rect.Y + (SelectedHitBox.Rect.Height / 2), SelectedHitBox.Rect.Width, SelectedHitBox.Rect.Height);
                spriteBatch.Draw(l.WhiteScreen, positionAdjusted, new Rectangle(0, 0, 2, 6), Color.Red, SelectedHitBox.Rect.Rotation, new Vector2(1, 3), SpriteEffects.None, 0);
            }

            Message.Draw(spriteBatch, gameTime);
        }

        /// <summary>
        /// Draws the overlay text for the editor.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void DrawOverlay(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(DefaultFont, OverlayText, OverlayPosition, Color.White);
        }

        /// <summary>
        /// Draws the Grid on the screen for us to use.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void DrawGrid(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < Level.HORIZONTAL_GRID_SPACES; i++)
            {
                for (int j = 0; j < Level.VERTICAL_GRID_SPACES; j++)
                {
                    spriteBatch.Draw(l.BlankPixel, new Rectangle(Level.GRID_WIDTH * i + (int)l.GridOrigin.X, Level.GRID_HEIGHT * j + (int)l.GridOrigin.Y, 1, Level.GRID_HEIGHT), Color.DarkGray);
                    spriteBatch.Draw(l.BlankPixel, new Rectangle(Level.GRID_WIDTH * i + (int)l.GridOrigin.X, Level.GRID_HEIGHT * j + (int)l.GridOrigin.Y, Level.GRID_WIDTH, 1), Color.DarkGray);

                    string s = "(" + i.ToString() + ", " + j.ToString() + ")";
                    spriteBatch.DrawString(DefaultFont, s, new Vector2(Level.GRID_WIDTH * i + (int)l.GridOrigin.X + Level.GRID_WIDTH / 2, Level.GRID_HEIGHT * j + (int)l.GridOrigin.Y + Level.GRID_HEIGHT / 2), Color.DarkGray, 0, DefaultFont.MeasureString(s) / 2, 1.0f, SpriteEffects.None, 0);
                }
            }

            // End Lines
            spriteBatch.Draw(l.BlankPixel, new Rectangle((int)l.GridOrigin.X, Level.GRID_HEIGHT * Level.VERTICAL_GRID_SPACES + (int)l.GridOrigin.Y, Level.GRID_WIDTH * Level.HORIZONTAL_GRID_SPACES, 1), Color.DarkGray);
            spriteBatch.Draw(l.BlankPixel, new Rectangle(Level.GRID_WIDTH * Level.HORIZONTAL_GRID_SPACES + (int)l.GridOrigin.X, (int)l.GridOrigin.Y, 1, Level.GRID_HEIGHT * Level.VERTICAL_GRID_SPACES), Color.DarkGray);
        }
    }
}
