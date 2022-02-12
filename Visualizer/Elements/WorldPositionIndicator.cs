using Microsoft.Xna.Framework;

namespace Visualizer.Elements
{
    public class WorldPositionIndicator : IUiElement
    {
        public void Draw(VisualizerGame game, ref Camera2D camera)
        {
            var worldUnit = game.Input.MouseWorldSpaceUnitPosition;
            var lb = camera.ScreenSpacePointToUnit(new Point(0, game.Window.ClientBounds.Height));
            camera.DrawShadowedString(game, worldUnit.ToString(), lb + new Vector2(0.1f, -0.5f),
                game.Palette.Positive, game.Palette.Negative, 2);
        }
    }
}
