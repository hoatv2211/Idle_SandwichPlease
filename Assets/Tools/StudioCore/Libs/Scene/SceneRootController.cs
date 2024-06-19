using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
namespace NekoMart.Scene
{
    public enum LoadingType
    {
        NOW_LOADING = 1,
        LOADING_SLIME = 2,
        LOADING_WITH_BG = 3,
        DASHBOARD_LOADING = 4
    }
    [DisallowMultipleComponent]
    public class SceneRootController : MonoBehaviour
    {
        [SerializeField]
        private bool autoDisableCamera = true;

        [SerializeField]
        private bool autoDisableEventSystem = true;
        [SerializeField]
        private Canvas rootCanvas = null;
        [SerializeField]
        private Canvas[] additionalCanvas = null;

        private const string HideObjectName = "_HideObject_";

        private GameObject simpleLoading = null;

        protected SceneTransitionRoot.TRANSITION_TYPE transIn = SceneTransitionRoot.TRANSITION_TYPE.NONE;
        public SceneTransitionRoot.TRANSITION_TYPE TransIn
        {
            get
            {
                return transIn;
            }
            set
            {
                transIn = value;
            }
        }

        public Canvas RootCanvas
        {
            get
            {
                return rootCanvas;
            }

            private set
            {
            }
        }

        public bool autoUnloadScene = true;

        internal virtual void OnLoaded(object data = null)
        {

            foreach (var i in gameObject.scene.GetRootGameObjects())
            {
                var cam = i.GetComponent<Camera>();
                var evSys = i.GetComponent<EventSystem>();
                if (cam != null && autoDisableCamera)
                {
                    cam.gameObject.SetActive(false);
                }

                if (evSys != null && autoDisableEventSystem)
                {
                    evSys.enabled = false;
                    evSys.gameObject.SetActive(false);
                }
            }

            SetWorldCamera();
            if (rootCanvas != null)
            {
                var trans = rootCanvas.GetComponentInChildren<SceneTransitionRoot>();
                if (trans != null)
                {
                    trans.SceneIn(transIn);
                }

#if DEV
                var child = rootCanvas.transform.Find(HideObjectName);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }
#endif
            }




        }

        internal virtual void StartTutorial(TutorialSendData data = null)
        {
        }

        public void SetEventSystem(bool enable)
        {
            foreach (var i in gameObject.scene.GetRootGameObjects())
            {
                var evSys = i.GetComponent<EventSystem>();

                if (evSys != null && autoDisableEventSystem)
                {
                    evSys.enabled = enable;
                    evSys.gameObject.SetActive(enable);
                }
            }
        }

        public void SetWorldCamera()
        {
            Camera rootCamera = null;
            UnityEngine.SceneManagement.Scene rootScene = SceneManager.GetActiveScene();
            foreach (var i in rootScene.GetRootGameObjects())
            {
                var cam = i.GetComponent<Camera>();
                if (cam != null)
                {
                    rootCamera = cam;
                    break;
                }
            }
            
            if (HeaderManager.Instance != null && HeaderManager.Instance.MainCamera.gameObject.activeSelf)
            {
                rootCamera = HeaderManager.Instance.MainCamera;
            }

            if (rootCanvas != null && rootCamera != null)
            {
                rootCanvas.worldCamera = rootCamera;
            }

            if (additionalCanvas != null && rootCamera != null)
            {
                foreach (var i in additionalCanvas)
                {
                    i.worldCamera = rootCamera;
                }
            }
        }

        internal virtual void OnUnload(SceneTransitionRoot.TRANSITION_TYPE transType = SceneTransitionRoot.TRANSITION_TYPE.NONE)
        {
            SceneTransitionRoot trans = null;
            if (rootCanvas != null)
            {
                trans = rootCanvas.GetComponentInChildren<SceneTransitionRoot>();
            }
            if (trans != null)
            {
                bool registered = trans.SceneOut(
                        transType,
                        () => SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex));

                if (registered == false)
                {
                    SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
                }
            }
            else
            {
                try
                {
                    SceneManager.UnloadSceneAsync(gameObject.scene.buildIndex);
                }
                catch
                {
                }
            }
        }

        public void ShowLoading()
        {
            ShowLoading(LoadingType.NOW_LOADING);
        }

        public void ShowLoading(LoadingType type)
        {
           
            var loadingPrefab = Resources.Load($"Effects/Prefabs/Loading{(int)type}");

            HideLoading();

            var loading = (GameObject)Instantiate(loadingPrefab);
            loading.transform.localScale = Vector3.one;
            loading.transform.position = Vector3.zero;
            loading.transform.SetParent(GetRootCanvas(false).transform, false);
            loading.transform.SetAsLastSibling();
            simpleLoading = loading;
            CheckTimeOut(60);
        }

        public void HideLoading()
        {
            if (simpleLoading != null)
            {
                Destroy(simpleLoading);
                simpleLoading = null;
            }
        }

        public Canvas GetRootCanvas(bool mainCanvasIsPriority = true)
        {
            if (mainCanvasIsPriority && RootCanvas != null){
                return RootCanvas;
            }
            
            List<Canvas> canvas = null;
          
            if (canvas == null || canvas.Count == 0)
            {
                canvas = new List<Canvas>(FindObjectsOfType<Canvas>());//canvas.FindAll(o => o.renderMode == RenderMode.ScreenSpaceOverlay)
            }
            canvas = canvas?.FindAll(o=>o.enabled);
            canvas?.Sort((a, b) => { return b.sortingOrder.CompareTo(a.sortingOrder); });
            return canvas[0];
        }

        protected async void CheckTimeOut(float time = 30)
        {
            await Task.Delay(System.TimeSpan.FromSeconds(time));
            if (simpleLoading != null)
            {
                HideLoading();
                /*
                new ModalObject(ModalCreatorPrefabsName.PopupOk)
                    .OnInit((components, onSucc, onFail) =>
                    {
                        components["content"].GetComponent<Text>().text = "Check your internet connection and try again";// res.message;
                    }).ShowPopup();
                */
            }
        }
    }

    internal class TutorialSendData
    {
    }

}