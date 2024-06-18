using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService
{
    public event Action OnAccelerationBegan;
    public event Action OnAccelerationEnded;
    public event Action<Vector2Int> OnSwipeDetected;

    private const float MinSwipeDistance = 50;
    private readonly InputActions _inputActions;

    private InputService()
    {
        _inputActions = new InputActions();
        _inputActions.SnakeControls.DirectionDelta.performed += DirectionChanged;
        _inputActions.SnakeControls.Acceleration.performed += _ => OnAccelerationBegan?.Invoke();
        _inputActions.SnakeControls.Acceleration.canceled += _ => OnAccelerationEnded?.Invoke();
    }

    public void SetActive(bool value)
    {
        if (value)
        {
            _inputActions.Enable();
            return;
        }
        _inputActions.Disable();
    }

    private void DirectionChanged(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        DetectSwipe(delta);
    }

    private void DetectSwipe(Vector2 touchDelta)
    {
        Vector2 swipeDirection = Math.Abs(touchDelta.x) >= Math.Abs(touchDelta.y)
            ? new Vector2(touchDelta.x, 0)
            : new Vector2(0, touchDelta.y);

        if (swipeDirection.magnitude < MinSwipeDistance)
        {
            return;
        }

        OnSwipeDetected?.Invoke(Vector2Int.RoundToInt(swipeDirection.normalized));
    }
}
