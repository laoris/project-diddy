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
        public int level = 1;  //starting level
        public int exp = 0;    //starting exp
        public int expNext = 20;       //exp for next level
        public int strength = 20;      //damage of the auto attack

        //determines what exp is needed to level up
        public void Level()
        {
            if (level == 1)
            {
                if(exp >= expNext)
                {
                    LevelUp();
                    exp = 0;
                    expNext = 40;
                }
            }
            if (level == 2)
            {
                if (exp >= expNext)
                {
                    LevelUp();
                    exp = 0;
                    expNext = 80;
                }
            }
            if (level == 3)
            {
                if (exp >= expNext)
                {
                    LevelUp();
                    exp = 0;
                    expNext = 160;
                }
            }
            if (level == 4)
            {
                if (exp >= expNext)
                {
                    LevelUp();
                    exp = 0;
                    expNext = 320;
                }
            }
        }

        //this is what happens when you level up
        public void LevelUp()
        {
            level += 1;
            maxHP += 8;
            maxSpirit += 6;
            strength += 2;
            healthRegen = healthRegen * 1.2f;
            spiritRegen = spiritRegen * 1.2f;
        }
    }
}
