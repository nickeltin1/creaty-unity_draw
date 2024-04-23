using System.Collections.Generic;
using Game.Cameras;
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
        [SerializeField] private CwPaintSphere _paintSphere;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            CameraController.CameraPointerDown += OnCameraDragStarted;
            CameraController.CameraPointerUp += OnCameraDragEnded;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            CameraController.CameraPointerDown -= OnCameraDragStarted;
            CameraController.CameraPointerUp -= OnCameraDragEnded;
        }
        
        private void OnCameraDragStarted()
        {
            // Disabling painting while dragging camera
            SetPaintingActive(false);
        }

        private void OnCameraDragEnded()
        {
            // Enabling painting while camera drag ended
            SetPaintingActive(true);
        }

        public void SetPaintingActive(bool isActive)
        {
            _paintSphere.gameObject.SetActive(isActive);
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