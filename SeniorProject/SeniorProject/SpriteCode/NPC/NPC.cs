using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//some of the code for this class is based on the tutorials from http://www.xnadevelopment.com/tutorials.shtml

namespace SeniorProject
{
    partial class NPC
    {
        //constants
        private const string HP_BAR = "HealthBar";      //the image file for the HP bar

        //variables passed in
        private int COLLISION_OFFSET;   //the number of pixels to shave off of the collision bounding boxes to make collision smoother
        private int NPC_SPEED;          //the movement speed of the NPC - I think this is pixels per second
        private int AGGRO_RADIUS; //the aggro radius in pixels
        private int INIT_X_POS;   //the initial x position
        private int INIT_Y_POS;   //the initial y position
        private string IMAGE_NAME;      //the image file for the sprite
        public int MAX_HP;              //the dudes max hp
        public int MAX_SPIRIT;
        private int RESPAWN_TIME;       //the respawn time in seconds
        private int ATTACK_RANGE;        //NPC attack range
        private float ATTACK_COOLDOWN;      //NPC attack cooldown
        private float STRENGTH;         //NPC strength
        private int EXPERIENCE;
        private Boolean SPIRIT_ATTACK;

        //class variables
        public Vector2 position = new Vector2(0, 0);    //The current position of the Sprite
        private float positionX;            //the current X position of the sprite
        private float positionY;            //the current Y position of the sprite
        public Texture2D texture;   //The texture object used when drawing the sprite
        public int Width;           //the width of the sprite
        public int Height;          //the height of the sprite
        public Rectangle spriteRectangleTop;        //top collision box
        public Rectangle spriteRectangleBottom;     //bottom collision box
        public Rectangle spriteRectangleLeft;       //left collision box
        public Rectangle spriteRectangleRight;      //right collision box
        public Rectangle spriteRectangle;           //box over the sprite
        public Rectangle aggroBox;      //the aggro range of the NPC
        private float stateTimeBegin = 0;           //starts a time interval - used for patrolling
        private float stateTimeEnd = 0;             //ends a time interval - used for patrolling
        private Vector2 target = new Vector2(0, 0);     //the location of the target (which will be the player)
        private Boolean needReset = false;          //true if aggro'd on the player, false upon resetting
        private Vector2 sumMoved = new Vector2(0, 0);   //tracks distance moved - used in resetting
        public Boolean collision;       //true if there is any collision
        public Boolean collisionTop = false;       //true if there is collision moving up
        public Boolean collisionBottom = false;    //true if there is collision moving down
        public Boolean collisionLeft = false;      //true if there is collision moving left
        public Boolean collisionRight = false;     //true if there is collision moving right
        private float respawnTimeStart = 0;         //tracks the respawn time
        private Texture2D hpBar;        //texture for HP bar
        private string hpValue;         //for displaying the hp text
        public Color[] npcTextureData;
        public Boolean alreadyHit = false;
        public Boolean alreadyHitVortex = false;
        public Boolean grantedEXP = false;
        private Loot mahNewLoot = new Loot();

        //stores the current NPC state
        enum State
        {
            Idle,       //doing nothing
            Patrol,     //patrolling
            Aggro,      //has aggro on the player
            Reset,      //in the process of resetting
            Dead        //poor guy
        }
        State currentState = State.Idle;      //default state is patrolling
        enum Patrol
        {
            Idle1,          //next is WalkingLeft
            Idle2,          //next is WalkingRight
            WalkingLeft,    //next is Idle2
            WalkingRight    //next is Idle1
        }
        Patrol patrolState = Patrol.Idle1;     //default state is Idle1

        //the constructor for NPCs - this is where all those stats for the particular guy are passed in
        public NPC(int collisionOffset, int npcSpeed, int aggroRadius, int initX, int initY, string imageName, int hp, int respawnTime, int attackRange, float attackCooldown, float strength, int maxSpirit, int experience, Boolean spiritAttack)
        {
            COLLISION_OFFSET = collisionOffset;
            NPC_SPEED = npcSpeed;
            AGGRO_RADIUS = aggroRadius;
            INIT_X_POS = initX;
            INIT_Y_POS = initY;
            IMAGE_NAME = imageName;
            MAX_HP = hp;
            currentHP = MAX_HP;
            MAX_SPIRIT = maxSpirit;
            currentSpirit = MAX_SPIRIT;
            RESPAWN_TIME = respawnTime;
            ATTACK_RANGE = attackRange;
            ATTACK_COOLDOWN = attackCooldown;
            STRENGTH = strength;
            EXPERIENCE = experience;
            SPIRIT_ATTACK = spiritAttack;

            positionX = INIT_X_POS;
            positionY = INIT_Y_POS;
        }

        //LOAD THINGS HERE
        public void LoadContent(ContentManager theContentManager)
        {
            texture = theContentManager.Load<Texture2D>(IMAGE_NAME);
            attackTexture = theContentManager.Load<Texture2D>(ATTACK_TEXTURE);
            hpBar = theContentManager.Load<Texture2D>(HP_BAR);
            Width = texture.Width;
            Height = texture.Height;
            npcTextureData = new Color[texture.Width * texture.Height];
            texture.GetData(npcTextureData);
            mahNewLoot.LootLoad(theContentManager);
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
            spriteRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            //create aggro box
            aggroBox = new Rectangle((int)position.X - AGGRO_RADIUS, (int)position.Y - AGGRO_RADIUS, texture.Width + (2 * AGGRO_RADIUS), texture.Height + (2 * AGGRO_RADIUS));

            AggroCheck(delta, otherSprite);

            //determine the state of the NPC
            if ((currentHP == 0) || (currentHP < 0) || (currentSpirit == 0) || (currentSpirit < 0))
            {
                currentState = State.Dead;
            }
            else if (aggroCheck == true)
            {
                currentState = State.Aggro;
            }
            else if (needReset == true)
            {
                currentState = State.Reset;
            }
            else if (needReset == false)
            {
                currentState = State.Patrol;
            }

            //do the appropriate thing
            if (currentState == State.Dead)
            {
                dead(delta, otherSprite);
            }
            if (currentState == State.Patrol)
            {
                patrol(gameTime, delta, otherSprite);
            }
            if (currentState == State.Aggro)    //make the NPC aggro
            {
                aggro(delta, otherSprite);
                autoAttack(otherSprite, gameTime);
            }
            if (currentState == State.Reset)    //make the NPC reset
            {
                reset(gameTime, delta, otherSprite);
            }

            //keeps this sprite in bounds of the world
            positionX = MathHelper.Clamp(positionX, 0, Background.WORLD_WIDTH - Width);
            positionY = MathHelper.Clamp(positionY, 0, Background.WORLD_HEIGHT - Height);

            HealthRegen(delta);
            SpiritRegen(delta);

            #region hp stuff
            hpValue = currentHP + "/" + MAX_HP;      //"current/max"

            //may not need the clamp here
            currentHP = (int)MathHelper.Clamp(currentHP, 0, MAX_HP);    //Keeps the health between 0 and maxHP
            #endregion
        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch spriteBatch, Camera2D camera, Player maSprite)
        {
            position = new Vector2(positionX, positionY);
            position = camera.Transform(position);
            if (currentState != State.Dead)
            {
                spriteBatch.Draw(texture, position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }

            //mahNewLoot.DrawLoot(spriteBatch, camera);

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

        //if the NPC is dead...
        public void dead(float delta, Player otherSprite)
        {
            damageDisplay = false;

            //mahNewLoot.LootUpdate(delta);

            if (grantedEXP == false)
            {
                otherSprite.exp += EXPERIENCE;
                grantedEXP = true;
            }

            respawnTimeStart += delta;      //start respawn timer
            if (respawnTimeStart > RESPAWN_TIME)    //respawn time is up
            {
                positionX = INIT_X_POS;
                positionY = INIT_Y_POS;

                //reset things
                respawnTimeStart = 0;
                currentHP = MAX_HP;
                currentSpirit = MAX_SPIRIT;
                sumMoved.X = 0;
                sumMoved.Y = 0;
                needReset = false;
                currentState = State.Patrol;
                damageDisplay = false;
                grantedEXP = false;
                mahNewLoot.spawnedLoot = false;
            }
        }

        //checks if the player is colliding with something
        public void collisionCheck(Player otherSprite)
        {
            if (currentState == State.Dead)     //don't cause collisions if dead
            {
                collision = false;
                collisionBottom = false;
                collisionLeft = false;
                collisionRight = false;
                collisionTop = false;
                return;
            }
            if (spriteRectangleTop.Intersects(otherSprite.spriteRectangle))
            {
                collisionBottom = true;
            }
            else
            {
                collisionBottom = false;
            }
            if (spriteRectangleBottom.Intersects(otherSprite.spriteRectangle))
            {
                collisionTop = true;
            }
            else
            {
                collisionTop = false;
            }
            if (spriteRectangleLeft.Intersects(otherSprite.spriteRectangle))
            {
                collisionRight = true;
            }
            else
            {
                collisionRight = false;
            }
            if (spriteRectangleRight.Intersects(otherSprite.spriteRectangle))
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
                    if (collision == false)
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
                if (((stateTimeEnd - stateTimeBegin) < 5) && (collision == false))
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
                if (((stateTimeEnd - stateTimeBegin) < 5) && (collision == false))
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
