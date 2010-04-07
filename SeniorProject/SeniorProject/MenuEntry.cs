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
    public class MenuEntry
    {
        #region Variables
        SpriteFont gameFont;
        public string text;
        private const string MENU = "Menu";
        private const string ITEM = "Items";
        #endregion
        public void LoadContent(ContentManager theContentManager)
        {
           gameFont = theContentManager.Load<SpriteFont>("SpriteFont1");     //loads a font
        }

        public MenuEntry(string text)
        {
            this.text = text;
        }

        public virtual void Draw(GameTime gameTime, Vector2 position, SpriteBatch spritebatch, bool isSelected, Color color)
        {
            if (isSelected)
            {
                spritebatch.DrawString(gameFont, text, position, Color.Fuchsia, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            }
            else
            {
                spritebatch.DrawString(gameFont, text, position, color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            }
        }


    }
}
