using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Visualizer
{
    public class Input : GameComponent
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public Input(Game game) : base(game) { }


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

        public Point MouseDeltaMovement => _currentMouseState.Position - _previousMouseState.Position;
    }
}
