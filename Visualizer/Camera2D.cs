using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer
{
    public class Camera2D
    {
        private readonly VisualizerGame _game;
        public Point PixelCenter;
        public double PixelPerUnit;
        private readonly double _defaultPpu;

        public Camera2D(VisualizerGame game, double defaultPpu)
        {
            _game = game;
            PixelCenter = default;
            PixelPerUnit = _defaultPpu = defaultPpu;
        }

        public void Reset(Vector2 unit = default)
        {
            PixelCenter = ToPixel(unit).ToPoint() + new Point(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2);
            PixelPerUnit = _defaultPpu;
        }

        public Matrix GetMatrix()
        {
            return Matrix.CreateTranslation(-PixelCenter.X + _game.Window.ClientBounds.Width / 2,
                -PixelCenter.Y + _game.Window.ClientBounds.Height / 2, 0);
        }

        public Vector2 ScreenSpacePointToUnit(Point screenSpacePosition)
        {
            var cameraMatrix = GetMatrix();
            var pixel = Vector2.Transform(screenSpacePosition.ToVector2(), Matrix.Invert(cameraMatrix));
            return ToUnit(pixel);
        }

        public Vector2 ToUnit(Vector2 pixel) => new((float)(pixel.X / PixelPerUnit), (float)(pixel.Y / PixelPerUnit));
        public float ToUnit(float pixel) => (float)(pixel / PixelPerUnit);
        public double ToUnit(double pixel) => (pixel / PixelPerUnit);
        public Vector2 ToPixel(Vector2 unit) => new((float)(unit.X * PixelPerUnit), (float)(unit.Y * PixelPerUnit));
        public float ToPixel(float unit) => (float)(unit * PixelPerUnit);

        public float ScaleFactor => (float)(PixelPerUnit / _defaultPpu);

        public void BatchBegin()
        {
            var cameraMatrix = GetMatrix();

            _game.Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                cameraMatrix);
        }

        public void DrawShadowedString(string value, Vector2 unitPosition, Color color,
            Color shadowColor, float scale = 1, float shadowOffset = 0.005f)
        {
            var offset = ToPixel(new Vector2(-shadowOffset, shadowOffset)) * scale;
            var pixelPosition = ToPixel(unitPosition);
            _game.Batch.DrawString(_game.GlobalContents.DefaultFont, value, pixelPosition + offset,
                shadowColor, 0, Vector2.Zero, ScaleFactor * scale, SpriteEffects.None, 0);
            _game.Batch.DrawString(_game.GlobalContents.DefaultFont, value, pixelPosition,
                color, 0, Vector2.Zero, ScaleFactor * scale, SpriteEffects.None, 0);
        }

        public void FillRectangle(Vector2 unitPosition, Vector2 unitSize, Color color)
        {
            _game.Batch.FillRectangle(ToPixel(unitPosition), ToPixel(unitSize), color);
        }

        public void DrawPoint(Vector2 unitPosition, Color color, int pxRadius = 2)
        {
            var pxPosition = ToPixel(unitPosition);
            _game.Batch.FillRectangle(pxPosition - new Vector2(pxRadius), new Vector2(pxRadius * 2), color);
        }
    }
}
