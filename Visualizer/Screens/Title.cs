using Microsoft.Xna.Framework;
using Visualizer.Elements;

namespace Visualizer.Screens
{
    public class Title : IScreen
    {
        public VisualizerGame Game { private get; set; } = null!;

        public void Enter()
        {
            Game.AddElementToWorldSpace(new PlainText
            {
                Value = "Visual",
                Position = new Vector2(0.5f, 0.3f),
                Scale = 4
            });

            Game.AddElementToWorldSpace(new PlainText
            {
                Value = "Playground",
                Position = new Vector2(0.5f, 1f),
                Scale = 4
            });

            Game.AddElementToWorldSpace(new Button
            {
                Footer = "#0", UnitPosition = new Point(1, 3),
                Pressed = () => Game.SwitchScreen<IntegrationComparison>()
            });
            Game.AddElementToWorldSpace(new PlainText
            {
                Value = "Integration Comparison",
                Position = new Vector2(1.3f, 2.7f),
                Scale = 3
            });
        }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime) { }
    }
}
