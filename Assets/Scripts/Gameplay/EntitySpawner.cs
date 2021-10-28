using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Merge;
using Messages;
using UnityEngine;

namespace Gameplay
{
    public partial class BoardController
    {
        public class EntitySpawner
        {
            private BoardController BoardController { get; }
            private EntityFactory Factory => BoardController._factory;
            private ICommandService CommandService => BoardController._commandService;

            private const float MERGE_RADIUS = 0.6f;

            public EntitySpawner(BoardController boardController)
            {
                BoardController = boardController;
            }

            public async Task<EntityView> SpawnExistingEntity(EntityState state, HashSet<EntityView> _pool)
            {
                var view = await Factory.Create(state);
                _pool.Add(view);

                view.Initialize(new EnemyViewData
                {
                    State = state,
                    OnDrag = (view) => view.State.Position = view.transform.position,
                    OnBeginDrag = (view) => BoardController._inputService.IsDraggingEntity = true,
                    OnEndDrag = async (view) =>
                    {
                        var targets = _pool.Where(x => x != view && Vector2.Distance(x.transform.position, view.transform.position) <= MERGE_RADIUS);

                        foreach (var target in targets)
                        {
                            if (!view.State.Data.CanBeMerged(target.State.Data))
                                continue;

                            var spawnPosition = await target.VisualMergeWith(view);

                            _pool.Remove(view);
                            _pool.Remove(target);

                            _ = CommandService.Apply(new DestroyEntitiesCommand
                            {
                                Entities = new EntityState[] { view.State, target.State },
                            });

                            var mergeResult = target.State.Data.Merge(view.State.Data);
                            await SpawnNewEntity(mergeResult, _pool, spawnPosition);
                            break;
                        }
                        BoardController._inputService.IsDraggingEntity = false;
                    },
                    OnClick = (view) => _ = BoardController.TrySpawnNewCoin(view.State)
                });

                view.transform.position = state.Position;

                if (view.State.Data.Profit > 0 && view.State.Data.ProfitSpawnPeriod > 0)
                {
                    var profitGenerator = view.gameObject.AddComponent<ProfitGenerator>();
                    profitGenerator.Initialize((entityState) =>
                    {
                        _ = BoardController.TrySpawnNewCoin(entityState);
                    });
                }

                return view;
            }

            public async Task<EntityView> SpawnNewEntity(EntityData data, HashSet<EntityView> _pool, Vector3 position)
            {
                var result = new EntityState(data)
                {
                    Position = position,
                };

                _ = CommandService.Apply(new SpawnEntitiesCommand
                {
                    Entities = new EntityState[] { result },
                });

                return await SpawnExistingEntity(result, _pool);
            }
        }
    }
}