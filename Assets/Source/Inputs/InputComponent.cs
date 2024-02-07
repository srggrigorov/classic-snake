using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputComponent : MonoBehaviour
{
    [SerializeField]
    private float minSwipeDistance;

    public event Action OnAccelerationBegan;
    public event Action OnAccelerationEnded;
    public event Action<Vector2Int> OnSwipeDetected;

    private InputActions inputActions;

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Awake()
    {
        inputActions = new InputActions();
        inputActions.SnakeControls.DirectionDelta.performed += DirectionChanged;
        inputActions.SnakeControls.Acceleration.performed += _ => OnAccelerationBegan?.Invoke();
        inputActions.SnakeControls.Acceleration.canceled += _ => OnAccelerationEnded?.Invoke();
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

        if (swipeDirection.magnitude < minSwipeDistance)
        {
            return;
        }

        OnSwipeDetected?.Invoke(Vector2Int.RoundToInt(swipeDirection.normalized));
    }
}