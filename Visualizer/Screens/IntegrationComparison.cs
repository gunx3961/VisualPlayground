using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Visualizer.Elements;

namespace Visualizer.Screens
{
    public class IntegrationComparison : IScreen
    {
        public VisualizerGame Game { private get; set; } = null!;

        private FloatControl _gravity = null!;
        private FloatControl _initialX = null!;
        private FloatControl _initialY = null!;

        private FloatControl _deltaTime = null!;

        private PlainText _equationText = null!;

        private List<IIntegration> _integrationBuffer = null!;
        private List<IIntegrationMeta> _integrationTypes = null!;

        public void Enter()
        {
            _gravity = Game.ElementFactory.BuildUiFloatControl(0, 0, "G", new Vector2(1, 10));
            _initialX = Game.ElementFactory.BuildUiFloatControl(0, 1, "(x,", new Vector2(1, 10), 0.7f);
            _initialY = Game.ElementFactory.BuildUiFloatControl(1, 1, "y)", new Vector2(-1, -20));

            _deltaTime = Game.ElementFactory.BuildUiFloatControl(2, 0, "dt", new Vector2(0.017f, 0.1f));


            Game.ElementFactory.BuildUiButton(0, 5, null, "Back", 0, v => { Game.SwitchScreen<Title>(); });

            Game.ElementFactory.BuildWorldPositionIndicator();

            Game.WorldCamera.Reset(new Vector2(-4, -4));

            _integrationTypes = new List<IIntegrationMeta>();
            _integrationTypes.Add(new NoobIntegrationMeta());
            _integrationTypes.Add(new VerletIntegrationMeta());

            void Reset(int value) => ResetIntegration(_integrationTypes[value]);

            foreach (var (it, i) in _integrationTypes.Select((it, i) => (it, i)))
            {
                Game.ElementFactory.BuildUiButton(0, 3 + i, it.Name, "Go", i, Reset);
            }

            _integrationBuffer = new List<IIntegration>();

            _equationText = Game.ElementFactory.BuildUiText(3.5f, 0, "");

            Reset(0);
        }

        public void Exit() { }

        public void Update(GameTime gameTime) { }

        public void Draw(GameTime gameTime)
        {
            var sampling = new CurveSampling { From = 0, To = 5, SamplingCount = 64 };
            // sampling.Draw<SimpleParabola>(Game);

            sampling.Draw(Game, Game.WorldCamera, new JumpCurve { Gravity = _gravity.Value, InitialVelocity = new Vector2(_initialX.Value, _initialY.Value) });

            var length = sampling.To - sampling.From;
            var frameCount = (int)(length / _deltaTime.Value) + 1;

            if (_integrationBuffer.Count < frameCount)
            {
                var newIntegration = _integrationBuffer.Last().Next(_deltaTime.Value);
                _integrationBuffer.Add(newIntegration);
            }

            foreach (var integration in _integrationBuffer)
            {
                Game.WorldCamera.DrawPoint(integration.Value, Color.Aqua);
            }
        }

        private void ResetIntegration(IIntegrationMeta integrationMeta)
        {
            _integrationBuffer.Clear();


            foreach (var integration in integrationMeta.GetInitialSequence(Vector2.Zero,
                         new Vector2(_initialX.Value, _initialY.Value), new Vector2(0, _gravity.Value),
                         _deltaTime.Value))
            {
                _integrationBuffer.Add(integration);
            }

            _equationText.Value = integrationMeta.Equation;
        }

        private struct JumpCurve : IParametricCurve
        {
            public Vector2 InitialVelocity;
            public float Gravity;

            public float X(float t) => t * InitialVelocity.X;

            public float Y(float t) => InitialVelocity.Y * t + 0.5f * Gravity * t * t;
        }

        private interface IIntegration
        {
            IIntegration Next(float deltaTime);
            Vector2 Value { get; }
        }

        private interface IIntegrationMeta
        {
            string Name { get; }
            string Equation { get; }
            IIntegration[] GetInitialSequence(Vector2 initialPosition, Vector2 initialVelocity, Vector2 initialAcceleration, float deltaTime);
        }

        private class NoobIntegration : IIntegration
        {
            public Vector2 PreviousPosition;
            public Vector2 PreviousVelocity;
            public Vector2 Acceleration;
            public Vector2 Value { get; set; }

            public IIntegration Next(float deltaTime)
            {
                var v = PreviousVelocity + Acceleration * deltaTime;
                var newPosition = PreviousPosition + v * deltaTime;

                return new NoobIntegration { PreviousPosition = newPosition, PreviousVelocity = v, Acceleration = Acceleration, Value = newPosition };
            }

            public IIntegration[] GetInitialSequence()
            {
                var s = new IIntegration[1];
                s[0] = this;
                return s;
            }
        }

        private class NoobIntegrationMeta : IIntegrationMeta
        {
            public string Name => "Noob";
            public string Equation => "P(n+1) = P(n) + v(t)dt = P(n) + (v(n) + Gdt)dt";

            public IIntegration[] GetInitialSequence(Vector2 initialPosition, Vector2 initialVelocity, Vector2 initialAcceleration, float deltaTime)
            {
                var initial = new NoobIntegration
                    { Acceleration = initialAcceleration, PreviousPosition = Vector2.Zero, Value = Vector2.Zero, PreviousVelocity = initialVelocity };
                return new IIntegration[] { initial };
            }
        }

        private class VerletIntegration : IIntegration
        {
            public Vector2 PreviousPosition;
            public Vector2 Acceleration;

            public IIntegration Next(float deltaTime)
            {
                var newPosition = 2 * Value - PreviousPosition + Acceleration * deltaTime * deltaTime;

                return new VerletIntegration
                {
                    Acceleration = Acceleration,
                    PreviousPosition = Value,
                    Value = newPosition
                };
            }

            public Vector2 Value { get; set; }
        }

        private class VerletIntegrationMeta : IIntegrationMeta
        {
            public string Name => "Verlet";
            public string Equation => "P1 = P0 + v0dt + 0.5adt^2, P(n+1) = 2P(n) - P(n-1) + adt^2";

            public IIntegration[] GetInitialSequence(Vector2 initialPosition, Vector2 initialVelocity, Vector2 initialAcceleration, float deltaTime)
            {
                var p0 = new VerletIntegration
                {
                    Value = initialPosition,
                    Acceleration = initialAcceleration
                };

                var p1 = new VerletIntegration
                {
                    PreviousPosition = initialPosition,
                    Value = initialPosition + initialVelocity * deltaTime + 0.5f * initialAcceleration * deltaTime * deltaTime,
                    Acceleration = initialAcceleration
                };

                return new IIntegration[] { p0, p1 };
            }
        }
    }
}
