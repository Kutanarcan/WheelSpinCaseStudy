using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CaseStudy.WheelSpin
{

    [DisallowMultipleComponent]
    public class ActionButtonView : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
    {
        [field: SerializeField] public bool Interactable { get; set; } = true;

        public event Action<PointerEventData> PointerDown;
        public event Action<PointerEventData> PointerUp;
        public event Action<PointerEventData> Clicked;
        public event Action Click;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Interactable) return;
            PointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Interactable) return;
            PointerUp?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable) return;
            Clicked?.Invoke(eventData);
            Click?.Invoke();
        }
    }
}