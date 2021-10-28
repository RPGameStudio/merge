using System;
using System.Collections;
using System.Collections.Generic;
using RX;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool ManagePosition { get; set; } = true;

    private ReactiveProperty<Vector3> _dragDelta = new ReactiveProperty<Vector3>();
    private ReactiveProperty<Vector3> _dragPosition = new ReactiveProperty<Vector3>();
    private Vector2 _prevDragPosition;
    private static Camera _camera;

    public event Action OnBeginDrag = delegate { };
    public event Action OnEndDrag = delegate { };
    public event Action OnDrag = delegate { };

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _prevDragPosition = _dragPosition.Value = (Vector2)_camera.ScreenToWorldPoint(eventData.position);

        if (ManagePosition)
            transform.position = _dragPosition.Value;

        OnBeginDrag?.Invoke();
    }

    internal void Reset()
    {
        OnEndDrag = null;
        OnBeginDrag = null;
        OnDrag = null;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _prevDragPosition = _dragPosition.Value;
        var pos = (Vector2)_camera.ScreenToWorldPoint(eventData.position);

        _dragDelta.Value = pos - _prevDragPosition;
        _dragPosition.Value = pos;

        if (ManagePosition)
            transform.position = _dragPosition;

        OnDrag?.Invoke();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) => OnEndDrag();
}
