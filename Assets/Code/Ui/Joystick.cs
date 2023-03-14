using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Joystick : MonoBehaviour
{
    [SerializeField]
    private RectTransform _controller;
    [SerializeField]
    private RectTransform _buttonImage;
    [SerializeField]
    private float _maxStickOffset = 100;
    [SerializeField]
    [Range(1, 10)]
    private float _stiffness = 1;

    public float CanvasScaleFactor { get; set; } = 1;
    public Vector3 MoveVector { get; private set; }

    private bool _wasPressed = false;
    private Vector3 _initialPosition;


    private void Update()
    {
        if (IsPressed(out var position))
        {
            if (_wasPressed)
            {
                OnPressed(position);
            }
            else
            {
                _wasPressed = true;
                OnGetPressed(position);
            }
        }
        else
        {
            if (_wasPressed)
            {
                _wasPressed = false;
                OnGetReleased();
            }
            else
            {
                OnReleased();
            }
        }
    }


    private bool IsPressed(out Vector3 screenPosition)
    {
#if PLATFORM_STANDALONE
        if (Input.GetMouseButton(0))
        {
            screenPosition = Input.mousePosition / CanvasScaleFactor;
            return true;
        }
#else

        var touch = Input.GetTouch(0);
        if (touch.pressure > 0f)
        {
            screenPosition = touch.position / CanvasScaleFactor;
            return true;
        }
#endif

        screenPosition = default;
        return false;
    }


    private void OnGetPressed(Vector3 position)
    {
        _controller.position = _initialPosition = position;
        _controller.gameObject.SetActive(true);
    }


    private void OnPressed(Vector3 position)
    {
        var offset = position - _initialPosition;
        var offsetLen = Mathf.Min(_maxStickOffset, offset.magnitude / _stiffness);
        var normOffset = offset.normalized;
        offset = normOffset * offsetLen;

        _buttonImage.transform.localPosition = offset;

        MoveVector = normOffset * (offsetLen / _maxStickOffset);
    }


    private void OnGetReleased()
    {
        _controller.gameObject.SetActive(false);
        MoveVector = Vector3.zero;
    }


    private void OnReleased()
    {

    }

}
