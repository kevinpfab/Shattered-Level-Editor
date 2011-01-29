using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Shattered
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShatteredGame : Microsoft.Xna.Framework.Game
    {
        // Game Instance Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Custom Enums
        /// <summary>
        /// One of four directions we can move in. Starts from North = 0, and moves clockwise.
        /// </summary>
        public enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }
        #endregion

        #region Custom Instance Variables
        public Level l;

        public Editor e;

        public ControllerState cs;
        #endregion

        // A throwback reference to our game, because we will only have one initiated at one time
        public static ShatteredGame Global;

        public ShatteredGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";

            Global = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            #region TESTING
            l = new Level(Content);
            e = new Editor(Content, l);
            #endregion

            // Our controllers
            cs = new ControllerState();

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Grabs input for our input devices
            cs.Update(gameTime);

            #region Testing
            if (e.IsActive)
                l.UpdateEditor(gameTime);
            else
                l.Update(gameTime);

            e.Update(gameTime);
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Testing
            spriteBatch.Begin();

            l.Draw(spriteBatch, gameTime);

            e.Draw(spriteBatch, gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
