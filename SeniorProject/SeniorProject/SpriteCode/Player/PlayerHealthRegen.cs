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
        public int maxHP = 150;             //the player's max hp
        public int currentHP = 140;         //the player's current hp
        private float healthTimer = 0.0f;   //tracks health regen
        private float healthTick = 1.0f;    //the health regen tick in seconds
        private float healthRegen = 1.0f;   //the amount of health regen per tick

        public void HealthRegen(float delta)
        {
            healthTimer += delta;
            if (healthTimer > healthTick)
            {
                if (currentHP < maxHP)
                {
                    currentHP += (int)healthRegen;
                }
                if (currentHP > maxHP)
                {
                    currentHP = maxHP;
                }
                healthTimer = 0.0f;
            }
        }
    }
}
