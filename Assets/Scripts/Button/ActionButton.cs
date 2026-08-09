using System;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class ActionButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    [field: SerializeField] public bool Interactable { get; set; } = true;

    public event Action<PointerEventData> PointerEntered;
    public event Action<PointerEventData> PointerExited;
    public event Action<PointerEventData> PointerDown;
    public event Action<PointerEventData> PointerUp;
    public event Action<PointerEventData> Clicked;

    public event Action Click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable) 
            return;

        PointerEntered?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        PointerExited?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        PointerDown?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable) 
            return;

        PointerUp?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        Clicked?.Invoke(eventData);
        Click?.Invoke();
    }
}