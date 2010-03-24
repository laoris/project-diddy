using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//much of the code for this class is based on the tutorial from http://www.xnadevelopment.com/tutorials.shtml

namespace SeniorProject
{
    class Sprite
    {
        //class variables
        public Vector2 position = new Vector2(0, 0);    //The current position of the Sprite
        public Texture2D texture;  //The texture object used when drawing the sprite
        public String AssetName;    //The asset name for the sprite's texture
        public double Angle = 90;   //The current angle of the Sprite
        public Rectangle size;      //The size of the sprite
        public float scale = 1.0f;  //The amount to increase/decrease the size of the original sprite
        public int Width;
        public int Height;

        //recalculates the size of the sprite when the scale is modified
        /*        public float scale2
                {
                    get{ return scale; }
                    set
                    {
                        scale = value;
                        //recalculate the size of the sprite with the new scale
                        size = new Rectangle(0, 0, (int)(texture.Width * scale), (int)(texture.Height * scale));
                    }
                }
        */
        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            texture = theContentManager.Load<Texture2D>(theAssetName);
            AssetName = theAssetName;
            size = new Rectangle(0, 0, (int)(texture.Width * scale), (int)(texture.Height * scale));
            Width = texture.Width;
            Height = texture.Height;
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time
        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            position += theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
