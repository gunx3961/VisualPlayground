using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Visualizer.Elements;

namespace Visualizer.Screens
{
    public class IntegrationComparison : IScreen
    {
        public VisualizerGame Game { private get; set; } = null!;

        private FloatControl _gravity = null!;
        private FloatControl _initialX = null!;
        private FloatControl _initialY = null!;

        public void Enter()
        {
            _gravity = new FloatControl("G", new Vector2(1, 10), 1f)
            {
                UnitPosition = new Point(-1, -1)
            };
            Game.AddElement(_gravity);

            _initialX = new FloatControl("(x,", new Vector2(-10, 10), 1f)
            {
                UnitPosition = new Point(0, -1)
            };
            Game.AddElement(_initialX);

            _initialY = new FloatControl("y)", new Vector2(-10, 10), 1f)
            {
                UnitPosition = new Point(1, -1)
            };
            Game.AddElement(_initialY);
        }

        public void Exit() { }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime)
        {
            var sampling = new CurveSampling { From = -1, To = 1, SamplingCount = 64 };
            // sampling.Draw<SimpleParabola>(Game);

            sampling.Draw(Game, new JumpCurve { Gravity = _gravity.Value, InitialVeolocity = new Vector2(_initialX.Value, _initialY.Value) });
        }

        private struct SimpleParabola : IParametricCurve
        {
            public float X(float t) => t;
            public float Y(float t) => t * t;
        }

        private struct JumpCurve : IParametricCurve
        {
            public Vector2 InitialVeolocity;
            public float Gravity;

            public float X(float t) => t * InitialVeolocity.X;

            public float Y(float t) => InitialVeolocity.Y * t + 0.5f * Gravity * t * t;
        }
    }
}
