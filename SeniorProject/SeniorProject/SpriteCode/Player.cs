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
        private const string WALKING_TEXTURE = "dawnwalking";   //the image file for walking animation - credit to Serg!o from WAH
        private const int PLAYER_START_X = 600;                 //the X coordinate of the vector to spawn the player
        private const int PLAYER_START_Y = 700;                 //the Y coordinate of the vector to spawn the player
        private const float PLAYER_SPEED = 200.0f;              //the speed of the player
        private const int WALKING_WIDTH = 40;       //the width of each frame of the walking animation
        private const int WALKING_HEIGHT = 54;      //the height of each frame of the walking animation
        private const int frameCount = 4;           //the number of frames in the sprite sheet for walking
        private const int frameCountAttack = 5;
        private const int ATTACK_WIDTH = 50;       //the width of each frame of the 
        private const int ATTACK_HEIGHT = 45;      //the height of each frame of the 
        private const int RANGE = 50;      //the attack range in pixels
        private const string ATTACK_IMAGE = "sword3";    //the image file for the attack

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
        public Boolean collisionTop = false;       //true if there is collision moving up
        public Boolean collisionBottom = false;    //true if there is collision moving down
        public Boolean collisionLeft = false;      //true if there is collision moving left
        public Boolean collisionRight = false;     //true if there is collision moving right
        private KeyboardState keyboardState;        //used for keyboard input
        private Rectangle sourceRect;               //used for walking sprite sheet
        private float timer = 0f;                   //used for walking animation
        private float interval = 1000f / 5f;        //5 frames per second for walking animation
        private int currentFrame = 0;               //keeps track of the current frame in the walking animation
        private Rectangle attackRect;                   //used to animate the attack
        private float timerAttack = 0f;                 //used for animating attack
        private float intervalAttack = 1000f / 10f;     //10 frames per second for animating attack
        private int currentFrameAttack = 0;             //keeps track of the current frame in the attack animation
        public Vector2 origin = new Vector2(0, 0);      //makes the sprite rotate properly
        private Boolean action1 = false;        //true if action1 is enabled
        private int actionChecker = 0;          //used for toggling auto attack
        public Texture2D textureAttack;         //The texture object used when drawing the sprite
        private float cooldownEnd = 3.0f;       //attack cooldown - in seconds
        private float cooldownStart = 0.0f;     //tracks cooldown
        private Rectangle hitBox;               //the attack's actual hit box
        private Vector2 hitBoxVector = new Vector2(0, 0);       //needed for drawing
        private float npcFraction;              //1 divided by the number of NPCs
        private int autoAttackDamage = 20;      //damage of the auto attack
        public int maxHP = 150;             //the player's max hp
        public int currentHP = 140;         //the player's current hp

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
            textureAttack = theContentManager.Load<Texture2D>(ATTACK_IMAGE);
            Width = texture.Width;
            Height = texture.Height;
            playerPosition = new Vector2(PLAYER_START_X, PLAYER_START_Y); //location to spawn the player

            origin.X = texture.Width / 2;
            origin.Y = texture.Height / 2;
        }

        //UPDATE THINGS HERE
        public void Update(GameTime gameTime, List<NPC> allNPCs)
        {
            //the delta time - super important for updating movement (and possibly other things)
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //things related to collision
            collisionBottom = false;
            collisionLeft = false;
            collisionRight = false;
            collisionTop = false;
            collision = false;
            spriteRectangle = new Rectangle((int)(position.X - (texture.Width / 2)), (int)(position.Y - (texture.Height / 2)), texture.Width, texture.Height);
            foreach (NPC otherSprite in allNPCs)
            {
                otherSprite.collisionCheck(this);    //checks for collisions
                if (otherSprite.collisionBottom == true)
                {
                    collisionBottom = true;
                }
                if (otherSprite.collisionTop == true)
                {
                    collisionTop = true;
                }
                if (otherSprite.collisionRight == true)
                {
                    collisionRight = true;
                }
                if (otherSprite.collisionLeft == true)
                {
                    collisionLeft = true;
                }
                if ((collisionTop == true) || (collisionBottom == true) || (collisionLeft == true) || (collisionRight == true))
                {
                    collision = true;
                }
            }

            npcFraction = (1.0f / allNPCs.Count());
            foreach (NPC otherSprite in allNPCs)
            {
                movement(delta, otherSprite);    //this allows the arrow keys to move the player
            }

            //keep the player in bounds of the world
            playerPosition.X = MathHelper.Clamp(playerPosition.X, (Width), Background.WORLD_WIDTH);
            playerPosition.Y = MathHelper.Clamp(playerPosition.Y, (Height), Background.WORLD_HEIGHT);

            //enable walking animation
            if (currentState == State.Walking)
            {
                walking(gameTime);
            }

            //check for action 1 and do it
            Action1(gameTime, allNPCs);
        }

        //DRAW THINGS HERE
        public void Draw(SpriteBatch spriteBatch, Camera2D camera)
        {
            position = camera.Transform(playerPosition);
            playerSourceRectangle = new Rectangle(0, 0, Width, Height);

            //the next 2 lines make it so that the player is centered instead of the top left corner of the sprite being in the center
            position.X = position.X - (Width / 2);
            position.Y = position.Y - (Height / 2);

            if (currentState == State.Idle)     //draw the idle player
            {
                spriteBatch.Draw(texture, position, playerSourceRectangle, Color.White, (-1) * facing,
                    origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
            }
            if (currentState == State.Walking)  //draw the walking player
            {
                spriteBatch.Draw(walkingTexture, position, sourceRect, Color.White, (-1) * facing,
                    origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
            }

            if (action1 == true && (cooldownStart > cooldownEnd))     //draw the animation for action 1 - auto attacking
            {
                spriteBatch.Draw(textureAttack, hitBox, attackRect, Color.White);
            }
        }

        //check for action 1 and do it - action 1 will probably be reserved for auto attacking as it needs to be toggled
        public void Action1(GameTime gameTime, List<NPC> allNPCs)
        {
            keyboardState = Keyboard.GetState();
            //these if statements are used to properly toggle the action on and off
            if (keyboardState.IsKeyDown(Keys.D1) && (actionChecker == 0))
            {
                actionChecker = 1;
            }
            if (keyboardState.IsKeyUp(Keys.D1) && (actionChecker == 1))
            {
                actionChecker = 2;
            }
            if (keyboardState.IsKeyDown(Keys.D1) && (actionChecker == 2))
            {
                actionChecker = 3;
            }
            if (keyboardState.IsKeyUp(Keys.D1) && (actionChecker == 3))
            {
                actionChecker = 0;
            }
            if (actionChecker == 1 || (actionChecker == 2))     //if auto attacking is currently enabled
            {
                action1 = true;

                //create the hit box
                hitBox = new Rectangle(((int)position.X - (RANGE / 2) - (int)(((Width + RANGE) / 2) * (float)Math.Sin(facing))), (int)position.Y - (RANGE / 2) - (int)((((Height / 2) + RANGE) / 2) * (float)Math.Cos(facing)), RANGE, RANGE);

                if (action1 == true)    //start the attack cooldown when the button is pressed
                {
                    cooldownStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (cooldownStart > cooldownEnd)     //attack cooldown is up
                {
                    timerAttack += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (timerAttack > intervalAttack)
                    {
                        //the NPC is in the hitbox
                        foreach (NPC otherSprite in allNPCs)
                        {
                            if ((hitBox.Intersects(otherSprite.spriteRectangleBottom) == true) || (hitBox.Intersects(otherSprite.spriteRectangleTop) == true) || (hitBox.Intersects(otherSprite.spriteRectangleRight) == true) || (hitBox.Intersects(otherSprite.spriteRectangleLeft) == true))
                            {
                                if (currentFrameAttack == 2)     //the middle of the animation
                                {
                                    otherSprite.currentHP -= autoAttackDamage;
                                }
                            }
                        }
                        //make the animation
                        currentFrameAttack++;
                        if (currentFrameAttack > frameCountAttack - 1)
                        {
                            currentFrameAttack = 0;
                            cooldownStart = 0;
                        }
                        timerAttack = 0f;
                    }
                    attackRect = new Rectangle(currentFrameAttack * ATTACK_WIDTH, 0, ATTACK_WIDTH, ATTACK_HEIGHT);
                }
            }

            if (actionChecker != 1 && (actionChecker != 2))
            {
                action1 = false;
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
                facing -= turnSpeed * npcFraction;
            }
            //LOOK LEFT
            if (keyboardState.IsKeyDown(Keys.A))
            {
                facing += turnSpeed * npcFraction;
            }

            //STRAFE RIGHT:
            if (keyboardState.IsKeyDown(Keys.E))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing - ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing - ((float)Math.PI / 2.0f))) * delta * npcFraction;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing - ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing - ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing - ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing - ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
            }

            //STRAFE LEFT:
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing + ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing + ((float)Math.PI / 2.0f))) * delta * npcFraction;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing + ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing + ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) > 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing + ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing + ((float)Math.PI / 2.0f)) < 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing + ((float)Math.PI / 2.0f))) * delta * npcFraction;
                    }
                }
            }

            //MOVE DOWN:
            if (keyboardState.IsKeyDown(Keys.S))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X += (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta * npcFraction;
                    playerPosition.Y += (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta * npcFraction;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing) < 0)
                    {
                        playerPosition.Y += (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta * npcFraction;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing) > 0)
                    {
                        playerPosition.Y += (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta * npcFraction;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing) < 0)
                    {
                        playerPosition.X += (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta * npcFraction;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing) > 0)
                    {
                        playerPosition.X += (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta * npcFraction;
                    }
                }
            }
            //MOVE FORWARD:
            if (keyboardState.IsKeyDown(Keys.W))
            {
                currentState = State.Walking;
                if (collision == false)
                {
                    playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta * npcFraction;
                    playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta * npcFraction;
                }
                if (collisionBottom == true)
                {
                    if ((float)Math.Cos(facing) > 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta * npcFraction;
                    }
                }
                if (collisionTop == true)
                {
                    if ((float)Math.Cos(facing) < 0)
                    {
                        playerPosition.Y -= (float)(PLAYER_SPEED * (float)Math.Cos(facing)) * delta * npcFraction;
                    }
                }
                if (collisionRight == true)
                {
                    if ((float)Math.Sin(facing) > 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta * npcFraction;
                    }
                }
                if (collisionLeft == true)
                {
                    if ((float)Math.Sin(facing) < 0)
                    {
                        playerPosition.X -= (float)(PLAYER_SPEED * (float)Math.Sin(facing)) * delta * npcFraction;
                    }
                }
            }
        }
    }
}
