using System.Collections.Generic;
using Game.Scripts.SimpleMVVM;
using UnityEngine;

namespace Game.Data
{
    public abstract class BindableComponent : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            foreach (var binding in GetBindings())
            {
                binding.BoxedValueChanged += BindingValueChanged;
            }
            
            Refresh();
        }
        
        protected virtual void OnDisable()
        {
            foreach (var binding in GetBindings())
            {
                binding.BoxedValueChanged -= BindingValueChanged;
            }
        }
        
        private void BindingValueChanged(object oldValue, object newValue)
        {
            Refresh();
        }

        protected abstract IEnumerable<IReadProperty> GetBindings();
        
        public abstract void Refresh();
    }
}