using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Zenject;
using Messages;
using RX;
using System.Linq;

namespace Data
{
    [Serializable, MarkForBind]
    public class BoardState : ICommandHandler
    {
        [SerializeField, JsonProperty("entities")] internal List<EntityState> _entities = new List<EntityState>();
        [SerializeField, JsonProperty("coins")] internal List<CoinState> _coins = new List<CoinState>();

        [JsonIgnore] public IEnumerable<EntityState> Entities => _entities;
        [JsonIgnore] public IEnumerable<CoinState> Coins => _coins;

        public void SetupCommandHandlers(ICommandService commandService)
        {
            commandService.Filter<SpawnEntitiesCommand>().Subscribe(async (command) =>
            {
                _entities.AddRange(command.Entities);
            });

            commandService.Filter<DestroyEntitiesCommand>().Subscribe(async (command) =>
            {
                foreach (var item in command.Entities)
                {
                    _entities.RemoveAll(x => command.Entities.Contains(x));
                }
            });

            commandService.Filter<SpawnCoinCommand>().Subscribe(async command =>
            {
                _coins.Add(command.Coin);
            });

            commandService.Filter<CollectCoinCommand>().Subscribe(async command =>
            {
                _coins.Remove(command.Coin);
            });
        }
    }

    public class SpawnCoinCommand : ICommand
    {
        public bool Important => false;
        public CoinState Coin;
    }

    public class CollectCoinCommand : ICommand
    {
        public bool Important => false;
        public CoinState Coin;
    }

    public class SpawnEntitiesCommand : ICommand
    {
        public bool Important => true;
        public IEnumerable<EntityState> Entities;
    }

    public class DestroyEntitiesCommand : ICommand
    {
        public bool Important => true;
        public IEnumerable<EntityState> Entities;
    }
}