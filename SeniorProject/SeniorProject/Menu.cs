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
    class Menu
    {
        #region Variables
        SpriteFont gameFont;
        SpriteFont gameFont2;
        public string text;
        private const string CHAR = "Character";
        private const string LIST = "Moves List";
        private const string OPTN = "Details";
        private const string EXIT = "Exit";
        private string DETAILS = "Menancing Dawn \n  W: move forward \n  S: move backward \n  A/D:"
               + " change player facing \n  Q/E: Strafe \n\n"
               + " Explore the world as your new \n character."
               + "  Avoid enemy square-\n people or use your"
               + " special \n abilities to ward them off!"
               + "\n\n (Also, the game doesn't \n stop just because you're \n reading this!) :D";
        private string other = "TAB to scroll through selecitons \nEnter to select an option \nESC to close this menu";
        private KeyboardState keyboardState;        //used for keyboard input
        public Boolean click = false;
        public Boolean oc = false;
        public Boolean ent = false;

        public Vector2 menuVector = new Vector2(580, 180);

        public Texture2D menuTexture;

        #endregion

        enum State
        {
            Active,    //the main menu is shown
            Idle,        //the main menu is hidden
        }
        enum Selections
        {
            None,
            Char,
            MoveList,
            Options,
            Exit
        }
        State menuState = State.Idle;     //default state is Idle'
        Selections selection = Selections.None;
        Selections selected = Selections.None;


        public void LoadContent(ContentManager theContentManager)
        {
            gameFont = theContentManager.Load<SpriteFont>("SpriteFont1");       //loads a font
            menuTexture = theContentManager.Load<Texture2D>("mainmenu");        //loads a background for mainmenu
            gameFont2 = theContentManager.Load<SpriteFont>("SpriteFont2");
        }

        public void Update(GameTime gameTime, Player maSprite)
        {
            menuchecker(gameTime);

            if (menuState == State.Active)
            {
                selectionchecker(gameTime);
                selectedEntry(gameTime);

            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (menuState == State.Active)
            {
                spriteBatch.Draw(menuTexture, new Vector2(490, 120), Color.White);

                #region character highlight
                if (selection == Selections.Char)
                {
                    spriteBatch.DrawString(gameFont, CHAR, menuVector, Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(gameFont, CHAR, menuVector, Color.Yellow);
                }
                #endregion

                #region Move list highlight
                if (selection == Selections.MoveList)
                {
                    spriteBatch.DrawString(gameFont, LIST, new Vector2(menuVector.X, menuVector.Y+50), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(gameFont, LIST, new Vector2(menuVector.X, menuVector.Y + 50), Color.Yellow);
                }
                #endregion

                #region Options (details) hightlight
                if (selection == Selections.Options)
                {
                    spriteBatch.DrawString(gameFont, OPTN, new Vector2(menuVector.X, menuVector.Y + 100), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(gameFont, OPTN, new Vector2(menuVector.X, menuVector.Y + 100), Color.Yellow);
                }
                #endregion

                #region Exit highlight
                if (selection == Selections.Exit)
                {
                    spriteBatch.DrawString(gameFont, EXIT, new Vector2(menuVector.X, menuVector.Y + 150), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(gameFont, EXIT, new Vector2(menuVector.X, menuVector.Y + 150), Color.Yellow);
                }
                #endregion

                spriteBatch.DrawString(gameFont2, other, new Vector2(menuVector.X-64, menuVector.Y + 250), Color.Gold);

                //Draws another panel that opens up from a selected menu item after you hit enter.
                if (selected == Selections.Options) //new Vector2(790, 120)
                {
                    spriteBatch.Draw(menuTexture, new Rectangle(790, 120, 400, 400), Color.White);
                    spriteBatch.DrawString(gameFont, DETAILS, new Vector2(810, 140), Color.LightSteelBlue);
                }
                if (selected == Selections.Char)
                {
                    spriteBatch.Draw(menuTexture, new Rectangle(790, 120, 400, 400), Color.White);
                }
                if (selected == Selections.MoveList)
                {
                    spriteBatch.Draw(menuTexture, new Rectangle(790, 120, 400, 400), Color.White);
                }

            }
        }

        //keeps track of when the menu is open/closed
        public void menuchecker(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if ((keyboardState.IsKeyDown(Keys.Escape)) && (oc == false))
            {
                oc = true;
                
            }

            if ((keyboardState.IsKeyUp(Keys.Escape)) && (oc == true))
            {
                if (menuState == State.Idle && (oc == true))
                {
                    menuState = State.Active;
                    oc = false;
                }
                else if (menuState == State.Active && (oc == true))
                {
                    menuState = State.Idle;
                    oc = false;
                }
            }

        }

        //keeps track of which option is highlighted
        public void selectionchecker(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (selection == Selections.None) { selection = Selections.Char; }

            if (keyboardState.IsKeyDown(Keys.Tab) && (click == false))
            {
                click = true;
            }

            if (keyboardState.IsKeyUp(Keys.Tab) && (click == true))
            {
                if (selection == Selections.Char && (click == true))
                {
                    selection = Selections.MoveList;
                    click = false;
                }
                else if (selection == Selections.MoveList && (click == true))
                {
                    selection = Selections.Options;
                    click = false;
                }
                else if (selection == Selections.Options && (click == true))
                {
                    selection = Selections.Exit;
                    click = false;
                }
                else if (selection == Selections.Exit && (click == true))
                {
                    selection = Selections.Char;
                    click = false;
                }
                
            }

                
        }

        //tracks which menu option is being selected to be viewed
        public void selectedEntry(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            

            if (keyboardState.IsKeyDown(Keys.Enter) && (ent == false))
            {
                ent = true;
            }

            if (keyboardState.IsKeyUp(Keys.Tab) && (ent == true))
            {
                if (selection == Selections.Char && (ent == true))
                {
                    selected = Selections.Char;
                    ent = false;
                }
                else if (selection == Selections.MoveList && (ent == true))
                {
                    selected = Selections.MoveList;
                    ent = false;
                }
                else if (selection == Selections.Options && (ent == true))
                {
                    selected = Selections.Options;
                    ent = false;
                }
                else if (selection == Selections.Exit && (ent == true))
                {
                    selected = Selections.Exit;
                    ent = false;
                }
                else
                {
                    selected = Selections.None;
                }

            }


        }
    }
}
