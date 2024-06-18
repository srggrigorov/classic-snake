using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider2D))]
public class FoodComponent : MonoBehaviour
{
    public event Action OnFoodCollected;

    private FoodSpawnerService _foodSpawnerService;

    [Inject]
    private void Construct(FoodSpawnerService foodSpawnerService)
    {
        _foodSpawnerService = foodSpawnerService;
        foodSpawnerService.OnOutOfFreeSpace += HandleOutOfFreeSpace;
    }

    private void Start()
    {
        this.OnTriggerEnter2DAsObservable().Select(x => x.GetComponent<SnakePartComponent>())
            .Where(x => x != null)
            .Subscribe(_ => { OnFoodCollected?.Invoke(); }).AddTo(this);
    }

    private void HandleOutOfFreeSpace()
    {
        _foodSpawnerService.OnOutOfFreeSpace -= HandleOutOfFreeSpace;
        Destroy(gameObject);
    }
}
