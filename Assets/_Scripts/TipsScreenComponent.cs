using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class TipsScreenComponent : MonoBehaviour
{
    [SerializeField]
    private Button _startGameButton;
    [SerializeField]
    private TMP_Text _tipText;

    private GameStateService _gameStateService;
    private ZenjectSceneLoader _sceneLoader;
    private AssetsManager _assetsManager;

    [Inject]
    private void Construct(GameStateService gameStateService, ZenjectSceneLoader sceneLoader, AssetsManager assetsManager)
    {
        (_gameStateService, _sceneLoader, _assetsManager) = (gameStateService, sceneLoader, assetsManager);
        _startGameButton.onClick.AddListener(StartButtonPressed);
    }

    private void StartButtonPressed()
    {
        _startGameButton.onClick.RemoveListener(StartButtonPressed);
        _gameStateService.OnGameEnded += ShowGameEndTip;
        _gameStateService.StartGame();
        _startGameButton.onClick.AddListener(ReloadGame);
        gameObject.SetActive(false);
    }

    private void ShowGameEndTip(bool isVictory)
    {
        _tipText.text = $"You {(isVictory ? "won" : "lost")}!<br>Tap to play again.";
        gameObject.SetActive(true);
    }

    private void ReloadGame()
    {
        _sceneLoader.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single,
            container => container.Bind<AssetsManager>().FromInstance(_assetsManager).AsSingle().NonLazy());
    }
}
