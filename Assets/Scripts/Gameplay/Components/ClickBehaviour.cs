using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Action _onClick;
    private Vector2 _touchPos;
    private float _clickTime;
    private static Camera _camera;

    private const float CLICK_TIME = 0.2f;
    private const float CLICK_DISTANCE = 0.2f;

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    public void Initialize(Action callback)
    {
        _onClick = callback;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _clickTime = Time.time;
        _touchPos = _camera.ScreenToWorldPoint(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 untouchPos = _camera.ScreenToWorldPoint(eventData.position);
        var distance = (_touchPos - untouchPos).magnitude;

        if (Time.time - _clickTime > CLICK_TIME)
            return;
        else if (distance > CLICK_DISTANCE)
            return;

        _onClick?.Invoke();
    }
}
