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
    class UserInterface
    {
        #region Variables
        private const string UI_FRAME = "UIframe";
        private const string HP_BAR = "HealthBar";
        private const string UI_BOX = "selectionbox";
        public Vector2 position = new Vector2(0, 576);    //The frame positioning
        public Vector2 hptextpos = new Vector2(420, 580); //The hptext position
        public Texture2D uitexture;   //texture of the frame
        public Texture2D hpbar;         //texture for hp
        public Texture2D movebox;       //texture for the moveboxes

        SpriteFont hpFont;          //font for the hpbar
        SpriteFont hotbarFont;      //font for the hotbar in the bottom middle
        public string HPvalue;
        public int currentHP = 90;  //should be used to store the player's current health
        public int maxHP;       //should be used to store the player's max health
        int jshift;

        //strings for the letter's that correspond to the hotkey: currently static keys
        public string mslot1 = "1";
        public string mslot2 = "2";
        public string mslot3 = "3";
        public string mslot4 = "4";
        public string mslot5 = "5";

        State currentState = State.On;

        private string npcHP;
        private string npcSpirit;
        private string playerSpirit;
        private string level;
        private string exp;
        private string strength;
        #endregion

        //stores the UI state
        enum State
        {
            On,
            Off
        }

        //LOAD CONTENT HERE
        public void LoadContent(ContentManager theContentManager)
        {
            uitexture = theContentManager.Load<Texture2D>(UI_FRAME); //loads the uiframe texture
            hpbar = theContentManager.Load<Texture2D>(HP_BAR);      //loads the hpbar texture
            hpFont = theContentManager.Load<SpriteFont>("SpriteFont1");     //loads a font
            hotbarFont = theContentManager.Load<SpriteFont>("SpriteFont2");     //loads a font
            movebox = theContentManager.Load<Texture2D>(UI_BOX);    //loads the movebox

        }

        //UPDATE THINGS HERE
        public void Update(GameTime gameTime, Player maSprite, NPC npcSprite)
        {
            //sets the variables to change when things happen to the player
            currentHP = maSprite.currentHP;
            maxHP = maSprite.maxHP;

            //some stuff scott is screwing around with - not permanent
            npcHP = "NPC HP: " + npcSprite.currentHP + "/" + npcSprite.MAX_HP;
            npcSpirit = "NPC Spirit: " + npcSprite.currentSpirit + "/" + npcSprite.MAX_SPIRIT;
            playerSpirit = "Spirit: " + maSprite.currentSpirit + "/" + maSprite.maxSpirit;
            level = "Level: " + maSprite.level;
            strength = "Strength: " + maSprite.strength;
            exp = "EXP: " + maSprite.exp + "/" + maSprite.expNext;

            //updates here
            //maxHP = 150;    //update the maxHP value
            //currentHP = 100;      //update the currentHP value
            HPvalue = currentHP + "/" + maxHP;      //"current/max"

            //may not need the clamp here
            currentHP = (int)MathHelper.Clamp(currentHP, 0, maxHP); //Keeps the health between 0 and maxHP
            jshift = 240;
        }

        //DRAW THINGS HERE
        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentState == State.On)
            {
                spriteBatch.Draw(uitexture, position, Color.White);     //the ui frame

                //draws the empty space for the health bar
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    574, hpbar.Width, 44), new Rectangle(0, 45, hpbar.Width, 44), Color.LightGray);
                //draws the current heatlh
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    574, (int)(hpbar.Width * ((double)currentHP / (double)maxHP)), 44),
                    new Rectangle(0, 45, hpbar.Width, 44), Color.Red);

                for (int i = 0; i < 5; i++)
                {
                    spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White);
                    jshift += 162;
                }


                #region reference comments
                /*
                //draws the health of the health bar
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    574, hpbar.Width, 44), new Rectangle(0, 45, hpbar.Width, 44), Color.Red);
                
                //draws the border of the health bar
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    574, hpbar.Width, 44), new Rectangle(0, 0, hpbar.Width, 44), Color.White);
                */
                #endregion

                //draw fonts to the screen:
                spriteBatch.DrawString(hpFont, HPvalue, hptextpos, Color.MediumBlue);
                spriteBatch.DrawString(hotbarFont, mslot1, new Vector2(311, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot2, new Vector2(473, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot3, new Vector2(635, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot4, new Vector2(797, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot5, new Vector2(959, 628), Color.Black);

                //again, screwing around, not permanent stuff
                spriteBatch.DrawString(hotbarFont, npcHP, new Vector2(1090, 610), Color.Black);
                spriteBatch.DrawString(hotbarFont, npcSpirit, new Vector2(1030, 635), Color.Black);
                spriteBatch.DrawString(hotbarFont, level, new Vector2(10, 585), Color.Black);
                spriteBatch.DrawString(hotbarFont, exp, new Vector2(10, 610), Color.Black);
                spriteBatch.DrawString(hotbarFont, strength, new Vector2(10, 635), Color.Black);
                spriteBatch.DrawString(hotbarFont, playerSpirit, new Vector2(10, 660), Color.Black);
            }
        }
    }
}
