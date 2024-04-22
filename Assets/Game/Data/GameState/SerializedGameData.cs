using System;
using UnityEngine;

namespace Game.Saving
{
    /// <summary>
    /// Represents saved data with transparent layout for general application data.
    /// This is container for relatively small data, texture for example saved with different system <see cref="TextureSaving"/>.
    /// Don't have any functionality
    /// </summary>
    [Serializable]
    public class SerializedGameData
    {
        [Serializable]
        public class BrushData
        {
            public float Size = 0.1f;
            public Color Color = Color.red;
        }
        
        [Serializable]
        public class CameraData
        {
            public float YRotation = 0;
            public float XRotation = 0;
        }
        
        public int SelectedObjectIndex = 0;
        public BrushData Brush = new();
        public CameraData Camera = new();
    }
}