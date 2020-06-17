using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using TwoWeekProject.Content;

namespace TwoWeekProject
{
    public struct StageData
    {
        public Vector2 Position;
        public int Type;
        public Texture2D Side;
    }


    public class Game1 : Game
    {
        private const int Width = 1120;
        private const int Height = 630;
        private const bool FullScreen = false;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Test
        private Texture2D _background1;
        private Texture2D _background2;
        private Texture2D _background3;
        private Texture2D _side1;
        private Texture2D _player;
        
        private Vector2 _backGround1Pos = new Vector2(1120 * 0 - 500, 0);
        private Vector2 _backGround2Pos = new Vector2(1120 * 1 - 500, 0);
        private Vector2 _backGround3Pos = new Vector2(1120 * 2 - 500, 0);
        private Vector2 _playerPos = new Vector2(50, 325);
        
        private int _playerAnimation = 0;
        private float _playerSpeed = 6.0f;
        private int _fixedPlayerAnimationTimer;

        private List<StageData> _stageData = new List<StageData>();

        private int _timer;
        private int _score = 100;
        private bool _isGameOver = false;

        private SpriteFont _font;
        private Song _song;
        private Camera _camera;
        private Random _rng = new Random();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.IsFullScreen = FullScreen;
            _graphics.PreferMultiSampling = true;
            _graphics.PreferredBackBufferHeight = Height;
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _camera = new Camera();

            // TODO: use this.Content to load your game content here
            _background1 = Content.Load<Texture2D>("background1");
            _background2 = Content.Load<Texture2D>("background2");
            _background3 = Content.Load<Texture2D>("background3");

            _player = Content.Load<Texture2D>("player");
            _side1 = Content.Load<Texture2D>("side1");
            
            SetStageData();

            _font = Content.Load<SpriteFont>("timer");
            _song = Content.Load<Song>("test");
            MediaPlayer.Play(_song);
        }

        protected override void Update(GameTime gameTime)
        {
            _timer++;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (Keyboard.GetState().IsKeyDown(Keys.R))
                Initialize();

            if (_playerPos.X - _backGround1Pos.X >= Width + 510) _backGround1Pos.X = _playerPos.X + Width * 2 - 510;
            if (_playerPos.X - _backGround2Pos.X >= Width + 510) _backGround2Pos.X = _playerPos.X + Width * 2 - 510;
            if (_playerPos.X - _backGround3Pos.X >= Width + 510) _backGround3Pos.X = _playerPos.X + Width * 2 - 510;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _playerPos.Y -= 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _playerPos.Y += 2;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _playerAnimation = 10;
                _fixedPlayerAnimationTimer = 20;
                _playerSpeed = 4;
            }

            if (_playerAnimation == 10)
            {
                _fixedPlayerAnimationTimer--;

                if (_fixedPlayerAnimationTimer <= 0)
                {
                    _playerAnimation = 0;
                    _playerSpeed = 6;
                }
            }
            else if (_timer % 10 == 0)
            {
                _playerAnimation++;
                if (_playerAnimation == 10) _playerAnimation = 0;

            }

            if (_playerPos.Y <= 300)
            {
                _playerPos.Y = 300;
            }
            if (_playerPos.Y >= 390)
            {
                _playerPos.Y = 390;
            }

            _playerPos.X += _playerSpeed;

            _camera.Follow(_playerPos);

            if(!_isGameOver && CheckCollision())
            {
                _score--;
            }

            if (_playerPos.X - _stageData.Last().Position.X > 1000.0f)
            {
                ResetStageData();
                SetStageData();
            }

            if (_score <= 0)
            {
                _score = 0;
                _isGameOver = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var playTime = MediaPlayer.PlayPosition;
            var songTime = _song.Duration;

            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            _spriteBatch.Draw(_background1, _backGround1Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            _spriteBatch.Draw(_background2, _backGround2Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            _spriteBatch.Draw(_background3, _backGround3Pos, new Rectangle(0, 0, 1120, 630), Color.White);
            
            for (var i = 0; i < 64; i++)
            {
                if (_stageData[i].Type == 1)
                    _spriteBatch.Draw(_stageData[i].Side, _stageData[i].Position, new Rectangle(0, 0, 150, 150), Color.White);
            }
            
            // Render Player
            if (!_isGameOver)
                _spriteBatch.Draw(_player, _playerPos, new Rectangle(150 * _playerAnimation, 0, 150, 150), Color.White);

            for (var i = 0; i < 64; i++)
            {
                if (_stageData[i].Type == 2)
                    _spriteBatch.Draw(_stageData[i].Side, _stageData[i].Position, new Rectangle(0, 0, 150, 150),
                        Color.White);
            }

            // DEBUG
            _spriteBatch.DrawString(_font, $"Elapsed Time: {_timer / 60}", new Vector2(_playerPos.X - 475.0f, 25.0f),
                Color.White, 0.0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.0f);
            _spriteBatch.DrawString(_font, $"Score: {_score}", new Vector2(_playerPos.X + 400.0f, 25.0f), Color.White, 0.0f,
                Vector2.Zero, 2.5f, SpriteEffects.None, 0.0f);

            if (_isGameOver)
            {
                _spriteBatch.DrawString(_font, $"Game Over", new Vector2(_playerPos.X - 225.0f, Height / 2.0f - 50.0f), Color.White, 0.0f,
                    Vector2.Zero, 7.5f, SpriteEffects.None, 0.0f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private string GetHumanReaderTime(TimeSpan timeSpan)
        {
            var minutes = timeSpan.Minutes;
            var seconds = timeSpan.Seconds;

            if (seconds < 10)
                return minutes + ":0" + seconds;
            
            return minutes + ":" + seconds;
        }

        private bool CheckCollision()
        {
            for (var i = 0; i < 64; i++)
            {
                if (!(_playerPos.X - _stageData[i].Position.X < 50) ||
                    !(_playerPos.X + 50 >= _stageData[i].Position.X))
                    continue;

                if (Math.Abs(_playerPos.Y - _stageData[i].Position.Y) < 30)
                    return true;
            }
            return false;
        }

        private void SetStageData()
        {
            for (var i = 0; i < 64; i++)
            {
                var type = _rng.Next(1, 3);
                var yPos = type == 1 ? 310.0f : 390.0f;
                
                var stageData = new StageData
                {
                    Position = new Vector2
                    {
                        X = (_playerPos.X + 1000.0f) + i * 100.0f,
                        Y = yPos 
                    },
                    Side = _side1,
                    Type = type,
                };
                
                _stageData.Add(stageData);
            }
        }

        private void ResetStageData()
            => _stageData.Clear();
    }
}
