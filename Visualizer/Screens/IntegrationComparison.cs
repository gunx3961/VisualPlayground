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

            _gravity = Game.ElementFactory.BuildUiFloatControl(0, 0, "G", new Vector2(1, 10));
            _initialX = Game.ElementFactory.BuildUiFloatControl(0, 1, "(x,", new Vector2(-10, 10), 0.7f);
            _initialY = Game.ElementFactory.BuildUiFloatControl(1, 1, "y)", new Vector2(-10, 10));

            Game.ElementFactory.BuildUiButton(0, 3, null, "Back", () => { Game.SwitchScreen<Title>(); });
            
            Game.ElementFactory.BuildWorldPositionIndicator();
        }

        public void Exit() { }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime)
        {
            var sampling = new CurveSampling { From = 0, To = 10, SamplingCount = 64 };
            // sampling.Draw<SimpleParabola>(Game);

            sampling.Draw(Game, Game.WorldCamera, new JumpCurve { Gravity = _gravity.Value, InitialVeolocity = new Vector2(_initialX.Value, _initialY.Value) });
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
