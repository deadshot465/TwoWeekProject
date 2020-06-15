using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using TwoWeekProject.Content;

namespace TwoWeekProject
{
    public struct STAGE_DATA
    {
        public Vector2 pos;
        public int type;
        public Texture2D side;

    }


    public class Game1 : Game
    {
        public const int WIDTH = 1120;
        public const int HEIGHT = 630;
        private const bool FULL_SCREEN = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Test
        private Texture2D backGround1;
        private Texture2D backGround2;
        private Texture2D backGround3;
        private Vector2 backGround1Pos;
        private Vector2 backGround2Pos;
        private Vector2 backGround3Pos;

        private Texture2D player;
        private Vector2 playerPos;
        private int playerAnimation;
        private float playerSpeed;
        private int fixedPlayerAnimationTimer;

        public STAGE_DATA[] stageData;

        private int timer;
        private int score;

        private SpriteFont font;
        private Song song;

        enum STAGE_TYPE
        {
            NOTHING,
            UPSIDE,
            DOWNSIDE,
            SPECIAL
        };


        private Camera _camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.IsFullScreen = FULL_SCREEN;
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferHeight = HEIGHT;
            _graphics.PreferredBackBufferWidth = WIDTH;
            _graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            backGround1Pos = new Vector2(1120 * 0 - 500, 0);
            backGround2Pos = new Vector2(1120 * 1 - 500, 0);
            backGround3Pos = new Vector2(1120 * 2 - 500, 0);

            playerPos = new Vector2(50, 325);
            playerAnimation = 0;
            playerSpeed = 6;

            score = 100;

            for (int i = 0; i < 64; i++)
            {
                SetStageData(stageData[i]);
            }        

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _camera = new Camera();

            // TODO: use this.Content to load your game content here
            backGround1 = Content.Load<Texture2D>("background1");
            backGround2 = Content.Load<Texture2D>("background2");
            backGround3 = Content.Load<Texture2D>("background3");

            player = Content.Load<Texture2D>("player");

            for (int i = 0; i < 64; i++)
            {
                stageData[i].side = Content.Load<Texture2D>("side1");
            }

            font = Content.Load<SpriteFont>("timer");
            song = Content.Load<Song>("test");
            MediaPlayer.Play(song);
        }

        protected override void Update(GameTime gameTime)
        {
            timer++;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.R)) Initialize();

            if (playerPos.X - backGround1Pos.X >= WIDTH + 500) backGround1Pos.X = playerPos.X + WIDTH * 2 - 500;
            if (playerPos.X - backGround2Pos.X >= WIDTH + 500) backGround2Pos.X = playerPos.X + WIDTH * 2 - 500;
            if (playerPos.X - backGround3Pos.X >= WIDTH + 500) backGround3Pos.X = playerPos.X + WIDTH * 2 - 500;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                playerPos.Y -= 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                playerPos.Y += 2;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                playerAnimation = 10;
                fixedPlayerAnimationTimer = 20;
                playerSpeed = 4;
            }

            if (playerAnimation == 10)
            {
                fixedPlayerAnimationTimer--;

                if (fixedPlayerAnimationTimer <= 0)
                {
                    playerAnimation = 0;
                    playerSpeed = 6;
                }
            }
            else if (timer % 10 == 0)
            {
                playerAnimation++;
                if (playerAnimation == 10) playerAnimation = 0;

            }

            if (playerPos.Y <= 300)
            {
                playerPos.Y = 300;
            }
            if (playerPos.Y >= 375)
            {
                playerPos.Y = 375;
            }

            playerPos.X += playerSpeed;

            _camera.Follow(playerPos);

            if(CheckCollision())
            {
                score--;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            TimeSpan playTime = MediaPlayer.PlayPosition;
            TimeSpan songTime = song.Duration;

            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            _spriteBatch.Draw(backGround1, backGround1Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            _spriteBatch.Draw(backGround2, backGround2Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            _spriteBatch.Draw(backGround3, backGround3Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            for (int i = 0; i < 64; i++)
            {
                if (stageData[i].type == 1 && stageData[i].type == 3)
                    _spriteBatch.Draw(stageData[i].side, stageData[i].pos, new Rectangle(0, 0, 150, 150), Color.White);
            }
            _spriteBatch.Draw(player, playerPos, new Rectangle(150 * playerAnimation, 0, 150, 150), Color.White);
            for (int i = 0; i < 64; i++)
            {
                if (stageData[i].type == 2)
                    _spriteBatch.Draw(stageData[i].side, stageData[i].pos, new Rectangle(0, 0, 150, 150), Color.White);
            }

            //DEBUG
            _spriteBatch.DrawString(font, "" + timer / 60, new Vector2(playerPos.X, 50), Color.White);
            _spriteBatch.DrawString(font, "" + score, new Vector2(playerPos.X, 100), Color.White);
            _spriteBatch.DrawString(font, GetHumanReaderTime(playTime) + "/" + GetHumanReaderTime(songTime), new Vector2(playerPos.X, 150), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        public string GetHumanReaderTime(TimeSpan timeSpan)
        {
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (seconds < 10) return minutes + ":0" + seconds;
            else return minutes + ":" + seconds;
        }

        public bool CheckCollision()
        {
            for (int i = 0; i < 64; i++)
            {
                if (playerPos.X - stageData[i].pos.X < 50 && playerPos.X +50 >= stageData[i].pos.X)
                {
                    if (stageData[i].type == 1 || stageData[i].type == 2) return true;
                    if (stageData[i].type == 3 && playerAnimation != 10) return true;
                }    
            }
            return false;
        }

        public void SetStageData(STAGE_DATA stageData)
        {
            Random ran = new Random();
            
            for(int i = 0; i < 64; i++)
            {
                stageData.type = 2;
                if (stageData.type == 1) stageData.pos.Y = 325;
                if (stageData.type == 2) stageData.pos.Y = 375;
                if (stageData.type == 3) stageData.pos.Y = 350;

                stageData.pos.X = 700 + i * 100;
            }

        }

    }
}
