using System;

namespace Visualizer
{
    public interface IPressable : ITiledUiElement
    {
        int Value { get; }
        Action<int> Pressed { get; }
        bool IsPressing { set; }
    }
}
