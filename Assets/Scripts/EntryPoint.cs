using System.Threading.Tasks;
using Merge;
using UnityEngine;
using Zenject;
using Data;
using System.Linq;
using Messages;
using System.Reflection;
using Gameplay;
using UI;
using UnityEngine.AddressableAssets;

public partial class EntryPoint : MonoInstaller
{
    [SerializeField] private AssetReference _mergeDb;
    [SerializeField] private AssetReference _config;

    [SerializeField] private BoardController _board;

    public override async void InstallBindings()
    {
        _board.gameObject.SetActive(false);

        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;

        var state = new GameState();
        var commandService = new CommandService();
        state.SetupCommandHandlers(commandService);

        Container.Bind<InputService>().FromNew().AsSingle();
        Container.Bind<BalanceTracker>().WithId("main_tracker").FromComponentInHierarchy().AsSingle();
        Container.Bind<GameState>().FromInstance(state).AsSingle();
        Container.Bind<EntityFactory>().FromInstance(new EntityFactory(_board.transform)).AsSingle();
        Container.Bind<ICommandService>().FromInstance(commandService).AsSingle();

        var datalayerLoadResult = await state.Initialize().Load();

        foreach (var item in datalayerLoadResult.DatalayerObjects)
        {
            var type = item.GetType();

            if (type.GetCustomAttribute<MarkForBindAttribute>(true) != null)
            {
                Container.BindInstances(item);
            }

            if (typeof(ICommandHandler).IsAssignableFrom(type))
            {
                (item as ICommandHandler).SetupCommandHandlers(commandService);
            }
        }

        var config = await _config.LoadFromHandle<Config>(this);
        var mergeDb = await _mergeDb.LoadFromHandle<MergeDatabase>(this);

        Container.BindInterfacesAndSelfTo<MergeDatabase>().FromInstance(mergeDb).AsSingle();
        Container.Bind<IContentStorage>().FromInstance(config).AsSingle();

        while (Container.IsInstalling)
            await Task.Yield();

        InitializeExtentions(config, mergeDb);
        await _board.Initialize(Container.Resolve<BoardState>(), config);
        _board.gameObject.SetActive(true);
    }

    private void InitializeExtentions(Config config, IMerger<EntityData> merger)
    {
        StateExtentions.Initialize(config);
        MergeExtentions.Initialize(merger);
        AddressablesExtentions.Initialize(Container);
    }
}