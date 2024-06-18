using System;
using UniRx;
using UnityEngine;

public class SnakeMovementService : IDisposable
{
    private readonly InputService _inputService;
    private readonly SnakeHeadComponent _snakeHead;
    private readonly SnakeSpeedSettings _settings;
    private readonly GameStateService _gameStateService;
    private float _currentSnakeMoveDuration;
    private float _nextSnakeMoveDuration;
    private Vector2Int _nextSnakeMoveDirection;
    private Vector2Int _currentSnakeMoveDirection;
    private IDisposable _snakeMoveDisposable;

    public SnakeMovementService(InputService inputService, AssetsManager assetsManager,
        GameStateService gameStateService, SnakeHeadComponent snakeHead)
    {
        (_inputService, _gameStateService) = (inputService, gameStateService);
        _settings = assetsManager.GetModuleSettings<SnakeSpeedSettings>();
        _snakeHead = snakeHead;

        _currentSnakeMoveDuration = _nextSnakeMoveDuration = _settings.MoveDurationSlow;
        _gameStateService.OnGameStarted += StartMovement;
    }

    public void Dispose()
    {
        _snakeMoveDisposable?.Dispose();
    }

    private void StartMovement()
    {
        _gameStateService.OnGameStarted -= StartMovement;
        _snakeHead.OnHeadCollided += StopMovement;
        _inputService.OnAccelerationBegan += IncreaseSnakeSpeed;
        _inputService.OnAccelerationEnded += DecreaseSnakeSpeed;
        _inputService.OnSwipeDetected += SetNextSnakeMoveDirection;
        _currentSnakeMoveDirection = Vector2Int.down;
        SetSnakeMovement(_currentSnakeMoveDuration);
    }

    private void StopMovement()
    {
        _snakeHead.OnHeadCollided -= StopMovement;
        _inputService.OnAccelerationBegan -= IncreaseSnakeSpeed;
        _inputService.OnAccelerationEnded -= DecreaseSnakeSpeed;
        _inputService.OnSwipeDetected -= SetNextSnakeMoveDirection;
        _snakeMoveDisposable?.Dispose();
    }

    private void MoveHead()
    {
        _snakeHead.MoveToLocalPosition(_snakeHead.transform.localPosition + (Vector3Int)_currentSnakeMoveDirection,
            _currentSnakeMoveDuration);
    }

    private void SetSnakeMovement(float duration)
    {
        _currentSnakeMoveDuration = duration;
        _snakeMoveDisposable?.Dispose();
        _snakeMoveDisposable = Observable.Interval(TimeSpan.FromSeconds(_currentSnakeMoveDuration))
            .Subscribe(_ =>
                {
                    if (Math.Abs(_nextSnakeMoveDuration - _currentSnakeMoveDuration) >= 0.001f)
                    {
                        SetSnakeMovement(_nextSnakeMoveDuration);
                        return;
                    }

                    ChangeSnakeMoveDirection();
                    MoveHead();
                });
    }

    private void SetNextSnakeMoveDirection(Vector2Int newDirection) =>
        _nextSnakeMoveDirection = newDirection;

    private void ChangeSnakeMoveDirection()
    {
        if (_nextSnakeMoveDirection == Vector2Int.zero ||
            Vector2.Dot(_currentSnakeMoveDirection, _nextSnakeMoveDirection) != 0)
        {
            return;
        }

        _currentSnakeMoveDirection = _nextSnakeMoveDirection;
    }

    private void DecreaseSnakeSpeed()
    {
        _nextSnakeMoveDuration = _settings.MoveDurationSlow;
    }

    private void IncreaseSnakeSpeed()
    {
        _nextSnakeMoveDuration = _settings.MoveDurationFast;
    }
}
