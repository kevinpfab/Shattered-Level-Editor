using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Shattered
{
    public class Player
    {
        #region Instance Variables
        // Level reference
        public Level Level;

        // Player Position (x, y)
        public Vector2 Position;

        // Player Velocity (dx, dy)
        public Vector2 Velocity;

        // Horizontal Movement Controllers
        public float MaxHorizontalSpeed;
        public float HorizontalAcceleration;
        public float StaticFriction;
        public float HorizontalDrag;

        // Vertical Movement Controllers
        public float MaxFallSpeed;
        public float GravitationalAcceleration;
        public float JumpHeight;

        // The Origin of the Player (to draw on)
        public Vector2 Origin;

        // The HitBox defining the boundary box on our player
        public HitBox Box;
        public const int PLAYER_HEIGHT = 90;
        public const int PLAYER_WIDTH = 60;

        // Boolean Knowledge Variables
        public bool IsOnGround;
        public bool IsHitRight;
        public bool IsHitLeft;
        public bool IsHitTop;
        #endregion

        // Creates the player object
        public Player(Level l)
        {
            this.Level = l;

            Initialize();
        }

        /// <summary>
        /// Creates the initial values for all reference variables in our Player.
        /// </summary>
        public void Initialize()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;

            Box = new HitBox((int)Position.X, (int)Position.Y, PLAYER_WIDTH, PLAYER_HEIGHT, 0);

            MaxHorizontalSpeed = 6;
            HorizontalAcceleration = 0.75f;
            StaticFriction = 0.9f;
            HorizontalDrag = 0.9f;

            MaxFallSpeed = 10;
            GravitationalAcceleration = 0.5f;
            JumpHeight = 12;

            ResetKnowledge();
        }

        /// <summary>
        /// Resets all the knowledge variables to their default values for recalculation.
        /// </summary>
        public void ResetKnowledge()
        {
            // Knowledge Variables
            IsOnGround = false;
            IsHitLeft = false;
            IsHitRight = false;
            IsHitTop = false;
        }

        /// <summary>
        /// Moves the player and all its components.
        /// </summary>
        /// <param name="x">The distance in the x direction.</param>
        /// <param name="y">The distance in the y direction.</param>
        public void Move(int x, int y)
        {
            // Moves the primary position
            Position.X += x;
            Position.Y += y;

            // Moves the HitBox surrounding our player
            Box.Shift(x, y);
        }

        // Updates the player
        public void Update(GameTime gameTime)
        {
            // Moves the player the calculated amount from the last frame.
            Move((int)Velocity.X, (int)Velocity.Y);

            // Forget the last frame's knowledge to recheck all variables.
            ResetKnowledge();

            // Moves the player out of any ground he is touching.
            HandleGround();

            // Run Velocity Modifiers
            ModifyVelocity();
        }

        // Draws the player onscreen
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draws the player onscreen
            // Draw animation here

            // Temporary Draw Code
            spriteBatch.Draw(Level.BlankPixel, Box.Rect.CollisionRectangle, Color.Black);
        }

        /// <summary>
        /// Applies gravitational pull to the player.
        /// </summary>
        public void ApplyGravity()
        {
            Velocity.Y += GravitationalAcceleration;
        }

        /// <summary>
        /// Handles hit detection with the rotated rectangles in the ground.
        /// </summary>
        public void HandleGround()
        {
            for (int i = 0; i < Level.HORIZONTAL_GRID_SPACES; i++)
            {
                for (int j = 0; j < Level.VERTICAL_GRID_SPACES; j++)
                {
                    GridSlot gs = Level.Grid[i,j];

                    // Grab the GridPiece in that slot, if it exists
                    GridPiece g = gs.gPiece;

                    // Preform HitTests
                    if (g != null)
                    {
                        // Loop through each HitBox in this GridPiece
                        foreach (HitBox hb in g.HitBoxes)
                        {
                            // Check for a collision between HitBoxes
                            if (hb.HitTest(Box))
                            {
                                IsOnGround = true;

                                // Move the player out of the ground
                                while (hb.HitTest(Box))
                                {
                                    Move(0, -1);
                                }

                                // Move the player back into the ground, 1 pixel, so we remain on the ground next frame.
                                Move(0, 1);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Change the velocity, in a specific order, depending on interpretted variables.
        /// </summary>
        public void ModifyVelocity()
        {
            /// -----------------
            /// Vertical Movement
            /// -----------------

            // Applies gravitational pull to the player.
            ApplyGravity();

            // Check for Ground Hit Detection
            if (IsOnGround)
                Velocity.Y = 0;

            // Handle Jump Input
            HandleJumping();



            /// -------------------
            /// Horizontal Movement
            /// -------------------

            // Apply Drag to our Player
            if(!ControllerState.Down(Keys.Left) && ! ControllerState.Down(Keys.Right))
                Velocity.X *= HorizontalDrag;

            // Stop Horizontal Movement if we are hit
            if (IsHitLeft || IsHitRight)
                Velocity.X = 0;

            // Left and Right Input
            HandleHorizontalInput();

            // Limit Horizontal Velocity
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxHorizontalSpeed, MaxHorizontalSpeed);

        }

        /// <summary>
        /// Jump, if we are trying to and allowed.
        /// </summary>
        public void HandleJumping()
        {
            if ((ControllerState.Down(Keys.Space) || ControllerState.Down(Keys.Up)) && IsOnGround)
            {
                Velocity.Y = -JumpHeight;
            }
        }

        /// <summary>
        /// Move left and right if we are trying to
        /// </summary>
        public void HandleHorizontalInput()
        {
            // Right if we aren't IsHitRight
            if (ControllerState.Down(Keys.Right) && !IsHitRight)
            {
                Velocity.X += HorizontalAcceleration;
            }

            // Left if we aren't IsHitLeft
            if (ControllerState.Down(Keys.Left) && !IsHitLeft)
            {
                Velocity.X -= HorizontalAcceleration;
            }
        }

    }
}
