using System;
using Microsoft.Xna.Framework;

namespace Visualizer.Screens
{
    public struct CurveSampling
    {
        public float From;
        public float To;
        public int SamplingCount;

        public void Draw<T>(VisualizerGame game, Camera2D camera)
            where T : struct, IParametricCurve
        {
            Draw(game, camera, new T());
        }

        public void Draw<T>(VisualizerGame game, Camera2D camera, T curve)
            where T : struct, IParametricCurve
        {
            var stepLength = (To - From) / (SamplingCount - 1);
            Span<Vector2> points = stackalloc Vector2[SamplingCount];
            for (var i = 0; i < points.Length; i += 1)
            {
                var t = From + i * stepLength;
                points[i] = camera.ToPixel(new Vector2(curve.X(t), curve.Y(t)));
            }

            for (var i = 1; i < points.Length; i += 1)
            {
                game.Batch.DrawLine(points[i - 1], points[i], game.Palette.HalfPositive, 2f);
            }
        }
    }
}
