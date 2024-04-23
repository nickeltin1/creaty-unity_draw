using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Cameras
{
    public class PlayerInputPanel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, 
        IBeginDragHandler, IEndDragHandler
    {
        public static event Action<PointerEventData> PointerDragged; 
        public static event Action<PointerEventData> PointerDragStarted; 
        public static event Action<PointerEventData> PointerDragEnded; 
        public static event Action<PointerEventData> PointerDown; 
        public static event Action<PointerEventData> PointerUp; 
        
        public void OnDrag(PointerEventData eventData)
        {
            PointerDragged?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp?.Invoke(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            PointerDragStarted?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            PointerDragEnded?.Invoke(eventData);
        }
    }
}