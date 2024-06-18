using TMPro;
using UnityEngine;
using Zenject;

public class ScoreViewComponent : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;

    private ScoreCounterService _scoreCounterService;

    [Inject]
    private void Construct(ScoreCounterService scoreCounterService)
    {
        _scoreCounterService = scoreCounterService;
        _scoreCounterService.OnScoreUpdated += UpdateScore;
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = score.ToString();
    }
}
