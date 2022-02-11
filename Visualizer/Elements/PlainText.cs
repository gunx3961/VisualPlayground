using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer.Elements
{
    public class PlainText : IUiElement
    {
        public string? Value;
        public Vector2 Position;
        public float Scale;

        public void Draw(VisualizerGame game)
        {
            var pixelPosition = game.ToPixel(Position);
            game.Batch.DrawString(game.GlobalContents.DefaultFont, Value, pixelPosition, game.Palette.Positive, 0,
                Vector2.Zero, game.ScaleFactor * Scale, SpriteEffects.None, 0);
        }
    }
}
