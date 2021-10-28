using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public interface IDatabaseEntryReadonly
    {
        string Guid { get; }
        string Name { get; }

        IEnumerable<IDatabaseEntryReadonly> InnerEntries { get; }
    }

    public interface IDatabaseEntry : IDatabaseEntryReadonly
    {
        new string Guid { get; set; }
        new string Name { get; set; }
    }

    [Serializable]
    public abstract class DatabaseEntry : ValidateEntry, IDatabaseEntry
    {
        public DatabaseEntry() => _guid = System.Guid.NewGuid().ToString("N");

        [SerializeField, JsonProperty("name"), Delayed] private string _name;
        [SerializeField, JsonProperty("guid"), ReadOnly] private string _guid;

        [JsonIgnore]
        public string Guid
        {
            get => _guid;
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    this.UpdateChildGuids();
                }
            }
        }
        [JsonIgnore]
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [JsonIgnore]
        IEnumerable<IDatabaseEntryReadonly> IDatabaseEntryReadonly.InnerEntries => this.GetInnerEntriesRecursive();

        public override void OnValidate()
        {
            base.OnValidate();

            if (string.IsNullOrEmpty(Guid))
            {
                _guid = System.Guid.NewGuid().ToString("N");
            }
        }

        protected override void OnValidateIEnumerable(IEnumerable<IValidateHandler> query)
        {
            base.OnValidateIEnumerable(query);

            var casted = query as IEnumerable<DatabaseEntry>;

            if (casted != null)
            {
                var repeatingGroups = casted.GroupBy(x => x.Guid).Where(x => x.Count() > 1);

                foreach (var group in repeatingGroups)
                {
                    foreach (var item in group.Skip(1))
                    {
                        item.Guid = System.Guid.NewGuid().ToString("N");
                    }
                }
            }
        }

        public override bool Equals(object obj) => obj is DatabaseEntry entry && Guid == entry.Guid;
        public override int GetHashCode() => -737073652 + EqualityComparer<string>.Default.GetHashCode(Guid);
    }
}
