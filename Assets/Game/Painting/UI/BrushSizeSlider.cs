using System.Collections.Generic;
using Game.Data;
using Game.Saving;
using Game.Scripts.SimpleMVVM;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class BrushSizeSlider : BindableComponent
    {
        [SerializeField] private Slider _slider;
        
        
        private void SubscribeForSliderEvents()
        {
            _slider.onValueChanged.AddListener(SliderValueChanged);
        }
        
        private void UnsubscribeFromSliderEvents()
        {
            _slider.onValueChanged.RemoveListener(SliderValueChanged);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();
            SubscribeForSliderEvents();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeFromSliderEvents();
        }

        private void SliderValueChanged(float t)
        {
            var data = GetBrushBinding().Value;
            data.Size = t;
            GetBrushBinding().Value = data;
            GameState.Save();
        }
        
        protected override IEnumerable<IReadProperty> GetBindings()
        {
            yield break;
        }

        private Property<RuntimeGameData.BrushData> GetBrushBinding() => GameState.RuntimeData.Brush;

        public override void Refresh()
        {
            var data = GetBrushBinding().Value;
            
            _slider.minValue = data.BrushMinMax.x;
            _slider.maxValue = data.BrushMinMax.y;
            _slider.value = data.Size;
        }
    }
}