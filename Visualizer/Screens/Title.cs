using Microsoft.Xna.Framework;
using Visualizer.Elements;

namespace Visualizer.Screens
{
    public class Title : IScreen
    {
        public VisualizerGame Game { private get; set; } = null!;

        public void Enter()
        {
            Game.AddElement(new PlainText
            {
                Value = "Visual Playground",
                Position = new Vector2(-2, -1),
                Scale = 4
            });

            Game.AddElement(new Button
            {
                Label = "Integrate", UnitPosition = new Point(-1, 0),
                OnClick = () => Game.SwitchScreen<IntegrationComparison>()
            });
        }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime) { }
    }
}
