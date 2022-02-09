using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer
{
    public class GlobalContents
    {
        public readonly SpriteFont DefaultFont;

        public GlobalContents(ContentManager contentManager)
        {
            DefaultFont = contentManager.Load<SpriteFont>("monogram");
        }
    }
}
