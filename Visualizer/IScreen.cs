using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer
{
    public interface IScreen
    {
        VisualizerGame Game { set; }
        void Enter() { }

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void Exit() { }
    }
}
