using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using NUnit.Framework;

namespace Data
{
    public static class DataEditorExtentions
    {
        [MenuItem("Tools/Data/Prepare content %S")]
        private static void PrepareContent()
        {
            var assets = FindAssetsByType<DatabaseItem>().Select(x => (x as IDatabaseItem<IDatabaseEntry>).Data);

            var result = assets.ToDictionary(x => x.Guid, x => x as IDatabaseEntryReadonly);

            foreach (var item in assets)
            {
                var entries = item.InnerEntries;

                foreach (var entry in entries)
                {
                    result[entry.Guid] = entry;
                }
            }

            var config = FindAssetsByType<Config>().SingleOrDefault();

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<Config>();
                AssetDatabase.CreateAsset(config, "Assets/Data/config.asset");
            }

            config._content = result.Select(x => new ContentObject
            {
                _data = x.Value,
                _guid = x.Key,
            }).ToList();

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        private static List<T> FindAssetsByType<T>() where T : class
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath) as T;
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
    }
}
