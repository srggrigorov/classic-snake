using System;
using System.Collections.Generic;
using UnityEngine;

public class SnakePartsCollectorService : IDisposable
{
    public event Action OnHeadCollided;
    public event Action OnTailCreated;
    public IReadOnlyDictionary<int, SnakePartComponent> SnakeParts => _snakeParts;

    private readonly SnakeHeadComponent _snakeHead;
    private readonly Dictionary<int, SnakePartComponent> _snakeParts;

    private SnakePartComponent _lastSnakePart;

    public SnakePartsCollectorService(SnakeHeadComponent snakeHead)
    {
        _snakeHead = snakeHead;
        _snakeParts = new Dictionary<int, SnakePartComponent>();

        _snakeHead.OnHeadCollided += HandleHeadCollided;
        _snakeParts[_snakeParts.Count] = _snakeHead;
        _lastSnakePart = _snakeHead;
        _lastSnakePart.OnTailCreated += HandleTailCreated;
    }

    private void HandleTailCreated(SnakePartComponent tailPart)
    {
        _lastSnakePart.OnTailCreated -= HandleTailCreated;
        _snakeParts[_snakeParts.Count] = tailPart;
        _lastSnakePart = tailPart;
        _lastSnakePart.OnTailCreated += HandleTailCreated;
        OnTailCreated?.Invoke();
    }

    private void HandleHeadCollided()
    {
        _lastSnakePart.OnTailCreated -= HandleTailCreated;
        _snakeHead.OnHeadCollided -= HandleHeadCollided;
        OnHeadCollided?.Invoke();
    }

    public Vector3[] GetSnakePartsCoordinates()
    {
        Vector3[] coordinates = new Vector3[_snakeParts.Count];
        for (int i = 0; i < coordinates.Length; i++)
        {
            coordinates[i] = _snakeParts[i].CurrentTargetLocalPosition;
        }

        return coordinates;
    }
    public void Dispose()
    {
        _snakeHead.OnHeadCollided -= HandleHeadCollided;
        _lastSnakePart.OnTailCreated -= HandleTailCreated;
    }
}
