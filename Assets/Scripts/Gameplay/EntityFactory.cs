using System.Threading.Tasks;
using Data;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class EntityFactory : IFactory<EntityState, Task<EntityView>>
    {
        private Transform _holder;

        public EntityFactory(Transform holder) => _holder = holder;

        public async Task<EntityView> Create(EntityState state)
        {
            return await state.Data.Prefab.InstantiateFromHandle<EntityView>(_holder);
        }
    }
}