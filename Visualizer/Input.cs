using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Visualizer
{
    public class Input : GameComponent
    {
        private readonly VisualizerGame _game;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public Input(VisualizerGame game) : base(game)
        {
            _game = game;
        }


        public override void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
        }

        public bool WasJustPressed(Keys key) => _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        public bool WasJustReleased(Keys key) => _currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key);


        public bool WasMouseMiddleButtonJustPressed =>
            _currentMouseState.MiddleButton is ButtonState.Pressed && _previousMouseState.MiddleButton is ButtonState.Released;

        public bool WasMouseMiddleButtonJustReleased =>
            _currentMouseState.MiddleButton is ButtonState.Released && _previousMouseState.MiddleButton is ButtonState.Pressed;

        public bool WasMouseLeftButtonJustPressed =>
            _currentMouseState.LeftButton is ButtonState.Pressed && _previousMouseState.LeftButton is ButtonState.Released;

        public bool WasMouseLeftButtonJustReleased =>
            _currentMouseState.LeftButton is ButtonState.Released && _previousMouseState.LeftButton is ButtonState.Pressed;

        public Point MouseDeltaMovement => _currentMouseState.Position - _previousMouseState.Position;

        public int MouseDeltaScrollWheelValue => _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;

        public Vector2 MouseWorldSpaceUnitPosition => _game.ScreenSpaceToWorldSpaceUnit(_currentMouseState.Position);
        // public Vector2 MouseUiSpaceUnitPosition => _game.ScreenSpaceToWorldSpaceUnit(_currentMouseState.Position);
        public Point MouseScreenPosition => _currentMouseState.Position;
    }
}
