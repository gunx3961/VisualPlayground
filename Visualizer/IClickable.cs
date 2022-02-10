using System;

namespace Visualizer
{
    public interface IClickable
    {
        Action OnClick { get; }
    }
}
