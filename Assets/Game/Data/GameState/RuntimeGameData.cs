using System;
using Game.Saving;
using UnityEngine;

namespace Game.Scripts.SimpleMVVM
{
    /// <summary>
    /// Binds saved data paths to operation in game state data.
    /// Layout of saved data might change during application development, as well as runtime data,
    /// therefore this binding is will be done here to keep compatability between versions.
    /// </summary>
    public class RuntimeGameData
    {
        public Property<int> SelectedObjectIndex { get; } = new();


        public Property<BrushData> Brush { get; }= new();
        public struct BrushData
        {
            public float Size;
            public Color Color;
        }
        
        
        public Property<CameraData> Camera { get; } = new();
        public struct CameraData
        {
            public float YRotation;
            public float XRotation;

            public override string ToString() => $"Y: {YRotation} X: {XRotation}";
        }
        
        /// <summary>
        /// By hand binding is not convenient when adding data, there should some binding system (reflections, expression trees, codegen)
        /// Also check <see cref="Unity.Properties"/>, not familiar with it but it might have something usefull  
        /// </summary>
        /// <param name="serializedData"></param>
        public void Read(SerializedGameData serializedData)
        {
            SelectedObjectIndex.Value = serializedData.SelectedObjectIndex;

            var brushData = serializedData.Brush;
            Brush.Value = new BrushData()
            {
                Color = brushData.Color,
                Size = brushData.Size,
            };

            var cameraData = serializedData.Camera;
            Camera.Value = new CameraData()
            {       
                YRotation = cameraData.YRotation,
                XRotation = cameraData.XRotation,
            };
        }

        public void Write(SerializedGameData serializedData)
        {
            serializedData.SelectedObjectIndex = SelectedObjectIndex;

            var brushData = Brush.Value;
            serializedData.Brush.Size = brushData.Size;
            serializedData.Brush.Color = brushData.Color;

            var cameraData = Camera.Value;
            serializedData.Camera.YRotation = cameraData.YRotation;
            serializedData.Camera.XRotation = cameraData.XRotation;
        }
    }
}