using System.Threading;
using Microsoft.Xna.Framework;

namespace Visualizer
{
    public struct Camera2D
    {
        public Point Center;
        public float Zoom;
        
        public void Reset()
        {
            Center = Point.Zero;
            Zoom = 1;
        }

        public Matrix GetMatrix(GraphicsDeviceManager graphics)
        {
            var l = Center.X - graphics.PreferredBackBufferWidth / 2;
            var r = l + graphics.PreferredBackBufferWidth;
            var t = Center.Y - graphics.PreferredBackBufferHeight / 2;
            var b = t + graphics.PreferredBackBufferHeight;

            return Matrix.CreateOrthographicOffCenter(l, r, t, b, -100, 100);
        }
    }
}
