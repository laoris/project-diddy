using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeniorProject
{
    class SquareGuy2 : NPC
    {
        //NPCs deserve constants too
        private const int COLLISION_OFFSET = 5; //the number of pixels to shave off of the collision bounding boxes to make collision smoother
        private const int NPC_SPEED = 120;      //the movement speed of the NPC - I think this is pixels per second
        private const int AGGRO_RADIUS = 200;   //the aggro radius in pixels
        private const int INIT_X_POS = 1500;    //the initial x position
        private const int INIT_Y_POS = 200;     //the initial y position
        private const String IMAGE_NAME = "SquareGuy";      //the image file for the sprite
        private const int MAX_HP = 80;              //the dudes hp
        private const int MAX_SPIRIT = 200;
        private const int RESPAWN_TIME = 10;        //respawn time in seconds
        private const int ATTACK_RANGE = 65;        //NPC attack range
        private const float ATTACK_COOLDOWN = 2.0f;     //NPC attack cooldown - time between auto attacks
        private const float STRENGTH = 10;               //NPC strength - directly related to his damage
        private const int EXPERIENCE = 10;

        public SquareGuy2()
            : base(COLLISION_OFFSET, NPC_SPEED, AGGRO_RADIUS, INIT_X_POS, INIT_Y_POS, IMAGE_NAME, MAX_HP, RESPAWN_TIME, ATTACK_RANGE, ATTACK_COOLDOWN, STRENGTH, MAX_SPIRIT, EXPERIENCE)
        {

        }
    }
}
