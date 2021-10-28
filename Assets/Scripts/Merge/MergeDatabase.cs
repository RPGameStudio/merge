using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Data;
using Database;

namespace Merge
{
    public interface IMerger<T>
    {
        public bool CanBeMerged(T a, T b);
        public T Merge(T a, T b);
    }

    [CreateAssetMenu(menuName = "Data/Merge database", fileName = "Merge databse")]
    public class MergeDatabase : ScriptableObjectValidate, IMerger<EntityData>
    {
        [SerializeField] private List<MergeData> _pairs;

        [NonSerialized] private Dictionary<EntityData, Dictionary<EntityData, EntityData>> _cache;
        public Dictionary<EntityData, Dictionary<EntityData, EntityData>> Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new Dictionary<EntityData, Dictionary<EntityData, EntityData>>();
                    //parse base list
                    ParseMergeData(_pairs.Select(x => (x._entity1.Data, x._entity2.Data, x._resultEntity.Data)));
                    //parse mirrored values without overriding existing pairs
                    ParseMergeData(_pairs.Select(x => (x._entity2.Data, x._entity1.Data, x._resultEntity.Data)));
                }

                return _cache;

                void ParseMergeData(IEnumerable<(EntityData _entity1, EntityData _entity2, EntityData _result)> mergeDatas)
                {
                    foreach (var pair in mergeDatas)
                    {
                        if (!_cache.ContainsKey(pair._entity1))
                        {
                            _cache[pair._entity1] = new Dictionary<EntityData, EntityData>();
                        }

                        if (!_cache[pair._entity1].ContainsKey(pair._entity2))
                        {
                            _cache[pair._entity1][pair._entity2] = pair._result;
                        }
                    }
                }
            }
        }

        public bool CanBeMerged(EntityData a, EntityData b)
        {
            if (Cache.ContainsKey(a))
            {
                if (Cache[a].ContainsKey(b))
                {
                    return true;
                }
            }

            return false;
        }

        public EntityData Merge(EntityData a, EntityData b)
        {
            try
            {
                return Cache[a][b];
            }
            catch
            {
                throw new InvalidOperationException("You should use CanBeMerged method before get merge result");
            }
        }
    }

    [Serializable]
    public class MergeData : IValidateHandler
    {
        [SerializeField, ReadOnly] internal string _id;
        [SerializeField] internal EntityDataObject _entity1;
        [SerializeField] internal EntityDataObject _entity2;
        [SerializeField] internal EntityDataObject _resultEntity;

        public void OnValidate() => _id = _entity1?.name + "+" + _entity2?.name + "=" + _resultEntity?.name;
    }
}