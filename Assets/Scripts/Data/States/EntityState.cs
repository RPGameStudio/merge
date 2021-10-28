using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Mathematics;

namespace Data
{
    [Serializable]
    public class EntityState : EntryState<EntityData>
    {
        [SerializeField, JsonProperty("position")] public Vector3 Position;

        public EntityState()
        {
        }

        public EntityState(EntityData entry) : base(entry)
        {
        }
    }
}