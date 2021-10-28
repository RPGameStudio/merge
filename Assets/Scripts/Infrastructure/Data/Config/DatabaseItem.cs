using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: InternalsVisibleTo("rpgames.database.editor")]

namespace Data
{
    public interface IDatabaseItem<out T> where T: IDatabaseEntryReadonly
    {
        T Data { get; }
    }

    public class DatabaseItem : ScriptableObjectValidate
    {
        [SerializeField, HideInInspector] protected string _guid;

#if UNITY_EDITOR
        protected override async Task Validate()
        {
            if (!Application.isPlaying)
            {
                long _;
                string guid;
                string defaultGuid = new GUID().ToString();

                do
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out guid, out _);
                    await Task.Delay(300);
                } while (guid == defaultGuid);

                if (string.IsNullOrEmpty(_guid) || _guid != guid)
                {
                    Debug.LogWarning($"recreate guids for {name}: from {_guid} to {guid}");
                    _guid = guid;
                }
            }

            await base.Validate();

            EditorUtility.SetDirty(this);
        }
#endif
    }

    public abstract class DatabaseItem<T> : DatabaseItem, IDatabaseItem<T> where T: IDatabaseEntry
    {
        [SerializeField] private T _data;

        [JsonIgnore] public T Data => _data;

        protected override async Task Validate()
        {
            await base.Validate();
            _data.Guid = _guid;
        }
    }
}
