#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace ICGGSAssignment

{
    /// <summary>
    /// ICGGS XNA Assignment 2010
    /// Assignment programmed by Josh Dadak
    /// Student No: 005578
    /// Staffordshire University
    /// </summary>

    public class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;

        // Width and Height of the game window, in pixels
        private int gameWidth;
        private int gameHeight;

        // Pacman variables
        private Point frameSize = new Point(64, 64);    // Pacman image size
        private Point currentFrame = new Point(1, 0);   // Start frame
        private Point sheetSize = new Point(2, 4);      // Spritesheet size
        private Vector2 pacmanPos;                      // Pacman position in pixels
        private int pacmanSpeed = 5;                    // Pacman movement speed in pixels

        // Game
        SpriteBatch spriteBatch;
        Texture2D munchie1, munchie2, pacman, enemy1,enemy2,enemy3,enemy4;
        //Scores
        int score = 0;
        //lives
        int lives = 3;
        //reset count
        int resetcount = 1;
        // bonus lives
        int bonus = 1;
        

        // Sounds
        SoundEffect cookiesong;
        SoundEffectInstance cookiesonginstance;
        SoundEffect ouch;
        SoundEffectInstance ouchinstance;

        //fonts
        SpriteFont highscoreFont;
        SpriteFont livesFont;

        // Random number generator
        Random rand = new Random();

        // Total number of munchies 
        private int noOfMunchies = 20;

        Vector2[] munchiePos;
        private int munchieSize;
        private int[] munchieAnimationCount;

        private int timeSinceLastFrame = 0;
        private int milliSecondsPerFrame = 500; // 2 Frames Per Second (fps)

        //enemy
        // amount of enemies
        int enemyCount = 1;
        Vector2[] enemyPos;
        private int enemySize;
        private int[] enemyAnimationCount;

        private int timeSinceLastFrame2 = 0;
        private int milliSecondsPerFrame2 = 500; // 2 Frames Per Second (fps)

        #endregion

        #region Initialization
        public GameplayScreen()
        {
            // Setup on and off transition times
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // Setup munchies
            munchiePos = new Vector2[noOfMunchies];
            munchieAnimationCount = new int[noOfMunchies];

            // Setup munchies
            enemyPos = new Vector2[enemyCount];
            enemyAnimationCount = new int[enemyCount];
        }

        private void resetGame()
        {
            // Setup pacman
            pacmanPos = new Vector2((gameWidth / 2) - (frameSize.X / 2),
                                    (gameHeight / 2) - (frameSize.Y / 2));

            // Generate random munchies
            for (int i = 0; i < noOfMunchies; i++)
            {
                // Random Positions
                munchiePos[i].X = Math.Max(0, rand.Next(gameWidth) - munchieSize);
                munchiePos[i].Y = Math.Max(0, rand.Next(gameHeight) - munchieSize);


                // Generate random enemy
                for (int ii = 1; ii < enemyCount; ii++)
                {
                    // Random Positions
                    enemyPos[ii].X = Math.Max(0, rand.Next(gameWidth) - enemySize);
                    enemyPos[ii].Y = Math.Max(0, rand.Next(gameHeight) - enemySize);

                    // TODO: Add in extra code here so that munchies do not overlap!

                    // Random animation frame
                    munchieAnimationCount[i] = rand.Next(2);
                    enemyAnimationCount[i] = rand.Next(2);
                }
                cookiesonginstance.Volume = 0.75f;
                cookiesonginstance.Play();
            }
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Save local copy of SpriteBatch, which can be used to draw textures.
            spriteBatch = ScreenManager.SpriteBatch;

            // Load textures
            munchie1 = content.Load<Texture2D>("Sprites/Munchie1");
            munchie2 = content.Load<Texture2D>("Sprites/Munchie2");
            munchieSize = munchie1.Width;

            pacman = content.Load<Texture2D>("Sprites/pacman");

            enemy1 = content.Load<Texture2D>("Sprites/enemy1");
            enemy2 = content.Load<Texture2D>("Sprites/enemy2");

            //load highscore font
            highscoreFont = content.Load<SpriteFont>(@"Fonts\highscoreFont");
            //Load lives font
            livesFont = content.Load<SpriteFont>(@"Fonts\livesFont");

            // Load Sounds
            cookiesong = content.Load<SoundEffect>("Sounds/cookiesong");
            cookiesonginstance = cookiesong.CreateInstance();
            ouch = content.Load<SoundEffect>("Sounds/ouch");
            ouchinstance = ouch.CreateInstance();
                        
            // Get screen width and height
            gameWidth = ScreenManager.GraphicsDevice.Viewport.Width;
            gameHeight = ScreenManager.GraphicsDevice.Viewport.Height;

            // Setup game
            resetGame();

            // A real game would probably have more content than this, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);



            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                // Game logic
                // Check for pacman hitting walls
                if (pacmanPos.X + frameSize.X > gameWidth)
                {
                    // Pacman hit right wall
                    pacmanPos.X = gameWidth - frameSize.X;
                }
                if (pacmanPos.X < 0)
                {
                    // Pacman hit Left wall
                    pacmanPos.X = 0;
                }
                if (pacmanPos.Y < 0)
                {
                    // Pacman hit Top wall
                    pacmanPos.Y = 0;
                }
                if (pacmanPos.Y + frameSize.Y > gameHeight)
                {
                    //Pacman Hit Bottom Wall
                    pacmanPos.Y = gameHeight - frameSize.Y;
                }

                // Circle Collision Detection
                // Check for pacman hitting munchie
                float radiusMunchie;
                float radiusPacman;
                float sum;

                float a;
                float b;
                float c;

                for (int i = 0; i < noOfMunchies; i++)
                {
                    radiusMunchie = munchieSize / 2.0f;
                    radiusPacman = frameSize.X / 2.0f;

                    sum = radiusMunchie + radiusPacman;
                    float sumSquared = sum * sum;

                    a = munchiePos[i].Y - pacmanPos.Y;
                    b = munchiePos[i].X - pacmanPos.X;
                    c = (a * a) + (b * b);

                    if (sumSquared > c)
                    {
                        munchiePos[i].X = -200;
                        score = score + 1;
                    }
                }

                if (score == (noOfMunchies * resetcount))
                {
                    resetcount = resetcount + 1;
                    //enemyCount = enemyCount + 1;
                    cookiesonginstance.Stop();
                    resetGame();
                }
                if (score == 100 * bonus)
                {
                    bonus = bonus + 1;
                    lives = lives + 1;
                }

                //collision with enemy
                float radiusenemy;
                float radiusPacman2;
                float sum2;

                float a2;
                float b2;
                float c2;

                for (int i = 0; i < enemyCount; i++)
                {
                    radiusenemy = enemySize / 2.0f;
                    radiusPacman = frameSize.X / 2.0f;

                    sum2 = radiusenemy + radiusPacman;
                    float sumSquared2 = sum2 * sum2;

                    a2 = enemyPos[i].Y - pacmanPos.Y;
                    b2 = enemyPos[i].X - pacmanPos.X;
                    c2 = (a2 * a2) + (b2 * b2);

                    if (sumSquared2 > c2)
                    {
                        ouchinstance.Play();
                        lives = lives - 1;
                        pacmanPos.X = pacmanPos.X + 100;
                        pacmanPos.Y = pacmanPos.Y + 100;
                    }
                }
                if (lives == 0)
                {
                    score = 0;
                    resetGame();
                    lives = 3;
                }

                // Animations
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > milliSecondsPerFrame)
                {
                    // Reset time
                    timeSinceLastFrame -= milliSecondsPerFrame;

                    if (currentFrame.X == 0)
                    {   
                        currentFrame.X = 1;
                    }
                    else
                    {
                        currentFrame.X = 0;
                    }

                    // Next frame
                    for (int i = 0; i < noOfMunchies; i++)
                    {
                        if (munchieAnimationCount[i] == 0)
                        {
                            munchieAnimationCount[i] = 1;
                        }
                        else
                        {
                            munchieAnimationCount[i] = 0;
                        }
                    }
                    //next frame2
                    for (int i = 0; i < enemyCount; i++)
                    {
                        if (enemyAnimationCount[i] == 0)
                        {
                            enemyAnimationCount[i] = 1;
                        }
                        else
                        {
                            enemyAnimationCount[i] = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Pacman move keys
                if ((gamePadState.DPad.Right == ButtonState.Pressed) ||
                    (keyboardState.IsKeyDown(Keys.Right)))
                {
                    // Move pacman Right
                    pacmanPos.X += pacmanSpeed;
                    //Change Sprite
                    currentFrame.Y = 0;
                }
                if ((gamePadState.DPad.Left == ButtonState.Pressed) ||
                     (keyboardState.IsKeyDown(Keys.Left)))
                {
                    //Move Pacman left
                    pacmanPos.X -= pacmanSpeed;
                    //Change Sprite
                    currentFrame.Y = 2;
                }
                if ((gamePadState.DPad.Down == ButtonState.Pressed) ||
                     (keyboardState.IsKeyDown(Keys.Down)))
                {
                    //Move Pacman down
                    pacmanPos.Y += pacmanSpeed;
                    //Change Sprite
                    currentFrame.Y = 1;
                }
                if ((gamePadState.DPad.Up == ButtonState.Pressed) ||
                     (keyboardState.IsKeyDown(Keys.Up)))
                {
                    //Move Pacman Up
                    pacmanPos.Y -= pacmanSpeed;
                    //Change Sprite
                    currentFrame.Y = 3;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Clear screen
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Draw munchies
            for (int i = 0; i < noOfMunchies; i++)
            {
                if (munchieAnimationCount[i] == 0)
                {
                    // Draw frame 1
                    spriteBatch.Draw(munchie1, munchiePos[i], Color.White);
                }
                else
                {
                    // Draw frame 2
                    spriteBatch.Draw(munchie2, munchiePos[i], Color.White);
                }
            }
            // Draw enemy
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemyAnimationCount[i] == 0)
                {
                    // Draw frame 1
                    spriteBatch.Draw(enemy1, enemyPos[i], Color.White);
                }
                else
                {
                    // Draw frame 2
                    spriteBatch.Draw(enemy2, enemyPos[i], Color.White);
                }
            }

            // Draw Pacman
            {

                spriteBatch.Draw(pacman, pacmanPos,
                                 new Rectangle(currentFrame.X * frameSize.X,
                                               currentFrame.Y * frameSize.Y,
                                               frameSize.X,
                                               frameSize.Y),
                                 Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            // draw scores & lives
            {
                spriteBatch.DrawString(highscoreFont, "Score: " + score,
                    new Vector2(10, 10), Color.GreenYellow, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                spriteBatch.DrawString(livesFont, "Lives: " + lives,
                    new Vector2(100, 10), Color.GreenYellow, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

                spriteBatch.End();
            }
            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        #endregion

    }
}
