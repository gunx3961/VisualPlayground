using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer.Screens
{
    public class Title : IScreen
    {
        public SpriteBatch Batch { private get; set; }
        public GlobalContents GlobalContents { private get; set; }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(GameTime gameTime)
        {
           Batch.DrawString(GlobalContents.DefaultFont, "Visual Playground", Vector2.One, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
        }
    }
}
