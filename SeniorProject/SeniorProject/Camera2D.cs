#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

//much of the code from this class is based on the tutorial found at http://arcez.com/blogs/2007/11/17/Basic2DcameraXNA.aspx

namespace SeniorProject
{
    public class Camera2D
    {
        //variables
        private Vector2 _position = Vector2.Zero;
        private float _viewportWidth = 0.0f;
        private float _viewportHeight = 0.0f;
        private float _moveSpeed = 1.5f;

        public Camera2D(GraphicsDeviceManager graphics, Vector2 position)
        {
            _viewportWidth = graphics.GraphicsDevice.Viewport.Width;    //the width of the screen
            _viewportHeight = graphics.GraphicsDevice.Viewport.Height;  //the height of the screen
            _position = position;   //the position of the object being followed (the original position?)
        }

        public void Update(GameTime gameTime, Vector2 position)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            position.X -= (_viewportWidth / 2.0f);
            position.Y -= (_viewportHeight / 2.0f);

            //finds the linear interpolation between the two vectors
            _position = Vector2.Lerp(_position, position, _moveSpeed * delta);
        }

        public Vector2 Transform(Vector2 point)
        {
            return new Vector2(
                point.X - _position.X,
                point.Y - _position.Y);
        }

        //not exactly sure whats going on here - think its trying to figure out if the object is in the current camera view
        //this method currently is never used
        public bool IsObjectVisible(Vector2 position, Texture2D obj)
        {
            if (((position.X) > _viewportWidth) || ((position.X + obj.Width) < 0.0f)) return false;
            if (((position.Y) > _viewportHeight) || ((position.Y + obj.Height) < 0.0f)) return false;

            return true;
        }
    }
}
