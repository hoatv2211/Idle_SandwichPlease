using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using NekoMart.Scene;
using UniRx;

public class GlobalNavSceneRootController : SceneRootController
{
    protected Action onLoadedHeader;
    internal override void OnLoaded(object data)
    {     
        SetEventSystem(false);
        if (HeaderManager.Instance == null)
        {
            SceneTransfer.LoadSceneAdditiveRx(HeaderManager.Instance, ScenePathList.Header, null, false, true)
                .Do((headerscene) =>
                {
                    if (headerscene.HasValue)
                    {
                        SceneManager.SetActiveScene(headerscene.Value);
                        base.OnLoaded(data);
                        HeaderManager.Instance.SetWorldCamera();
                        if (onLoadedHeader != null)
                        {
                            onLoadedHeader();
                            onLoadedHeader = null;
                        }
                    }
                    else
                    {
                        base.OnLoaded(data);
                    }
                })
                //.Zip(SceneTransfer.LoadSceneAdditiveRx(FooterManager.Instance, ScenePathList.Footer, null, false, true), (arg1, arg2) => 0)
            .Subscribe();
        }
        else
        {
            HeaderManager.Instance.SetActive(true);
            base.OnLoaded(data);
        }
        
    }
}
