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
        //this class is for the borrowed spirit ability - 
        //a temporary buff that converts incoming damage to spirit
        //the player still takes damage, but also gains spirit equal to the amount of damage taken
        //other abilities also cost no spirit while this is active

        private const string BORROWED_SPIRIT_IMAGE = "BorrowedSpirit";
        private const float BORROWED_SPIRIT_COOLDOWN = 30.0f;
        private const float BORROWED_SPIRIT_DURATION = 5.0f;     //the time in seconds that the projectile is in air before disappearing
        private const int BORROWED_SPIRIT_FRAME_COUNT = 8;
        private const float BORROWED_SPIRIT_FRAME_RATE = 1.0f / 12;     //12 frames per second for the animation

        private int actionCheckerU = 0;             //this guy tracks what step we are on in the process
        private Texture2D textureBorrowedSpirit;
        private float borrowedSpiritTimer = 0.0f;    //tracks duration
        private Rectangle borrowedSpiritLocation;     //where the projectile currently is
        public float borrowedSpiritCooldownTimer = 0.0f;    //tracks cooldown
        private int borrowedSpiritCurrentFrame = 0;
        private Rectangle borrowedSpiritRect;    //this is the current frame of the animation to be drawn
        private float borrowedSpiritFrameTimer = 0.0f;
        private Boolean abilitiesNoSpirit = false;
        public Boolean borrowedSpiritActive = false;
        private float tempHP;
        private float differenceHP;
        private float borrowedSpiritDirection = 0.0f;
        private Vector2 borrowedSpiritOrigin = new Vector2(0, 0);

        //called from the player class load method
        public void BorrowedSpiritLoad(ContentManager theContentManager)
        {
            textureBorrowedSpirit = theContentManager.Load<Texture2D>(BORROWED_SPIRIT_IMAGE);
            borrowedSpiritOrigin.X = textureBorrowedSpirit.Width / 2;
            borrowedSpiritOrigin.Y = textureBorrowedSpirit.Height / 2;
        }

        //called from the player class update method
        public void BorrowedSpirit(float delta)
        {
            keyboardState = Keyboard.GetState();
            //these if statements are used to determine when the key is actually pressed and released
            if ((keyboardState.IsKeyDown(Keys.U)) && (actionCheckerU == 0)
                && (borrowedSpiritTimer == 0) && (borrowedSpiritCooldownTimer == 0))
            {
                actionCheckerU = 1;
            }
            if (keyboardState.IsKeyUp(Keys.U) && (actionCheckerU == 1)
                && (borrowedSpiritTimer == 0) && (borrowedSpiritCooldownTimer == 0))
            {
                actionCheckerU = 2;
                borrowedSpiritTimer = 0.001f;    //we call this a bunch of hax
                tempHP = currentHP;
            }
            if (borrowedSpiritTimer != 0)     //is active
            {
                borrowedSpiritActive = true;
                borrowedSpiritTimer += delta;    //track time active

                if(tempHP > currentHP)  //this makes the player gain spirit from taking damage
                {
                    differenceHP = (tempHP - currentHP);
                    currentSpirit += (int)differenceHP;
                    tempHP = currentHP;
                }

                if (abilitiesNoSpirit == false)     //make abilities cost no spirit
                {
                    AbilitiesNoSpirit();
                }

                //make the location
                borrowedSpiritDirection = facing;
                borrowedSpiritLocation = new Rectangle(
                    (int)position.X, 
                    (int)position.Y, 
                    (Width * 2), 
                    (Height * 2));

/*
                //just in case we give it an animation
                borrowedSpiritFrameTimer += delta;
                if (borrowedSpiritFrameTimer > BORROWED_SPIRIT_FRAME_RATE)
                {
                    borrowedSpiritCurrentFrame++;
                    borrowedSpiritFrameTimer = 0.0f;
                }
                if (borrowedSpiritCurrentFrame > BORROWED_SPIRIT_FRAME_COUNT - 1)
                {
                    borrowedSpiritCurrentFrame = 0;
                }
                borrowedSpiritRect = new Rectangle(borrowedSpiritCurrentFrame * BORROWED_SPIRIT_WIDTH, 0, BORROWED_SPIRIT_WIDTH, BORROWED_SPIRIT_HEIGHT);
 */
            }

            if ((borrowedSpiritTimer > BORROWED_SPIRIT_DURATION) && (actionCheckerU == 2))    //times up
            {
                borrowedSpiritTimer = 0;
                borrowedSpiritActive = false;
                actionCheckerU = 3;
                AbilitiesCostSpirit();
            }
            if (actionCheckerU == 3)    //start cooldown
            {
                borrowedSpiritCooldownTimer += delta;   //track cooldown
                if (borrowedSpiritCooldownTimer > BORROWED_SPIRIT_COOLDOWN)     //cooldown up
                {
                    borrowedSpiritTimer = 0;
                    borrowedSpiritCooldownTimer = 0;
                    actionCheckerU = 0;
                }
            }
        }

        //called from the player class draw method
        public void DrawBorrowedSpirit(SpriteBatch spriteBatch)
        {
            if (borrowedSpiritTimer != 0)
            {
                spriteBatch.Draw(textureBorrowedSpirit, borrowedSpiritLocation, null, Color.White, 
                    (-1) * borrowedSpiritDirection, borrowedSpiritOrigin, SpriteEffects.None, 0.0f);
            }
        }

        //this method will make all abilities cost no mana
        public void AbilitiesNoSpirit()
        {
            //save spirit cost values
            elementalBlastSpiritCostTemp = elementalBlastSpiritCost;
            forceWaveSpiritCostTemp = forceWaveSpiritCost;
            spiritAttackCostTemp = spiritAttackCost;
            spiritBlastSpiritCostTemp = spiritBlastSpiritCost;
            vortexSpiritCostTemp = vortexSpiritCost;

            //make the abilities cost no spirit
            elementalBlastSpiritCost = 0;
            forceWaveSpiritCost = 0;
            spiritAttackCost = 0;
            spiritBlastSpiritCost = 0;
            vortexSpiritCost = 0;

            abilitiesNoSpirit = true;
        }

        //make the abilities cost spirit again
        public void AbilitiesCostSpirit()
        {
            elementalBlastSpiritCost = elementalBlastSpiritCostTemp;
            forceWaveSpiritCost = forceWaveSpiritCostTemp;
            spiritAttackCost = spiritAttackCostTemp;
            spiritBlastSpiritCost = spiritBlastSpiritCostTemp;
            vortexSpiritCost = vortexSpiritCostTemp;

            abilitiesNoSpirit = false;
        }
    }
}
