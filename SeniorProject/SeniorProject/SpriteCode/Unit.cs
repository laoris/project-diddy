using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SeniorProject
{
    class Unit
    {
        #region Fields

        private double facing; // angle that represents (in bearing) the facing of the unit
        private Vector2 position;
        private Texture2D mahGraphic;
        private const float moveSpeed = 2.0f;
        private const float turnSpeed = ((float)Math.PI/180.0f);

        #endregion

        #region Properties

        #endregion

        #region Initialization

        public Unit()
        {
            position = Vector2.Zero;
        }

        public Unit(Vector2 initPosition, Texture2D texture)
        {
            position = initPosition;
            mahGraphic = texture;
        }

        #endregion

        #region Update/Draw

        public void Draw(GameTime gameTime, SpriteBatch spritebatch)
        {
            spritebatch.Draw(mahGraphic, position, Color.White);
        }

        #endregion

        #region HandleInput

        /*
        public void HandleInput(InputState input)
        {
            
        }
         * */

        #endregion


        #region Public Methods


        public void moveLeft()
        {
            facing -= turnSpeed;
        }

        public void moveRight()
        {
            facing += turnSpeed;
        }

        public void moveForward()
        {
            position.X += (float)(moveSpeed * (float)Math.Sin(facing));
            position.Y += (float)(moveSpeed * (float)Math.Cos(facing));
        }

        public void moveBack()
        {
            position.X -= (float)moveSpeed * (float)Math.Sin(facing);
            position.Y -= (float)moveSpeed * (float)Math.Cos(facing);
        }

        #endregion

    }
}
