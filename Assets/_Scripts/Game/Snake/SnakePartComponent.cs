using DG.Tweening;
using System;
using UnityEngine;
using Zenject;

public partial class SnakePartComponent : MonoBehaviour
{
    public event Action<SnakePartComponent> OnTailCreated;
    public Vector3 CurrentTargetLocalPosition { get; private set; }
    public Transform PartTransform { get; private set; }

    private SnakePartComponent _tailPart;
    private Collider2D _partCollider;
    private Factory _factory;

    [Inject]
    private void Construct(Factory factory)
    {
        _factory = factory;
    }

    private void Awake()
    {
        PartTransform = transform;
        CurrentTargetLocalPosition = PartTransform.localPosition;
    }


    public virtual void MoveToLocalPosition(Vector3 targetLocalPosition, float moveDuration)
    {
        CurrentTargetLocalPosition = targetLocalPosition;
        Vector2 moveDirection = PartTransform.localPosition - targetLocalPosition;
        PartTransform.DOLocalMove(targetLocalPosition, moveDuration).SetEase(Ease.Linear);
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        float quaternionRotationZ = Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles.z;
        PartTransform.DORotate(new Vector3(0, 0, quaternionRotationZ + 90), moveDuration / 2).SetEase(Ease.Linear);

        if (_tailPart)
        {
            _tailPart.MoveToLocalPosition(PartTransform.localPosition, moveDuration);
        }
    }

    protected void CreateTail()
    {
        if (_tailPart)
        {
            _tailPart.CreateTail();
            return;
        }

        var tail = _factory.Create(
            PartTransform.position,
            PartTransform.rotation,
            PartTransform.parent);

        _tailPart = tail;

        OnTailCreated?.Invoke(tail);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
