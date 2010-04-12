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
    partial class NPC
    {
        public int currentHP;
        private float healthTimer = 0.0f;   //tracks health regen
        private float healthTick = 1.0f;    //the health regen tick in seconds
        private float healthRegenCombat = 1.0f;     //the amount of health regen per tick in combat
        private float healthRegenOOC = 3.0f;        //the health regen per tick out of combat

        public void HealthRegen(float delta)
        {
            healthTimer += delta;
            if (healthTimer > healthTick)
            {
                if (currentState == State.Aggro)    //in combat
                {
                    if (currentHP < MAX_HP)
                    {
                        currentHP += (int)healthRegenCombat;
                    }
                }
                else if((currentState == State.Patrol) || currentState == State.Reset)
                {
                    if (currentHP < MAX_HP)
                    {
                        currentHP += (int)healthRegenOOC;
                    }
                }
                if (currentHP > MAX_HP)
                {
                    currentHP = MAX_HP;
                }
                healthTimer = 0.0f;
            }
        }
    }
}
