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
        public Camera2D UiCamera;

        private const int DefaultPpu = 64;
        private const double MaxPpu = 2048;
        private const double MinPpu = 0.5;

        // private const int PpuStep = 64;
        private const int PpuScalingStep = 2;

        private bool _isDragging;

        public readonly ElementFactory ElementFactory;
        private readonly UiSpace _worldUiSpace;
        private readonly UiSpace _uiSpace;


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

            Input = new Input(this);

            WorldCamera = new Camera2D(this, DefaultPpu);
            UiCamera = new Camera2D(this, DefaultPpu);

            ElementFactory = new ElementFactory(this);
            _worldUiSpace = new UiSpace(Input, WorldCamera);
            _uiSpace = new UiSpace(Input, UiCamera);

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

            const int uiPaddingPx = 16;
            UiCamera.Reset();
            UiCamera.PixelCenter -= new Point(uiPaddingPx);

            _uiSpace.Update();
            _worldUiSpace.Update();

            _currentScreen?.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(Palette.Negative);


            WorldCamera.BatchBegin();

            // Camera center
            // Batch.DrawCircle(_camera.Center.ToVector2(), 16, 8, Color.Yellow);

            // Grids
            const int radius = 4;
            const int gapPixel = 128;
            var measureUnitLength = Math.Clamp((int)(WorldCamera.ToUnit(gapPixel) * 2), 1, int.MaxValue);

            void DrawGrid(Vector2 pixelPosition)
            {
                Batch.DrawLine(pixelPosition + new Vector2(-radius, 0), pixelPosition + new Vector2(radius - 1, 0), Palette.HalfNegative);
                Batch.DrawLine(pixelPosition + new Vector2(0, -radius + 1), pixelPosition + new Vector2(0, radius), Palette.HalfNegative);
            }

            void DrawUnitMeasureText(Point unit, int left, int top)
            {
                var (x, y) = unit;
                if (x % measureUnitLength != 0 || y % measureUnitLength != 0) return;

                var xAxisPosition = new Vector2(WorldCamera.ToPixel(x) + 1, top - 2);
                Batch.DrawString(GlobalContents.DefaultFont, ZString.Format("{0}", x), xAxisPosition, Palette.HalfPositive);

                var yAxisPosition = new Vector2(left + 1, WorldCamera.ToPixel(y) - 2);
                Batch.DrawString(GlobalContents.DefaultFont, ZString.Format("{0}", y), yAxisPosition, Palette.HalfPositive);
            }

            var l = WorldCamera.PixelCenter.X - Window.ClientBounds.Width / 2;
            var r = l + Window.ClientBounds.Width;
            var t = WorldCamera.PixelCenter.Y - Window.ClientBounds.Height / 2;
            var b = t + Window.ClientBounds.Height;

            var ltUnit = WorldCamera.ToUnit(new Vector2(l, t));
            var xStartPixel = (double)WorldCamera.ToPixel(MathF.Floor(ltUnit.X));
            var yStartPixel = (double)WorldCamera.ToPixel(MathF.Floor(ltUnit.Y));
            // var xStart = (l / gap - 1) * gap;
            // var yStart = (t / gap - 1) * gap;

            var x = xStartPixel;
            if (WorldCamera.PixelPerUnit > gapPixel) // FIXME
            {
                while (x <= r + gapPixel)
                {
                    var y = yStartPixel;
                    while (y <= b + gapPixel)
                    {
                        var pixelPosition = new Vector2((float)x, (float)y);
                        DrawGrid(pixelPosition);
                        DrawUnitMeasureText(WorldCamera.ToUnit(pixelPosition).ToPoint(), l, t);

                        y += gapPixel;
                    }

                    x += gapPixel;
                }
            }

            DrawCurrentScreenWorld(gameTime);

            Batch.End();

            // Draw UI space
            UiCamera.BatchBegin();

            DrawCurrentScreenUi(gameTime);

            Batch.End();
        }

        private void DrawCurrentScreenWorld(GameTime gameTime)
        {
            _currentScreen?.Draw(gameTime);
            foreach (var element in _worldUiSpace)
            {
                element.Draw(this, ref WorldCamera);
            }
        }

        private void DrawCurrentScreenUi(GameTime gameTime)
        {
            foreach (var element in _uiSpace)
            {
                element.Draw(this, ref UiCamera);
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
            // WorldCamera.PixelPerUnit = Math.Clamp(WorldCamera.PixelPerUnit + PpuStep * deltaSign, MinPpu, MaxPpu);
            WorldCamera.PixelPerUnit = Math.Clamp(
                deltaSign == 1 ? WorldCamera.PixelPerUnit * PpuScalingStep : WorldCamera.PixelPerUnit / PpuScalingStep,
                MinPpu, MaxPpu);
            var pixelPositionOfIntentAfter = WorldCamera.ToPixel(intentOrigin);

            WorldCamera.PixelCenter += (pixelPositionOfIntentAfter - pixelPositionOfIntentBefore).ToPoint();
        }

        private void ResetFieldOfView()
        {
            WorldCamera.Reset();
        }


        #region Utilities

        public void SwitchScreen<T>()
            where T : IScreen, new()
        {
            _currentScreen?.Exit();
            _worldUiSpace.Reset();
            _uiSpace.Reset();

            ResetFieldOfView();
            
            var s = new T();
            s.Game = this;
            s.Enter();

            _currentScreen = s;
        }
        public Vector2 ScreenSpaceToWorldSpaceUnit(Point screenSpacePosition) => WorldCamera.ScreenSpacePointToUnit(screenSpacePosition);
        public Vector2 ScreenSpaceToUiSpaceUnit(Point screenSpacePosition) => UiCamera.ScreenSpacePointToUnit(screenSpacePosition);

        public void AddElementToWorldSpace(IUiElement element)
        {
            _worldUiSpace.Register(element);
        }

        public void AddElementToUiSpace(IUiElement element)
        {
            _uiSpace.Register(element);
        }

        #endregion
    }
}
