using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Visualizer.Screens
{
    public class IntegrationComparison : IScreen
    {
        public VisualizerGame Game { private get; set; }
        public List<IUiElement> UiSpace { get; set; }

        public void Enter()
        {
            
        }

        public void Exit()
        {
        }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime)
        {
            var sampling = new CurveSampling { From = -1, To = 1, SamplingCount = 64 };
            sampling.Draw<SimpleParabola>(Game);
        }

        private struct SimpleParabola : IParametricCurve
        {
            public float X(float t) => t;
            public float Y(float t) => t * t;
        }
    }
}
