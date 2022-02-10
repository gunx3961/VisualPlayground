using System;
using System.Collections.Generic;
using Cysharp.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Visualizer.Screens;

namespace Visualizer
{
    public class VisualizerGame : Game
    {
        private GraphicsDeviceManager _graphics;

        private IScreen? _currentScreen;
        private Camera2D _camera;

        private const int DefaultPpu = 128;
        private const int MaxPpu = 2048;
        private const int MinPpu = 64;
        private const int PpuStep = 64;
        private int _pixelPerUnit;

        private bool _isDragging;

        private readonly Dictionary<string, ITiledUiElement> _simpleTileHash;
        private readonly List<IUiElement> _uiSpace;
        private ITiledUiElement? _currentHovered;


        public SpriteBatch Batch { get; private set; }
        public readonly Input Input;
        public GlobalContents GlobalContents { get; private set; }
        public IPalette Palette;


        public VisualizerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = "Visualizer";
            Window.AllowUserResizing = true;

            _camera = new Camera2D();
            _pixelPerUnit = DefaultPpu;

            Input = new Input(this);
            Components.Add(Input);

            _simpleTileHash = new Dictionary<string, ITiledUiElement>();
            _uiSpace = new List<IUiElement>();

            Palette = new SimplePalette
            {
                Negative = new Color(72, 99, 117),
                HalfNegative = new Color(12, 199, 150),
                MidTone = new Color(95, 213, 151),
                HalfPositive = new Color(205, 253, 119),
                Positive = new Color(247, 247, 247)
            };
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);

            GlobalContents = new GlobalContents(Content);

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

            if (Input.WasMouseMiddleButtonJustReleased)
            {
                _isDragging = false;
            }
            else if (_isDragging)
            {
                _camera.Center -= Input.MouseDeltaMovement;
            }
            else if (Input.WasMouseMiddleButtonJustPressed)
            {
                _isDragging = true;
            }

            if (Input.WasJustPressed(Keys.NumPad0) || Input.WasJustPressed(Keys.D0)) ResetFieldOfView();
            if (Input.WasJustPressed(Keys.Escape) && _currentScreen is not Title) SwitchScreen<Title>();

            if (Input.WasJustPressed(Keys.D1)) SwitchScreen<IntegrationComparison>();

            MaintainPpu();

            MaintainUI();

            _currentScreen?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Palette.Negative);

            var cameraMatrix = _camera.GetMatrix(Window);

            Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                cameraMatrix);

            // Camera center
            // Batch.DrawCircle(_camera.Center.ToVector2(), 16, 8, Color.Yellow);

            // Grids
            const int radius = 6;
            var gap = _pixelPerUnit;

            void DrawGrid(Vector2 point)
            {
                Batch.DrawLine(point + new Vector2(-radius, 0), point + new Vector2(radius, 0), Palette.HalfNegative);
                Batch.DrawLine(point + new Vector2(0, -radius), point + new Vector2(0, radius), Palette.HalfNegative);
            }

            void DrawUnitMeasureText(Point point)
            {
                if (point.X % 5 != 0 || point.Y % 5 != 0) return;
                Batch.DrawString(GlobalContents.DefaultFont, ZString.Format("{0},{1}", point.X, point.Y),
                    ToPixelPosition(point.ToVector2()) + new Vector2(1, -2), Palette.HalfNegative);
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
                    var pixelPosition = new Vector2(x, y);
                    DrawGrid(pixelPosition);
                    DrawUnitMeasureText(ToUnitPosition(pixelPosition).ToPoint());

                    y += gap;
                }

                x += gap;
            }

            DrawCurrentScreen(gameTime);

            Batch.End();
        }

        private void DrawCurrentScreen(GameTime gameTime)
        {
            _currentScreen?.Draw(gameTime);
            foreach (var ui in _uiSpace)
            {
                ui.Draw(this);
            }
        }

        private void MaintainPpu()
        {
            var deltaWheel = Input.MouseDeltaScrollWheelValue;
            if (deltaWheel == 0) return;
            var deltaSign = deltaWheel > 0 ? 1 : -1;
            if (deltaSign == 1 && _pixelPerUnit == MaxPpu || deltaSign == -1 && _pixelPerUnit == MinPpu) return;

            var intentOrigin = Input.MouseWorldUnitPosition;
            var pixelPositionOfIntentBefore = ToPixelPosition(intentOrigin);
            _pixelPerUnit = Math.Clamp(_pixelPerUnit + PpuStep * deltaSign, MinPpu, MaxPpu);
            var pixelPositionOfIntentAfter = ToPixelPosition(intentOrigin);

            _camera.Center += (pixelPositionOfIntentAfter - pixelPositionOfIntentBefore).ToPoint();
        }

        private void MaintainUI()
        {
            var mouseUnitPosition = Input.MouseWorldUnitPosition;
            var mouseUnitPoint = new Point((int)MathF.Floor(mouseUnitPosition.X), (int)MathF.Floor(mouseUnitPosition.Y));
            var key = ZString.Format("{0}_{1}", mouseUnitPoint.X, mouseUnitPoint.Y);
            if (_currentHovered is not null) _currentHovered.IsHover = false;

            if (!_simpleTileHash.TryGetValue(key, out var newHovered)) return;

            _currentHovered = newHovered;
            _currentHovered.IsHover = true;

            if (Input.WasMouseLeftButtonJustPressed && _currentHovered is IClickable clickable)
            {
                clickable.OnClick();
            }
        }

        private void ResetFieldOfView()
        {
            _camera.Reset();
            _pixelPerUnit = DefaultPpu;
        }


        #region Utilities

        public void SwitchScreen<T>()
            where T : IScreen, new()
        {
            _currentScreen?.Exit();
            _uiSpace.Clear();
            _simpleTileHash.Clear();

            var s = new T();
            s.Game = this;
            s.Enter();

            _currentScreen = s;

            ResetFieldOfView();
        }

        public Vector2 ToUnitPosition(Vector2 pixelPosition) => pixelPosition / _pixelPerUnit;
        public Vector2 ToPixelPosition(Vector2 unitPosition) => unitPosition * _pixelPerUnit;

        public Vector2 ScreenSpaceToWorldSpaceUnit(Point screenSpacePosition)
        {
            var cameraMatrix = _camera.GetMatrix(Window);
            var worldPixel = Vector2.Transform(screenSpacePosition.ToVector2(), Matrix.Invert(cameraMatrix));
            return ToUnitPosition(worldPixel);
        }

        public void AddElement(IUiElement element)
        {
            _uiSpace.Add(element);

            if (element is ITiledUiElement tiledElement)
            {
                _simpleTileHash.Add(ZString.Format("{0}_{1}", tiledElement.UnitPosition.X, tiledElement.UnitPosition.Y), tiledElement);
            }
        }

        public float ScaleFactor => _pixelPerUnit / (float)DefaultPpu;

        #endregion
    }
}
