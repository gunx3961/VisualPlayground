using Microsoft.Xna.Framework;
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

            var cameraMatrix = Matrix.CreateTranslation(-_camera.Center.X + Window.ClientBounds.Width / 2,
                -_camera.Center.Y + Window.ClientBounds.Height / 2, 0);

            // var cameraMatrix = _camera.GetMatrix(_graphics);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                cameraMatrix);

            // Grids
            const int radius = 6;
            const int gap = 128;

            void DrawGrid(Vector2 point)
            {
                _spriteBatch.DrawLine(point + new Vector2(-radius, 0), point + new Vector2(radius, 0), Color.DarkGray);
                _spriteBatch.DrawLine(point + new Vector2(0, -radius), point + new Vector2(0, radius), Color.DarkGray);
            }

            var l = _camera.Center.X - Window.ClientBounds.Width / 2;
            var r = l + Window.ClientBounds.Width;
            var t = _camera.Center.Y - Window.ClientBounds.Height / 2;
            var b = t + Window.ClientBounds.Height;

            var xStart = (l / gap - 1) * gap;
            var yStart = (t / gap - 1) * gap;

            var x = xStart;
            while (x <= r + gap)
            {
                var y = yStart;
                while (y <= b + gap)
                {
                    DrawGrid(new Vector2(x, y));

                    y += gap;
                }

                x += gap;
            }

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
