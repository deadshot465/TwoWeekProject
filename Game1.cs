using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using TwoWeekProject.Content;

namespace TwoWeekProject
{
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
        private int fixedPlayerTimer;
        private bool fixedPlayerFlag;
        
        private Texture2D side;
        private const int upSide = 325;
        private const int downSide = 375;

        private int timer;
        private int score;
        private int[,] musicData;
        private SpriteFont font;
        private Song song;
        private const int musicDataMax = 30;
        private int speed;

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
            
            backGround1Pos = new Vector2(1120 * 0, 0);
            backGround2Pos = new Vector2(1120 * 1, 0);
            backGround3Pos = new Vector2(1120 * 2, 0);

            playerPos = new Vector2(50, 350);
            playerAnimation = 0;
            fixedPlayerTimer = 0;
            fixedPlayerFlag = false;


            score = 100;
            speed = 4;

            musicData = new int[musicDataMax, 2]{
                {700, upSide},//1
                {800, downSide},//2
                {900, upSide},//3
                {0, 0},//4
                {0, 0},//5
                {0, 0},//6
                {1300, upSide},//7
                {1400, downSide},//8
                {1500, upSide},//9
                {1600, downSide},//10
                {1700, upSide},//1
                {0, 0},//2
                {1900, upSide},//3
                {2000, downSide},//4
                {2100, upSide},//5
                {0, 0},//6
                {0, 0},//7
                {2400, downSide},//8
                {2500, upSide},//9
                {2600, downSide},//10
                {2700, upSide},//1
                {2800, downSide},//2
                {0, 0},//3
                {0, 0},//4
                {3100, upSide},//5
                {0, 0},//6
                {0, 0},//7
                {3400, downSide},//8
                {0, 0},//9
                {0, 0},//10
            };
            
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
            
            side = Content.Load<Texture2D>("side1");
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
            
            if (playerPos.X - backGround1Pos.X >= WIDTH + 150) backGround1Pos.X = playerPos.X + WIDTH * 2 - 150; 
            if (playerPos.X - backGround2Pos.X >= WIDTH + 150) backGround2Pos.X = playerPos.X + WIDTH * 2 - 150;
            if (playerPos.X - backGround3Pos.X >= WIDTH + 150) backGround3Pos.X = playerPos.X + WIDTH * 2 - 150;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                playerPos.Y = 325;
                fixedPlayerTimer = 0;
                fixedPlayerFlag = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                playerPos.Y = 375;
                fixedPlayerTimer = 0;
                fixedPlayerFlag = true;
            }
            else if ( fixedPlayerTimer >= 30 && fixedPlayerFlag)
            {
                playerPos.Y = 350;
                fixedPlayerTimer = 0;
                fixedPlayerFlag = false;
            }
            fixedPlayerTimer++;
            
            if (timer % 10 == 0)
            {
                playerAnimation++;
                if (playerAnimation == 10) playerAnimation = 0;
            }

            playerPos.X += speed;

            _camera.Follow(playerPos);

            


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            TimeSpan playTime = MediaPlayer.PlayPosition;
            TimeSpan songTime = song.Duration;

            _spriteBatch.Begin(transformMatrix:_camera.Transform);
            
            _spriteBatch.Draw(backGround1, backGround1Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            _spriteBatch.Draw(backGround2, backGround2Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            _spriteBatch.Draw(backGround3, backGround3Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            for (int i = 0; i < musicDataMax; i++)
            {
                if(musicData[i,1] == upSide)
                    _spriteBatch.Draw(side, new Vector2(musicData[i, 0], upSide), new Rectangle(0, 0, 150, 150), Color.White);
            }

                _spriteBatch.Draw(player, playerPos, new Rectangle(150 * playerAnimation, 0, 150, 150), Color.White);
            
            for (int i = 0; i < musicDataMax; i++)
            {
                if (musicData[i, 1] == downSide)
                    _spriteBatch.Draw(side, new Vector2(musicData[i, 0], downSide), new Rectangle(0, 0, 150, 150), Color.White);
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
    }
}
