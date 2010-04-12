using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeniorProject
{
    partial class NPC
    {
        private const float AGGRO_DURATION = 5.0f;      //this is how long in seconds the player has to not hit the NPC for it to stop caring

        public Boolean damageAggro = false;    //true if the player is in the aggro radius
        private Boolean radiusAggro = false;    //true if it should aggro from taking damage
        private Boolean aggroCheck = false;     //true if either aggro condition is true
        public float aggroTimer = 0.0f;

        //this method determines if the NPC should aggro the player
        public void AggroCheck(float delta, Player otherSprite)
        {
            //proximity aggro
            if (aggroBox.Intersects(otherSprite.spriteRectangle))
            {
                radiusAggro = true;
            }
            else
            {
                radiusAggro = false;
            }

            //aggro from being attacked
            if (damageAggro == true)
            {
                aggroTimer += delta;
            }
            if (aggroTimer > AGGRO_DURATION)
            {
                damageAggro = false;
            }

            //determine aggro
            if ((damageAggro == true) || (radiusAggro == true))
            {
                aggroCheck = true;
            }
            if ((damageAggro == false) && (radiusAggro == false))
            {
                aggroCheck = false;
            }
        }
    }
}
