using Microsoft.Xna.Framework;
using Visualizer.Elements;

namespace Visualizer.Screens
{
    public class Title : IScreen
    {
        public VisualizerGame Game { private get; set; }

        public void Enter()
        {
            Game.AddElement(new PlainText
            {
                Value = "Visual Playground",
                Position = new Vector2(-1, 0),
                Scale = 4
            });

            Game.AddElement(new Button
            {
                Label = "Integration Comparison", Position = new Vector2(-1, -1),
                OnClick = () => Game.SwitchScreen<IntegrationComparison>(),
                Scale = 2
            });
        }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime) { }
    }
}
