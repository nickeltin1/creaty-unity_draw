using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts
{
    /// <summary>
    /// Just some inefficient recursion-based ID markers allowing to create hierarchies for saving game objects state.
    /// Definitely not the best solution but will do just fine here.
    /// It would not be required if not for complex models consisted from multiple meshes/textures. 
    /// </summary>
    public class SaveIDComponent : MonoBehaviour
    {
        public string ID;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(ID))
            {
                ID = transform.GetSiblingIndex().ToString();
            }
        }

        public SaveIDComponent GetRoot()
        {
            return CollectUpperHierarchy().Last();
        }

        public IEnumerable<SaveIDComponent> CollectUpperHierarchy()
        {
            yield return this;
            var current = transform.parent;
            while (current != null)
            {
                var currentID = current.GetComponentInParent<SaveIDComponent>();
                if (currentID != null)
                {
                    yield return currentID;
                    current = current.transform.parent;
                }
                else
                {
                    current = null;
                }
            }
        }

        private static readonly List<SaveIDComponent> _hierarchyCache = new();
        
        public IEnumerable<SaveIDComponent> CollectLowerHierarchy()
        {
            _hierarchyCache.Clear();
            GetComponentsInChildren(_hierarchyCache);
            foreach (var textureID in _hierarchyCache)
                yield return textureID;
        }

        public string CalculateSavePath(string extension)
        {
            var hierarchy = CollectUpperHierarchy().ToList();
            hierarchy.Reverse();
            return string.Join("/", hierarchy.Select(id => id.ID)) + "." + extension;
        }

        [Button]
        private void TryPrintSavePath()
        {
            try
            {
                var path = CalculateSavePath("png");
                Debug.Log(path);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}