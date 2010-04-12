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
        public int currentSpirit;
        private float spiritTimer = 0.0f;           //tracks spirit regen
        private float spiritTick = 1.0f;            //the spirit regen tick in seconds
        private float spiritRegenCombat = 1.0f;     //the amount of spirit regen per tick in combat
        private float spiritRegenOOC = 3.0f;        //the spirit regen per tick out of combat

        public void SpiritRegen(float delta)
        {
            spiritTimer += delta;
            if (spiritTimer > spiritTick)
            {
                if (currentState == State.Aggro)    //in combat
                {
                    if (currentSpirit < MAX_SPIRIT)
                    {
                        currentSpirit += (int)spiritRegenCombat;
                    }
                }
                else if ((currentState == State.Patrol) || currentState == State.Reset)
                {
                    if (currentSpirit < MAX_SPIRIT)
                    {
                        currentSpirit += (int)spiritRegenOOC;
                    }
                }
                if (currentSpirit > MAX_SPIRIT)
                {
                    currentSpirit = MAX_SPIRIT;
                }
                spiritTimer = 0.0f;
            }
        }
    }
}
