using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SnakeDPadComponent : MonoBehaviour
{
    public event Action<Vector2Int> OnDirectionChanged;

    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private Button rightButton;
    [SerializeField]
    private Button downButton;
    [SerializeField]
    private Button upButton;

    private CompositeDisposable disposable = new();
    private Vector2Int currentDirection = Vector2Int.zero;

    private void Awake()
    {
        disposable.Add(leftButton.onClick.AsObservable().Subscribe(_ => ChangeDirection(Vector2Int.left)));
        disposable.Add(rightButton.onClick.AsObservable().Subscribe(_ => ChangeDirection(Vector2Int.right)));
        disposable.Add(downButton.onClick.AsObservable().Subscribe(_ => ChangeDirection(Vector2Int.down)));
        disposable.Add(upButton.onClick.AsObservable().Subscribe(_ => ChangeDirection(Vector2Int.up)));
    }

    private void ChangeDirection(Vector2Int direction)
    {
        if (Vector2.Dot(direction, currentDirection) != 0)
        {
            return;
        }

        currentDirection = direction;
        OnDirectionChanged?.Invoke(currentDirection);
    }

    private void OnDestroy()
    {
        disposable.Dispose();
    }
}