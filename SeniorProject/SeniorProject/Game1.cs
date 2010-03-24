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

namespace SeniorProject
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //variables
        private Player maSprite;        //the player sprite
        private NPC squareGuySprite;    //the enemy sprite
        private Background background;  //the background
        public Camera2D camera;    //the camera object

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //sets the window size
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        //INITIALIZE THINGS HERE
        protected override void Initialize()
        {
            maSprite = new Player();    //the player
            squareGuySprite = new NPC();    //the square guy
            camera = new Camera2D(graphics, maSprite.playerPosition);    //the camera
            background = new Background();  //the background

            base.Initialize();
        }

        //LOAD THINGS HERE
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            maSprite.LoadContent(this.Content);                             //load diddy kong sprite
            squareGuySprite.LoadContent(this.Content, "SquareGuy");         //load square guy sprite
            background.LoadContent(this.Content);   //load background
        }

        // UnloadContent will be called once per game and is the place to unload all content.
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //UPDATE THINGS HERE
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //the delta time - super important for updating movement
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //call the appropriate update methods for each thing
            camera.Update(gameTime, maSprite.playerPosition);
            squareGuySprite.Update(gameTime, maSprite);
            maSprite.Update(gameTime, squareGuySprite);

            base.Update(gameTime);
        }

        //DRAW THINGS HERE
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //remember - things are drawn in layers!  later things are drawn on top of earlier things
            //note - scaling sprites won't scale my collision boxes

            spriteBatch.Begin();

            background.Draw(spriteBatch, camera);       //draw mah new background
            squareGuySprite.Draw(spriteBatch, camera);  //draw the square guy
            maSprite.Draw(spriteBatch, camera);         //draw the player

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
