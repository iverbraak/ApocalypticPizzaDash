using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ApocalypticPizzaDash
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        // general game attributes
        enum GameState { Menu, Game, GameOver }
        GameState gState, gStatePrev;
        KeyboardState kState, kStatePrev;
        double timer;
        bool isColliding, isPaused; // just for Milestone 2, will be added to attack method in Milestone 3
        Texture2D backdrop, pause, gameover;

        //player attributes
        Player player;
        const int PLAYER_HEIGHT = 23;
        const int PLAYER_WIDTH = 15;

        //animating player attacking 
        const int PLAYER_ATTACK_HEIGHT = 23;
        const int PLAYER_ATTACK_WIDTH = 21;
        int playerAttackFrame;
        int numPlayerAttackFrames = 3;
        int playerAttackFramesElapsed;
        double timePerPlayerAttackFrame = 100;

        //animating player movement
        int numPlayerFrames = 7;
        int playerFrame;
        int playerFramesElapsed;


        double timePerPlayerFrame = 100;

        // zombie attributes
        List<Zombie> zombies;
        const int ZOMBIE_HEIGHT = 23;
        const int ZOMBIE_WIDTH = 13;
        int zombieFrame;
        int numZombieFrames = 5;
        int zombieFramesElapsed;
        double timePerZombieFrame = 100;
        
        // eventually, these Vector2 objects will become attributes of the Character class,
        // but for testing purposes they're in Game1 because we're only using one zombie to
        // test stuff out
        Vector2 playerLoc, zombieLoc;


        SpriteFont font;
        int[] types;
        BinaryReader reader;
        private Texture2D background, playerImage, UI, playerAttack;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont testFont;

        // milestone 3 => replace Texture2D UI w/a list of Texture2D objects
        // b/c by default, the UI will have markers on the map for houses
        // that need pizzas. when you deliver a pizza, the marker should
        // disappear; it'll have to be a separate object from the rest of
        // the UI.

        // MORE STUFF TO DO IN THE FUTURE (not sure if milestone 3 or 4, but needed)
        // ACTUALLY IMPLEMENT LEVEL LOADING WITH THE BINARYREADER!!
        // in draw, layer order (this will be very important after level loading is implemented)
        // layer 0 = sky and road
        // layer 1 = buildings
        // layer 2 = characters (player and all zombies)

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // by default, the player isn't colliding w/anything
            isColliding = false;

            // start game out unpaused
            isPaused = false;

            // initializing the player object
            player = new Player(null, new Rectangle(0, GraphicsDevice.Viewport.Height - 75,
            PLAYER_WIDTH, PLAYER_HEIGHT), 3);

            // initializing the player's Rectangle position as a Vector2 (needed for Draw)
            playerLoc = new Vector2(player.Rect.X, player.Rect.Y);

            // initializing Zombie list and addig just one zombie for testing purposes
            zombies = new List<Zombie>();
            zombies.Add(new Zombie(null, new Rectangle(GraphicsDevice.Viewport.Width,
            GraphicsDevice.Viewport.Height - 75, ZOMBIE_WIDTH, ZOMBIE_HEIGHT), 100));

            // initializing the zombie's Rectangle position as a Vector2 (needed for Draw)
            for (int i = 0; i < zombies.Count; i++)
            {
                zombieLoc = new Vector2(zombies[0].Rect.X, zombies[0].Rect.Y);
            }

            // by default, the game starts at the menu
            gState = GameState.Menu;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // this background image is only used at the menu
            background = Content.Load<Texture2D>("title screen");

            backdrop = Content.Load<Texture2D>("backdrop");

            gameover = Content.Load<Texture2D>("gameover");

            pause = Content.Load<Texture2D>("paused");

            // now giving the player and zombie their respective sprites
            player.Image = Content.Load<Texture2D>("spritesheet");
            playerAttack = Content.Load<Texture2D>("attack2");

            for(int i = 0; i < zombies.Count; i++)
            {
                zombies[i].Image = Content.Load<Texture2D>("Zombie1");
            }
            testFont = Content.Load<SpriteFont>("Arial14Bold");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            player.Attack(kState);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (gState)
            {
                case GameState.Menu:

                    // respawn the player
                    if(player.Die())
                    {
                      player = new Player(player.Image, new Rectangle(0, GraphicsDevice.Viewport.Height - 75,
                      PLAYER_WIDTH, PLAYER_HEIGHT), 3);
                    }

                    // respawn the zombie
                    if(zombies[0].Die())
                    {
                        zombies[0] = new Zombie(zombies[0].Image, new Rectangle(GraphicsDevice.Viewport.Width,
                            GraphicsDevice.Viewport.Height - 75, PLAYER_WIDTH, PLAYER_HEIGHT), 100);
                    }

                    // when the user hits "enter", the game begins
                    kState = Keyboard.GetState();
                    if(kState.IsKeyDown(Keys.Enter) && kStatePrev.IsKeyUp(Keys.Enter))
                    {
                        gState = GameState.Game;
                    }
                    kStatePrev = kState;
                    break;

                case GameState.Game:

                    // get current keyboard state
                    kState = Keyboard.GetState();

                    // pause and unpause the game
                    if(kState.IsKeyDown(Keys.Enter) && kStatePrev.IsKeyUp(Keys.Enter))
                    {
                        if(!isPaused)
                        {
                            isPaused = true;
                        }
                        else
                        {
                            isPaused = false;
                        }
                    }

                    // set previous keyboard state
                    kStatePrev = kState;

                    // normally, run the game when it isn't paused
                    if (!isPaused)
                    {
                        // by default, no objects are colliding
                        player.IsColliding = false;
                        zombies[0].IsColliding = false;

                        // if player's attack box collides with zombie
                        if (player.AttackBox.Intersects(zombies[0].Rect) && zombies[0].CurrentHealth > 0)
                        {
                            // zombie takes damage
                            zombies[0].IsColliding = true;
                            zombies[0].Collision();

                            // zombie gets pushed back
                            if (zombies[0].Dir == Direction.MoveRight)
                            {
                                if(player.Dir == Direction.MoveRight || player.Dir == Direction.FaceRight)
                                {
                                    zombies[0].Rect = new Rectangle(zombies[0].Rect.X + 13, zombies[0].Rect.Y, zombies[0].Rect.Width, zombies[0].Rect.Height);
                                }
                                else if (player.Dir == Direction.MoveLeft || player.Dir == Direction.FaceLeft)
                                {
                                    zombies[0].Rect = new Rectangle(zombies[0].Rect.X - 13, zombies[0].Rect.Y, zombies[0].Rect.Width, zombies[0].Rect.Height);
                                }
                            }
                            if (zombies[0].Dir == Direction.MoveLeft)
                            {
                                if(player.Dir == Direction.MoveRight || player.Dir == Direction.FaceRight)
                                {
                                    zombies[0].Rect = new Rectangle(zombies[0].Rect.X + 13, zombies[0].Rect.Y, zombies[0].Rect.Width, zombies[0].Rect.Height);
                                }
                                else if (player.Dir == Direction.MoveLeft || player.Dir == Direction.FaceLeft)
                                {
                                    zombies[0].Rect = new Rectangle(zombies[0].Rect.X - 13, zombies[0].Rect.Y, zombies[0].Rect.Width, zombies[0].Rect.Height);
                                }
                            }
                        }
                        // if zombie collides with player, player takes damage
                        if (zombies[0].Rect.Intersects(player.Rect) && player.CurrentHealth > 0)
                        {
                            player.IsColliding = true;
                            player.Collision();
                        }

                        // saving the previous collision states of each object
                        zombies[0].WasColliding = zombies[0].IsColliding;
                        player.WasColliding = player.IsColliding;

                        // getting total number of frames elapsed thus far in the existence of each object 
                        playerAttackFramesElapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds / timePerPlayerAttackFrame);
                        playerFramesElapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds / timePerPlayerFrame);
                        zombieFramesElapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds / timePerZombieFrame);

                        //  on every frame, update the Vector2s to have the positions of their
                        // respective objects
                        playerLoc = new Vector2(player.Rect.X, player.Rect.Y);
                        zombieLoc = new Vector2(zombies[0].Rect.X, zombies[0].Rect.Y);

                        // when the player runs out of health, the game ends
                        if (player.Die())
                        {
                            gState = GameState.GameOver;
                        }

                        // when the zombie runs out of health, it dies
                        if (zombies[0].CurrentHealth <= 0)
                        {
                            zombies[0].Rect = Rectangle.Empty;
                        }

                        // move player normally if not attacking
                        if (!player.Attack(kState))
                        {
                            // disable attack box
                            player.AttackBox = Rectangle.Empty;
                            
                            // handle input to move player
                            player.Move(kState, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height - 75, GraphicsDevice.Viewport.Height - 120);

                            // animate player
                            switch (player.Dir)
                            {
                                // when the player is standing still, only draw frame 0 (standing idle)
                                case Direction.FaceLeft:
                                    playerFrame = 0;
                                    break;
                                case Direction.FaceRight:
                                    playerFrame = 0;
                                    break;
                                // otherwise, loop through the walk cycle
                                default:
                                    playerFrame = playerFramesElapsed % numPlayerFrames + 1;
                                    break;
                            }
                        }
                        else
                        {
                            // animate the player attacking
                            playerAttackFrame = 0;
                            playerAttackFrame = playerAttackFramesElapsed % numPlayerAttackFrames + 1;
                        }

                        // move and animate the zombie
                        zombies[0].Move(GraphicsDevice.Viewport.Width);
                        zombieFrame = zombieFramesElapsed % numZombieFrames + 1;
                        
                    }
                    break;

                case GameState.GameOver:

                    // hit "enter" to return to the menu
                    kState = Keyboard.GetState();
                    if(kState.IsKeyDown(Keys.Enter) && kStatePrev.IsKeyUp(Keys.Enter))
                    {
                        gState = GameState.Menu;
                        timer = 0;
                    }
                    kStatePrev = kState;
                    break;
            }

            // set the previous game state
            gStatePrev = gState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();

            // when not paused
            if(!isPaused)
            {
                switch (gState)
                {
                    case GameState.Menu:

                        // draw menu graphic
                        spriteBatch.Draw(background, new Rectangle(0, 0, 800, 450), Color.White);

                        break;

                    case GameState.Game:

                        // draw the in-game backdrop
                        spriteBatch.Draw(backdrop, new Rectangle(0, 0, 800, 450), Color.White);

                        // draw the player's health and zombie's health
                        spriteBatch.DrawString(testFont, "Player health: " + player.CurrentHealth, new Vector2(0, 0), Color.Black);
                        spriteBatch.DrawString(testFont, "Zombie health: " + zombies[0].CurrentHealth, new Vector2(GraphicsDevice.Viewport.Width - 200, 0), Color.Black);
                        
                        // test if player is colliding with zombie on current frame
                        // when collision occurs,  turn player red
                        if (player.Rect.Intersects(zombies[0].Rect))
                        {
                            player.Color = Color.Red;
                        }
                        else
                        {
                            player.Color = Color.White;
                        }

                        // when attack box collides with zombie, turn zombie red
                        if(player.AttackBox.Intersects(zombies[0].Rect))
                        {
                            zombies[0].Color = Color.Red;
                        }
                        else
                        {
                            zombies[0].Color = Color.White;
                        }


                        // draw the attack box (for debugging purposes, TO BE REMOVED IN FINAL GAME)
                        if(player.Attack(kState))
                        {
                            spriteBatch.Draw(gameover, new Rectangle(player.AttackBox.X, player.AttackBox.Y, player.AttackBox.Width, player.AttackBox.Height), Color.White);
                        }
                        

                        // draw the player
                        if (player.CurrentHealth > 0)
                        {
                            // draw the player's attack
                            if (player.Attack(kState))
                            {
                                if (player.Dir == Direction.FaceLeft || player.Dir == Direction.MoveLeft)
                                {
                                    spriteBatch.Draw(playerAttack, new Vector2(player.Rect.X - 6, player.Rect.Y), new Rectangle(playerAttackFrame * PLAYER_ATTACK_WIDTH, 0, PLAYER_ATTACK_WIDTH, PLAYER_ATTACK_HEIGHT), player.Color, 0, Vector2.Zero, 1,
                                        SpriteEffects.FlipHorizontally, 0);
                                }
                                else if (player.Dir == Direction.FaceRight || player.Dir == Direction.MoveRight)
                                {
                                    spriteBatch.Draw(playerAttack, new Vector2(player.Rect.X, player.Rect.Y), new Rectangle(playerAttackFrame * PLAYER_ATTACK_WIDTH, 0, PLAYER_ATTACK_WIDTH, PLAYER_ATTACK_HEIGHT), player.Color);
                                }
                            }
                            // draw the player moving
                            else
                            {
                                if (player.Dir == Direction.FaceLeft || player.Dir == Direction.MoveLeft)
                                {
                                    spriteBatch.Draw(player.Image, playerLoc, new Rectangle(playerFrame * PLAYER_WIDTH, 0, PLAYER_WIDTH, PLAYER_HEIGHT), player.Color, 0, Vector2.Zero, 1,
                                        SpriteEffects.FlipHorizontally, 0);
                                }
                                else if (player.Dir == Direction.FaceRight || player.Dir == Direction.MoveRight)
                                {
                                    spriteBatch.Draw(player.Image, playerLoc, new Rectangle(playerFrame * PLAYER_WIDTH, 0, PLAYER_WIDTH, PLAYER_HEIGHT), player.Color);
                                }
                            }
                        }

                        // draw the zombie
                        if (zombies[0].CurrentHealth > 0)
                        {
                            if (zombies[0].Dir == Direction.MoveLeft)
                            {
                                spriteBatch.Draw(zombies[0].Image, zombieLoc, new Rectangle(zombieFrame * ZOMBIE_WIDTH, 0, ZOMBIE_WIDTH, ZOMBIE_HEIGHT), zombies[0].Color, 0, Vector2.Zero, 1,
                                    SpriteEffects.FlipHorizontally, 0);
                            }
                            else if (zombies[0].Dir == Direction.MoveRight)
                            {
                                spriteBatch.Draw(zombies[0].Image, zombieLoc, new Rectangle(zombieFrame * ZOMBIE_WIDTH, 0, ZOMBIE_WIDTH, ZOMBIE_HEIGHT), zombies[0].Color);
                            }

                        }
                        break;

                    case GameState.GameOver:
                        // draw the game over screen and prompting player to try again
                        spriteBatch.Draw(gameover, new Rectangle(0, 0, 800, 450), Color.White);
                        break;
                }
            }
            // draw the pause screen
            else
            {
                GraphicsDevice.Clear(Color.White);
                spriteBatch.Draw(pause, new Rectangle(0, 0, 800, 450), Color.White);
            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
