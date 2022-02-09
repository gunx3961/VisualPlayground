﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using Visualizer.Screens;

namespace Visualizer
{
    public class VisualizerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private GlobalContents _globalContents;

        private IScreen _currentScreen;

        private Input _input;
        private Camera2D _camera;

        private bool _isDragging;

        public VisualizerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = "Visualizer";
            Window.AllowUserResizing = true;

            _camera = new Camera2D { Zoom = 1f };

            _input = new Input(this);
            Components.Add(_input);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _globalContents = new GlobalContents(Content);

            SwitchScreen<Title>();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // if (Keyboard.GetState().IsKeyDown(Keys.Left)) _camera.Center.X -= 10;
            // if (Keyboard.GetState().IsKeyDown(Keys.Right)) _camera.Center.X += 10;
            // if (Keyboard.GetState().IsKeyDown(Keys.Up)) _camera.Center.Y -= 10;
            // if (Keyboard.GetState().IsKeyDown(Keys.Down)) _camera.Center.Y += 10;
            // if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) _camera.Zoom -= 0.05f;
            // if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) _camera.Zoom += 0.05f;

            if (_input.WasMouseMiddleButtonJustReleased)
            {
                _isDragging = false;
            }
            else if (_isDragging)
            {
                _camera.Center -= _input.MouseDeltaMovement;
            } 
            else if (_input.WasMouseMiddleButtonJustPressed)
            {
                _isDragging = true;
            }

            _currentScreen?.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Color.DarkSlateGray);

            var cameraMatrix = Matrix.CreateTranslation(-_camera.Center.X + _graphics.PreferredBackBufferWidth / 2,
                -_camera.Center.Y + _graphics.PreferredBackBufferHeight / 2, 0);

            // var cameraMatrix = _camera.GetMatrix(_graphics);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                cameraMatrix);

            // _spriteBatch.DrawCircle(_camera.Center.ToVector2(), 16, 16, Color.Yellow);

            _currentScreen?.Draw(gameTime);

            _spriteBatch.End();

        }

        private void SwitchScreen<T>()
            where T : IScreen, new()
        {
            _currentScreen?.Exit();

            var s = new T();
            s.Batch = _spriteBatch;
            s.GlobalContents = _globalContents;
            s.Enter();

            _currentScreen = s;
        }
    }
}