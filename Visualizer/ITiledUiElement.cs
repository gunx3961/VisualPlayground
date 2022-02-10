using Microsoft.Xna.Framework;

namespace Visualizer
{
    public interface ITiledUiElement : IUiElement
    {
        Point UnitPosition { get; }
        bool IsHover { set; }
    }
}
