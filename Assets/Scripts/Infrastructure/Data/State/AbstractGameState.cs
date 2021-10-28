using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Data
{
    public class LoadResult
    {
        public IEnumerable<object> DatalayerObjects;
    }

    [Serializable]
    public abstract class AbstractGameState
    {
        private bool _isLoadingStarted;

        [JsonIgnore] private string SavePath => Application.persistentDataPath + $"/{Name}.us";
        [JsonIgnore] protected virtual string Name => GetType().Name;
        [JsonIgnore] protected virtual bool AutosaveEnabled { get; } = false;

        private IDataSerializer _serializer = new DataSerializer();
        private IDataStorage _storage;

        public AbstractGameState Initialize()
        {
#if UNITY_ANDROID
            //var platform = PlayGamesPlatform.Instance;
            //if (platform.IsAuthenticated() && Application.internetReachability != NetworkReachability.NotReachable)
            //{
            //    _storage = new AndroidDataStorage(SavePath);
            //}
            //else
            {
                _storage = new LocalDataStorage(SavePath);
            }
#elif UNITY_IOS
            throw new NotImplementedException();
#endif

            return this;
        }

        public async Task Save()
        {
            var serializedContent = await _serializer.SerializeObject(this);

             _ = _storage.Save(serializedContent);

            OnSaved();
        }

        public async Task<LoadResult> Load()
        {
            if (_isLoadingStarted)
                throw new InvalidOperationException($"State {GetType()} was already loaded!");

            _isLoadingStarted = true;

            var content = await _storage.Load();

            if (string.IsNullOrEmpty(content))
            {
                SetDefaults();
            }
            else
            {
                await _serializer.PopulateFromContent(content, this);
            }

            OnLoaded();

            if (AutosaveEnabled)
                _ = Autosave();

            return new LoadResult
            {
                DatalayerObjects = SearchObjectsForBind(GetType(), this, GetType().Assembly, new HashSet<object>()),
            };
        }

        private IEnumerable<object> SearchObjectsForBind(Type type, object context, Assembly assembly, HashSet<object> result)
        {
            if (type.Assembly != assembly)
                return result;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var newContext = field.GetValue(context);

                if (newContext == null)
                {
                    return result;
                }

                result.Add(newContext);
                SearchObjectsForBind(field.FieldType, newContext, assembly, result);
            }

            return result;
        }

        protected virtual void SetDefaults() { }
        protected virtual void OnSaved() { }
        protected virtual void OnLoaded() { }

        private async Task Autosave()
        {
            while (true)
            {
                await Task.Delay(5000);
                await Save();
            }
        }
    }
}