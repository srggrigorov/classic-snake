using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SnakeFoodComponent : MonoBehaviour
{
    public event Action OnFoodCollected;

    private void Start()
    {
        this.OnTriggerEnter2DAsObservable().Select(x => x.GetComponent<SnakePartComponent>())
            .Where(x => x != null)
            .Subscribe(_ => { OnFoodCollected?.Invoke(); }).AddTo(this);
    }

    public void RespawnFood(Vector3 position) => transform.localPosition = position;
}