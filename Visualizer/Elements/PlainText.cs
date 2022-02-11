using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer.Elements
{
    public class PlainText : IUiElement
    {
        public string? Value;
        public Vector2 Position;
        public float Scale;

        public void Draw(VisualizerGame game, ref Camera2D camera)
        {
            camera.DrawShadowedString(game, Value, Position, game.Palette.Positive, game.Palette.Negative, Scale);
        }
    }
}
