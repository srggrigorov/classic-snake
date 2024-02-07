using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SnakeBodyComponent : MonoBehaviour
{
    public event Action OnSnakeDied;

    [SerializeField]
    private InputComponent touchInput;
    [SerializeField]
    private SnakeHeadComponent snakeHead;
    [SerializeField]
    [Min(0.01f)]
    private float snakeMoveDurationSlow;
    [SerializeField]
    [Min(0.01f)]
    private float snakeMoveDurationFast;
    private SnakePartComponent lastSnakePart;
    public readonly Dictionary<int, SnakePartComponent> SnakeParts = new();
    [SerializeField]
    private LineRenderer snakeLineRenderer;

    private float currentSnakeMoveDuration;
    private float nextSnakeMoveDuration;
    private Vector2Int nextSnakeMoveDirection;
    private Vector2Int currentSnakeMoveDirection;
    private IDisposable snakeMoveDisposable;


    private void Awake()
    {
        currentSnakeMoveDuration = nextSnakeMoveDuration = snakeMoveDurationSlow;
        SnakeParts[SnakeParts.Count] = snakeHead;
        lastSnakePart = snakeHead;
        snakeLineRenderer.positionCount = 1;
    }

    private void Update()
    {
        for (int i = 0; i < SnakeParts.Count; i++)
        {
            snakeLineRenderer.SetPosition(i, SnakeParts[i].PartTransform.localPosition);
        }
    }

    public void StartMovement()
    {
        lastSnakePart.OnTailCreated += OnTailCreated;
        snakeHead.OnHeadCollided += OnHeadCollided;
        touchInput.OnAccelerationBegan += IncreaseSnakeSpeed;
        touchInput.OnAccelerationEnded += DecreaseSnakeSpeed;
        touchInput.OnSwipeDetected += SetNextSnakeMoveDirection;
        currentSnakeMoveDirection = Vector2Int.down;
        SetSnakeMovement(currentSnakeMoveDuration);
    }

    private void SetSnakeMovement(float duration)
    {
        currentSnakeMoveDuration = duration;
        snakeMoveDisposable?.Dispose();
        snakeMoveDisposable = Observable.Interval(TimeSpan.FromSeconds(currentSnakeMoveDuration))
            .Subscribe(_ =>
            {
                if (Math.Abs(nextSnakeMoveDuration - currentSnakeMoveDuration) >= 0.001f)
                {
                    SetSnakeMovement(nextSnakeMoveDuration);
                    return;
                }

                ChangeSnakeMoveDirection();
                MoveHead();
            }).AddTo(this);
    }

    private void SetNextSnakeMoveDirection(Vector2Int newDirection) =>
        nextSnakeMoveDirection = newDirection;

    private void ChangeSnakeMoveDirection()
    {
        if (nextSnakeMoveDirection == Vector2Int.zero ||
            Vector2.Dot(currentSnakeMoveDirection, nextSnakeMoveDirection) != 0)
        {
            return;
        }

        currentSnakeMoveDirection = nextSnakeMoveDirection;
    }

    private void DecreaseSnakeSpeed()
    {
        nextSnakeMoveDuration = snakeMoveDurationSlow;
    }

    private void IncreaseSnakeSpeed()
    {
        nextSnakeMoveDuration = snakeMoveDurationFast;
    }


    private void OnTailCreated(SnakePartComponent tailPart)
    {
        lastSnakePart.OnTailCreated -= OnTailCreated;
        SnakeParts[SnakeParts.Count] = tailPart;
        lastSnakePart = tailPart;
        lastSnakePart.OnTailCreated += OnTailCreated;
        snakeLineRenderer.positionCount++;
    }

    private void MoveHead()
    {
        snakeHead.MoveToLocalPosition(snakeHead.transform.localPosition + (Vector3Int)currentSnakeMoveDirection,
            currentSnakeMoveDuration);
    }

    private void OnHeadCollided()
    {
        StopMovement();
        OnSnakeDied?.Invoke();
    }

    public void StopMovement()
    {
        lastSnakePart.OnTailCreated -= OnTailCreated;
        snakeHead.OnHeadCollided -= OnHeadCollided;
        touchInput.OnAccelerationBegan -= IncreaseSnakeSpeed;
        touchInput.OnAccelerationEnded -= DecreaseSnakeSpeed;
        touchInput.OnSwipeDetected -= SetNextSnakeMoveDirection;
        snakeMoveDisposable?.Dispose();
    }

    public Vector3[] GetSnakePartsCoordinates()
    {
        Vector3[] coordinates = new Vector3[SnakeParts.Count];
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i] = SnakeParts[i].CurrentTargetLocalPosition;
        }

        return coordinates;
    }
}