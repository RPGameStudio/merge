using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("rpgame.data.generators")]

namespace Data
{
    public interface IValidateHandler
    {
        void OnValidate();
    }

    [Serializable]
    public abstract class ScriptableObjectValidate : ScriptableObject, IValidateHandler
    {
        public async void OnValidate()
        {
            try
            {
                var task = Validate();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        protected virtual async Task Validate()
        {
            var fields = GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

            foreach (var item in fields)
            {
                if (typeof(IValidateHandler).IsAssignableFrom(item.FieldType))
                {
                    OnValidateItem(item.GetValue(this) as IValidateHandler);
                }
                else if (typeof(IEnumerable<IValidateHandler>).IsAssignableFrom(item.FieldType))
                {
                    OnValidateIEnumerable(item.GetValue(this) as IEnumerable<IValidateHandler>);
                }
            }
        }

        protected virtual void OnValidateIEnumerable(IEnumerable<IValidateHandler> query)
        {
            foreach (var val in query)
            {
                OnValidateItem(val);
            }
        }

        protected virtual void OnValidateItem(IValidateHandler item) => item?.OnValidate();
    }

    [Serializable]
    public abstract class ScriptableObject<T> : ScriptableObjectValidate
    {
        [SerializeField] internal T _data;

        [JsonIgnore] public T Data => _data;
    }
}
