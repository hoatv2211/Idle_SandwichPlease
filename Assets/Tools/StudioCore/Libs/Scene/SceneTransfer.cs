using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoMart.Utils;
using UniRx;

namespace NekoMart.Scene
{
    public struct History
    {
        public LoadSceneMode mode;
        public string scenePath;

        public History(string scenePath, LoadSceneMode mode)
        {
            this.mode = mode;
            this.scenePath = scenePath;
        }

        public override string ToString()
        {
            return string.Format("[History]{0},{1}", scenePath, mode);
        }
    }

    /// <summary>
    /// Scene transfer.
    /// </summary>
    public class SceneTransfer : SingletonMonoBehaviour<SceneTransfer>
    {
     
        private static Dictionary<Guid, IEnumerator> proccessList = new Dictionary<Guid, IEnumerator>();
        public static bool SceneIsTransferring
        {
            get
            {
                return proccessList.Count > 0;
            }
        }

        // Debug確認用
        public string[] his;
        public int numProccess;

     
        private static Stack<History> history = new Stack<History>();
        public static Stack<History> History
        {
            get
            {
                return history;
            }
            private set
            {
                history = value;
            }
        }

        public static string LatestScenePath
        {
            get
            {
                return History.Count > 0 ? History.Peek().scenePath : string.Empty;
            }
        }


        public static event Action OnLoadStart;

    
        public static event Action OnLoadComplete;

        private static bool doingTransition;

        public static bool DoingTransition
        {
            get
            {
                return doingTransition;
            }

            private set
            {
                doingTransition = value;
            }
        }

        private void Start(){
            MakePersitence();
        }
        public static void JumpScene(
            string scenePath,
            Action<UnityEngine.SceneManagement.Scene> callback = null,
            object data = null)
        {
            LoadSceneSingle(scenePath, callback, data);
        }

        public static Guid LoadSceneSingle(
            string scenePath,
            Action<UnityEngine.SceneManagement.Scene> callback = null,
            object data = null,
            bool addHistory = false,
            bool isAppendScene = false)
        {
            Guid id = Guid.NewGuid();
            IEnumerator ie = LoadScene(scenePath, id, callback, data, LoadSceneMode.Single, addHistory, isAppendScene);
            proccessList.Add(id, ie);
            Instance.StartCoroutine(ie);
            return id;
        }

        public static Guid LoadSceneAdditive(
            string scenePath,
            Action<UnityEngine.SceneManagement.Scene> callback = null,
            object data = null,
            bool addHistory = false,
            bool isAppendScene = false,
            bool startOnLoad = true)
        {
            Guid id = Guid.NewGuid();
            IEnumerator ie = LoadScene(scenePath, id, callback, data, LoadSceneMode.Additive, addHistory, isAppendScene);
            proccessList.Add(id, ie);
            if (startOnLoad)
            {
                Instance.StartCoroutine(ie);
            }
            return id;
        }

        public static void StartSceneLoad(Guid id)
        {
            Instance.StartCoroutine(proccessList[id]);
        }

        /// <param name="scenePath">Scene path.</param>
        /// <param name="data">Data.</param>
        /// <param name="addHistory">If set to <c>true</c> add history.</param>
        /// <param name="isAppendScene">If set to <c>true</c> is append scene.</param>
        public static IObservable<UnityEngine.SceneManagement.Scene?> LoadSceneAdditiveRx(
            object instanceToCheck,
            string scenePath,
            object data = null,
            bool addHistory = true,
            bool isAppendScene = false)
        {
            if (instanceToCheck != null)
            {
                return Observable.Return<UnityEngine.SceneManagement.Scene?>(null);
            }

            Guid id = Guid.NewGuid();
            Subject<UnityEngine.SceneManagement.Scene?> subject = new Subject<UnityEngine.SceneManagement.Scene?>();

            // Publish to Subject in callback
            Action<UnityEngine.SceneManagement.Scene> callback = (obj) =>
            {
                subject.OnNext(obj);
            };

            IEnumerator ie = LoadScene(scenePath, id, callback, data, LoadSceneMode.Additive, addHistory, isAppendScene);
            proccessList.Add(id, ie);

            Observable.FromCoroutine((arg1) => ie).Last().Subscribe(_ => subject.OnCompleted());

            return subject.AsObservable();
        }


        private static IEnumerator LoadScene(
            string scenePath,
            Guid id,
            Action<UnityEngine.SceneManagement.Scene> callback = null,
            object data = null,
            LoadSceneMode loadSceneMode = LoadSceneMode.Additive,
            bool addHistory = true,
            bool isAppendScene = false)
        {
            var trans = Transition(loadSceneMode, scenePath);
            Instance.numProccess = proccessList.Count;
            string beforeScene = LatestScenePath;
            Debug.LogFormat("{0} -> <color=blue>{1}</color> [history:{2}]", beforeScene, scenePath, addHistory);

            if (addHistory)
            {
                if (History.Count > 0 && scenePath == History.Peek().scenePath)
                {
                    proccessList.Remove(id);
                    yield break;
                }
                History.Push(new History(scenePath, loadSceneMode));

                var existedScene =  SceneManager.GetSceneByName(scenePath);

                SceneManager.UnloadSceneAsync(existedScene.buildIndex);
            }

            Instance.his = history.Select(i => i.scenePath).ToArray();

            if (OnLoadStart != null)
            {
                OnLoadStart.Invoke();
            }

            doingTransition = true;

            var esys = EventSystem.current;
            if (esys != null)
            {
                esys.enabled = false;
            }

            var unloadTargets = GetUnloadTargets();

            if (!isAppendScene)
            {
                unloadTargets.All(s =>
                {
                    TrySaveSceneCache(s);

                    if (loadSceneMode == LoadSceneMode.Single && s is SceneRootController)
                    {
                        s.OnUnload();
                    }

                    return true;
                });
            }

            yield return SceneManager.LoadSceneAsync(scenePath, loadSceneMode);
            var scene = SceneManager.GetSceneByName(scenePath);
            var srm = GetSceneRootController(scene);

            if (loadSceneMode == LoadSceneMode.Single && HeaderManager.Instance != null)
            {
                HeaderManager.Instance.SetActive(false);
            }

            if (srm != null)
            {
                if (srm is ICacheable)
                {
                    if (SceneCacheStore.HasCache(scenePath))
                    {
                        SceneCacheStore.Restore((ICacheable)srm, scenePath);
                    }
                }
                srm.TransIn = trans.Item1;
                srm.OnLoaded(data);
            }

            if (esys != null)
            {
                esys.enabled = true;
            }

            if (callback != null)
            {
                callback(scene);
            }

            if (!isAppendScene && loadSceneMode == LoadSceneMode.Additive)
            {
                unloadTargets.All(s =>
                {
                    s.OnUnload(trans.Item2);
                    return true;
                });
            }

            proccessList.Remove(id);

            if (OnLoadComplete != null)
            {
                OnLoadComplete.Invoke();
            }

            doingTransition = false;
            Instance.numProccess = proccessList.Count;
            yield return scene;
        }

        /// <returns>The transition.</returns>
        /// <param name="loadSceneMode">Load scene mode.</param>
        /// <param name="scenePath">Scene path.</param>
        private static Tuple<SceneTransitionRoot.TRANSITION_TYPE, SceneTransitionRoot.TRANSITION_TYPE> Transition(
            LoadSceneMode loadSceneMode,
            string scenePath)
        {
            string beforeScene = LatestScenePath;

            SceneTransitionRoot.TRANSITION_TYPE transIn = SceneTransitionRoot.TRANSITION_TYPE.NONE;
            SceneTransitionRoot.TRANSITION_TYPE transOut = SceneTransitionRoot.TRANSITION_TYPE.NONE;
            if (!string.IsNullOrEmpty(beforeScene) && loadSceneMode == LoadSceneMode.Additive)
            {
                beforeScene = beforeScene.Substring(beforeScene.LastIndexOf("/") + 1);
                string afterScene = scenePath.Substring(scenePath.LastIndexOf("/") + 1);
                int beforeIndex = -1;
                int afterIndex = -1;
                if (SceneBuildIndex.SceneIndex.TryGetValue(beforeScene, out beforeIndex)
                    && SceneBuildIndex.SceneIndex.TryGetValue(afterScene, out afterIndex))
                {
                    //                    Debug.LogFormat("{0} to {1}", beforeIndex, afterIndex);
                    if (beforeIndex < afterIndex)
                    {
                        transIn = SceneTransitionRoot.TRANSITION_TYPE.RIGHT_TO_CENTER;
                        transOut = SceneTransitionRoot.TRANSITION_TYPE.CENTER_TO_LEFT;
                    }
                    else
                    {
                        transIn = SceneTransitionRoot.TRANSITION_TYPE.LEFT_TO_CENTER;
                        transOut = SceneTransitionRoot.TRANSITION_TYPE.CENTER_TO_RIGHT;
                    }
                }
            }

            return new Tuple<SceneTransitionRoot.TRANSITION_TYPE, SceneTransitionRoot.TRANSITION_TYPE>(transIn, transOut);
        }

        /// <param name="scene">Scene.</param>
        public static void UnloadScene(UnityEngine.SceneManagement.Scene scene)
        {
            var rootCtl = GetSceneRootController(scene);
            if (rootCtl != null && rootCtl.autoUnloadScene)
            {
                TrySaveSceneCache(rootCtl);
                rootCtl.OnUnload();
            }
        }

        private static SceneRootController[] GetUnloadTargets()
        {
            int len = SceneManager.sceneCount;
            List<SceneRootController> deleteList = new List<SceneRootController>();
            for (var i = 0; i < len; i++)
            {
                var scene = SceneManager.GetSceneAt(i);

                if (!scene.isLoaded)
                {
                    continue;
                }

                var rootCtl = GetSceneRootController(scene);
                if (rootCtl != null && !rootCtl.autoUnloadScene)
                {
                    continue;
                }
                else if (rootCtl != null)
                {
                    deleteList.Add(rootCtl);
                }
            }

            return deleteList.ToArray();
        }

      
        /// <param name="scene">Scene.</param>
        private static void TrySaveSceneCache(SceneRootController scene)
        {
            if (!(scene is ICacheable))
            {
                return;
            }

            var cache = ((ICacheable)scene).Cache;

            if (string.IsNullOrEmpty(cache.key))
            {
                cache.key = scene.gameObject.scene.name;
            }

            //Debug.Log("save" + cache.ToString());
            SceneCacheStore.Save(cache);
        }

        public static void Kill(Guid id)
        {
            Instance.StopCoroutine(proccessList[id]);
            proccessList.Remove(id);
        }

        public static void KillAll()
        {
            foreach (var i in proccessList)
            {
                Instance.StopCoroutine(i.Value);
            }

            proccessList.Clear();
        }

        /// <returns>The scene root controller.</returns>
        /// <param name="scene">Scene.</param>
        public static SceneRootController GetSceneRootController(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (var i in scene.GetRootGameObjects())
            {
                var so = i.GetComponent<SceneRootController>();
                if (so != null)
                {
                    return so;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the scene root controller.
        /// </summary>
        /// <returns>The scene root controller.</returns>
        /// <param name="scene">Scene.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T GetSceneRootController<T>(UnityEngine.SceneManagement.Scene scene) where T : SceneRootController
        {
            foreach (var i in scene.GetRootGameObjects())
            {
                var so = i.GetComponent<T>();
                if (so != null)
                {
                    return so;
                }
            }

            return null;
        }

        /// <param name="callback">Callback.</param>
        /// <param name="data">Data.</param>
        public static Guid Back(
            Action<UnityEngine.SceneManagement.Scene> callback = null,
            object data = null)
        {
            if (History.Count < 2)
            {
                return Guid.Empty;
            }

            string beforeScene = History.Count > 0 ? History.Peek().scenePath : string.Empty;
            History.Pop();
            var h = History.Peek();

            Debug.LogFormat("<color=blue>Back</color>:{0} -> {1}", beforeScene, h.scenePath);

            Guid id = Guid.NewGuid();
            IEnumerator ie = LoadScene(h.scenePath, id, callback, data, h.mode, false);
            proccessList.Add(id, ie);
            Instance.StartCoroutine(ie);
            return id;
        }

        public static void ClearHistory()
        {
            if (History != null
                && History.Count > 0)
            {
                History.Clear();
            }
        }

        public static void PopLast()
        {
            History.Pop();
        }

        public static void PushHistory(string scenePath, LoadSceneMode loadSceneMode)
        {
            History.Push(new History(scenePath, loadSceneMode));
        }
    }
}