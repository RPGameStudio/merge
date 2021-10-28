using System;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace Data
{
    [Serializable]
    public class CoinState
    {
        [SerializeField, JsonProperty("position")] internal Vector3 _position;
        [SerializeField, JsonProperty("profit")] internal int _profit;

        [JsonIgnore] public int Profit => _profit;
        [JsonIgnore] public Vector3 Position => _position;
    }
}