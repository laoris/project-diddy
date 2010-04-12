using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeniorProject
{
    partial class Player
    {
        //this class is for the spirit blast attack - a projectile that only damages the spirit of what it hits

        private const string SPIRIT_BLAST_IMAGE = "SpiritBlast";
        private const int SPIRIT_BLAST_SPEED = 350;
        private const float SPIRIT_BLAST_COOLDOWN = 2.0f;
        private const int SPIRIT_BLAST_SPIRIT_COST = 15;
        private const float SPIRIT_BLAST_FLY_DURATION = 1.0f;    //the time in seconds that the projectile is in air before disappearing
        private const float SPIRIT_BLAST_BASE_DAMAGE = 35.0f;    //base damage of the spell

        private int actionCheckerK = 0;             //this guy tracks what step we are on in the process
        private Texture2D textureSpiritBlast;
        private float spiritBlastFlyTimer = 0.0f;    //tracks projectile time in flight
        private Vector2 spiritBlastLocation = new Vector2(0, 0);     //where the projectile currently is
        private float spiritBlastDirection = 0.0f;
        private float spiritBlastCooldownTimer = 0.0f;
        private Rectangle spiritBlastHitBox;
        private Vector2 spiritBlastOrigin = new Vector2(0, 0);

        public void SpiritBlastLoad(ContentManager theContentManager)
        {
            textureSpiritBlast = theContentManager.Load<Texture2D>(SPIRIT_BLAST_IMAGE);
            spiritBlastOrigin.X = textureSpiritBlast.Width / 2;
            spiritBlastOrigin.Y = textureSpiritBlast.Height / 2;
        }

        //called from the player class update method
        public void SpiritBlast(float delta, List<NPC> allNPCs)
        {
            if (currentSpirit > SPIRIT_BLAST_SPIRIT_COST)   //enough mana
            {
                keyboardState = Keyboard.GetState();
                //these if statements are used to determine when the key is actually pressed and released
                if ((keyboardState.IsKeyDown(Keys.K)) && (actionCheckerK == 0)
                    && (spiritBlastFlyTimer == 0) && (spiritBlastCooldownTimer == 0))
                {
                    actionCheckerK = 1;
                }
                if (keyboardState.IsKeyUp(Keys.K) && (actionCheckerK == 1)
                    && (spiritBlastFlyTimer == 0) && (spiritBlastCooldownTimer == 0))
                {
                    actionCheckerK = 2;
                    currentSpirit -= SPIRIT_BLAST_SPIRIT_COST;   //om nom nom take your spirit
                    spiritBlastFlyTimer = 0.001f;    //we call this a bunch of hax
                    spiritBlastLocation.X = (int)position.X;
                    spiritBlastLocation.Y = (int)position.Y;
                    spiritBlastDirection = facing;
                }
            }
            if (spiritBlastFlyTimer != 0)
            {
                spiritBlastFlyTimer += delta;    //track time in flght

                spiritBlastLocation.X -= (float)(SPIRIT_BLAST_SPEED * (float)Math.Sin(spiritBlastDirection)) * delta;
                spiritBlastLocation.Y -= (float)(SPIRIT_BLAST_SPEED * (float)Math.Cos(spiritBlastDirection)) * delta;

                spiritBlastHitBox = new Rectangle((int)spiritBlastLocation.X, (int)spiritBlastLocation.Y,
                    textureSpiritBlast.Width, textureSpiritBlast.Height);

                foreach (NPC otherSprite in allNPCs)
                {
                    if (spiritBlastHitBox.Intersects(otherSprite.spriteRectangle))   //if projectile hits something
                    {
                        spiritBlastFlyTimer = 0;     //destroy the projectile
                        spiritBlastHitBox = new Rectangle(-100000, -100000, 0, 0);   //move the hit box far away with size 0x0
                        otherSprite.currentSpirit -= (int)SPIRIT_BLAST_BASE_DAMAGE;      //damage the NPC
                        otherSprite.damageAggro = true;
                        otherSprite.aggroTimer = 0.0f;
                    }
                }
            }
            if (spiritBlastFlyTimer > SPIRIT_BLAST_FLY_DURATION)
            {
                spiritBlastFlyTimer = 0;
            }
            if (actionCheckerK == 2)
            {
                spiritBlastCooldownTimer += delta;   //track cooldown
            }
            if (spiritBlastCooldownTimer > SPIRIT_BLAST_COOLDOWN)
            {
                actionCheckerK = 0;
                spiritBlastCooldownTimer = 0.0f;
            }
        }

        //called from the player class draw method
        public void DrawSpiritBlast(SpriteBatch spriteBatch)
        {
            if (spiritBlastFlyTimer != 0)
            {
                spriteBatch.Draw(textureSpiritBlast, spiritBlastLocation, null, Color.White, 
                    (-1) * spiritBlastDirection, spiritBlastOrigin, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
