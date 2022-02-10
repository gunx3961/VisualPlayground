using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Visualizer.Elements
{
    public class Button : ITiledUiElement, IClickable
    {
        public string Label;
        public Point UnitPosition { get; set; }
        public bool IsHover { private get; set; }
        public Action OnClick { get; set; }


        public void Draw(VisualizerGame game)
        {
            // const int 
            var tilePosition = game.ToPixelPosition(UnitPosition.ToVector2());
            var tileSize = game.ToPixelPosition(Vector2.One);

            if (IsHover)
            {
                game.Batch.FillRectangle(tilePosition, tileSize, game.Palette.MidTone);
            }

            game.Batch.DrawString(game.GlobalContents.DefaultFont, Label, game.ToPixelPosition(UnitPosition.ToVector2()), game.Palette.Positive,
                0, Vector2.Zero, game.ScaleFactor, SpriteEffects.None, 0);
        }
    }
}
