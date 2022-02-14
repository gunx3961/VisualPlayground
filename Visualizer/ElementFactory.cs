using System;
using Microsoft.Xna.Framework;
using Visualizer.Elements;

namespace Visualizer
{
    public class ElementFactory
    {
        private readonly VisualizerGame _game;

        public ElementFactory(VisualizerGame game)
        {
            _game = game;
        }

        public void BuildWorldPositionIndicator()
        {
            _game.AddElementToUiSpace(new WorldPositionIndicator());
        }

        public Button BuildUiButton(int x, int y, string? text, string? footer, int value, Action<int> pressed)
        {
            var b = new Button
            {
                Text = text, Footer = footer,
                UnitPosition = new Point(x, y),
                Pressed = pressed,
                Value = value
            };
            _game.AddElementToUiSpace(b);
            return b;
        }

        public FloatControl BuildUiFloatControl(int x, int y, string name, Vector2 range, float normalizedDefault = 0.5f)
        {
            var f = new FloatControl(name, range, normalizedDefault)
            {
                UnitPosition = new Point(x, y),
            };
            _game.AddElementToUiSpace(f);
            return f;
        }

        public PlainText BuildUiText(float x, float y, string value, float scale = 2)
        {
            var text = new PlainText
            {
                Value = value,
                Position = new Vector2(x, y),
                Scale = scale,
            };
            _game.AddElementToUiSpace(text);
            return text;
        }
    }
}
