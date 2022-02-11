using System;
using Cysharp.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer.Elements
{
    public class FloatControl : ITiledUiElement, IControl1D
    {
        public string Name;
        public float NormalizedValue;
        public Vector2 Range;
        public Point UnitPosition { get; set; }
        public bool IsHover { get; set; }
        public float Value => Range.X * (1f - NormalizedValue) + Range.Y * NormalizedValue;

        public FloatControl(string name, Vector2 range, float normalizedDefault)
        {
            Name = name;
            Range = range;
            NormalizedValue = normalizedDefault;
        }

        public void OnChange(float delta)
        {
            NormalizedValue = Math.Clamp(NormalizedValue + delta, 0, 1);
        }

        public void Draw(VisualizerGame game, ref Camera2D camera)
        {
            var tileOrigin = UnitPosition.ToVector2();
            const float unitMargin = 0.025f;
            var hoverSize = new Vector2(1 - unitMargin * 2);
            var lowerFrom = tileOrigin + new Vector2(0, unitMargin * 2);

            camera.FillRectangle(game, lowerFrom, hoverSize, game.Palette.HalfNegative);

            // Value visualization
            const float minimalHeight = 0.05f;
            var valueSize = new Vector2(hoverSize.X, minimalHeight + (hoverSize.Y - minimalHeight) * NormalizedValue);
            var upperFrom = tileOrigin + new Vector2(unitMargin * 2, 1 - unitMargin * 2 - valueSize.Y);
            camera.FillRectangle(game, upperFrom, valueSize, game.Palette.MidTone);

            var valuePosition = tileOrigin + new Vector2(0.1f, 0.1f);
            camera.DrawShadowedString(game, ZString.Format("{0:#.####}", Value), valuePosition, game.Palette.Positive, game.Palette.Negative, 2);

            var namePosition = valuePosition + new Vector2(0f, 0.4f);
            camera.DrawShadowedString(game, Name, namePosition, game.Palette.Positive, game.Palette.Negative, 4);
        }
    }
}
