using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer.Elements
{
    public class Button : IUiElement
    {
        public string Label;
        public Vector2 Position;
        public float Scale;
        public Action OnClick;

        public void Draw(VisualizerGame game)
        {
            game.Batch.DrawString(game.GlobalContents.DefaultFont, Label, game.ToPixelPosition(Position), Color.White, 
                0, Vector2.Zero, Scale * game.ScaleFactor, SpriteEffects.None, 0);
        }
    }
}
