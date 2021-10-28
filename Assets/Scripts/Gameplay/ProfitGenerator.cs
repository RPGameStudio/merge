using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(EntityView))]
    public class ProfitGenerator : MonoBehaviour
    {
        private EntityView _view;

        private EntityState State => _view.State;
        private float _lastProfitTime;
        private Action<EntityState> _spawnProfit;
        private float _spawnPeriod;

        public void Initialize(Action<EntityState> spawnProfit)
        {
            Reset();
            _spawnProfit = spawnProfit;
            ResetSpawnPeriod();
        }

        private void Update()
        {
            _lastProfitTime += Time.deltaTime;

            if (_lastProfitTime < _spawnPeriod)
                return;

            _spawnProfit?.Invoke(State);
            _lastProfitTime = 0;
            ResetSpawnPeriod();
        }

        private void ResetSpawnPeriod() => _spawnPeriod = UnityEngine.Random.Range(_view.State.Data.ProfitSpawnPeriod * 0.9f, _view.State.Data.ProfitSpawnPeriod * 1.1f);
        private void Reset() => _view = GetComponent<EntityView>();
    }
}