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
        //constants
        private const string PLAYER_ASSETNAME = "dawn";         //the image file to use for the sprite
        private const string WALKING_TEXTURE = "dawnwalking";
        private const int PLAYER_START_X = 600;                 //the X coordinate of the vector to spawn the player
        private const int PLAYER_START_Y = 700;                 //the Y coordinate of the vector to spawn the player
        private const float PLAYER_SPEED = 200.0f;              //the speed of the player

        //you're a variable
        public Vector2 position = new Vector2(0, 0);    //The current position of the Sprite
        public Texture2D texture;   //The texture object used when drawing the sprite
        public int Width;           //the width of the sprite
        public int Height;          //the height of the sprite
        public Vector2 playerPosition = Vector2.Zero;   //current position of the player via updates
        public Rectangle playerSourceRectangle;
        public Rectangle spriteRectangle;           //a rectangle around the sprite, used for collisions
        public Boolean collision;       //true if there is any collision
        private Boolean collisionTop = false;        //true if there is collision moving up
        private Boolean collisionBottom = false;     //true if there is collision moving down
        private Boolean collisionLeft = false;       //true if there is collision moving left
        private Boolean collisionRight = false;      //true if there is collision moving right
        private KeyboardState keyboardState;
        private Texture2D walkingTexture;
        private Rectangle destinationRect;
        private Rectangle sourceRect;
        private int walkingWidth = 40;
        private int walkingHeight = 54;
        float timer = 0f;
        float interval = 1000f / 25f;   //25 frames per second
        int frameCount = 4;
        int currentFrame = 0;

        //stores the current player state
        enum State
        {
            Walking,
            Idle
        }
        State currentState = State.Idle;     //default state is walking

        //LOAD CONTENT HERE
        public void LoadContent(ContentManager theContentManager)
        {
            texture = theContentManager.Load<Texture2D>(PLAYER_ASSETNAME);
            walkingTexture = theContentManager.Load<Texture2D>(WALKING_TEXTURE);
            Width = texture.Width;
            Height = texture.Height;
            playerPosition = new Vector2(PLAYER_START_X, PLAYER_START_Y); //location to spawn the player
            destinationRect = new Rectangle(0, 0, walkingWidth, walkingHeight);
        }

        //UPDATE THINGS HERE
        public void Update(GameTime gameTime, NPC otherSprite)
        {
            //the delta time - super important for updating movement (and possibly other things)
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            movement(delta);    //this allows the arrow keys to move the player

            //things related to collision
            spriteRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            collisionCheck(otherSprite);    //checks for collisions

            //keep the player in bounds of the world
            playerPosition.X = MathHelper.Clamp(playerPosition.X, (Width / 2), Background.WORLD_WIDTH - (Width / 2));
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, (Height / 2), Background.WORLD_HEIGHT - (Height / 2));

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
                spriteBatch.Draw(texture, position, playerSourceRectangle, Color.White, 0.0f,
                    Vector2.Zero, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
            }
            if (currentState == State.Walking)
            {
                spriteBatch.Draw(walkingTexture, position, sourceRect, Color.White);
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
            sourceRect = new Rectangle(currentFrame * walkingWidth, 0, walkingWidth, walkingHeight);
        }

        //this allows the arrow keys to move the player
        public void movement(float delta)
        {
            if (currentState == State.Walking)
            {
                keyboardState = Keyboard.GetState();
                if ((keyboardState.IsKeyDown(Keys.Right)) && (collisionRight == false))
                {
                    playerPosition.X += PLAYER_SPEED * delta;
                }
                if ((keyboardState.IsKeyDown(Keys.Left)) && (collisionLeft == false))
                {
                    playerPosition.X -= PLAYER_SPEED * delta;
                }
                if ((keyboardState.IsKeyDown(Keys.Down)) && (collisionBottom == false))
                {
                    playerPosition.Y += PLAYER_SPEED * delta;
                }
                if ((keyboardState.IsKeyDown(Keys.Up)) && (collisionTop == false))
                {
                    playerPosition.Y -= PLAYER_SPEED * delta;
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
