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
        private const string ELEMENTAL_BLAST_IMAGE = "ElementalBlast";
        private const int ELEMENTAL_BLAST_SPEED = 350;
        private const float ELEMENTAL_BLAST_COOLDOWN = 2.0f;
        private const int ELEMENTAL_BLAST_SPIRIT_COST = 10;
        private const float ELEMENTAL_BLAST_FLY_DURATION = 1.0f;    //the time in seconds that the projectile is in air before disappearing
        private const float ELEMENTAL_BLAST_BASE_DAMAGE = 30.0f;    //base damage of the spell

        private int actionCheckerJ = 0;             //this guy tracks what step we are on in the process
        private Texture2D textureElementalBlast;
        private float elementalBlastFlyTimer = 0.0f;    //tracks projectile time in flight
        private Vector2 elementalBlastLocation = new Vector2(0, 0);     //where the projectile currently is
        private float elementalBlastDirection = 0.0f;
        private float elementalBlastCooldownTimer = 0.0f;
        private Rectangle elementalBlastHitBox;

        //called from the player class update method
        public void ElementalBlast(float delta, List<NPC> allNPCs)
        {
            if(currentSpirit >= ELEMENTAL_BLAST_SPIRIT_COST)   //enough mana
            {
                keyboardState = Keyboard.GetState();
                //these if statements are used to determine when the key is actually pressed and released
                if ((keyboardState.IsKeyDown(Keys.J)) && (actionCheckerJ == 0)
                    && (elementalBlastFlyTimer == 0) && (elementalBlastCooldownTimer == 0))
                {
                    actionCheckerJ = 1;
                }
                if (keyboardState.IsKeyUp(Keys.J) && (actionCheckerJ == 1)
                    && (elementalBlastFlyTimer == 0) && (elementalBlastCooldownTimer == 0))
                {
                    actionCheckerJ = 2;
                    currentSpirit -= ELEMENTAL_BLAST_SPIRIT_COST;   //om nom nom take your spirit
                    elementalBlastFlyTimer = 0.001f;    //we call this a bunch of hax
                    elementalBlastLocation.X = (int)position.X;
                    elementalBlastLocation.Y = (int)position.Y;
                    elementalBlastDirection = facing;
                }
            }
            if (elementalBlastFlyTimer != 0)
            {
                elementalBlastFlyTimer += delta;    //track time in flght

                elementalBlastLocation.X -= (float)(ELEMENTAL_BLAST_SPEED * (float)Math.Sin(elementalBlastDirection)) * delta;
                elementalBlastLocation.Y -= (float)(ELEMENTAL_BLAST_SPEED * (float)Math.Cos(elementalBlastDirection)) * delta;

                elementalBlastHitBox = new Rectangle((int)elementalBlastLocation.X, (int)elementalBlastLocation.Y, 
                    textureElementalBlast.Width, textureElementalBlast.Height);

                foreach (NPC otherSprite in allNPCs)
                {
                    if (elementalBlastHitBox.Intersects(otherSprite.spriteRectangle))   //if projectile hits something
                    {
                        elementalBlastFlyTimer = 0;     //destroy the projectile
                        elementalBlastHitBox = new Rectangle(-100000, -100000, 0, 0);   //move the hit box far away with size 0x0
                        otherSprite.currentHP -= (int)ELEMENTAL_BLAST_BASE_DAMAGE;      //damage the NPC
                        otherSprite.damageAggro = true;
                        otherSprite.aggroTimer = 0.0f;
                    }
                }
            }
            if (elementalBlastFlyTimer > ELEMENTAL_BLAST_FLY_DURATION)
            {
                elementalBlastFlyTimer = 0;
            }
            if (actionCheckerJ == 2)
            {
                elementalBlastCooldownTimer += delta;   //track cooldown
            }
            if(elementalBlastCooldownTimer > ELEMENTAL_BLAST_COOLDOWN)
            {
                actionCheckerJ = 0;
                elementalBlastCooldownTimer = 0.0f;
            }
        }

        //called from the player class draw method
        public void DrawElementalBlast(SpriteBatch spriteBatch)
        {
            if (elementalBlastFlyTimer != 0)
            {
                spriteBatch.Draw(textureElementalBlast, elementalBlastLocation, Color.White);
            }
        }
    }
}
