using Microsoft.Xna.Framework.Graphics;

namespace Visualizer
{
    public interface IUiElement
    {
        public void Draw(VisualizerGame game, ref Camera2D camera);
    }
}
