using System.Threading;
using Microsoft.Xna.Framework;

namespace Visualizer
{
    public struct Camera2D
    {
        public Point Center;

        public void Reset()
        {
            Center = Point.Zero;
        }

        public Matrix GetMatrix(GameWindow window)
        {
            return Matrix.CreateTranslation(-Center.X + window.ClientBounds.Width / 2,
                -Center.Y + window.ClientBounds.Height / 2, 0);
        }
    }
}
