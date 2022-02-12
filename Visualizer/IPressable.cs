using System;

namespace Visualizer
{
    public interface IPressable : ITiledUiElement
    {
        Action Pressed { get; }
        bool IsPressing { set; }
    }
}
