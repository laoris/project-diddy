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
        //this class is for the spirit auto attack
        //which is an auto attack that does 50% of strength as hp damage and 50% of strength as spirit damage

        private const string SPIRIT_ATTACK_IMAGE = "SpiritSword";
        private const float SPIRIT_ATTACK_COST = 5.0f;      //the spirit cost of each attack of the spirit auto attack

        private Texture2D textureSpiritAttack;
        private int actionCheckerShift1 = 0;
        public Boolean actionShift1 = false;
        private float cooldownSpiritAttackStart = 0.0f;
        private float timerSpiritAttack = 0.0f;
        private int currentFrameSpiritAttack = 0;             //keeps track of the current frame in the attack animation

        //check for action shift+1 and do it - action shift+1 will probably be reserved for spirit auto attacking as it needs to be toggled
        public void ActionShift1(GameTime gameTime, List<NPC> allNPCs)
        {
            if (currentSpirit > SPIRIT_ATTACK_COST)
            {
                keyboardState = Keyboard.GetState();
                //these if statements are used to properly toggle the action on and off
                if (((keyboardState.IsKeyDown(Keys.LeftShift)) || (keyboardState.IsKeyDown(Keys.RightShift))) && (keyboardState.IsKeyDown(Keys.D1)) && (actionCheckerShift1 == 0))
                {
                    actionCheckerShift1 = 1;
                }
                if (keyboardState.IsKeyUp(Keys.D1) && (actionCheckerShift1 == 1))
                {
                    actionCheckerShift1 = 2;
                }
                if (((keyboardState.IsKeyDown(Keys.LeftShift)) || (keyboardState.IsKeyDown(Keys.RightShift))) && keyboardState.IsKeyDown(Keys.D1) && (actionCheckerShift1 == 2))
                {
                    actionCheckerShift1 = 3;
                }
                if (keyboardState.IsKeyUp(Keys.D1) && (actionCheckerShift1 == 3))
                {
                    actionCheckerShift1 = 0;
                }
                if (actionCheckerShift1 == 0 || actionCheckerShift1 == 3)       //if auto attacking is cancelled, reset the cooldown and animation
                {
                    cooldownSpiritAttackStart = 0;
                    currentFrameSpiritAttack = 0;
                }
                if (actionCheckerShift1 == 1 || (actionCheckerShift1 == 2))     //if auto attacking is currently enabled
                {
                    actionShift1 = true;

                    //create the hit box
                    hitBox = new Rectangle(
                        ((int)position.X - (RANGE / 2) - (int)(((Width + RANGE) / 2) * (float)Math.Sin(facing))), 
                        (int)position.Y - (RANGE / 2) - (int)((((Height / 2) + RANGE) / 2) * (float)Math.Cos(facing)), 
                        RANGE, RANGE);

                    if (actionShift1 == true)    //start the attack cooldown when the button is pressed
                    {
                        cooldownSpiritAttackStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    if (cooldownSpiritAttackStart == 0)     //if attack is cancelled, reset the animation
                    {
                        currentFrameSpiritAttack = 0;
                    }

                    if (cooldownSpiritAttackStart > cooldownEnd)     //attack cooldown is up
                    {
                        timerSpiritAttack += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (timerSpiritAttack > intervalAttack)
                        {
                            if (currentFrameSpiritAttack == 1)
                            {
                                currentSpirit -= (int)SPIRIT_ATTACK_COST;    //each attack costs 2 spirit
                            }
                            //the NPC is in the hitbox
                            foreach (NPC otherSprite in allNPCs)
                            {
                                if (hitBox.Intersects(otherSprite.spriteRectangle) == true)
                                {
                                    if (currentFrameSpiritAttack == 2)     //the middle of the animation
                                    {
                                        otherSprite.currentHP -= (strength / 2);
                                        otherSprite.currentSpirit -= (strength / 2);
                                        otherSprite.damageAggro = true;
                                        otherSprite.aggroTimer = 0.0f;
                                    }
                                }
                            }
                            //make the animation
                            currentFrameSpiritAttack++;
                            if (currentFrameSpiritAttack > frameCountAttack - 1)
                            {
                                currentFrameSpiritAttack = 0;
                                cooldownSpiritAttackStart = 0;
                            }
                            timerSpiritAttack = 0f;
                        }
                        attackRect = new Rectangle(currentFrameSpiritAttack * ATTACK_WIDTH, 0, ATTACK_WIDTH, ATTACK_HEIGHT);
                    }
                }

                if (actionCheckerShift1 != 1 && (actionCheckerShift1 != 2))
                {
                    actionShift1 = false;
                }
            }
        }

        //use this to reset the attack cooldown and animation
        public void SpiritAttackReset()
        {
            cooldownSpiritAttackStart = 0.0f;
            currentFrameSpiritAttack = 0;
        }
    }
}
