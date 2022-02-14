using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Visualizer
{
    public static class DrawUtilities
    {
        public static void DrawPoints(IEnumerable<Vector2> points, Camera2D camera)
        {
            foreach (var point in points)
            {
                camera.DrawPoint(point, Color.Aqua);
            }
        }
    }
}
