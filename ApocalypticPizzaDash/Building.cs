using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApocalypticPizzaDash
{
    class Building
    {
        // attributes
        private int type;
        private Rectangle rect;
        private Texture2D image;

        public Building(int type, Rectangle rect, Texture2D image)
        {
            this.type = type;
            this.rect = rect;
            this.image = image;
        }

        // properties
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }
    }
}
