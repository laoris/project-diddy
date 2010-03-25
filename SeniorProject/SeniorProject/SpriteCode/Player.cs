using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//much of the code for this class is based on the tutorials from http://www.xnadevelopment.com/tutorials.shtml

namespace SeniorProject
{
    class Player
    {
        #region Variables
        //constants
        private const string PLAYER_ASSETNAME = "dawn";   //the image file to use for the sprite
        private const string WALKING_TEXTURE = "dawnwalking";   //the image file for walking animation
        private const int PLAYER_START_X = 600;                 //the X coordinate of the vector to spawn the player
        private const int PLAYER_START_Y = 700;                 //the Y coordinate of the vector to spawn the player
        private const float PLAYER_SPEED = 200.0f;              //the speed of the player
        private const int WALKING_WIDTH = 40;       //the width of each frame of the walking animation
        private const int WALKING_HEIGHT = 54;      //the height of each frame of the walking animation
        private const int frameCount = 4;           //the number of frames in the sprite sheet for walking

        //you're a variable
        public Vector2 position = new Vector2(0, 0);    //The current position of the Sprite
        public Texture2D texture;   //The texture object used when drawing the sprite
        private Texture2D walkingTexture;   //the sprite sheet used for the walking animation - see load method
        public int Width;           //the width of the sprite
        public int Height;          //the height of the sprite
        public Vector2 playerPosition = Vector2.Zero;   //current position of the player via updates
        public Rectangle playerSourceRectangle;
        public Rectangle spriteRectangle;           //a rectangle around the sprite, used for collisions
        public Boolean collision;       //true if there is any collision
        private Boolean collisionTop = false;       //true if there is collision moving up
        private Boolean collisionBottom = false;    //true if there is collision moving down
        private Boolean collisionLeft = false;      //true if there is collision moving left
        private Boolean collisionRight = false;     //true if there is collision moving right
        private KeyboardState keyboardState;        //used for keyboard input
        private Rectangle sourceRect;               //used for walking sprite sheet
        private float timer = 0f;                   //used for walking animation
        private float interval = 1000f / 5f;        //5 frames per second for walking animation
        private int currentFrame = 0;               //keeps track of the current frame in the walking animation
        public Vector2 origin = new Vector2(0, 0);      //makes the sprite rotate properly

        //some variables for turning
        private float facing = 0; // angle that represents (in bearing) the facing of the unit
        private const float turnSpeed = ((float)Math.PI / 180.0f) * 4;

        #endregion
        
        //stores the current player state
        enum State
        {
            Walking,    //the player is walking
            Idle        //the player is idle
        }
        State currentState = State.Idle;     //default state is Idle

        //LOAD CONTENT HERE
        public void LoadContent(ContentManager theContentManager)
        {
            texture = theContentManager.Load<Texture2D>(PLAYER_ASSETNAME);
            walkingTexture = theContentManager.Load<Texture2D>(WALKING_TEXTURE);
            Width = texture.Width;
            Height = texture.Height;
            playerPosition = new Vector2(PLAYER_START_X, PLAYER_START_Y); //location to spawn the player

            origin.X = texture.Width / 2;
            origin.Y = texture.Height / 2;
        }

        //UPDATE THINGS HERE
        public void Update(GameTime gameTime, NPC otherSprite)
        {
            //the delta time - super important for updating movement (and possibly other things)
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            movement(delta, otherSprite);    //this allows the arrow keys to move the player

            //things related to collision
            spriteRectangle = new Rectangle((int)(position.X - (texture.Width / 2)), (int)(position.Y - (texture.Height / 2)), texture.Width, texture.Height);
            collisionCheck(otherSprite);    //checks for collisions

            //keep the player in bounds of the world
            playerPosition.X = MathHelper.Clamp(playerPosition.X, (Width), Background.WORLD_WIDTH - (Width));
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, (Height), Background.WORLD_HEIGHT - (Height));

            //enable walking animation
            if (currentState == State.Walking)
            {
                walking(gameTime);
            }
        }

        //DRAW THINGS HERE
        public void Draw(SpriteBatch spriteBatch, Camera2D camera)
        {
            position = camera.Transform(playerPosition);
            playerSourceRectangle = new Rectangle(0, 0, Width, Height);

            //the next 2 lines make it so that the player is centered instead of the top left corner of the sprite being in the center
            position.X = position.X - (Width / 2);
            position.Y = position.Y - (Height / 2);

            if (currentState == State.Idle)
            {
                spriteBatch.Draw(texture, position, playerSourceRectangle, Color.White, (-1) * facing,
                    origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
            }
            if (currentState == State.Walking)
            {
                spriteBatch.Draw(walkingTexture, position, sourceRect, Color.White, (-1) * facing,
                    origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
            }
        }

        //animate walking
        public void walking(GameTime gameTime)
        {
            //this method was created with help from the tutorial http://www.xnafusion.com/xna-animated-sprite-tutorial-beginner/

            timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > interval)
            {
                currentFrame++;
                if (currentFrame > frameCount - 1)
                {
                    currentFrame = 0;
                }
                timer = 0f;
            }
            sourceRect = new Rectangle(currentFrame * WALKING_WIDTH, 0, WALKING_WIDTH, WALKING_HEIGHT);
        }

        //this allows the keys to move the player
        public void movement(float delta, NPC otherSprite)
        {
            keyboardState = Keyboard.GetState();
            if ((keyboardState.IsKeyUp(Keys.W)) && (keyboardState.IsKeyUp(Keys.S)) && (keyboardState.IsKeyUp(Keys.E)) && (keyboardState.IsKeyUp(Keys.Q)))
            {
                currentState = State.Idle;
            }

            //LOOK RIGHT:
            if (keyboardState.IsKeyDown(Keys.D))
            {
                facing -= turnSpeed;
                //playerPosition.X += PLAYER_SPEED * delta;
            }
            //LOOK LEFT
            if (keyboardState.IsKeyDown(Keys.A))
            {
                facing += turnSpeed;
                //playerPosition.X -= PLAYER_SPEED * delta;
            }

            //STRAFE RIGHT:
            if (keyboardState.IsKeyDown(Keys.E))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing - ((float)Math.PI / 2.0f))) * delta;
                    playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing - ((float)Math.PI / 2.0f))) * delta;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing - ((float)Math.PI / 2.0f))) * delta;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing - ((float)Math.PI / 2.0f))) * delta;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing - ((float)Math.PI / 2.0f))) * delta;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing - ((float)Math.PI / 2.0f))) * delta;
                    }
                }
            }

            //STRAFE LEFT:
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing + ((float)Math.PI / 2.0f))) * delta;
                    playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing + ((float)Math.PI / 2.0f))) * delta;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing + ((float)Math.PI / 2.0f))) * delta;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing + ((float)Math.PI / 2.0f))) * delta;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing + ((float)Math.PI / 2.0f))) * delta;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing + ((float)Math.PI / 2.0f))) * delta;
                    }
                }
            }

            //MOVE DOWN:
            if (keyboardState.IsKeyDown(Keys.S))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X += (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta;
                    playerPosition.Y += (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing) < 0)
                    {
                    playerPosition.Y += (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing) > 0)
                    {
                        playerPosition.Y += (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing) < 0)
                    {
                        playerPosition.X += (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing) > 0)
                    {
                        playerPosition.X += (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta;
                    }
                }
            }
            //MOVE FORWARD:
            if (keyboardState.IsKeyDown(Keys.W))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta;
                    playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing) > 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing) < 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing) > 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing) < 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta;
                    }
                }
            }
        }

        //checks if the player is colliding with something
        public void collisionCheck(NPC otherSprite)
        {
            if (spriteRectangle.Intersects(otherSprite.spriteRectangleTop))
            {
                collisionBottom = true;
            }
            else
            {
                collisionBottom = false;
            }
            if (spriteRectangle.Intersects(otherSprite.spriteRectangleBottom))
            {
                collisionTop = true;
            }
            else
            {
                collisionTop = false;
            }
            if (spriteRectangle.Intersects(otherSprite.spriteRectangleLeft))
            {
                collisionRight = true;
            }
            else
            {
                collisionRight = false;
            }
            if (spriteRectangle.Intersects(otherSprite.spriteRectangleRight))
            {
                collisionLeft = true;
            }
            else
            {
                collisionLeft = false;
            }
            if ((collisionTop == true) || (collisionBottom == true) || (collisionLeft == true) || (collisionRight == true))
            {
                collision = true;
            }
            else
            {
                collision = false;
            }
        }
    }
}
