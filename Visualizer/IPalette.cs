using Microsoft.Xna.Framework;

namespace Visualizer
{
    public interface IPalette
    {
        Color Negative { get; }
        Color HalfNegative { get; }
        Color MidTone { get; }
        Color HalfPositive { get; }
        Color Positive { get; }
    }

    public class SimplePalette : IPalette
    {
        public Color Negative { get; set; }
        public Color HalfNegative { get; set; }
        public Color MidTone { get; set; }
        public Color HalfPositive { get; set; }
        public Color Positive { get; set; }
    }
}
