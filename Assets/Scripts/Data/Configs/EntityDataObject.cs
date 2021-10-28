using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using Data;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Data/Entity data", fileName = "New entity data")]
public class EntityDataObject : DatabaseItem<EntityData>
{
    protected override async Task Validate()
    {
        await base.Validate();
        Data.Name = name;
    }
}

[Serializable]
public class EntityData : DatabaseEntry
{
    [SerializeField, JsonProperty("profit")] internal int _profit;
    [SerializeField, JsonProperty("prefab")] internal AssetReference _prefab;
    [SerializeField, JsonProperty("profit_spawn_period")] internal float _profitSpawnPeriod;
    [SerializeField, JsonProperty("spawn_period"), Min(0.1f)] internal float _spawnPeriod;

    [JsonIgnore] public float SpawnPeriod => _spawnPeriod;
    [JsonIgnore] public float ProfitSpawnPeriod => _profitSpawnPeriod;
    [JsonIgnore] public AssetReference Prefab => _prefab;
    [JsonIgnore] public int Profit => _profit;

}
