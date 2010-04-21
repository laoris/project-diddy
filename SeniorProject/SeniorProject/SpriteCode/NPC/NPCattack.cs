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
        private const string ATTACK_TEXTURE = "red_star";

        private Rectangle hitBox;
        private float attackCooldownStart = 0.0f;
        private float damageDisplayStart = 0.0f;
        private float damageDisplayEnd = 0.5f;
        public Boolean damageDisplay = false;
        public Texture2D attackTexture;

        public void autoAttack(Player otherSprite, GameTime gameTime)
        {
            //his hitbox is actually an aoe around him
            hitBox = new Rectangle((int)position.X - ATTACK_RANGE, (int)position.Y - ATTACK_RANGE, texture.Width + (2 * ATTACK_RANGE), texture.Height + (2 * ATTACK_RANGE));

            if (hitBox.Intersects(otherSprite.spriteRectangle))     //if in range to attack
            {
                attackCooldownStart += (float)gameTime.ElapsedGameTime.TotalSeconds;    //gogo attack cooldown
            
                if (attackCooldownStart > ATTACK_COOLDOWN)     //attack cooldown is up
                {
                    damageDisplay = true;
                    if (SPIRIT_ATTACK == true)
                    {
                        otherSprite.currentHP -= (int)(STRENGTH / 2);     //deal damage to hp
                        otherSprite.currentSpirit -= (int)(STRENGTH / 2);     //deal damage to spirit
                    }
                    else if (SPIRIT_ATTACK == false)
                    {
                        otherSprite.currentHP -= (int)STRENGTH;     //deal damage
                    }
                    attackCooldownStart = 0.0f;                 //reset cooldown
                }
            }

            if (damageDisplay == true)      //show the taking damage image
            {
                damageDisplayStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (damageDisplayStart > damageDisplayEnd)
                {
                    damageDisplayStart = 0.0f;
                    damageDisplay = false;
                }
            }
        }
    }
}
