using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoodSpawnerService : IDisposable
{
    public event Action OnOutOfFreeSpace;
    public event Action OnFoodCollected;

    private readonly Vector2Int _bordersX = new Vector2Int(-8, 8);
    private readonly Vector2Int _bordersY = new Vector2Int(-13, 10);

    private readonly FoodComponent _food;

    private readonly HashSet<Vector3> _coordinatesInBorders = new HashSet<Vector3>();
    private readonly SnakePartsCollectorService _snakePartsCollectorService;
    private readonly GameStateService _gameStateService;

    public FoodSpawnerService(FoodComponent food, SnakePartsCollectorService snakePartsCollectorService,
        GameStateService gameStateService)
    {
        (_food, _snakePartsCollectorService, _gameStateService) = (food, snakePartsCollectorService, gameStateService);
        _food.OnFoodCollected += HandleFoodCollected;
        OnOutOfFreeSpace += _gameStateService.HandleGameLost;
        _gameStateService.OnGameStarted += HandleGameStarted;

        SetUpFoodSpawnCoordinates();
    }

    private void HandleGameStarted()
    {
        _gameStateService.OnGameStarted -= HandleGameStarted;
        HandleFoodCollected();
    }

    private void RespawnFood(Vector3 position) => _food.transform.localPosition = position;
    private void HandleFoodCollected()
    {
        OnFoodCollected?.Invoke();
        var coordinateWithoutSnake = RandomCoordinatesWithoutSnake();
        if (coordinateWithoutSnake is null)
        {
            OnOutOfFreeSpace?.Invoke();
            OnOutOfFreeSpace -= _gameStateService.HandleGameLost;
            return;
        }

        RespawnFood(coordinateWithoutSnake.Value);
    }

    private void SetUpFoodSpawnCoordinates()
    {
        for (int x = _bordersX.x + 1; x < _bordersX.y; x++)
        {
            for (int y = _bordersY.x + 1; y < _bordersY.y; y++)
            {
                _coordinatesInBorders.Add(new Vector3(x, y, 0));
            }
        }
    }

    private Vector3? RandomCoordinatesWithoutSnake()
    {
        List<Vector3> coordinatesWithoutSnake =
            _coordinatesInBorders.Except(_snakePartsCollectorService.GetSnakePartsCoordinates()).ToList();

        if (!coordinatesWithoutSnake.Any())
        {
            coordinatesWithoutSnake.Clear();
            return null;
        }

        Vector3 coordinateWithoutSnake = coordinatesWithoutSnake[Random.Range(0, coordinatesWithoutSnake.Count)];
        coordinatesWithoutSnake.Clear();
        return coordinateWithoutSnake;
    }
    public void Dispose()
    {
        OnOutOfFreeSpace -= _gameStateService.HandleGameLost;
        _food.OnFoodCollected -= HandleFoodCollected;
    }
}
