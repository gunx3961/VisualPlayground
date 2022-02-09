using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer
{
    public interface IScreen
    {
        SpriteBatch Batch { set; }
        GlobalContents GlobalContents { set; }

        void Enter() { }

        void Update(GameTime gameTime);

        void Draw(GameTime gameTime);

        void Exit() { }
    }
}
