using System.Threading.Tasks;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay
{
    public partial class BoardController
    {
        public class CoinSpawner
        {
            private BoardController BoardController { get; }
            private AssetReferenceGameObject CoinPrefab => BoardController._coin;

            public CoinSpawner(BoardController boardController) => BoardController = boardController;

            public async Task<CoinView> SpawnCoin(CoinState coinState)
            {
                var coinView = await CoinPrefab.InstantiateFromHandle<CoinView>(BoardController.transform);

                coinView.Initialize(new CoinViewInitialData
                {
                    State = coinState,
                    OnCollect = (state) =>
                    {
                        BoardController._commandService.Apply(new CollectCoinCommand
                        {
                            Coin = state,
                        });
                    }
                });

                coinView.transform.position = coinState.Position;

                return coinView;
            }

            public async Task<CoinView> SpawnCoin(EntityState entityState, Vector3 position)
            {
                var coinState = new CoinState()
                {
                    _position = position,
                    _profit = entityState.Data.Profit,
                };

                return await SpawnCoin(coinState);
            }
        }
    }
}