using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//much of the code for this class is based on the tutorials from http://www.xnadevelopment.com/tutorials.shtml

namespace SeniorProject
{
    class NPC
    {
        //NPCs deserve constants too
        private const int COLLISION_OFFSET = 5; //the number of pixels to shave off of the collision bounding boxes to make collision smoother
        private const int NPC_SPEED = 120;      //the movement speed of the NPC - I think this is pixels per second
        private const int AGGRO_RADIUS = 200;   //the aggro radius in pixels
        private const int INIT_X_POS = 1100;    //the initial x position
        private const int INIT_Y_POS = 500;     //the initial y position

        //class variables
        public Vector2 position = new Vector2(0, 0);    //The current position of the Sprite
        public float positionX = INIT_X_POS;            //the current X position of the sprite
        public float positionY = INIT_Y_POS;            //the current Y position of the sprite
        public Texture2D texture;   //The texture object used when drawing the sprite
        public int Width;           //the width of the sprite
        public int Height;          //the height of the sprite
        public Rectangle spriteRectangleTop;        //top collision box
        public Rectangle spriteRectangleBottom;     //bottom collision box
        public Rectangle spriteRectangleLeft;       //left collision box
        public Rectangle spriteRectangleRight;      //right collision box
        public Rectangle aggroBox;      //the aggro range of the NPC
        private float stateTimeBegin = 0;           //starts a time interval - used for patrolling
        private float stateTimeEnd = 0;             //ends a time interval - used for patrolling
        private Vector2 target = new Vector2(0, 0);     //the location of the target (which will be the player)
        private Boolean needReset = false;          //true if aggro'd on the player, false upon resetting
        private Vector2 sumMoved = new Vector2(0, 0);   //tracks distance moved - used in resetting

        //hp stuff
        public string nHPvalue;
        public int ncurrentHP = 90;  //should be used to store the player's current health
        public int nmaxHP;       //should be used to store the player's max health
        public Texture2D nhpbar;         //texture for hp
        private const string nHP_BAR = "HealthBar";

        //stores the current NPC state
        enum State
        {
            Idle,       //doing nothing
            Patrol,     //patrolling
            Aggro,      //has aggro on the player
            Reset       //in the process of resetting
        }
        State currentState = State.Patrol;      //default state is patrolling
        enum Patrol
        {
            Idle1,          //next is WalkingLeft
            Idle2,          //next is WalkingRight
            WalkingLeft,    //next is Idle2
            WalkingRight    //next is Idle1
        }
        Patrol patrolState = Patrol.Idle1;     //default state is Idle1

        //LOAD THINGS HERE
        //parameter assetName = the filename for the sprite image
        public void LoadContent(ContentManager theContentManager, string assetName)
        {
            texture = theContentManager.Load<Texture2D>(assetName);
            Width = texture.Width;
            Height = texture.Height;

            nhpbar = theContentManager.Load<Texture2D>(nHP_BAR);      //loads the hpbar texture
        }

        //UPDATE THINGS HERE
        public void Update(GameTime gameTime, Player otherSprite)
        {
            //the delta time - super important for updating movement (and possibly other things)
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //creates bounding boxes around the sprite - used for collisions
            spriteRectangleTop = new Rectangle((int)position.X + COLLISION_OFFSET, (int)position.Y, texture.Width - (COLLISION_OFFSET * 2), 1);
            spriteRectangleBottom = new Rectangle((int)position.X + COLLISION_OFFSET, (int)position.Y + texture.Height, texture.Width - (COLLISION_OFFSET * 2), 1);
            spriteRectangleLeft = new Rectangle((int)position.X, (int)position.Y + COLLISION_OFFSET, 1, texture.Height - (COLLISION_OFFSET * 2));
            spriteRectangleRight = new Rectangle((int)position.X + texture.Width, (int)position.Y + COLLISION_OFFSET, 1, texture.Height - (COLLISION_OFFSET * 2));

            //create aggro box
            aggroBox = new Rectangle((int)position.X - AGGRO_RADIUS, (int)position.Y - AGGRO_RADIUS, texture.Width + (2 * AGGRO_RADIUS), texture.Height + (2 * AGGRO_RADIUS));

            //determine the state of the NPC
            if (aggroBox.Intersects(otherSprite.spriteRectangle))
            {
                currentState = State.Aggro;
            }
            else
            {
                if (needReset == true)
                {
                    currentState = State.Reset;
                }
                if (needReset == false)
                {
                    currentState = State.Patrol;
                }
            }

            //make the NPC patrol left and right
            if (currentState == State.Patrol)
            {
                patrol(gameTime, delta, otherSprite);
            }
            //make the NPC aggro
            if (currentState == State.Aggro)
            {
                aggro(delta, otherSprite);
            }

            //make the NPC reset
            if (currentState == State.Reset)
            {
                reset(gameTime, delta, otherSprite);
            }

            //keeps this sprite in bounds of the world
            positionX = MathHelper.Clamp(positionX, 0, Background.WORLD_WIDTH - Width);
            positionY = MathHelper.Clamp(positionY, 0, Background.WORLD_HEIGHT - Height);

            #region hp stuff
            nmaxHP = 150;    //update the maxHP value
            //currentHP = 100;      //update the currentHP value
            nHPvalue = ncurrentHP + "/" + nmaxHP;      //"current/max"

            //may not need the clamp here
            ncurrentHP = (int)MathHelper.Clamp(ncurrentHP, 0, nmaxHP); //Keeps the health between 0 and maxHP
            #endregion
        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch spriteBatch, Camera2D camera)
        {
            position = new Vector2(positionX, positionY);
            position = camera.Transform(position);
            spriteBatch.Draw(texture, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            //draws the empty space for the health bar
            /*
            spriteBatch.Draw(nhpbar, new Vector2(positionX, positionY), new Rectangle((int)positionX, (int)positionY - 45, nhpbar.Width, 44),
                    Color.Red, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(nhpbar, new Rectangle((int)positionX / 2 - nhpbar.Width / 2,
                (int)positionY, nhpbar.Width, 44), new Rectangle(0, 45, nhpbar.Width, 44), Color.LightGray);
            //draws the current heatlh
            spriteBatch.Draw(nhpbar, new Rectangle((int)positionX / 2 - nhpbar.Width / 2,
                (int)positionY, (int)(nhpbar.Width * ((double)ncurrentHP / (double)nmaxHP)), 44),
                new Rectangle(0, 45, nhpbar.Width, 44), Color.Red);
            */
        }

        //make the NPC reset
        public void reset(GameTime gameTime, float delta, Player otherSprite)
        {
            Vector2 aggroDelta = new Vector2(0, 0);
            aggroDelta.X = -(sumMoved.X);
            aggroDelta.Y = -(sumMoved.Y);

            //when x and y are within 1 pixel of 0, reset sumMoved to 0
            if (((sumMoved.X < 1) && (sumMoved.X > -1) && (sumMoved.Y < 1) && (sumMoved.Y > -1)))
            {
                sumMoved.X = 0;
                sumMoved.Y = 0;
                needReset = false;
            }

            if (!((sumMoved.X == 0) && (sumMoved.Y == 0)))
            {
                aggroDelta.Normalize();
                positionX += aggroDelta.X * NPC_SPEED * delta;
                positionY += aggroDelta.Y * NPC_SPEED * delta;

                sumMoved.X += aggroDelta.X * NPC_SPEED * delta;
                sumMoved.Y += aggroDelta.Y * NPC_SPEED * delta;
            }
        }

        //make the NPC aggro
        public void aggro(float delta, Player otherSprite)
        {
            //this method was made with help from the tutorial http://www.xnaresources.com/pages.asp?pageid=48

            needReset = true;       //will reset if de-aggro'd
            target.X = otherSprite.position.X - otherSprite.Width;      //get position of target (player)
            target.Y = otherSprite.position.Y - otherSprite.Height;

            if (target != null)
            {
                // Get a vector pointing from the current location of the sprite to the destination.
                Vector2 aggroDelta = new Vector2(target.X - position.X, target.Y - position.Y);

                if (aggroDelta.Length() > 1)
                {
                    if (otherSprite.collision == false)
                    {
                        aggroDelta.Normalize();
                        positionX += aggroDelta.X * NPC_SPEED * delta;
                        positionY += aggroDelta.Y * NPC_SPEED * delta;

                        sumMoved.X += aggroDelta.X * NPC_SPEED * delta;
                        sumMoved.Y += aggroDelta.Y * NPC_SPEED * delta;
                    }
                }
            }
        }

        //make the NPC patrol left and right
        public void patrol(GameTime gameTime, float delta, Player otherSprite)
        {
            stateTimeEnd += (float)gameTime.ElapsedGameTime.TotalSeconds;   //tracks time elapsed
            if (patrolState == Patrol.Idle1)
            {
                if (stateTimeBegin == 0)
                {
                    stateTimeBegin = (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if ((stateTimeEnd - stateTimeBegin) > 5)
                {
                    patrolState = Patrol.WalkingLeft;
                    stateTimeEnd = 0;
                    stateTimeBegin = 0;
                }
            }
            if (patrolState == Patrol.Idle2)
            {
                if (stateTimeBegin == 0)
                {
                    stateTimeBegin = (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if ((stateTimeEnd - stateTimeBegin) > 5)
                {
                    patrolState = Patrol.WalkingRight;
                    stateTimeEnd = 0;
                    stateTimeBegin = 0;
                }
            }
            if (patrolState == Patrol.WalkingLeft)
            {
                if (stateTimeBegin == 0)
                {
                    stateTimeBegin = (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (((stateTimeEnd - stateTimeBegin) < 5) && (otherSprite.collision == false))
                {
                    //move left
                    positionX -= NPC_SPEED * delta;
                }
                if ((stateTimeEnd - stateTimeBegin) > 5)
                {
                    patrolState = Patrol.Idle2;
                    stateTimeEnd = 0;
                    stateTimeBegin = 0;
                }
            }
            if (patrolState == Patrol.WalkingRight)
            {
                if (stateTimeBegin == 0)
                {
                    stateTimeBegin = (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (((stateTimeEnd - stateTimeBegin) < 5) && (otherSprite.collision == false))
                {
                    //move right
                    positionX += NPC_SPEED * delta;
                }
                if ((stateTimeEnd - stateTimeBegin) > 5)
                {
                    patrolState = Patrol.Idle1;
                    stateTimeEnd = 0;
                    stateTimeBegin = 0;
                }
            }
        }
    }
}
