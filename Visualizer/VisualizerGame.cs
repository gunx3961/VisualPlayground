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

        public Camera2D WorldCamera;
        // private Camera2D _uiCamera;

        private const int DefaultPpu = 128;
        private const int MaxPpu = 2048;
        private const int MinPpu = 64;
        private const int PpuStep = 64;
        // private int _pixelPerUnit;

        private const double UiPpu = 64;

        private bool _isDragging;

        private readonly UiSpace _worldUiSpace;


        public SpriteBatch Batch { get; private set; } = null!;
        public readonly Input Input;
        public GlobalContents GlobalContents { get; private set; } = null!;
        public IPalette Palette;


        public VisualizerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.Title = "Visualizer";
            Window.AllowUserResizing = true;

            WorldCamera = new Camera2D(DefaultPpu);

            _worldUiSpace = new UiSpace(this);

            Input = new Input(this);
            Components.Add(Input);

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

            if (Input.WasMouseMiddleButtonJustReleased)
            {
                _isDragging = false;
            }
            else if (_isDragging)
            {
                WorldCamera.PixelCenter -= Input.MouseDeltaMovement;
            }
            else if (Input.WasMouseMiddleButtonJustPressed)
            {
                _isDragging = true;
            }

            if (Input.WasJustPressed(Keys.NumPad0) || Input.WasJustPressed(Keys.D0)) ResetFieldOfView();
            if (Input.WasJustPressed(Keys.Escape) && _currentScreen is not Title) SwitchScreen<Title>();

            if (Input.WasJustPressed(Keys.D1)) SwitchScreen<IntegrationComparison>();

            MaintainWorldPpu();

            _worldUiSpace.Update();

            _currentScreen?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Palette.Negative);


            WorldCamera.BatchBegin(Window, Batch);

            // Camera center
            // Batch.DrawCircle(_camera.Center.ToVector2(), 16, 8, Color.Yellow);

            // Grids
            const int radius = 4;
            var gap = (int)WorldCamera.PixelPerUnit;

            void DrawGrid(Vector2 point)
            {
                Batch.DrawLine(point + new Vector2(-radius, 0), point + new Vector2(radius, 0), Palette.HalfNegative);
                Batch.DrawLine(point + new Vector2(0, -radius), point + new Vector2(0, radius), Palette.HalfNegative);
            }

            void DrawUnitMeasureText(Point point, int left, int top)
            {
                var (x, y) = point;
                if (x % 5 != 0 || y % 5 != 0) return;

                var xAxisPosition = new Vector2(WorldCamera.ToPixel(x) + 1, top - 2);
                Batch.DrawString(GlobalContents.DefaultFont, ZString.Format("{0}", x), xAxisPosition, Palette.HalfPositive);

                var yAxisPosition = new Vector2(left + 1, WorldCamera.ToPixel(y) - 2);
                Batch.DrawString(GlobalContents.DefaultFont, ZString.Format("{0}", y), yAxisPosition, Palette.HalfPositive);
            }

            var l = WorldCamera.PixelCenter.X - Window.ClientBounds.Width / 2;
            var r = l + Window.ClientBounds.Width;
            var t = WorldCamera.PixelCenter.Y - Window.ClientBounds.Height / 2;
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
                    DrawUnitMeasureText(WorldCamera.ToUnit(pixelPosition).ToPoint(), l, t);

                    y += gap;
                }

                x += gap;
            }

            DrawCurrentScreen(gameTime);

            Batch.End();

            // Draw UI space
            const int uiSpacePpu = DefaultPpu / 2;
        }

        private void DrawCurrentScreen(GameTime gameTime)
        {
            _currentScreen?.Draw(gameTime);
            foreach (var ui in _worldUiSpace)
            {
                ui.Draw(this, ref WorldCamera);
            }
        }

        private void MaintainWorldPpu()
        {
            var deltaWheel = Input.MouseDeltaScrollWheelValue;
            if (deltaWheel == 0) return;
            var deltaSign = deltaWheel > 0 ? 1 : -1;
            if (deltaSign == 1 && WorldCamera.PixelPerUnit >= MaxPpu || deltaSign == -1 && WorldCamera.PixelPerUnit <= MinPpu) return;

            var intentOrigin = Input.MouseWorldSpaceUnitPosition;
            var pixelPositionOfIntentBefore = WorldCamera.ToPixel(intentOrigin);
            WorldCamera.PixelPerUnit = Math.Clamp(WorldCamera.PixelPerUnit + PpuStep * deltaSign, MinPpu, MaxPpu);
            var pixelPositionOfIntentAfter = WorldCamera.ToPixel(intentOrigin);

            WorldCamera.PixelCenter += (pixelPositionOfIntentAfter - pixelPositionOfIntentBefore).ToPoint();
        }

        private void ResetFieldOfView()
        {
            WorldCamera.Reset(Window);
        }


        #region Utilities

        public void SwitchScreen<T>()
            where T : IScreen, new()
        {
            _currentScreen?.Exit();
           _worldUiSpace.Clear();

            var s = new T();
            s.Game = this;
            s.Enter();

            _currentScreen = s;

            ResetFieldOfView();
        }

        public Vector2 ScreenSpaceToWorldSpaceUnit(Point screenSpacePosition) => WorldCamera.ScreenSpacePointToUnit(Window, screenSpacePosition);
        // public Vector2 ScreenSpaceToUiSpaceUnit(Point screenSpacePosition) => _uiSpace.ScreenSpacePointToUnit(Window, screenSpacePosition);

        public void AddElementToWorldSpace(IUiElement element)
        {
            _worldUiSpace.Register(element);
        }

        public void AddElementToUiSpace(IUiElement element)
        {
            
        }

        #endregion
    }
}
