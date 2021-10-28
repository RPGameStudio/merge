using System;
using UnityEngine;
using Newtonsoft.Json;
using Messages;
using RX;

namespace Data
{
    [Serializable, MarkForBind]
    public class UserState : ICommandHandler
    {
        [SerializeField, JsonProperty("balance")] internal ReactiveProperty<int> _balance = new ReactiveProperty<int>();

        [JsonIgnore] public IReadonlyReactiveProperty<int> Balance => _balance;

        public void SetupCommandHandlers(ICommandService commandService)
        {
            commandService.Filter<CollectCoinCommand>().Subscribe(async command =>
            {
                _balance.Value += command.Coin.Profit;
            });
        }
    }
}