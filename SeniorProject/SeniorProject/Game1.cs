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
        private Player maSprite;            //the player sprite
        private SquareGuy1 squareGuy1;      //the enemy sprite
        private SquareGuy2 squareGuy2;      //another enemy sprite
        private List<NPC> allNPCs;          //a list of all the NPCs
        private Background background;      //the background
        private UserInterface bottomBar;    //the bottom bar
        public Camera2D camera;             //the camera object

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
            maSprite = new Player();            //the player
            squareGuy1 = new SquareGuy1();      //the square guy
            squareGuy2 = new SquareGuy2();      //another square guy
            allNPCs = new List<NPC>();

            camera = new Camera2D(graphics, maSprite.playerPosition);
            background = new Background();
            bottomBar = new UserInterface();

            base.Initialize();
        }

        //LOAD THINGS HERE
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            maSprite.LoadContent(this.Content);
            squareGuy1.LoadContent(this.Content);
            squareGuy2.LoadContent(this.Content);

            //make mah list of NPCs
            allNPCs.Add(squareGuy1);
            allNPCs.Add(squareGuy2);

            background.LoadContent(this.Content);
            bottomBar.LoadContent(this.Content);
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

            //call the appropriate update methods for each thing
            camera.Update(gameTime, maSprite.playerPosition);
            squareGuy1.Update(gameTime, maSprite);
            squareGuy2.Update(gameTime, maSprite);
            maSprite.Update(gameTime, allNPCs);
            bottomBar.Update(gameTime, maSprite, squareGuy1);

            base.Update(gameTime);
        }

        //DRAW THINGS HERE
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //remember - things are drawn in layers!  later things are drawn on top of earlier things

            spriteBatch.Begin();

            background.Draw(spriteBatch, camera);
            squareGuy1.Draw(spriteBatch, camera, maSprite);
            squareGuy2.Draw(spriteBatch, camera, maSprite);
            maSprite.Draw(spriteBatch, camera, allNPCs);
            bottomBar.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
