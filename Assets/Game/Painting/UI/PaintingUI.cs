using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Scripts.UI
{
    public class PaintingUI : MonoBehaviour
    {
        [SerializeField] private Button _nextObjectButton;
        [SerializeField] private Button _previousObjectButton;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _redoButton;

        private PaintableObjectsManager _objectsManager;
        private PaintingManager _paintingManager;
       

        private void Awake()
        {
            BindComponents();
        }

        private void OnEnable()
        {
            FindDependency(out _paintingManager);
            FindDependency(out _objectsManager);
            
            // _colorButton.SetColor(_paintingManager.Color);
        }

        /// <summary>
        /// Of course this is not efficient and for proper game we've use either some MVVM binding, or inject dependencies
        /// </summary>
        private static void FindDependency<T>(out T field) where T : Object
        {
            field = FindObjectOfType<T>();
            if (field == null)
            {
                throw new Exception($"{nameof(PaintingUI)} can't work without instance of {typeof(T).Name}");
            }
        } 
        
        private void BindComponents()
        {
            _nextObjectButton.onClick.AddListener(() => _objectsManager.SpawnNext());
            _previousObjectButton.onClick.AddListener(() => _objectsManager.SpawnPrevious());

            _saveButton.onClick.AddListener(() => GetPO().Save());
            _loadButton.onClick.AddListener(() => GetPO().Load());
            _clearButton.onClick.AddListener(() => GetPO().Clear());
            
            _undoButton.onClick.AddListener(() => GetPO().UndoTextures());
            _redoButton.onClick.AddListener(() => GetPO().RedoTextures());
            return;

            PaintableObject GetPO()
            {
                return _objectsManager.CurrentObject.GetValue();
            }
        }
    }
}