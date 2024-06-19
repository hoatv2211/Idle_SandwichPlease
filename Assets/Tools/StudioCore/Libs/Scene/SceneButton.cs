using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

namespace NekoMart.Scene
{
    [RequireComponent(typeof(Button))]
    public class SceneButton : MonoBehaviour
    {
        public ScenePathList.ShortCuts sortcuts;
        public string sceneName;

        public string data;

        private void OnValidate()
        {
            if (sortcuts != ScenePathList.ShortCuts.None)
            {
                sceneName = ScenePathList.GetScenePath(sortcuts);
            }
        }

        private void Start()
        {
            GetComponent<Button>().OnClickAsObservable()
                                  .Where(_ => !SceneTransfer.SceneIsTransferring)
                                  .Subscribe(_ =>
            {
                if (sortcuts != ScenePathList.ShortCuts.None && sortcuts != ScenePathList.ShortCuts.Back && !string.IsNullOrEmpty(sceneName))
                {
                    SceneTransfer.LoadSceneAdditive(sceneName, null, !string.IsNullOrEmpty(data) ? data : null);
                }
                else
                {
                    SceneTransfer.Back();
                }
            });
        }
    }
}