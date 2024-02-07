using DG.Tweening;
using System;
using UnityEngine;

public class SnakePartComponent : MonoBehaviour
{
    public event Action<SnakePartComponent> OnTailCreated;
    public Vector3 CurrentTargetLocalPosition { get; private set; }
    [SerializeField]
    private SnakePartComponent tailPartPrefab;

    private SnakePartComponent tailPart;
    public Transform PartTransform { get; private set; }
    private Collider2D partCollider;

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
        var angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        var quaternionRotationZ = Quaternion.AngleAxis(angle, Vector3.forward).eulerAngles.z;
        PartTransform.DORotate(new Vector3(0, 0, quaternionRotationZ + 90), moveDuration / 2)
            .SetEase(Ease.Linear);
        tailPart?.MoveToLocalPosition(PartTransform.localPosition, moveDuration);
    }

    public void CreateTail()
    {
        if (tailPart)
        {
            tailPart.CreateTail();
            return;
        }

        var tail = Instantiate(
            tailPartPrefab,
            PartTransform.position,
            PartTransform.rotation,
            PartTransform.parent);

        tailPart = tail;

        OnTailCreated?.Invoke(tail);
    }
    
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}