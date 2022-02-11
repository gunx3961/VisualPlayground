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
        private readonly VisualizerGame _game;
        private readonly List<IUiElement> _elements;
        private readonly Dictionary<string, ITiledUiElement> _simpleTileHash;
        private ITiledUiElement? _currentHovered;
        private IControl1D? _currentControlling;

        public UiSpace(VisualizerGame game)
        {
            _game = game;
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


        public void Clear()
        {
            _elements.Clear();
            _simpleTileHash.Clear();
        }

        public void Update()
        {
            // Hover and target
            var mouseUnitPosition = _game.Input.MouseWorldSpaceUnitPosition;
            var mouseUnitPoint = new Point((int)MathF.Floor(mouseUnitPosition.X), (int)MathF.Floor(mouseUnitPosition.Y));
            var key = ZString.Format("{0}_{1}", mouseUnitPoint.X, mouseUnitPoint.Y);
            if (_currentHovered is not null) _currentHovered.IsHover = false;

            if (_simpleTileHash.TryGetValue(key, out var newHovered))
            {
                _currentHovered = newHovered;
                _currentHovered.IsHover = true;
            }

            // Operations
            if (_game.Input.WasMouseLeftButtonJustPressed)
            {
                if (_currentHovered is IClickable clickable)
                {
                    clickable.OnClick();
                }

                if (_currentHovered is IControl1D control1D)
                {
                    _currentControlling = control1D;
                }
            }
            else if (_game.Input.WasMouseLeftButtonJustReleased)
            {
                _currentControlling = null;
            }
            else
            {
                var delta1D = -_game.Input.MouseDeltaMovement.Y / 256f;
                _currentControlling?.OnChange(delta1D);
            }
        }
    }
}
