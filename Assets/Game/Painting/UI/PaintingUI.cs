using System;
using PaintCore;
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

        private PaintableObjectsManager _objectsManager;
        private PaintingManager _paintingManager;
        
       
        public PaintableObject CurrentObject => _objectsManager.CurrentObject.GetValue();

        private void Awake()
        {
            BindComponents();
        }

        private static T EnsureComponent<T>(Component component) where T : Component
        {
            var instance = component.GetComponent<T>();
            if (instance == null)
            {
                instance = component.gameObject.AddComponent<T>();
            }
            return instance;
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

            _saveButton.onClick.AddListener(() => CurrentObject.Save());
            _loadButton.onClick.AddListener(() => CurrentObject.Load());
            _clearButton.onClick.AddListener(() => CurrentObject.Clear());
            
        }

        private void Update()
        {
            // Of course this is inefficient to do this in update, but buttons state depends on PaintIn3D and I don't want
            // to figure out its events right now, easier just to update state every frame
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
        }
    }
}