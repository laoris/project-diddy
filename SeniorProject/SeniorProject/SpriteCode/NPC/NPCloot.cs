using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SeniorProject
{
    class Loot
    {
        private const string BOOSTER_IMAGE = "booster";
        private const int BOOSTER_WIDTH = 31;
        private const int BOOSTER_HEIGHT = 46;
        private const float LOOT_DURATION = 30.0f;      //how long the loot stays around in seconds
        private const int BOOSTER_INIT_X = 500;
        private const int BOOSTER_INIT_Y = 500;

        private Texture2D boosterTexture;
        public Rectangle boosterBox;    //BOOSTER BOX
        public Boolean lootActive = false;
        public Boolean spawnedLoot = false;
        private float lootTimer = 0.0f;
        private Vector2 boosterVector = new Vector2(0, 0);

        public void LootLoad(ContentManager theContentManager)
        {
            boosterTexture = theContentManager.Load<Texture2D>(BOOSTER_IMAGE);
            boosterVector = new Vector2(BOOSTER_INIT_X, BOOSTER_INIT_Y);
        }

        public void LootUpdate(float delta)
        {
            if (spawnedLoot == false)
            {
/*                boosterBox = new Rectangle(
                    spriteRectangle.X + (Width / 2),
                    spriteRectangle.Y + (Height / 2),
                    BOOSTER_WIDTH, BOOSTER_HEIGHT);
*/
                lootActive = true;
                spawnedLoot = true;
                lootTimer = 0;
            }
            if (lootActive == true)
            {
                lootTimer += delta;
            }
            if (lootTimer > LOOT_DURATION)
            {
                lootActive = false;
                lootTimer = 0;
            }
        }

        public void DrawLoot(SpriteBatch spriteBatch, Camera2D camera)
        {
            if (lootActive == true)
            {
                boosterVector = new Vector2(500, 500);
                camera.Transform(boosterVector);
                spriteBatch.Draw(boosterTexture, boosterVector, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
