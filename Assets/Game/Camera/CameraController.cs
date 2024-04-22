using System;
using System.Linq;
using Cinemachine;
using Game.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _yRotation;
        [SerializeField] private Transform _xRotation;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        [SerializeField] private float _sensitivityMult = 1;
        [SerializeField] private float _verticalSensitivity = 1;
        [SerializeField] private float _horizontalSensitivity = 1;

        

        private CinemachineTransposer _transposer;
        private Vector2 _delta;
        private bool _dragStartedFromPaintableObject;

        public static event Action CameraDragStarted;
        public static event Action CameraDragEnded;

        public float ZOffset
        {
            get => _transposer.m_FollowOffset.z;
            set => _transposer.m_FollowOffset.z = value;
        }
        
        private void Awake()
        {
            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void OnEnable()
        {
            PlayerInputPanel.PointerDragged += OnPointerDragged;
            PlayerInputPanel.PointerDragEnded += DragEnded;
            PlayerInputPanel.PointerDragStarted += DragStarted;
        }
        
        private void OnDisable()
        {
            PlayerInputPanel.PointerDragged -= OnPointerDragged;
            PlayerInputPanel.PointerDragEnded -= DragEnded;
            PlayerInputPanel.PointerDragStarted -= DragStarted;
        }
        
        private void DragStarted(PointerEventData obj)
        {
            // Doing raycast to determine whether drag started at the paintable object
            // Yeah, yeah, main camera and raycast with allocation, but whatever, its not even every frame
            var ray = Camera.main!.ScreenPointToRay(obj.position);
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics.RaycastAll(ray);
            var hasAnyPaintableObjects = hits.Any(hit => hit.transform.GetComponentInParent<PaintableObject>() != null);
            _dragStartedFromPaintableObject = hasAnyPaintableObjects;
            
            // If drag started not from paintable object means it was valid drag
            if (!_dragStartedFromPaintableObject)
            {
                CameraDragStarted?.Invoke();
            }
        }
        
        private void OnPointerDragged(PointerEventData obj)
        {
            if (_dragStartedFromPaintableObject)
            {
                return;
            }
            
            _delta = obj.delta * Time.deltaTime * _sensitivityMult 
                        * new Vector2(_horizontalSensitivity, _verticalSensitivity);
            
            _yRotation.localRotation *= Quaternion.AngleAxis(_delta.x, Vector3.up);
            _xRotation.localRotation *= Quaternion.AngleAxis(-_delta.y, Vector3.right);
        }
        
        private void DragEnded(PointerEventData obj)
        {
            // If drag started not from paintable object means it was valid drag
            if (!_dragStartedFromPaintableObject)
            {
                CameraDragEnded?.Invoke();
            }
            _dragStartedFromPaintableObject = false;
            _delta = Vector2.zero;
        }
    }
}