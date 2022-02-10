using System;
using Microsoft.Xna.Framework;

namespace Visualizer.Screens
{
    public struct CurveSampling
    {
        public float From;
        public float To;
        public int SamplingCount;

        public void Draw<T>(VisualizerGame game)
            where T : IParametricCurve, new()
        {
            Draw(game, new T());
        }
        
        public void Draw(VisualizerGame game, IParametricCurve curve)
        {
            var stepLength = (To - From) / (SamplingCount - 1);
            Span<Vector2> points = stackalloc Vector2[SamplingCount];
            for (var i = 0; i < points.Length; i += 1)
            {
                var t = From + i * stepLength;
                points[i] = game.ToPixelPosition(new Vector2(curve.X(t), curve.Y(t)));
            }

            for (var i = 1; i < points.Length; i += 1)
            {
                game.Batch.DrawLine(points[i - 1], points[i], Color.Aqua, 2f);
            }
        }
    }
}
