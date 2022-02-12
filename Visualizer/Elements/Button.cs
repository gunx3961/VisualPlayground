using System;
using Microsoft.Xna.Framework;

namespace Visualizer.Elements
{
    public class Button : IPressable
    {
        public string? Text;
        public string? Footer;
        public Point UnitPosition { get; set; }
        public bool IsHover { private get; set; }
        public bool IsPressing { private get; set; }
        public Action Pressed { get; set; } = null!;

        public void Draw(VisualizerGame game, ref Camera2D camera)
        {
            var tileOrigin = UnitPosition.ToVector2();
            const float unitMargin = 0.025f;
            var hoverSize = new Vector2(1 - unitMargin * 2);
            var lowerFrom = tileOrigin + new Vector2(0, unitMargin * 2);
            var upperFrom = IsPressing ?
                tileOrigin + new Vector2(unitMargin) :
                tileOrigin + new Vector2(unitMargin * 2, 0);
            camera.FillRectangle(game, lowerFrom, hoverSize, game.Palette.HalfNegative);
            camera.FillRectangle(game, upperFrom, hoverSize, IsHover ? game.Palette.HalfPositive : game.Palette.MidTone);

            camera.DrawShadowedString(game, Text ?? "", upperFrom + new Vector2(0.05f, -0.1f),
                game.Palette.Positive, game.Palette.Negative, 3);

            camera.DrawShadowedString(game, Footer ?? "", upperFrom + new Vector2(0.05f, 0.4f),
                game.Palette.Positive, game.Palette.Negative, 3);
        }
    }
}
