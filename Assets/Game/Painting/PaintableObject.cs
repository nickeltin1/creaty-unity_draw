using System;
using System.Collections.Generic;
using System.Linq;
using Game.Saving;
using PaintCore;
using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Scripts
{
    public class PaintableObject : MonoBehaviour
    {
        [Title("PaintIn3D")]
        [SerializeField, Required] private CwPaintableMesh _paintableMesh;
        [SerializeField, Required] private CwMaterialCloner _materialCloner;
        [SerializeField, Required] private CwPaintableMeshTexture _paintableMeshTexture;
        


#if UNITY_EDITOR
        [Button, DisableInPlayMode]
        private void EnsurePaint3DComponents()
        {
            IEnumerable<MonoBehaviour> GetMainComponents()
            {
                yield return _paintableMesh;
                yield return _materialCloner;
                yield return _paintableMeshTexture;
            }

            var mainComponents = GetMainComponents().ToDictionary(behaviour => behaviour.GetType());
            
            // Querying mesh renderers that don't have paintable mesh components yet
            var meshRenderers = GetComponentsInChildren<MeshRenderer>()
                .Where(meshRenderer => !meshRenderer.TryGetComponent(mainComponents.Keys.First(), out _));
            
            Undo.IncrementCurrentGroup();
            var undoGroup = Undo.GetCurrentGroup();
            
            Undo.SetCurrentGroupName("Populating Paint 3D Components");
            
            // Populating missing components
            foreach (var meshRenderer in meshRenderers)
            {
                foreach (var type in mainComponents.Keys)
                {
                    ObjectFactory.AddComponent(meshRenderer.gameObject, type);
                }
            }
            
            // Copying settings from main components
            var components = mainComponents.Keys
                .SelectMany(GetComponentsInChildren) // Querying for all required components
                .Except(mainComponents.Values); // But skipping main
            
            foreach (var component in components)
            {
                // Retrieving main component by type, will not work if using several components of same type.
                if (mainComponents.TryGetValue(component.GetType(), out var mainComponent))
                {
                    EditorUtility.CopySerialized(mainComponent, component);
                }
            }
            
            Undo.CollapseUndoOperations(undoGroup);
        }
#endif
        
        private HashSet<CwPaintableTexture> _textures;

        public ISet<CwPaintableTexture> PaintableTextures => _textures;
        
        private void Awake()
        {
            _textures = GetComponentsInChildren<CwPaintableTexture>().ToHashSet();
        }

        public void Save()
        {
            TextureSaving.WriteTexturesWithIDComponents(_textures.ToArray());
        }

        public void Load()
        {
            TextureSaving.ReadTexturesWithIDComponents(_textures.ToArray());
        }

        public void Clear()
        {
            foreach (var texture in _textures) texture.Clear();
        }
    }
}