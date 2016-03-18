using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApocalypticPizzaDash
{
    class Zombie : Character
    {
        // attributes
        private int index;
        private Random rand;
        // design note: for milestone 3, make all frame animation variables attributes and params of constructor

        public Zombie(Texture2D image, Rectangle rect, int health):base(image, rect, health)
        {
            rand = new Random();
            index = rand.Next(0, 2);
        }

        /// <summary>
        /// randomizes the zombies' movements
        /// </summary>
        /// <param name="screenWidth"></param>
        public void Move(int screenWidth)
        {
            // moving the zombie left
            if(index == 0)
            {
                Dir = Direction.MoveLeft;
                Rect = new Rectangle(Rect.X - 1, Rect.Y, Rect.Width, Rect.Height);

                // when zombie hits boundary, it moves right
                if (Rect.X <= 0)
                {
                    Dir = Direction.MoveRight;
                    index = 1;
                }
            }
            // moving the zombie right
            else if (index == 1)
            {
                Dir = Direction.MoveRight;
                Rect = new Rectangle(Rect.X + 1, Rect.Y, Rect.Width, Rect.Height);

                // when zombie hits boundary, it moves left
                if (Rect.X + Rect.Width >= screenWidth)
                {
                    Dir = Direction.MoveLeft;
                    index = 0;
                }
            }
        }

        /// <summary>
        /// overrides the Collision method such that the zombie will not glow red
        /// </summary>
        public override void Collision()
        {
            // upon collision...
            if(isColliding && !wasColliding)
            {
                // zombie's color doesn't change
                Color = Color.White;

                // but health is still decremented
                CurrentHealth--;
            }
        }
    }
}
