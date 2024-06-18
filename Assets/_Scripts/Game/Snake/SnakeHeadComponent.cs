using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class SnakeHeadComponent : SnakePartComponent
{
    public event Action OnHeadCollided;

    [FormerlySerializedAs("tongue")]
    [SerializeField]
    private Transform _tongue;

    private SnakePartsCollectorService _snakeBodyViewService;
    private Sequence _tongueSequence;

    [Inject]
    private void Construct(SnakePartsCollectorService bodyViewService)
    {
        _snakeBodyViewService = bodyViewService;
    }

    private void Start()
    {
        this.OnCollisionEnter2DAsObservable().Take(1).Subscribe(_ =>
            {
                GetComponent<Collider2D>().enabled = false;
                OnHeadCollided?.Invoke();
            }).AddTo(this);

        this.OnTriggerEnter2DAsObservable().Where(x => x.TryGetComponent<FoodComponent>(out _)).Subscribe(_ =>
            {
                CreateTail();
            }).AddTo(this);

        StartTongueAnimation();
    }

    public override void MoveToLocalPosition(Vector3 targetLocalPosition, float moveDuration)
    {
        base.MoveToLocalPosition(targetLocalPosition, moveDuration);
        for (int i = 1; i < _snakeBodyViewService.SnakeParts.Count; i++)
        {
            if (_snakeBodyViewService.SnakeParts[i].CurrentTargetLocalPosition != CurrentTargetLocalPosition) continue;
            OnHeadCollided?.Invoke();
            _tongueSequence.Kill(true);
        }
    }

    private void StartTongueAnimation()
    {
        _tongueSequence = DOTween.Sequence();
        _tongueSequence.Append(_tongue.DOLocalMoveY(1, 0.1f));
        _tongueSequence.Append(_tongue.DOLocalMoveY(0, 0.1f));
        _tongueSequence.AppendInterval(1);
        _tongueSequence.SetLoops(-1, LoopType.Restart);
        _tongueSequence.Play();
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
