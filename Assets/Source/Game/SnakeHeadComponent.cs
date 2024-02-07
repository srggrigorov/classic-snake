using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SnakeHeadComponent : SnakePartComponent
{
    public event Action OnHeadCollided;
    [SerializeField]
    private SnakeBodyComponent snakeBody;
    [SerializeField]
    private Transform tongue;

    private Sequence tongueSequence;

    private void Start()
    {
        this.OnCollisionEnter2DAsObservable().Take(1).Subscribe(_ =>
        {
            GetComponent<Collider2D>().enabled = false;
            OnHeadCollided?.Invoke();
        }).AddTo(this);

        this.OnTriggerEnter2DAsObservable().Where(x => x.TryGetComponent<SnakeFoodComponent>(out _)).Subscribe(_ =>
        {
            CreateTail();
        }).AddTo(this);

        StartTongueAnimation();
    }

    public override void MoveToLocalPosition(Vector3 targetLocalPosition, float moveDuration)
    {
        base.MoveToLocalPosition(targetLocalPosition, moveDuration);
        for (int i = 1; i < snakeBody.SnakeParts.Count; i++)
        {
            if (snakeBody.SnakeParts[i].CurrentTargetLocalPosition != CurrentTargetLocalPosition) continue;
            OnHeadCollided?.Invoke();
            tongueSequence.Kill(true);
        }
    }

    private void StartTongueAnimation()
    {
        tongueSequence = DOTween.Sequence();
        tongueSequence.Append(tongue.DOLocalMoveY(1, 0.1f));
        tongueSequence.Append(tongue.DOLocalMoveY(0, 0.1f));
        tongueSequence.AppendInterval(1);
        tongueSequence.SetLoops(-1, LoopType.Restart);
        tongueSequence.Play();
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}