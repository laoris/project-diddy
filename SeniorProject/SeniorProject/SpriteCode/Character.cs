using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//much of the code for this class is based on the tutorial from http://www.xnadevelopment.com/tutorials.shtml


namespace SeniorProject
{
    class Character : Sprite
    {
        /*
        const string CHARACTER_ASSETNAME = "BabyDK2";
        const int START_POSITION_X = 125;
        const int START_POSITION_Y = 245;
        const int CHARACTER_SPEED = 160;
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;

        //assigns a state
        enum State
        {
            Walking
        }
        //default state is set to walking
        State mCurrentState = State.Walking;

        //direction and speed for character
        Vector2 mDirection = Vector2.Zero;
        Vector2 mSpeed = Vector2.Zero;

        //angle of character
        double mAngle = 90;


        //for the last action from the keyboard
        KeyboardState mPreviousKeyboardState;



        public void LoadContent(ContentManager theContentManager)
        {
            
            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.Loadcontent(theContentManager, CHARACTER_ASSETNAME);
        }

        public void Update(GameTime theGameTime)
        {
            //gets current state of the keyboard
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();
            //calls method below with passed in key
            UpdateMovement(aCurrentKeyboardState);
            //stores the current keyboard state as the last keyboard state
            mPreviousKeyboardState = aCurrentKeyboardState;
            //call update method from Sprite class
            base.Update(theGameTime, mSpeed, mDirection, mAngle);
        }

        private void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            //check that it is okay for the player to be walking
            if(mCurrentState == State.Walking)
            {
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;

                if(aCurrentKeyboardState.IsKeyDown(Keys.A) == true)
                {
                    mSpeed.X = CHARACTER_SPEED;
                    mDirection.X = MOVE_LEFT;
                }
                else if(aCurrentKeyboardState.IsKeyDown(Keys.D) == true)
                {
                    mSpeed.X = CHARACTER_SPEED;
                    mDirection.X = MOVE_RIGHT;
                }

                if(aCurrentKeyboardState.IsKeyDown(Keys.W) == true)
                {
                    mSpeed.Y = CHARACTER_SPEED;
                    mDirection.Y = MOVE_UP;
                }
                else if(aCurrentKeyboardState.IsKeyDown(Keys.S) == true)
                {
                    mSpeed.Y = CHARACTER_SPEED;
                    mDirection.Y = MOVE_DOWN;
                }
            }
        }

        */
    }
}
