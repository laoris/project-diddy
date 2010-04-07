﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeniorProject
{
    class SquareGuy1 : NPC
    {
        //NPCs deserve constants too
        private const int COLLISION_OFFSET = 5; //the number of pixels to shave off of the collision bounding boxes to make collision smoother
        private const int NPC_SPEED = 120;      //the movement speed of the NPC - I think this is pixels per second
        private const int AGGRO_RADIUS = 200;   //the aggro radius in pixels
        private const int INIT_X_POS = 1100;    //the initial x position
        private const int INIT_Y_POS = 500;     //the initial y position
        private const String IMAGE_NAME = "SquareGuy";      //the image file for the sprite
        private const int MAX_HP = 80;              //the dudes hp
        private const int RESPAWN_TIME = 10;        //respawn time in seconds

        public SquareGuy1(): base(COLLISION_OFFSET, NPC_SPEED, AGGRO_RADIUS, INIT_X_POS, INIT_Y_POS, IMAGE_NAME, MAX_HP, RESPAWN_TIME)
        {

        }
    }
}
