using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    [Serializable]
    public abstract class EntryState
    {
    }


    [Serializable]
    public abstract class EntryState<T> : EntryState
        where T : IDatabaseEntry
    {
        [SerializeField, JsonProperty("database_guid")] internal string _dbGuid;

        public EntryState() { }
        public EntryState(T entry) => _dbGuid = entry.Guid;

        [JsonIgnore] public T Data => this.GetData();
    }
}