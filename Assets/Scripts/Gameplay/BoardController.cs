using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Messages;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using RX;

namespace Gameplay
{
    public partial class BoardController : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _coin;

        private float _timeSinceLastSpawn = float.MaxValue;
        private HashSet<EntityView> _entityViews;
        private HashSet<CoinView> _coinViews;
        private EntitySpawner _entitySpawner;
        private CoinSpawner _coinSpawner;
        private EntityData _defaultData;
        private IContentStorage _contenStorage;

        [Inject] private EntityFactory _factory;
        [Inject] private ICommandService _commandService;
        [Inject] private InputService _inputService;

        private const float MIN_SPAWN_DISTANCE = 0.5f;
        private const int MAX_ITEMS_ON_SCENE = 20;
        private const int MAX_COINS_AMOUNT = 100;

        public async Task Initialize(BoardState boardState, IContentStorage contentStorage)
        {
            _contenStorage = contentStorage;
            _entityViews = new HashSet<EntityView>();
            _coinViews = new HashSet<CoinView>();
            _entitySpawner = new EntitySpawner(this);
            _coinSpawner = new CoinSpawner(this);
            _defaultData = _contenStorage.GetEntriesOfType<EntityData>().OrderBy(x => x.Profit).First();

            _commandService.Filter<CollectCoinCommand>().Subscribe(async command =>
            {
                _coinViews.RemoveWhere(x => x.Data.State == command.Coin);
            });

            await RestoreState(boardState);
        }

        private async Task RestoreState(BoardState boardState)
        {
            foreach (var entity in boardState.Entities)
            {
                await _entitySpawner.SpawnExistingEntity(entity, _entityViews);
            }

            foreach (var coinState in boardState.Coins)
            {
                await _coinSpawner.SpawnCoin(coinState);
            }
        }

        private async void Update()
        {
            if (_entityViews.Count >= MAX_ITEMS_ON_SCENE)
                return;

            _timeSinceLastSpawn += Time.deltaTime;

            var data = _defaultData;

            if (_timeSinceLastSpawn < data.SpawnPeriod)
                return;

            _timeSinceLastSpawn = 0;
            await _entitySpawner.SpawnNewEntity(data, _entityViews, _entityViews.GetFreePosition(MIN_SPAWN_DISTANCE, 0.5f));
        }

        private async Task TrySpawnNewCoin(EntityState entityState)
        {
            if (_coinViews.Count >= MAX_COINS_AMOUNT)
                return;

            var view = await _coinSpawner.SpawnCoin(entityState, entityState.Position.GetNearestRandomPosition(MIN_SPAWN_DISTANCE / 2, 0.2f));

            _coinViews.Add(view);

            _ = _commandService.Apply(new SpawnCoinCommand
            {
                Coin = view.Data.State
            });
        }
    }
}