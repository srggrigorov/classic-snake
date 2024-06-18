using System;

public class GameStateService
{
    public delegate void GameEndDelegate(bool isVictory);
    public event GameEndDelegate OnGameEnded;
    public event Action OnGameStarted;

    private readonly SnakePartsCollectorService _snakePartsCollectorService;
    private readonly InputService _inputService;

    public GameStateService(SnakePartsCollectorService snakePartsCollectorService, InputService inputService)
    {
        (_snakePartsCollectorService, _inputService) = (snakePartsCollectorService, inputService);
    }

    public void StartGame()
    {
        OnGameStarted?.Invoke();
        _snakePartsCollectorService.OnHeadCollided += HandleGameLost;
        _inputService.SetActive(true);
    }

    public void HandleGameLost() => StopGame();

    private void StopGame(bool isVictory = false)
    {
        _inputService.SetActive(false);
        _snakePartsCollectorService.OnHeadCollided -= HandleGameLost;
        OnGameEnded?.Invoke(isVictory);
    }
}
