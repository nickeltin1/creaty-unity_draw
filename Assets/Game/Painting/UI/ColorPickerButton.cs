using System.Collections.Generic;
using Game.Data;
using Game.Saving;
using Game.Scripts.SimpleMVVM;
using RuntimeInspectorNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class ColorPickerButton : BindableComponent
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _colorFill;
        [SerializeField] private Canvas _referenceCanvas;
        
        private void Awake()
        { 
            _button.onClick.AddListener(ShowObjectPicker);
        }

        protected override IEnumerable<IReadProperty> GetBindings()
        {
            yield return GetBrushBinding();
        }

        public override void Refresh()
        {
            _colorFill.color = GetBrushBinding().Value.Color;
        }

        public virtual Property<RuntimeGameData.BrushData> GetBrushBinding() => GameState.RuntimeData.Brush;

        private void SetColor(Color color)
        {
            var data = GetBrushBinding().Value;
            data.Color = color;
            GetBrushBinding().Value = data;
        }
        
        private void ShowObjectPicker()
        {
            var instance = ColorPicker.Instance;
            instance.Show(color =>
            {
                SetColor(color);
            }, color =>
            { 
                SetColor(color);
                GameState.Save();
            }, GetBrushBinding().Value.Color, _referenceCanvas);
        }
    }
}