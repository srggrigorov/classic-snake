using System;

public class ScoreCounterService : IDisposable
{
    public Action<int> OnScoreUpdated;

    private int _currentScore;
    private readonly FoodSpawnerService _foodSpawnerService;
    private readonly GameStateService _gameStateService;

    public ScoreCounterService(FoodSpawnerService foodSpawnerService, GameStateService gameStateService)
    {
        (_foodSpawnerService, _gameStateService) = (foodSpawnerService, gameStateService);
        _gameStateService.OnGameStarted += BeginScoreUpdate;
    }

    private void BeginScoreUpdate()
    {
        _gameStateService.OnGameStarted -= BeginScoreUpdate;
        _foodSpawnerService.OnFoodCollected += UpdateScore;
    }

    private void UpdateScore()
    {
        _currentScore++;
        OnScoreUpdated?.Invoke(_currentScore);
    }

    public void Dispose()
    {
        _foodSpawnerService.OnFoodCollected -= UpdateScore;
    }
}
