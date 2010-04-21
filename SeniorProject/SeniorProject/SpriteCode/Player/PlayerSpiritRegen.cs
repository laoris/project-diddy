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
        public int maxSpirit = 120;         //the max spirit of the player
        public int currentSpirit = 120;     //the current spirit of the player
        private float percentSpirit;        //will give the percentage of spirit remaining
        private float spiritTimer = 0.0f;   //tracks spirit regen
        private float spiritTick = 1.0f;    //the spirit regen tick in seconds
        private float spiritRegen = 1.0f;   //the amount of spirit regen per tick

        public void SpiritRegen(float delta)
        {
            spiritTimer += delta;
            if (spiritTimer > spiritTick)
            {
                if (currentSpirit < maxSpirit)  //tick
                {
                    currentSpirit += (int)spiritRegen;  //regen
                }
                if (currentSpirit > maxSpirit)  //oops!
                {
                    currentSpirit = maxSpirit;
                }
                spiritTimer = 0.0f;     //reset timer
            }
        }

        public float PercentSpirit()
        {
            percentSpirit = (((float)currentSpirit / (float)maxSpirit) * 100.0f);
            return percentSpirit;
        }
    }
}
