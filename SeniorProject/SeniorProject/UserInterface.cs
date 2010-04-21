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
        public Vector2 hptextpos = new Vector2(414, 573); //The hptext position
        public Vector2 sptextpos = new Vector2(414, 594);
        public Texture2D uitexture;   //texture of the frame
        public Texture2D hpbar;         //texture for hp
        public Texture2D movebox;       //texture for the moveboxes
        public Texture2D attack;
        public Texture2D sattack;
        public Texture2D eblast;
        public Texture2D sblast;
        public Texture2D fwave;
        public Texture2D vortex;
        public Texture2D Bspirit;

        SpriteFont hpFont;          //font for the hpbar
        SpriteFont hotbarFont;      //font for the hotbar in the bottom middle
        public string HPvalue;
        public string SPvalue;
        public int currentHP = 90;  //should be used to store the player's current health
        public int maxHP;       //should be used to store the player's max health
        public int currentSpirit;
        public int maxSpirit;
        public int maLevel;
        int jshift;

        //strings for the letter's that correspond to the hotkey: currently static keys
        public string mslot1 = "1";
        public string mslot2 = "s1";
        public string mslot3 = "J";
        public string mslot4 = "K";
        public string mslot5 = "L";
        public string mslot6 = ";";
        public string mslot7 = "U";

        private Boolean move1;
        private Boolean move2;
        private float move3;
        private float move4;
        private float move5;
        private float move6;
        private float move7;

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
            attack = theContentManager.Load<Texture2D>("sword4");
            sattack = theContentManager.Load<Texture2D>("ssword");
            eblast = theContentManager.Load<Texture2D>("ElementalBlast");
            sblast = theContentManager.Load<Texture2D>("SpiritBlast");
            fwave = theContentManager.Load<Texture2D>("fwave");
            vortex = theContentManager.Load<Texture2D>("BabyDK2");
            Bspirit = theContentManager.Load<Texture2D>("BorrowedSpirit");

        }

        //UPDATE THINGS HERE
        public void Update(GameTime gameTime, Player maSprite, NPC npcSprite)
        {
            //sets the variables to change when things happen to the player
            currentHP = maSprite.currentHP;
            maxHP = maSprite.maxHP;
            currentSpirit = maSprite.currentSpirit;
            maxSpirit = maSprite.maxSpirit;

            //sets the variables for the different abilites that are made
            move1 = maSprite.action1;
            move2 = maSprite.actionShift1;
            move3 = maSprite.elementalBlastCooldownTimer;
            move4 = maSprite.spiritBlastCooldownTimer;
            move5 = maSprite.forceWaveCooldownTimer;
            move6 = maSprite.vortexCooldownTimer;
            move7 = maSprite.borrowedSpiritCooldownTimer;
            //in the future move1-5 will be getting whatever ability is assigned to that slot from whereever
            //they would be assigned from, maybe like a movelist class or something
            //right now it is just the boolean of whether the predefined moves are active or not

            maLevel = maSprite.level;


            //some stuff scott is screwing around with - not permanent
            //let's make some of it permanent to help show what's going on
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
            SPvalue = currentSpirit + "/" + maxSpirit;

            //may not need the clamp here
            currentHP = (int)MathHelper.Clamp(currentHP, 0, maxHP); //Keeps the health between 0 and maxHP
            currentSpirit = (int)MathHelper.Clamp(currentSpirit, 0, maxSpirit);
            jshift = 240;
        }

        //DRAW THINGS HERE
        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentState == State.On)
            {
                spriteBatch.Draw(uitexture, position, Color.White);     //the ui frame

                #region health/spirit bars
                //draws the empty space for the health bar and spirit bar
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    574, hpbar.Width, 44), new Rectangle(0, 45, hpbar.Width, 44), Color.LightGray);
                //draws the current heatlh
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    574, (int)(hpbar.Width * ((double)currentHP / (double)maxHP)), 22),
                    new Rectangle(0, 45, hpbar.Width, 22), Color.Red);
                //draws the current spirit under current health
                spriteBatch.Draw(hpbar, new Rectangle(1280 / 2 - hpbar.Width / 2,
                    596, (int)(hpbar.Width * ((double)currentSpirit / (double)maxSpirit)), 22),
                    new Rectangle(0, 45, hpbar.Width, 22), Color.Blue);
                #endregion

                #region color coded ability boxes
                for (int i = 1; i <= 7; i++)
                {
                    //spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White);

                    if (i == 1)
                    {
                        if (move1) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }
                    if (i == 2)
                    {
                        if (move2) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }
                    if (i == 3)
                    {
                        if (move3 <= 0.0f) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }
                    if (i == 4)
                    {
                        if (move4 <= 0.0f) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }
                    if (i == 5)
                    {
                        if (move5 <= 0.0f && maLevel >= 2) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }
                    if (i == 6)
                    {
                        if (move6 <= 0.0f && maLevel >= 3) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }
                    if (i == 7)
                    {
                        if (move7 <= 0.0f && maLevel >= 1) { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White); }
                        else { spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.Red); }
                    }

                    jshift += 115;
                }
                #endregion

                #region ability icons drawn to hotbar
                for (int i = 1; i <= 7; i++)
                {
                    //spriteBatch.Draw(movebox, new Vector2(jshift, 626), Color.White);

                    if (i == 1)
                    {
                        spriteBatch.Draw(attack, new Rectangle(244, 630, 80, 80), Color.White);
                    }
                    if (i == 2)
                    {
                        spriteBatch.Draw(sattack, new Rectangle(359, 630, 80, 80), Color.White);
                    }
                    if (i == 3)
                    {
                        spriteBatch.Draw(eblast, new Rectangle(474, 630, 80, 80), Color.White);
                    }
                    if (i == 4)
                    {
                        spriteBatch.Draw(sblast, new Rectangle(589, 630, 80, 80), Color.White);
                    }
                    if (i == 5)
                    {
                        spriteBatch.Draw(fwave, new Rectangle(704, 630, 80, 80), Color.White);
                    }
                    if (i == 6)
                    {
                        spriteBatch.Draw(vortex, new Rectangle(819, 630, 80, 80), Color.White);
                    }
                    if (i == 7)
                    {
                        spriteBatch.Draw(Bspirit, new Rectangle(934, 630, 80, 80), Color.White);
                    }

                    //jshift += 115;
                }
                #endregion



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
                spriteBatch.DrawString(hpFont, SPvalue, sptextpos, Color.Red);
                spriteBatch.DrawString(hotbarFont, mslot1, new Vector2(308, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot2, new Vector2(423, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot3, new Vector2(538, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot4, new Vector2(653, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot5, new Vector2(768, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot6, new Vector2(883, 628), Color.Black);
                spriteBatch.DrawString(hotbarFont, mslot7, new Vector2(998, 628), Color.Black);

                //again, screwing around, not permanent stuff
                spriteBatch.DrawString(hotbarFont, npcHP, new Vector2(1130, 590), Color.Black);
                spriteBatch.DrawString(hotbarFont, npcSpirit, new Vector2(1100, 610), Color.Black);
                spriteBatch.DrawString(hotbarFont, level, new Vector2(10, 585), Color.Black);
                spriteBatch.DrawString(hotbarFont, exp, new Vector2(10, 610), Color.Black);
                spriteBatch.DrawString(hotbarFont, strength, new Vector2(10, 635), Color.Black);
                //spriteBatch.DrawString(hotbarFont, playerSpirit, new Vector2(10, 660), Color.Black);
                spriteBatch.DrawString(hotbarFont, "ESC: opens menu", new Vector2(10, 690), Color.Black);
            }
        }
    }
}
