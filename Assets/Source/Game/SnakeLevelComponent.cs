using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SnakeLevelComponent : MonoBehaviour
{
    [SerializeField]
    private Vector2Int bordersX;
    [SerializeField]
    private Vector2Int bordersY;
    [SerializeField]
    private SnakeFoodComponent foodItem;
    [SerializeField]
    private SnakeBodyComponent snakeBody;
    [SerializeField]
    private Button tipScreenButton;
    [SerializeField]
    private TMP_Text tipScreenText;
    [SerializeField]
    private TMP_Text scoreText;

    private readonly HashSet<Vector3> coordinatesInBorders = new();

    private int currentScore;

    private void Start()
    {
        SetUpLevel();
        tipScreenButton.onClick.AddListener(StartGame);
    }

    public void SetUpLevel()
    {
        for (int x = bordersX.x + 1; x < bordersX.y; x++)
        {
            for (int y = bordersY.x + 1; y < bordersY.y; y++)
            {
                coordinatesInBorders.Add(new Vector3(x, y, 0));
            }
        }
    }

    private void StartGame()
    {
        tipScreenButton.onClick.RemoveListener(StartGame);
        tipScreenButton.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        tipScreenButton.gameObject.SetActive(false);
        foodItem.OnFoodCollected += OnFoodCollected;
        snakeBody.OnSnakeDied += OnGameLost;
        snakeBody.StartMovement();
        foodItem.RespawnFood(RandomCoordinatesWithoutSnake().GetValueOrDefault());
    }

    private void OnFoodCollected()
    {
        currentScore++;
        scoreText.text = currentScore.ToString();
        var coordinateWithoutSnake = RandomCoordinatesWithoutSnake();
        if (coordinateWithoutSnake == null)
        {
            OnGameWin();
            return;
        }

        foodItem.RespawnFood(
            coordinateWithoutSnake.Value);
    }

    private void OnGameWin()
    {
        tipScreenText.text = "You won!<br>Tap to play again.";
        tipScreenButton.gameObject.SetActive(true);
        StopGame();
    }

    private void OnGameLost()
    {
        tipScreenText.text = "You lost!<br>Tap to play again.";
        tipScreenButton.gameObject.SetActive(true);
        StopGame();
    }

    private void StopGame()
    {
        foodItem.OnFoodCollected -= OnFoodCollected;
        snakeBody.StopMovement();
        snakeBody.OnSnakeDied -= OnGameLost;
    }

    private Vector3? RandomCoordinatesWithoutSnake()
    {
        var coordinatesWithoutSnake =
            coordinatesInBorders.Except(snakeBody.GetSnakePartsCoordinates()).ToList();
        if (!coordinatesWithoutSnake.Any())
        {
            coordinatesWithoutSnake.Clear();
            return null;
        }

        var coordinateWithoutSnake =
            coordinatesWithoutSnake[Random.Range(0, coordinatesWithoutSnake.Count)];
        coordinatesWithoutSnake.Clear();
        return coordinateWithoutSnake;
    }
}