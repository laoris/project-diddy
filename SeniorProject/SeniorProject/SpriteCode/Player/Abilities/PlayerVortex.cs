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
        //this class is for the vortex attack - 
        //an aoe attack with a cast time
        //it does more damage based on the player having more spirit

        private const string VORTEX_IMAGE = "vortex";
        private const float VORTEX_COOLDOWN = 2.0f;
        private const float VORTEX_DURATION = 0.5f;     //the time in seconds that the projectile is in air before disappearing
        private const float VORTEX_BASE_DAMAGE = 0.5f;     //it does this much damage for each 1% of spirit the player has
        private const int VORTEX_FRAME_COUNT = 8;
        private const int VORTEX_WIDTH = 70;
        private const int VORTEX_HEIGHT = 75;
        private const float VORTEX_FRAME_RATE = 1.0f / 12;     //12 frames per second for the animation
        private const float VORTEX_CAST_TIME = 2.0f;       //time to cast the spell

        private int actionCheckerSemicolon = 0;             //this guy tracks what step we are on in the process
        private Texture2D textureVortex;
        private float vortexTimer = 0.0f;    //tracks projectile time in flight
        private Vector2 vortexLocation = new Vector2(0, 0);     //where the projectile currently is
        public float vortexCooldownTimer = 0.0f;
        private Rectangle vortexHitBox;
        private int vortexCurrentFrame = 0;
        private Rectangle vortexRect;    //this is the current frame of the animation to be drawn
        private float vortexFrameTimer = 0.0f;
        private float vortexCastTimer = 0.0f;     //tracks the time for casting
        private float vortexSpiritCost = 20.0f;          //the spirit cost
        private float vortexSpiritCostTemp = 20.0f;

        //called from the player class load method
        public void VortexLoad(ContentManager theContentManager)
        {
            textureVortex = theContentManager.Load<Texture2D>(VORTEX_IMAGE);
        }

        //called from the player class update method
        public void Vortex(float delta, List<NPC> allNPCs)
        {
            if (currentSpirit > vortexSpiritCost)   //enough mana
            {
                keyboardState = Keyboard.GetState();
                //these if statements are used to determine when the key is actually pressed and released
                if ((keyboardState.IsKeyDown(Keys.OemSemicolon)) && (actionCheckerSemicolon == 0)
                    && (vortexTimer == 0) && (vortexCooldownTimer == 0))
                {
                    actionCheckerSemicolon = 1;
                }
                if (keyboardState.IsKeyUp(Keys.OemSemicolon) && (actionCheckerSemicolon == 1)
                    && (vortexTimer == 0) && (vortexCooldownTimer == 0))
                {
                    actionCheckerSemicolon = 2;
                }
            }
            if (actionCheckerSemicolon == 2)    //we're casting
            {
                vortexCastTimer += delta;
                casting = true;
                AttackReset();          //reset auto attack if active
                SpiritAttackReset();    //reset spirit auto attack if active
            }
            if ((vortexCastTimer > VORTEX_CAST_TIME) && (actionCheckerSemicolon == 2))
            {
                if (currentSpirit > vortexSpiritCost)   //enough spirit
                {
                    actionCheckerSemicolon = 3;
                    currentSpirit -= (int)(vortexSpiritCost);   //om nom nom take your spirit
                    AttackReset();          //reset auto attack if active
                    SpiritAttackReset();    //reset spirit auto attack if active
                    vortexTimer = 0.001f;    //we call this a bunch of hax
                }
                else    //not enough spirit
                {
                    vortexTimer = 0;
                    vortexCastTimer = 0.0f;
                    actionCheckerSemicolon = 0;
                }
                casting = false;
            }
            if (vortexTimer != 0)     //is in flight
            {
                vortexTimer += delta;    //track time in flght

                vortexLocation.X = (int)position.X - (VORTEX_WIDTH);
                vortexLocation.Y = (int)position.Y - (VORTEX_HEIGHT);

                vortexHitBox = new Rectangle(
                    (int)vortexLocation.X,
                    (int)vortexLocation.Y,
                    VORTEX_WIDTH * 2,
                    VORTEX_HEIGHT * 2);

                //animate
                vortexFrameTimer += delta;
                if (vortexFrameTimer > VORTEX_FRAME_RATE)
                {
                    vortexCurrentFrame++;
                    vortexFrameTimer = 0.0f;
                }
                if (vortexCurrentFrame > VORTEX_FRAME_COUNT - 1)
                {
                    vortexCurrentFrame = 0;
                }
                vortexRect = new Rectangle(vortexCurrentFrame * VORTEX_WIDTH, 0, VORTEX_WIDTH, VORTEX_HEIGHT);

                foreach (NPC otherSprite in allNPCs)
                {
                    if (vortexHitBox.Intersects(otherSprite.spriteRectangle) && (otherSprite.alreadyHitVortex == false))
                    {
                        otherSprite.currentHP -= (int)(VORTEX_BASE_DAMAGE * PercentSpirit());      //damage the NPC
                        otherSprite.damageAggro = true;
                        otherSprite.aggroTimer = 0.0f;
                        otherSprite.alreadyHitVortex = true;
                    }
                }
            }
            if ((vortexTimer > VORTEX_DURATION) && (actionCheckerSemicolon == 3))    //times up
            {
                vortexTimer = 0;
                vortexCastTimer = 0.0f;
                foreach (NPC otherSprite in allNPCs)
                {
                    otherSprite.alreadyHitVortex = false;
                }
                actionCheckerSemicolon = 0;
            }
        }

        //called from the player class draw method
        public void DrawVortex(SpriteBatch spriteBatch)
        {
            if (vortexTimer != 0)
            {
                spriteBatch.Draw(textureVortex, vortexLocation, vortexRect, Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None,0.0f);
            }
        }
    }
}
