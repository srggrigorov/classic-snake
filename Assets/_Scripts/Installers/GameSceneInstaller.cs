using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private SnakePartComponent _snakePartPrefab;
    [SerializeField] private SnakeHeadComponent _snakeHead;
    [SerializeField] private FoodComponent _food;
    [SerializeField] private LineRenderer _snakeLineRenderer;

    public override void InstallBindings()
    {
        Container.Bind<InputService>().AsSingle().NonLazy();

        Container.BindFactory<SnakePartComponent, SnakePartComponent.Factory>()
            .FromComponentInNewPrefab(_snakePartPrefab).AsSingle().NonLazy();

        Container.Bind<SnakePartsCollectorService>().AsSingle().WithArguments(_snakeHead).NonLazy();
        Container.Bind<GameStateService>().AsSingle().NonLazy();
        Container.Bind<FoodSpawnerService>().AsSingle().WithArguments(_food).NonLazy();
        Container.Bind<ScoreCounterService>().AsSingle().NonLazy();
        Container.Bind<ILateTickable>().To<SnakeBodyViewService>().AsSingle().WithArguments(_snakeLineRenderer).NonLazy();
        Container.Bind<SnakeMovementService>().AsSingle().WithArguments(_snakeHead).NonLazy();
    }
}
