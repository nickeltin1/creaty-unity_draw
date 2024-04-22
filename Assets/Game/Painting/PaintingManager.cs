using System.Collections.Generic;
using Game.Data;
using Game.Saving;
using Game.Scripts.SimpleMVVM;
using PaintIn3D;
using UnityEngine;

namespace Game.Scripts
{
    public class PaintingManager : BindableComponent
    {
        [SerializeField] private PaintableObjectsManager _objectsManager;
        [SerializeField] private Property<Vector2> _brushSizeMinMax = new Vector2(0.1f, 10);
        [SerializeField] private CwPaintSphere _paintSphere;
        

        private void Awake()
        {
            // This is not necessarily to bind, since its right now only adjustable from editor,
            // but UI already uses this binding so whatever... 
            _brushSizeMinMax.ValueChanged += (value, newValue) => SendMinMax();
            
            SendMinMax();
            
            void SendMinMax()
            {
                var data = GameState.RuntimeData.Brush.Value;
                data.BrushMinMax = _brushSizeMinMax.Value;
                GameState.RuntimeData.Brush.Value = data;
            }
        }

        private Property<RuntimeGameData.BrushData> GetBrushBinding() => GameState.RuntimeData.Brush;
        
        protected override IEnumerable<IReadProperty> GetBindings()
        {
            yield return GetBrushBinding();
        }

        public override void Refresh()
        {
            var data = GetBrushBinding().Value;
            _paintSphere.Color = data.Color;
            _paintSphere.Radius = data.Size;
        }
    }
}