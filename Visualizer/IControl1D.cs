using System;

namespace Visualizer
{
    public interface IControl1D : ITiledUiElement
    {
        void OnChange(float delta);
    }
}
