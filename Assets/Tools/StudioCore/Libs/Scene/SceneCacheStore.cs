using UnityEngine;
using System;
using System.Collections.Generic;

namespace NekoMart.Scene
{
    /// <summary>
    /// Cacheを保存、復元するためのインターフェイス
    /// </summary>
    public interface ICacheable
    {
        /// <summary>
        /// 保存対象のCache
        /// </summary>
        /// <value>The state.</value>
        CacheObject Cache
        {
            get;
        }

        /// <summary>
        /// 復元したCacheを受け取る
        /// </summary>
        /// <param name="cache">Cache.</param>
        void OnRestore(CacheObject cache);
    }

    /// <summary>
    /// Cacheデータ
    /// </summary>
    public struct CacheObject
    {
        public DateTime savedTime;
        public object value;
        public string key;

        public override string ToString()
        {
            return string.Format("[SceneCache]{0}, {1}, {2}", key, savedTime, value);
        }
    }

    /// <summary>
    /// Scene cache store.
    /// 保存、取得、削除を行う
    /// </summary>
    public static class SceneCacheStore
    {
        /// <summary>
        /// キャッシュリスト
        /// </summary>
        private static Dictionary<string, CacheObject> cacheList = new Dictionary<string, CacheObject>();

        public static void Save(CacheObject cache)
        {
            cache.savedTime = DateTime.Now;

            if (HasCache(cache))
            {
                cacheList[cache.key] = cache;
            }
            else
            {
                cacheList.Add(cache.key, cache);
            }
        }

        public static void Delete(CacheObject cache)
        {
            if (!HasCache(cache))
            {
                return;
            }

            cacheList.Remove(cache.key);
        }

        public static void DeleteAll()
        {
            cacheList.Clear();
        }

        public static bool HasCache(CacheObject cache)
        {
            return HasCache(cache.key);
        }

        public static bool HasCache(string scenePath)
        {
            return cacheList.ContainsKey(scenePath);
        }

        public static CacheObject Get(string scenePath)
        {
            return cacheList[scenePath];
        }

        public static void Restore(ICacheable target, string scenePath)
        {
            target.OnRestore(cacheList[scenePath]);
        }
    }
}