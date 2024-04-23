using System;
using System.Linq;
using Cinemachine;
using Game.Saving;
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
        private bool _pointerDownedFromPaintableObject;

        public static event Action CameraPointerDown;
        public static event Action CameraPointerUp;

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
            PlayerInputPanel.PointerUp += PointerUp;
            PlayerInputPanel.PointerDown += PointerDown;

            var cameraData = GameState.RuntimeData.Camera.Value;
            _yRotation.localRotation = Quaternion.AngleAxis(cameraData.YRotation, Vector3.up);
            _xRotation.localRotation = Quaternion.AngleAxis(cameraData.XRotation, Vector3.right);
        }
        
        private void OnDisable()
        {
            PlayerInputPanel.PointerDragged -= OnPointerDragged;
            PlayerInputPanel.PointerDragEnded -= DragEnded;
            PlayerInputPanel.PointerDragStarted -= DragStarted;
            PlayerInputPanel.PointerUp -= PointerUp;
            PlayerInputPanel.PointerDown -= PointerDown;
            
            var cameraData = GameState.RuntimeData.Camera.Value;
            cameraData.YRotation = _yRotation.localEulerAngles.y;
            cameraData.XRotation = _xRotation.localEulerAngles.x;
            GameState.RuntimeData.Camera.Value = cameraData;
            
            GameState.Save();
        }
        
        private void PointerDown(PointerEventData obj)
        {
            // Doing raycast to determine whether drag started at the paintable object
            // Yeah, yeah, main camera and raycast with allocation, but whatever, its not even every frame
            var ray = Camera.main!.ScreenPointToRay(obj.position);
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics.RaycastAll(ray);
            var hasAnyPaintableObjects = hits.Any(hit => hit.transform.GetComponentInParent<PaintableObject>() != null);
            _pointerDownedFromPaintableObject = hasAnyPaintableObjects;
            
            // If drag started not from paintable object means it was valid drag
            if (!_pointerDownedFromPaintableObject)
            {
                CameraPointerDown?.Invoke();
            }
        }

        private void PointerUp(PointerEventData obj)
        {
            // If drag started not from paintable object means it was valid drag
            if (!_pointerDownedFromPaintableObject)
            {
                CameraPointerUp?.Invoke();
            }
            _pointerDownedFromPaintableObject = false;
            _delta = Vector2.zero;
        }

        
        private void DragStarted(PointerEventData obj) { }
        
        private void OnPointerDragged(PointerEventData obj)
        {
            if (_pointerDownedFromPaintableObject)
            {
                return;
            }
            
            _delta = obj.delta * Time.deltaTime * _sensitivityMult 
                        * new Vector2(_horizontalSensitivity, _verticalSensitivity);
            
            _yRotation.localRotation *= Quaternion.AngleAxis(_delta.x, Vector3.up);
            _xRotation.localRotation *= Quaternion.AngleAxis(-_delta.y, Vector3.right);
        }
        
        private void DragEnded(PointerEventData obj) { }
    }
}