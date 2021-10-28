using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Messages;
using RX;

namespace Data
{
    [Serializable]
    public class GameState : AbstractGameState, ICommandHandler
    {
        [SerializeField, JsonProperty("board_state")] internal BoardState _boardState;
        [SerializeField, JsonProperty("user_state")] internal UserState _userState;

        public void SetupCommandHandlers(ICommandService commandService)
        {
            commandService.Subscribe(async x =>
            {
                if (x.Important)
                    await Save();
            }, false, int.MaxValue);
        }

        protected override void SetDefaults()
        {
            base.SetDefaults();
            _userState = new UserState();
            _boardState = new BoardState();
        }
    }
}