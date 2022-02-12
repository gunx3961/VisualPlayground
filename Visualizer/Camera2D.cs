using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer
{
    public class Camera2D
    {
        private readonly GameWindow _window;
        public Point PixelCenter;
        public double PixelPerUnit;
        public readonly double DefaultPpu;

        public Camera2D(GameWindow window, double defaultPpu)
        {
            _window = window;
            PixelCenter = default;
            PixelPerUnit = DefaultPpu = defaultPpu;
        }

        public void Reset()
        {
            PixelCenter = Point.Zero + new Point(_window.ClientBounds.Width / 2, _window.ClientBounds.Height / 2);
            PixelPerUnit = DefaultPpu;
        }

        public Matrix GetMatrix()
        {
            return Matrix.CreateTranslation(-PixelCenter.X + _window.ClientBounds.Width / 2,
                -PixelCenter.Y + _window.ClientBounds.Height / 2, 0);
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

        public float ScaleFactor => (float)(PixelPerUnit / DefaultPpu);

        public void BatchBegin(SpriteBatch batch)
        {
            var cameraMatrix = GetMatrix();

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null,
                cameraMatrix);
        }

        public void DrawShadowedString(VisualizerGame game, string value, Vector2 unitPosition, Color color,
            Color shadowColor, float scale = 1, float shadowOffset = 0.005f)
        {
            var offset = ToPixel(new Vector2(-shadowOffset, shadowOffset)) * scale;
            var pixelPosition = ToPixel(unitPosition);
            game.Batch.DrawString(game.GlobalContents.DefaultFont, value, pixelPosition + offset,
                shadowColor, 0, Vector2.Zero, ScaleFactor * scale, SpriteEffects.None, 0);
            game.Batch.DrawString(game.GlobalContents.DefaultFont, value, pixelPosition,
                color, 0, Vector2.Zero, ScaleFactor * scale, SpriteEffects.None, 0);
        }

        public void FillRectangle(VisualizerGame game, Vector2 unitPosition, Vector2 unitSize, Color color)
        {
            game.Batch.FillRectangle(ToPixel(unitPosition), ToPixel(unitSize), color);
        }
    }
}
