using System;
using System.Collections.Generic;
using Game.Saving;
using Game.Scripts.SimpleMVVM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    /// <summary>
    /// Handles paintable object spawning
    /// </summary>
    public class PaintableObjectsManager : MonoBehaviour
    {
        [SerializeField] private Transform _objectsRoot = null;
        
        [SerializeField, AssetsOnly] private List<PaintableObject> _paintableObjects = new();
        
        public IReadOnlyList<PaintableObject> Objects => _paintableObjects;

        public int CurrentObjectIndex { get; private set; } = -1;
        

        private readonly Property<PaintableObject> _currentObject = new();
        public IReadProperty<PaintableObject> CurrentObject => _currentObject;


        private void Start()
        {
            var i = GameState.RuntimeData.SelectedObjectIndex.Value;
            i = Mathf.Clamp(i, 0, _paintableObjects.Count);
            SpawnPaintableObject(i);
        }
        
        public void SpawnPaintableObject(int index)
        {
            if (index < 0 || index >= _paintableObjects.Count)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range");
            }

            var prefab = _paintableObjects[index];
            if (prefab == null)
            {
                throw new NullReferenceException($"Prefab at index {index} is null");
            }
            
            SpawnPaintableObject(prefab);
            
            CurrentObjectIndex = index;
        }
        
        public void SpawnPaintableObject(PaintableObject prefab)
        {
            // Setting to -1 just in case object is not actually spawned
            CurrentObjectIndex = -1;
            if (prefab == null)
            {
                throw new NullReferenceException($"Object you trying to spawn is null");
            }

            var old = _currentObject.Value;
            if (old != null)
            {
                old.Destruct();
                Destroy(old.gameObject);
            }
            
            var instance = Instantiate(prefab, _objectsRoot);
            instance.Init();

            _currentObject.Value = instance;
        }

        /// <summary>
        /// Spawns next object, if at the end of the sequence then loops
        /// </summary>
        public void SpawnNext() => SpawnWithOffset(1);

        /// <summary>
        /// Spawns previous object, if at the beginning of the sequence then loops
        /// </summary>
        public void SpawnPrevious() => SpawnWithOffset(-1);

        /// <summary>
        /// Verifies is current object on main sequence, them tries to spawn at index with offset, cycling the index if it comes out of bounds.
        /// </summary>
        /// <param name="offset"></param>
        private void SpawnWithOffset(int offset)
        {
            VerifyCurrentObjectIndex();
            var index = (int)Mathf.Repeat(CurrentObjectIndex + offset, Objects.Count);
            SpawnPaintableObject(index);
        }

        /// <summary>
        /// If <see cref="CurrentObjectIndex"/> is == -1 that means current object is not spawned from main sequence of objects (<see cref="Objects"/>). 
        /// </summary>
        private void VerifyCurrentObjectIndex()
        {
            if (CurrentObjectIndex < 0)
            {
                throw new Exception($"Current object is not on the main sequence");
            }
        }
    }
}