using UnityEngine;
using Zenject;

public partial class SnakePartComponent
{
    public class Factory : PlaceholderFactory<SnakePartComponent>
    {
        private SnakePartComponent _snakePartPrefab;

        public SnakePartComponent Create(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            SnakePartComponent snakePart = Create();
            Transform snakePartTransform = snakePart.transform;
            snakePartTransform.SetPositionAndRotation(position, rotation);
            snakePartTransform.SetParent(parent);
            return snakePart;
        }
    }
}
