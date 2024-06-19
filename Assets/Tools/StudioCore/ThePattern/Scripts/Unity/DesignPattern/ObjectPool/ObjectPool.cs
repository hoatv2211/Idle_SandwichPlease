using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using ThePattern.Attributes;

namespace ThePattern.Unity
{
    [Resource(isDontDestroyOnLoad: false)]
    public sealed class ObjectPool : Singleton<ObjectPool>
    {
        // public enum EStartupPoolMode
        // {
        //     Awake,
        //     Start,
        //     CallManually
        // }
        [System.Serializable]
        public class StartupPool
        {
            public int size;
            public GameObject prefab;
        }
        private static List<GameObject> _tempList = new List<GameObject>();
        private readonly Dictionary<GameObject, List<GameObject>> _pooledObjects = new Dictionary<GameObject, List<GameObject>>();
        private readonly Dictionary<GameObject, GameObject> _spawnedObjects = new Dictionary<GameObject, GameObject>();
        // private EStartupPoolMode _startupPoolMode;
        private StartupPool[] _startupPools;
        private bool _startupPoolCreated;

        // public EStartupPoolMode StartupMode => _startupPoolMode;

        // private void Awake()
        // {
        //     if (this._startupPoolMode != EStartupPoolMode.Awake)
        //     {
        //         return;
        //     }
        //     CreateStartupPools();
        // }

        // private void Start()
        // {
        //     if (this._startupPoolMode != EStartupPoolMode.Start)
        //     {
        //         return;
        //     }
        //     CreateStartupPools();
        // }

        protected override void Init()
        {
            if (Instance._startupPoolCreated)
            {
                return;
            }
            _startupPoolCreated = true;
            StartupPool[] startupPools = _startupPools;
            if (startupPools != null && (uint)startupPools.Length > 0U)
            {
                for (int i = 0; i < startupPools.Length; ++i)
                {
                    CreatePool(startupPools[i].prefab, startupPools[i].size);
                }
            }
        }

        // private static void CreateStartupPools()
        // {
            
        // }

        internal static void SetDefaultStartUpPools(StartupPool[] pools)
        {
            Instance._startupPools = pools;
        }
        public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
        {
            CreatePool(prefab.gameObject, initialPoolSize);
        }
        public static void CreatePool(GameObject prefab, int initialPoolSize)
        {
            if (prefab == null || Instance._pooledObjects.ContainsKey(prefab))
            {
                return;
            }
            List<GameObject> gameObjectList = new List<GameObject>();
            Instance._pooledObjects.Add(prefab, gameObjectList);
            if (initialPoolSize > 0)
            {
                bool activeSelf = prefab.activeSelf;
                prefab.SetActive(false);
                Transform transform = Instance.transform;
                while (gameObjectList.Count < initialPoolSize)
                {
                    GameObject gameObject = Instantiate(prefab);
                    gameObject.transform.SetParent(transform);
                    gameObjectList.Add(gameObject);
                }
                prefab.SetActive(activeSelf);
            }
        }

        public static GameObject Spawn(string resourcePath, Transform parent, Vector3 position, Quaternion rotation)
        {
            return (Resources.Load(resourcePath) as GameObject).Spawn(parent, position, rotation);
        }
        public static GameObject Spawn(string resourcePath, Transform parent, Vector3 position)
        {
            return Spawn(resourcePath, parent, position, Quaternion.identity);
        }
        public static GameObject Spawn(string resourcePath, Vector3 position)
        {
            return Spawn(resourcePath, null, position, Quaternion.identity);
        }
        public static GameObject Spawn(string resourcePath, Transform parent)
        {
            return Spawn(resourcePath, parent, Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(string resourcePath)
        {
            return Spawn(resourcePath, null, Vector3.zero, Quaternion.identity);
        }

        public static T Spawn<T>(string resourcePath, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(resourcePath, parent, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(string resourcePath, Transform parent, Vector3 position) where T : Component
        {
            return Spawn<T>(resourcePath, parent, position, Quaternion.identity);
        }
        public static T Spawn<T>(string resourcePath, Transform parent) where T : Component
        {
            return Spawn<T>(resourcePath, parent, Vector3.zero, Quaternion.identity);
        }
        public static T Spawn<T>(string resourcePath, Vector3 position) where T : Component
        {
            return Spawn<T>(resourcePath, null, position, Quaternion.identity);
        }
        public static T Spawn<T>(string resourcePath) where T : Component
        {
            return Spawn<T>(resourcePath, null, Vector3.zero, Quaternion.identity);
        }
        public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent) where T : Component
        {
            return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab) where T : Component
        {
            return Spawn(prefab.gameObject, (Transform)null, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }

        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            List<GameObject> gameObjectList;
            if (Instance._pooledObjects.TryGetValue(prefab, out gameObjectList))
            {
                GameObject key1 = null;
                if (gameObjectList.Count > 0)
                {
                    while (key1 == null && gameObjectList.Count > 0)
                    {
                        key1 = gameObjectList[0];
                        gameObjectList.RemoveAt(0);
                    }
                    if (key1 != null)
                    {
                        Transform transform1 = key1.transform;
                        transform1.SetParent(parent);
                        transform1.localPosition = position;
                        transform1.localRotation = rotation;
                        key1.SetActive(true);
                        Instance._spawnedObjects.Add(key1, prefab);
                        return key1;
                    }
                }
                GameObject key2 = Instantiate(prefab);
                Transform transform2 = key2.transform;
                transform2.SetParent(parent);
                transform2.localPosition = position;
                transform2.localRotation = rotation;
                Instance._spawnedObjects.Add(key2, prefab);
                return key2;
            }
            GameObject key3 = UnityEngine.Object.Instantiate(prefab);
            Transform component = key3.GetComponent<Transform>();
            component.SetParent(parent);
            component.localPosition = position;
            component.localRotation = rotation;
            Instance._spawnedObjects.Add(key3, prefab);
            Instance._pooledObjects.Add(prefab, new List<GameObject>());
            return key3;
        }
        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
        {
            return Spawn(prefab, parent, position, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, (Transform)null, position, rotation);
        }
        public static GameObject Spawn(GameObject prefab, Transform parent)
        {
            return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position)
        {
            return Spawn(prefab, (Transform)null, position, Quaternion.identity);
        }

        public static GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, (Transform)null, Vector3.zero, Quaternion.identity);
        }

        public static void Recycle<T>(T obj) where T : Component
        {
            Recycle(obj.gameObject);
        }
        public static void Recycle(GameObject obj)
        {
            GameObject prefab;
            if (Instance._spawnedObjects.TryGetValue(obj, out prefab))
            {
                Recycle(obj, prefab);
            }
            else
            {
                Destroy(obj);
            }
        }
        private static void Recycle(GameObject obj, GameObject prefab)
        {
            Instance._pooledObjects[prefab].Add(obj);
            Instance._spawnedObjects.Remove(obj);
            obj.transform.SetParent(Instance.transform);
            obj.SetActive(false);
        }

        public static void RecycleAll<T>(T prefab) where T : Component
        {
            RecycleAll(prefab.gameObject);
        }
        public static void RecycleAll(GameObject prefab)
        {
            using (Dictionary<GameObject, GameObject>.Enumerator enumerator = Instance._spawnedObjects.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<GameObject, GameObject> current = enumerator.Current;
                    if (current.Value == prefab)
                    {
                        _tempList.Add(current.Key);
                    }

                }
            }
            for (int index = 0; index < _tempList.Count; ++index)
            {
                Recycle(_tempList[index]);
            }
            _tempList.Clear();
        }
        public static void RecycleAll()
        {
            _tempList.AddRange((IEnumerable<GameObject>)Instance._spawnedObjects.Keys);
            for (int index = 0; index < _tempList.Count; ++index)
            {
                Recycle(_tempList[index]);
            }
            _tempList.Clear();
        }

        public static bool IsSpawned(GameObject obj)
        {
            return Instance._spawnedObjects.ContainsKey(obj);
        }

        public static int CountPooled<T>(T prefab) where T : Component
        {
            return CountPooled(((Component)(object)prefab).gameObject);
        }
        public static int CountPooled(GameObject prefab)
        {
            List<GameObject> gameObjectList;
            return Instance._pooledObjects.TryGetValue(prefab, out gameObjectList) ? gameObjectList.Count : 0;
        }

        public static int CountSpawned<T>(T prefab) where T : Component
        {
            return CountSpawned(((Component)(object)prefab).gameObject);
        }
        public static int CountSpawned(GameObject prefab)
        {
            int num = 0;
            using (Dictionary<GameObject, GameObject>.ValueCollection.Enumerator enumerator = Instance._spawnedObjects.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    GameObject current = enumerator.Current;
                    if (UnityEngine.Object.ReferenceEquals((UnityEngine.Object)prefab, (UnityEngine.Object)current))
                    {
                        ++num;
                    }
                }
            }
            return num;
        }

        public static int CountAllPooled()
        {
            int num = 0;
            using (Dictionary<GameObject, List<GameObject>>.ValueCollection.Enumerator enumerator = Instance._pooledObjects.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    List<GameObject> current = enumerator.Current;
                    num += current.Count;
                }
            }
            return num;
        }

        public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
        {
            if (list == null)
            {
                list = new List<GameObject>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            List<GameObject> gameObjectList;
            if (Instance._pooledObjects.TryGetValue(prefab, out gameObjectList))
            {
                list.AddRange((IEnumerable<GameObject>)gameObjectList);
            }
            return list;
        }
        public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
        {
            if (list == null)
            {
                list = new List<T>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            List<GameObject> gameObjectList;
            if (Instance._pooledObjects.TryGetValue(((Component)(object)prefab).gameObject, out gameObjectList))
            {
                for (int i = 0; i < gameObjectList.Count; ++i)
                {
                    list.Add(gameObjectList[i].GetComponent<T>());
                }

            }
            return list;
        }

        public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
        {
            if (list == null)
            {
                list = new List<GameObject>();
            }
            if (!appendList)
            {
                list.Clear();
            }
            using (Dictionary<GameObject, GameObject>.Enumerator enumerator = Instance._spawnedObjects.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<GameObject, GameObject> current = enumerator.Current;
                    if (UnityEngine.Object.ReferenceEquals((UnityEngine.Object)current.Value, (UnityEngine.Object)prefab))
                    {
                        list.Add(current.Key);
                    }
                }
            }
            return list;
        }
        public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
        {
            if (list == null)
            {
                list = new List<T>();
            }
            if (!appendList)
            {
                list.Clear();
            }

            GameObject gameObject = ((Component)(object)prefab).gameObject;
            using (Dictionary<GameObject, GameObject>.Enumerator enumerator = Instance._spawnedObjects.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<GameObject, GameObject> current = enumerator.Current;
                    if (UnityEngine.Object.ReferenceEquals((UnityEngine.Object)current.Value, (UnityEngine.Object)prefab))
                    {
                        list.Add(current.Key.GetComponent<T>());
                    }
                }
            }
            return list;
        }

        public static void DestroyPooled(GameObject prefab)
        {
            List<GameObject> gameObjectList;
            if (!Instance._pooledObjects.TryGetValue(prefab, out gameObjectList))
            {
                return;
            }
            for (int i = 0; i < gameObjectList.Count; ++i)
            {
                UnityEngine.Object.Destroy((UnityEngine.Object)gameObjectList[i]);
            }
            gameObjectList.Clear();
        }
        public static void DestroyPooled<T>(T prefab) where T : Component
        {
            DestroyPooled(((Component)(object)prefab).gameObject);
        }
        public static void DestroyAll(GameObject prefab)
        {
            RecycleAll(prefab);
            DestroyPooled(prefab);
        }
        public static void DestroyAll<T>(T prefab) where T : Component
        {
            DestroyAll(((Component)(object)prefab).gameObject);
        }
    }
}

