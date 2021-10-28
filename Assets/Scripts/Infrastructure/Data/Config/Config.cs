using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Database;
using System;
using System.Linq;

[assembly: InternalsVisibleTo("rpgames.database.editor")]

namespace Data
{
    public interface IContentStorage
    {
        T RequestData<T>(string guid) where T: IDatabaseEntryReadonly;
        IEnumerable<T> GetEntriesOfType<T>() where T : IDatabaseEntryReadonly;
    }

    public class Config : ScriptableObject, IContentStorage
    {
        [SerializeField, JsonProperty("content"), ReadOnly] internal List<ContentObject> _content;

        private Dictionary<string, IDatabaseEntryReadonly> _cache;
        private Dictionary<string, IDatabaseEntryReadonly> Cache
        {
            get
            {
                if (_cache == null)
                    _cache = _content.ToDictionary(x => x.Guid, x => x.Data);

                return _cache;
            }
        }

        public IEnumerable<T> GetEntriesOfType<T>() where T : IDatabaseEntryReadonly
        {
            return Cache.Values.Where(x => x is T).Select(x => (T)x);
        }

        public T RequestData<T>(string guid) where T : IDatabaseEntryReadonly
        {
            return (T)Cache[guid];
        }
    }

    [Serializable]
    public class ContentObject
    {
        [SerializeField, JsonProperty("guid")] internal string _guid;
        [SerializeReference, JsonProperty("data")] internal IDatabaseEntryReadonly _data;

        [JsonIgnore] public IDatabaseEntryReadonly Data => _data;
        [JsonIgnore] public string Guid => _guid;
    }
}
