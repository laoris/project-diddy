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
        //this class is for the force wave attack - 
        //an attack that unleashes a wave in front of the character that has spirit cost based on remaining spirit
        //the more spirit the player has, the more spirit the ability costs

        private const string FORCE_WAVE_IMAGE = "ForceWave2";
        private const int FORCE_WAVE_SPEED = 150;
        private const float FORCE_WAVE_COOLDOWN = 2.0f;
        private const float FORCE_WAVE_SPIRIT_COST = 20.0f;          //the spirit cost in percentage
        private const float FORCE_WAVE_FLY_DURATION = 0.5f;     //the time in seconds that the projectile is in air before disappearing
        private const float FORCE_WAVE_BASE_DAMAGE = 15.0f;     //base damage of the spell
        private const int frameCountForceWave = 3;
        private const int FORCE_WAVE_WIDTH = 61;
        private const int FORCE_WAVE_HEIGHT = 61;
        private const float FORCE_WAVE_FRAME_RATE = 1.0f / 12;     //18 frames per second for the animation

        private int actionCheckerL = 0;             //this guy tracks what step we are on in the process
        private Texture2D textureForceWave;
        private float forceWaveFlyTimer = 0.0f;    //tracks projectile time in flight
        private Vector2 forceWaveLocation = new Vector2(0, 0);     //where the projectile currently is
        private float forceWaveDirection = 0.0f;
        private float forceWaveCooldownTimer = 0.0f;
        private Rectangle forceWaveHitBox;
        private int currentFrameForceWave = 0;
        private Rectangle forceWaveRect;    //this is the current frame of the animation to be drawn
        private Vector2 forceWaveOrigin = new Vector2(0, 0);
        private float forceWaveFrameTimer = 0.0f;
        Color[] forceWaveTextureData;

        public void ForceWaveLoad(ContentManager theContentManager)
        {
            textureForceWave = theContentManager.Load<Texture2D>(FORCE_WAVE_IMAGE);

            forceWaveOrigin.X = textureForceWave.Width / (2 * frameCountForceWave);
            forceWaveOrigin.Y = textureForceWave.Height / 2;

            forceWaveTextureData = new Color[textureForceWave.Width * textureForceWave.Height];
            textureForceWave.GetData(forceWaveTextureData);
        }

        //called from the player class update method
        public void ForceWave(float delta, List<NPC> allNPCs)
        {
            if (currentSpirit > 1)   //enough mana
            {
                keyboardState = Keyboard.GetState();
                //these if statements are used to determine when the key is actually pressed and released
                if ((keyboardState.IsKeyDown(Keys.L)) && (actionCheckerL == 0)
                    && (forceWaveFlyTimer == 0) && (forceWaveCooldownTimer == 0))
                {
                    actionCheckerL = 1;
                }
                if (keyboardState.IsKeyUp(Keys.L) && (actionCheckerL == 1)
                    && (forceWaveFlyTimer == 0) && (forceWaveCooldownTimer == 0))
                {
                    actionCheckerL = 2;
                    currentSpirit -= (int)(currentSpirit * (FORCE_WAVE_SPIRIT_COST / 100));   //om nom nom take your spirit
                    AttackReset();          //reset auto attack if active
                    SpiritAttackReset();    //reset spirit auto attack if active
                    forceWaveFlyTimer = 0.001f;    //we call this a bunch of hax
                }
            }
            if (forceWaveFlyTimer != 0)     //is in flight
            {
                forceWaveFlyTimer += delta;    //track time in flght

                forceWaveDirection = facing;
                forceWaveLocation.X = (int)position.X - (float)((FORCE_WAVE_HEIGHT / 4) * (float)Math.Sin(forceWaveDirection)) 
                    + (float)(FORCE_WAVE_SPEED * (float)Math.Sin(forceWaveDirection)) * delta;
                forceWaveLocation.Y = (int)position.Y - (float)((FORCE_WAVE_HEIGHT / 4) * (float)Math.Cos(forceWaveDirection))
                    + (float)(FORCE_WAVE_SPEED * (float)Math.Cos(forceWaveDirection)) * delta;

                forceWaveHitBox = new Rectangle(
                    (int)forceWaveLocation.X - (FORCE_WAVE_HEIGHT / 2) - (int)(((Width + FORCE_WAVE_HEIGHT) / 2) * (float)Math.Sin(forceWaveDirection)),
                    (int)forceWaveLocation.Y - (FORCE_WAVE_HEIGHT / 2) - (int)((((Height / 2) + FORCE_WAVE_HEIGHT) / 2) * (float)Math.Cos(forceWaveDirection)), 
                    FORCE_WAVE_HEIGHT,
                    FORCE_WAVE_HEIGHT);
                Console.WriteLine(forceWaveHitBox);

                //animate
                forceWaveFrameTimer += delta;
                if(forceWaveFrameTimer > FORCE_WAVE_FRAME_RATE)
                {
                    currentFrameForceWave++;
                    forceWaveFrameTimer = 0.0f;
                }
                if (currentFrameForceWave > frameCountForceWave - 1)
                {
                    currentFrameForceWave = 0;
                }
                forceWaveRect = new Rectangle(currentFrameForceWave * FORCE_WAVE_WIDTH, 0, FORCE_WAVE_WIDTH, FORCE_WAVE_HEIGHT);

                foreach (NPC otherSprite in allNPCs)
                {
                    if (forceWaveHitBox.Intersects(otherSprite.spriteRectangle) && (otherSprite.alreadyHit == false))
                    {
                        otherSprite.currentHP -= (int)FORCE_WAVE_BASE_DAMAGE;      //damage the NPC
                        otherSprite.damageAggro = true;
                        otherSprite.aggroTimer = 0.0f;
                        otherSprite.alreadyHit = true;
                    }
                }
            }
            if (forceWaveFlyTimer > FORCE_WAVE_FLY_DURATION)    //times up
            {
                forceWaveFlyTimer = 0;
                foreach (NPC otherSprite in allNPCs)
                {
                    otherSprite.alreadyHit = false;
                }

            }
            if (actionCheckerL == 2)    //is on cooldown
            {
                forceWaveCooldownTimer += delta;   //track cooldown
            }
            if (forceWaveCooldownTimer > FORCE_WAVE_COOLDOWN)   //cooldown up
            {
                actionCheckerL = 0;
                forceWaveCooldownTimer = 0.0f;
            }
        }

        //called from the player class draw method
        public void DrawForceWave(SpriteBatch spriteBatch)
        {
            if (forceWaveFlyTimer != 0)
            {
                spriteBatch.Draw(textureForceWave, forceWaveLocation, forceWaveRect, Color.White,
                    (-1) * forceWaveDirection, forceWaveOrigin, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
