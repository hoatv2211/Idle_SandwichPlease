using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NekoMart.Scene;
using System;
using UniRx;
using UniRx.Triggers;
using System.Collections.Generic;
using System.Linq;

public class HeaderManager : SceneRootController
{
    public static HeaderManager Instance { get; private set; }

    [SerializeField]
    private CanvasGroup baseWindow;

    [SerializeField] private HeaderUI headerUI;
    [SerializeField] private Camera cam = null;
    [SerializeField] private GameObject eventObject;

    public Camera MainCamera
    {
        get
        {
            return cam;
        }
    }
    private bool isShortHeader;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = null;
            Debug.LogWarning("HeaderManager ...");
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        NekoMart.Scene.SceneTransfer.OnLoadComplete += () =>
        {
            if (NekoMart.Scene.SceneTransfer.History != null
                && NekoMart.Scene.SceneTransfer.History.Count > 0)
            {
                var currentScenePath = NekoMart.Scene.SceneTransfer.History.Peek().scenePath;
                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    var shortHeader = ScenePathList.Home == currentScenePath;
                    if (shortHeader != this.isShortHeader)
                    {
                        ChangeHeaderUI(shortHeader);
                    }
                }
            }
        };

    }

    public void SetActive(bool active)
    {
        if (active)
        {
            //headerUI.Initialize();

            if (NekoMart.Scene.SceneTransfer.History != null
                    && NekoMart.Scene.SceneTransfer.History.Count > 0)
            {
                var currentScenePath = NekoMart.Scene.SceneTransfer.LatestScenePath;
                //                Debug.Log(currentScenePath);
                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    var shortHeader = ScenePathList.Home == currentScenePath;
                    if (shortHeader != this.isShortHeader)
                    {
                        ChangeHeaderUI(shortHeader);
                    }
                }
                else
                {
                    if (!this.isShortHeader) ChangeHeaderUI(true);
                }
            }
            else
            {
                if (!this.isShortHeader) ChangeHeaderUI(true);
            }
            DoAnimation(true);
        }

        MainCamera.gameObject.SetActive(active);
        eventObject.SetActive(active);
        baseWindow.gameObject.SetActive(active);

    }

    public void DoAnimation(bool show)
    {

    }

    private void ChangeHeaderUI(bool isShortHeader)
    {
        this.isShortHeader = isShortHeader;
    }

    internal override void OnLoaded(object data = null)
    {
        base.OnLoaded(data);

        SetActive(true);
    }

    public void Destroy()
    {
        OnDestroy();
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        Instance = null;
    }

}