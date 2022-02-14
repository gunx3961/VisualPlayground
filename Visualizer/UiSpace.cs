using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cysharp.Text;
using Microsoft.Xna.Framework;

namespace Visualizer
{
    public class UiSpace
    {
        private readonly Input _input;
        private readonly Camera2D _camera;
        private readonly List<IUiElement> _elements;
        private readonly Dictionary<string, ITiledUiElement> _simpleTileHash;
        private ITiledUiElement? _currentPointerOver;
        private IPressable? _targetPressable;
        private IControl1D? _currentControlling;

        public UiSpace(Input input, Camera2D camera)
        {
            _input = input;
            _camera = camera;
            _elements = new List<IUiElement>();
            _simpleTileHash = new Dictionary<string, ITiledUiElement>();
        }

        public void Register(IUiElement element)
        {
            if (element is ITiledUiElement tiledElement)
            {
                _simpleTileHash.Add(ZString.Format("{0}_{1}", tiledElement.UnitPosition.X, tiledElement.UnitPosition.Y), tiledElement);
            }

            _elements.Add(element);
        }

        public IEnumerator<IUiElement> GetEnumerator() => _elements.GetEnumerator();


        public void Reset()
        {
            _elements.Clear();
            _simpleTileHash.Clear();
            _currentPointerOver = null;
            _currentControlling = null;
        }

        public void Update()
        {
            // Hover and target
            var mouseUnitPosition = _camera.ScreenSpacePointToUnit(_input.MouseScreenPosition);
            var mouseUnitPoint = new Point((int)MathF.Floor(mouseUnitPosition.X), (int)MathF.Floor(mouseUnitPosition.Y));
            var key = ZString.Format("{0}_{1}", mouseUnitPoint.X, mouseUnitPoint.Y);
            if (_currentPointerOver is not null)
            {
                _currentPointerOver.IsHover = false;
                _currentPointerOver = null;
            }

            if (_simpleTileHash.TryGetValue(key, out var newPointerOver))
            {
                _currentPointerOver = newPointerOver;
                _currentPointerOver.IsHover = true;
            }

            // Operations
            if (_input.WasMouseLeftButtonJustPressed)
            {
                if (_currentPointerOver is IPressable pressable)
                {
                    _targetPressable = pressable;
                    _targetPressable.IsPressing = true;
                }

                if (_currentPointerOver is IControl1D control1D)
                {
                    _currentControlling = control1D;
                }
            }
            else if (_input.WasMouseLeftButtonJustReleased)
            {
                _currentControlling = null;

                if (_targetPressable is not null)
                {
                    if (_currentPointerOver == _targetPressable)
                    {
                        _targetPressable.Pressed(_targetPressable.Value);
                    }

                    _targetPressable.IsPressing = false;
                    _targetPressable = null;
                }
            }
            else if (_currentControlling is not null)
            {
                var delta1D = -_input.MouseDeltaMovement.Y / 256f;
                if (delta1D != 0) _currentControlling.OnChange(delta1D);
            }
            else if (_targetPressable is not null && _targetPressable != _currentPointerOver)
            {
                _targetPressable.IsPressing = false;
                _targetPressable = null;
            }
        }
    }
}
