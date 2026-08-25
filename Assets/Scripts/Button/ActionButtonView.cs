using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace CaseStudy.WheelSpin
{
    [DisallowMultipleComponent]
    public class ActionButtonView : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler
    {
        [FormerlySerializedAs("<Interactable>k__BackingField")]
        [SerializeField] private bool _interactable = true;

        public event Action<PointerEventData> PointerDown;
        public event Action<PointerEventData> PointerUp;
        public event Action Click;

        /// <summary>
        /// Raised only when the value actually changes, so views that tint themselves can react to
        /// it instead of polling this property every frame.
        /// </summary>
        public event Action<bool> InteractableChanged;

        public bool Interactable
        {
            get => _interactable;
            set
            {
                if (_interactable == value)
                    return;

                _interactable = value;
                InteractableChanged?.Invoke(value);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            PointerDown?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            PointerUp?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            Click?.Invoke();
        }

#if UNITY_EDITOR

        /// Inspector edits write the field directly and skip the property setter, so the change is
        /// re-announced here — otherwise a value toggled during play would never repaint.
        private void OnValidate()
        {
            if (Application.isPlaying)
                InteractableChanged?.Invoke(_interactable);
        }
#endif
    }
}
