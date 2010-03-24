using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace SeniorProject
{
    class Background
    {
        //constants are so overpowered
        private const string BG_ASSET_NAME = "earth-map-huge";  //the filename of the background image
        private const int BG_POS_X = 0;   //sets the x position where the background is drawn
        private const int BG_POS_Y = 0;   //sets the y position where the background is drawn
        public const int WORLD_WIDTH = 3200;    //width of the world
        public const int WORLD_HEIGHT = 1600;   //height of the world

        //these are some underpowered variables
        private Texture2D background;    //the background
        private Vector2 bgPosition = Vector2.Zero;     //this is where the background will be drawn

        //LOAD THINGS HERE
        public void LoadContent(ContentManager theContentManager)
        {
            background = theContentManager.Load<Texture2D>(BG_ASSET_NAME);    //load background
        }

        //DRAW THINGS HERE
        //these parameters should be objects created in Game1.cs
        public void Draw(SpriteBatch spriteBatch, Camera2D camera)
        {
            bgPosition = new Vector2(BG_POS_X, BG_POS_Y); //where to draw the background or something
            bgPosition = camera.Transform(bgPosition);
            spriteBatch.Draw(background, bgPosition, null, Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);
        }
    }
}
