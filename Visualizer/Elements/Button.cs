using System;
using Microsoft.Xna.Framework;

namespace Visualizer.Elements
{
    public class Button : ITiledUiElement, IClickable
    {
        public string Label = null!;
        public string? Footer;
        public Point UnitPosition { get; set; }
        public bool IsHover { private get; set; }
        public Action OnClick { get; set; } = null!;

        public void Draw(VisualizerGame game)
        {
            var tileOrigin = UnitPosition.ToVector2();
            const float unitMargin = 0.025f;
            var hoverSize = new Vector2(1 - unitMargin * 2);
            var lowerFrom = tileOrigin + new Vector2(0, unitMargin * 2);
            var upperFrom = IsHover ?
                tileOrigin + new Vector2(unitMargin * 2, 0) :
                tileOrigin + new Vector2(unitMargin);
            game.FillRectangle(lowerFrom, hoverSize, game.Palette.HalfNegative);
            game.FillRectangle(upperFrom, hoverSize, game.Palette.MidTone);

            game.DrawShadowedString(Label, upperFrom + new Vector2(0.05f, 0),
                game.Palette.Positive, game.Palette.Negative, 2);
        }
    }
}
