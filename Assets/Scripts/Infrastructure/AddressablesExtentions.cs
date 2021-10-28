using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

public static class AddressablesExtentions
{
    private static object DEFAULT_LINK = new object();
    private static Dictionary<object, Dictionary<AssetReference, AsyncOperationHandle>> _handles = new Dictionary<object, Dictionary<AssetReference, AsyncOperationHandle>>();
    private static DiContainer _container;


    public static void Initialize(DiContainer container)
    {
        _handles.Clear();
        _container = container;
        _ = CheckForBrokenLinks();
    }

    private static async Task CheckForBrokenLinks()
    {
        do
        {
            await Task.Delay(5000);

            foreach (var dict in _handles)
            {
                if (dict.Key == null || (dict.Key != DEFAULT_LINK && dict.Key as Object == null))
                {
                    foreach (var handle in dict.Value.Values)
                    {
                        Addressables.Release(handle);
                    }

                    dict.Value.Clear();
                }
            }

            _handles = _handles.Where(item => item.Key != null && (item.Key == DEFAULT_LINK || item.Key as Object != null)).ToDictionary(x => x.Key, v => v.Value);
        } while (true);
    }

    public static async Task<Sprite> LoadFromHandle(this AssetReferenceSprite reference)
    {
        return await LoadFromHandle<Sprite>(reference, DEFAULT_LINK);
    }
    public static async Task<Sprite> LoadFromHandle(this AssetReferenceAtlasedSprite reference)
    {
        return await LoadFromHandle<Sprite>(reference, DEFAULT_LINK);
    }
    public static async Task<GameObject> LoadFromHandle(this AssetReferenceGameObject reference)
    {
        return await LoadFromHandle<GameObject>(reference, DEFAULT_LINK);
    }
    public static async Task<Sprite> LoadFromHandle(this AssetReferenceSprite reference, object link)
    {
        return await LoadFromHandle<Sprite>(reference, link);
    }
    public static async Task<Sprite> LoadFromHandle(this AssetReferenceAtlasedSprite reference, object link)
    {
        return await LoadFromHandle<Sprite>(reference, link);
    }
    public static async Task<GameObject> LoadFromHandle(this AssetReferenceGameObject reference, object link)
    {
        return await LoadFromHandle<GameObject>(reference, link);
    }
    public static async Task<T> LoadFromHandle<T>(this AssetReference reference)
    {
        return await LoadFromHandle<T>(reference, DEFAULT_LINK);
    }
    public static async Task<T> LoadFromHandle<T>(this AssetReference reference, object link)
    {
        if (link == null)
        {
            throw new ArgumentNullException(nameof(link));
        }

        if (!_handles.ContainsKey(link))
        {
            _handles[link] = new Dictionary<AssetReference, AsyncOperationHandle>();
        }

        if (!_handles[link].ContainsKey(reference))
        {
            _handles[link][reference] = Addressables.LoadAssetAsync<T>(reference);
        }

        return (T)await _handles[link][reference].Task;
    }

    public static void ReleaseHandle(this AssetReference reference)
    {
        ReleaseHandle(reference, DEFAULT_LINK);
    }

    public static void ReleaseHandle(this AssetReference reference, object link)
    {
        if (link == null)
        {
            throw new ArgumentNullException(nameof(link));
        }

        if (!_handles.ContainsKey(link))
        {
            if (Application.isPlaying)
                Debug.LogWarning($"Can't release reference because there is no such link {link}!");

            return;
        }

        var handles = _handles[link];

        if (!handles.ContainsKey(reference))
        {
            if (Application.isPlaying)
                Debug.LogWarning($"Can't release reference because reference {reference.AssetGUID} is not registered!");

            return;
        }

        Addressables.Release(handles[reference]);

        handles.Remove(reference);

        if (handles.Count <= 0)
            _handles.Remove(link);
    }


    public static void ReleaseHandle(object link = default)
    {
        if (link is null)
        {
            throw new ArgumentNullException(nameof(link));
        }

        if (!_handles.ContainsKey(link))
            return;

        var handles = _handles[link];

        foreach (var item in handles.Values)
        {
            Addressables.Release(item);
        }

        handles.Clear();
        _handles.Remove(link);
    }

    public static async Task<T> InstantiateFromHandle<T>(this AssetReference reference, Transform holder = null) where T : Object
    {
        return _container.InstantiatePrefabForComponent<T>(await reference.LoadFromHandle<GameObject>(), holder);
    }
    public static async Task<T> InstantiateFromHandle<T>(this AssetReference reference, Transform holder = null, object link = null) where T : Object
    {
        return _container.InstantiatePrefabForComponent<T>(await reference.LoadFromHandle<GameObject>(link), holder);
    }
    public static async Task<GameObject> InstantiateFromHandle(this AssetReferenceGameObject reference, Transform holder = null, object link = null)
    {
        return _container.InstantiatePrefab(await reference.LoadFromHandle<GameObject>(link), holder);
    }
}