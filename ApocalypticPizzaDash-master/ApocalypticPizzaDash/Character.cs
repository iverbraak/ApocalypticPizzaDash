using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApocalypticPizzaDash
{
    /// <summary>
    /// This class serves as  a parent class for the Zombie and Player classes. Since the zombie and player behave similarly, they share similar attributes.
    /// Some methods are set to virtual so that they can be overridden by either the Player or Zombie classes
    /// </summary>

    // this enum declaration is outside of the class so that other classes can know what a Direction is
    public enum Direction { FaceLeft, FaceRight, MoveLeft, MoveRight } //directional states that the character could have

    class Character
    {
        // attributes
        private Rectangle rect; //A rectangle object that will store the charatcer's X, Y, width, height, etc.
        private Direction dir;
        private Texture2D image; // the character's sprite
        private int currentHealth, totalHealth;
        protected bool isAlive, isColliding, wasColliding;
        private Color color;

        public Character(Texture2D image, Rectangle rect, int health)
        {
            this.image = image;
            this.rect = rect;
            currentHealth = health;
            totalHealth = health;
            color = Color.White;

            isColliding = false;
            wasColliding = false;
        }

        // properties
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        public Direction Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        public int TotalHealth
        {
            get { return totalHealth; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public bool IsColliding
        {
            get { return isColliding; }
            set { isColliding = value; }
        }

        public bool WasColliding
        {
            get { return wasColliding; }
            set { wasColliding = value; }
        }
        
        /// <summary>
        /// Spawns the character and sets their health and life
        /// </summary>
        public virtual void SpawnCharacter()
        {
            currentHealth = 100;
            totalHealth = 100;
            isAlive = true;
            dir = Direction.FaceRight;
        }

        /// <summary>
        /// Draws the character on each frame
        /// </summary>
        /// <param name="spr"></param>
        public void Draw(SpriteBatch spr)
        {
            // the character is only drawn if it's alive
            if (isAlive)
            {
                spr.Draw(image, rect, Color);
            }
        }

        /// <summary>
        /// Makes the character take damage
        /// </summary>
        public virtual void TakeDamage()
        {
            // we'll change values later of how much damage is dealt
            currentHealth--;

            // in player version, make player invulnerable
        }

        /// <summary>
        /// Returns true if the character is dead
        /// </summary>
        public bool Die()
        {
            // by default, the character is alive
            bool status = false;

            // when there is no more currentHealth, the character dies
            if(currentHealth <= 0)
            {
                status = true;
                Rect = Rectangle.Empty;
            }
            return status;
        }

        /// <summary>
        /// Character only takes damage if it is the first frame in which it is colliding with another character
        /// </summary>
        public virtual void Collision()
        {
            if(isColliding && !wasColliding)
            {
                Color = Color.Red;
                CurrentHealth--;
            }
        }
    }
}
