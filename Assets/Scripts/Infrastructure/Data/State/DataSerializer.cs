using System;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using Cryptography;

namespace Data
{
    public interface IDataSerializer
    {
        public Task<string> SerializeObject(object target);
        public Task PopulateFromContent(string content, object target);
    }


    public class DataSerializer : IDataSerializer
    {
        //do not remove or change! used in defines for encrypt profile data.
        private const string ENCRYPTION_KEY = "NWl41BvbHNxDSU7TPaWhlz67btSnMkaB12g2o5xd";

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
#if UNITY_EDITOR
            Formatting = Formatting.Indented,
#else
            Formatting = Formatting.None,
#endif
        };

        public async Task PopulateFromContent(string content, object target)
        {
#if !UNITY_EDITOR
            content = content.Decrypt(ENCRYPTION_KEY);
#endif
            JsonConvert.PopulateObject(content, target, _settings);
        }

        public async Task<string> SerializeObject(object target)
        {
            string serializedString = JsonConvert.SerializeObject(target, _settings);

#if !UNITY_EDITOR
                serializedString = serializedString.Encrypt(ENCRYPTION_KEY);
#endif

            return serializedString;
        }
    }
}