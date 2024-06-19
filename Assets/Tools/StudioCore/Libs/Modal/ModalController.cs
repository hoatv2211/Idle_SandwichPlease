#define QUEUE
//#define ENABLE_BLUR
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoMart.Utils;
using UniRx;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ModalController : SingletonMonoBehaviour<ModalController>
{
    private class EventBlocker : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
        }
    }

    private Color blockingCanvasColor = new Color(0, 0, 0, 0.8f);
    private const float BlurAmount = 1.5f;
    private Vector2 referenceResolution = new Vector2(1080, 1920);
    private Canvas canvas;
    private RectTransform blockingCanvas = null;
    private Material blurMat = null;
    private Dictionary<string, RectTransform> prefabCache = new Dictionary<string, RectTransform>();
    private List<RectTransform> child = new List<RectTransform>();
#if QUEUE
    private Queue modalQueue = new Queue();
#endif
    private Stack modalStack = new Stack();
    public enum SortingLayerModeType
    {
        Normal,
        Battle,
    }
    public SortingLayerModeType sortingLayerMode = SortingLayerModeType.Normal;
    public SortingLayerModeType SortingLayerMode { set { sortingLayerMode = value; } }

    // Use this for initialization
    protected override void Awake()
    {
        SortingLayerMode = SortingLayerModeType.Normal;

        canvas = transform.GetOrAddComponent<GraphicRaycaster>()
            .GetOrAddComponent<CanvasScaler>()
            .GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.sortingOrder = 499;
        canvas.sortingLayerName = this.SortingLayerName();

        CanvasScaler scale = transform.GetComponent<CanvasScaler>();

        scale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scale.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        scale.referenceResolution = referenceResolution;
        scale.referencePixelsPerUnit = 100;

        canvas.enabled = false;
    }

    private string SortingLayerName()
    {
        switch (sortingLayerMode)
        {
            case SortingLayerModeType.Battle:
                return "UIFront";
            default:
                return "UIMiddle";
        }
    }

    //[System.Obsolete("deprecated")]
    private void SetupNormalMode(bool mode)
    {
        if (mode)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    private void SetupParticleMode()
    {
        var cnt = SceneManager.sceneCount;
        for (int i = 0; i < cnt; ++i)
        {
            var sc = SceneManager.GetSceneAt(i);

            if (!sc.isLoaded || !sc.IsValid())
            {
                continue;
            }

            if (sc.GetRootGameObjects().FirstOrDefault((arg) =>
             {
                 var cam = arg.GetComponent<Camera>();
                 if (cam != null && cam.gameObject.activeSelf)
                 {
                     canvas.worldCamera = cam;
                     return true;
                 }

                 return false;
             }) != null)
            {
                break;
            }
        }

        if (canvas.worldCamera == null)
        {
            Camera camera = Resources.Load<Camera>("UICamera");
            if (camera != null)
            {
                Camera newUICam = Instantiate<Camera>(camera);
                newUICam.transform.position = new Vector3(0, 50, 0);
                canvas.worldCamera = newUICam;
            }
        }


        canvas.sortingLayerName = this.SortingLayerName();
    }

    public Vector2 ConvertScaling(ModalObject.ModalScaleEnum scaleType)
    {
        Vector2 scaling = ModalObject.AutoSize;
        RectTransform rt = canvas.transform as RectTransform;
        switch (scaleType)
        {
            case ModalObject.ModalScaleEnum.FullScreen:
                scaling = new Vector2(rt.sizeDelta.x - 100, rt.sizeDelta.y - 100);
                break;

            case ModalObject.ModalScaleEnum.OneThird:
                scaling = new Vector2(rt.sizeDelta.x / 3.0f, rt.sizeDelta.y / 3.0f);
                break;

            case ModalObject.ModalScaleEnum.TwoThird:
                scaling = new Vector2(rt.sizeDelta.x * 2.0f / 3.0f, rt.sizeDelta.y * 2.0f / 3.0f);
                //Debug.Log(scaling);
                break;

            default:
                break;
        }

        return scaling;
    }

    public RectTransform LoadPrefab(string prefabName)
    {
        RectTransform modalItem = null;
        if (!prefabCache.TryGetValue(prefabName, out modalItem))
        {
            modalItem = Resources.Load<RectTransform>(prefabName);
            prefabCache.Add(prefabName, modalItem);
        }

        return modalItem;
    }

    private void OnDestroyModal(RectTransform modalItem, ModalObject modal)
    {
        if (modalItem == null)
        {
            return;
        }

        Destroy(modalItem.gameObject);
        child.Clear();
        //Debug.Log("<color=red>child clear </color>");

        if (modal.StackedPopup != null &&
           modal.StackedPopup.Count > 0)
        {
            while (modal.StackedPopup.Count > 0)
            {
                var pop = modal.StackedPopup.Pop();
                pop.SetParent(blockingCanvas, true);
                pop.SetAsLastSibling();
                child.Add(pop);
                //Debug.Log("<color=red>child pop added </color>" + modalItem);
            }
        }
        else
        {
            OnCompleteDestroy();
        }
    }

    private void _OnShowModal(ModalObject modal)
    {
        modalStack.Push(modal);

        RectTransform canvasrt = canvas.transform as RectTransform;
        // blockingCanvas.sizeDelta = canvasrt.sizeDelta;

        RectTransform modalItem = Instantiate<RectTransform>(modal.BasePrefab);
        modalItem.SetParent(blockingCanvas, false);
        child.Add(modalItem);
        //Debug.Log("<color=red>child added </color>" + modalItem);

        modalItem.GetOrAddComponent<EventBlocker>();

        var cc = modalItem.GetComponentsInChildren<Canvas>(true);
        var pp = modalItem.GetComponentsInChildren<ParticleSystem>(true);

        Observable.NextFrame()
                  .Subscribe(_ =>
        {
            foreach (var i in cc)
            {
                i.sortingOrder += 20001;
                i.sortingLayerName = this.SortingLayerName();
            }

            foreach (var i in pp)
            {
                var r = i.GetComponent<Renderer>();
                r.sortingOrder += 20001;
                r.sortingLayerName = this.SortingLayerName();
            }
        });
        SetupNormalMode(modal.mode == ModalObject.ModalMode.WITH_PARTICLE);
        SetupParticleMode();

        modalItem.gameObject.SetActive(false);

        modal.OnInitCreatorItems();

        if (modal.TopPrefab != null)
        {
            foreach (RectTransform t in modal.TopPrefab)
            {
                t.SetParent(modalItem, false);
                t.SetAsLastSibling();
            }
        }

        var buttonArea = modalItem.GetComponentInChildren<ModalButtonArea>();
        if (buttonArea != null)
        {
            RectTransform buttonArea2 = buttonArea.transform as RectTransform;
            if (modal.BottomPrefab != null)
            {
                foreach (var b in modal.BottomPrefab)
                {
                    b.Item2.SetParent(buttonArea2, false);
                    b.Item2.SetAsLastSibling();

                    var compbtn = b.Item2.GetComponent<ModalComponent>();
                    compbtn.ComponentDismiss = b.Item1.DismissType;

                    if (b.Item1.ChildIndex >= 0)
                    {
                        b.Item2.SetSiblingIndex(b.Item1.ChildIndex);
                    }
                }
            }

            buttonArea2.SetAsLastSibling();
        }

        if (modal.PreferedSizeEnum != ModalObject.ModalScaleEnum.NoScale)
        {
            modal.PreferedSize = ConvertScaling(modal.PreferedSizeEnum);
        }

        Vector2 newSize = modalItem.sizeDelta;
        if (modal.PreferedSize.x >= 0.0f)
        {
            newSize.x = Mathf.Min(modal.PreferedSize.x, canvasrt.sizeDelta.x);
        }

        if (modal.PreferedSize.y >= 0.0f)
        {
            newSize.y = Mathf.Min(modal.PreferedSize.y, canvasrt.sizeDelta.y);
        }

        if (System.Math.Abs(modalItem.sizeDelta.sqrMagnitude - newSize.sqrMagnitude) > 0.0f)
        {
            var contentSizeFitter = modalItem.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter != null) { contentSizeFitter.enabled = false; };
            modalItem.sizeDelta = newSize;
        }

        ModalComponent[] components = modalItem.gameObject.GetComponentsInChildren<ModalComponent>();

        Dictionary<string, ModalComponent> dicto = components.ToDictionary(x => x.ComponentName, x => x);

        // remove items
        if (modal.ComponentToRemove != null)
        {
            ModalComponent item;
            foreach (string n in modal.ComponentToRemove)
            {
                if (dicto.TryGetValue(n, out item))
                {
                    Destroy(item.gameObject);
                    dicto.Remove(n);
                }
            }
        }

        modal.Components = dicto;

        UnityEngine.Events.UnityAction onSuccessCallback = () =>
        {
            StopAllCoroutines();

            if (modal.GetOnDisappear() != null)
            {
                Observable.FromCoroutine((arg) => modal.GetOnDisappear()(modalItem))
                          .Subscribe(_ =>
                {
                    OnDestroyModal(modalItem, modal);

                    if (modal.GetOnClose() != null)
                    {
                        modal.GetOnClose()();
                    }
                });
            }
            else
            {
                OnDestroyModal(modalItem, modal);

                if (modal.GetOnClose() != null)
                {
                    modal.GetOnClose()();
                }
            }

            if (modal.GetOnSuccess() != null)
            {
                modal.GetOnSuccess()(dicto);
            }

        };

        UnityEngine.Events.UnityAction onFailureCallback = () =>
        {
            StopAllCoroutines();

            if (modal.GetOnDisappear() != null)
            {
                Observable.FromCoroutine((arg) => modal.GetOnDisappear()(modalItem))
                          .Subscribe(_ =>
                {
                    OnDestroyModal(modalItem, modal);

                    if (modal.GetOnClose() != null)
                    {
                        modal.GetOnClose()();
                    }
                });
            }
            else
            {
                OnDestroyModal(modalItem, modal);

                if (modal.GetOnClose() != null)
                {
                    modal.GetOnClose()();
                }
            }
            
            if (modal.GetOnCancel() != null)
            {
                modal.GetOnCancel()();
            }
        };

        if (modal.GetOnInit() != null)
        {
            modal.GetOnInit()(dicto, onSuccessCallback, onFailureCallback);
        }

        var dismissCount = 0;
        foreach (ModalComponent c in components)
        {
            switch (c.ComponentDismiss)
            {
                case ModalComponent.ComponentDissmissEnum.None:
                    break;

                case ModalComponent.ComponentDissmissEnum.Success:
                    {
                        ++dismissCount;
                        var successComponent = c;
                        c.GetComponent<Button>()
                         .OnClickAsObservable()
                         .ThrottleFirst(System.TimeSpan.FromMilliseconds(1000))
                         .Subscribe(_ =>
                        {
                            dicto[ModalConstant.PRESSED_KEY] = successComponent;
                            onSuccessCallback();
                            modal.GetOnButtonClick()?.Invoke(c);
                        }).AddTo(c);
                    }
                    break;

                case ModalComponent.ComponentDissmissEnum.Cancel:
                    ++dismissCount;
                    c.GetComponent<Button>()
                        .OnClickAsObservable()
                        .ThrottleFirst(System.TimeSpan.FromMilliseconds(1000))
                        .Subscribe(_ =>
                       {
                           onFailureCallback();
                           modal.GetOnButtonClick()?.Invoke(c);
                       }).AddTo(c);
                    break;

                case ModalComponent.ComponentDissmissEnum.Fail:
                    // PlaceHolder
                    ++dismissCount;
                    break;

                default:
                    break;
            }
        }

        modalItem.gameObject.SetActive(true);

        if (modal.GetOnAppear() != null)
        {
            StartCoroutine(modal.GetOnAppear()(modalItem));
        }
        else
        {
            //playSE
        }

        if (modal.DismissDataItem.after > 0.0f)
        {
            StartCoroutine(DismissAfterSubroutine(modal.DismissDataItem, onSuccessCallback, onFailureCallback));
        }
        if (blockingCanvas != null)
        {
            var btn = blockingCanvas.GetOrAddComponent<Button>();
            // blockingCanvas.GetComponent<Ensign.GUIButton>().IsTriggerAnim = false;
            btn.OnClickAsObservable()
                .ThrottleFirst(System.TimeSpan.FromMilliseconds(1000))
                .Where(_ => modalItem.parent == blockingCanvas)
                .Subscribe(_ =>
                {
                    onSuccessCallback();
                }).AddTo(modalItem.gameObject);

            if (!modal.DontDissmissOnTap)//dismissCount == 1 && 
            {
                btn.enabled = true;
            }
            else
            {
                btn.enabled = false;
            }
        }
        modal.controllerDissmissCallbackSuccess = onSuccessCallback;
        modal.controllerDissmissCallbackCancel = modal.controllerDissmissCallbackFail = onFailureCallback;
    }

    private IEnumerator DismissAfterSubroutine(ModalObject.DissmissData data, UnityEngine.Events.UnityAction onSuccess, UnityEngine.Events.UnityAction onCancel)
    {
        yield return new WaitForSeconds(data.after);

        switch (data.type)
        {
            case ModalComponent.ComponentDissmissEnum.None:
                onSuccess();
                break;

            case ModalComponent.ComponentDissmissEnum.Success:
                onSuccess();
                break;

            case ModalComponent.ComponentDissmissEnum.Cancel:
                onCancel();
                break;

            case ModalComponent.ComponentDissmissEnum.Fail:
                // PlaceHolder
                onCancel();
                break;

            default:
                break;
        }
    }

    public void OnPushModal(ModalObject modal)
    {
        Observable.Timer(System.TimeSpan.FromSeconds(modal.ShowAfterSeconds))
                  .ThrottleFrame(1)
                  .Subscribe(_ =>
                  {
                      if (modal.BasePrefab == null)
                      {
                          return;
                      }

                      if (modal.StackedPopup == null)
                      {
                          return;
                      }

                      if (blockingCanvas == null)
                      {
                          OnShowModal(modal);
                          return;
                      }

                      var stack = modal.StackedPopup;
                      for (int i = child.Count - 1; i >= 0; --i)
                      {
                          var rt = child[i];
                          if (rt != null)
                          {
                              stack.Push(rt);
                              rt.SetParent(blockingCanvas.parent, true);
                              rt.SetAsFirstSibling();
                          }
                          child.RemoveAt(i);
                      }
                      _OnShowModal(modal);
                  });
    }

    private System.IDisposable waitTimer = null;
    public void OnShowModal(ModalObject modal, bool fromQueue = false)
    {
        Observable.NextFrame(FrameCountType.EndOfFrame).Subscribe(unit =>
        {
            if (modal.BasePrefab == null)
            {
                return;
            }

#if QUEUE
            if ((blockingCanvas != null && !fromQueue) || waitTimer != null)
            {
                modalQueue.Enqueue(modal);
                return;
            }
#endif

#if ENABLE_BLUR
    blurMat = blurMat ?? Instantiate<Material>(Resources.Load<Material>(ModalCreatorPrefabsName.UiBlurMaterial));
#endif

            RectTransform canvasrt = canvas.transform as RectTransform;

            // blocking image
            if (blockingCanvas == null)
            {
                canvas.enabled = true;
                canvas.sortingLayerName = this.SortingLayerName();

                //addButton = true;
                blockingCanvas = new GameObject("BlockingAgent", typeof(Image)).GetComponent<RectTransform>();
                //blockingCanvas = Instantiate<RectTransform>(Resources.Load<RectTransform>("UI/BlockingAgent"), canvas.gameObject.GetComponent<RectTransform>());
                if (modal.mode == ModalObject.ModalMode.WITH_PARTICLE)
                {
                    this.gameObject.layer = LayerMask.NameToLayer("UI");
                    blockingCanvas.gameObject.layer = LayerMask.NameToLayer("UI");
                }
                blockingCanvas.GetComponent<Image>().color = blockingCanvasColor;
                blockingCanvas.SetParent(canvas.gameObject.transform, false);
                blockingCanvas.sizeDelta = canvasrt.sizeDelta;
#if ENABLE_BLUR
        blockingCanvas.GetComponent<Image>().material = blurMat;

        blurMat.SetFloat("_Range", 0.0f);
        blurMat.DOFloat(BlurAmount, "_Range", 0.3f);
#endif
            }

            waitTimer = Observable.Timer(System.TimeSpan.FromSeconds(modal.ShowAfterSeconds))
                        .ThrottleFrame(1)
                        .Subscribe(_ =>
                        {
                            _OnShowModal(modal);
                            waitTimer = null;
                        });
        });
    }

    public void OnCompleteDestroy()
    {
        if (blockingCanvas == null)
        {
            return;
        }

#if QUEUE
        if (modalQueue.Count != 0)
        {
            ModalObject q = modalQueue.Dequeue() as ModalObject;
            OnShowModal(q, true);
            return;
        }
#else
        if (blockingCanvas.childCount > 1)
        {
            return;
        }
#endif

#if ENABLE_BLUR
        var tweener = blurMat.DOFloat(0.0f, "_Range", 0.3f)
                        .OnComplete(() =>
                        {
                            Destroy(blockingCanvas.gameObject);
                            blockingCanvas = null;
                        });

        tweener.OnUpdate(() =>
                         {
                             if (modalQueue.Count != 0)
                             {
                                 tweener.Kill(true);
                                 ModalObject q = modalQueue.Dequeue() as ModalObject;
                                 OnShowModal(q, true);
                             }
                         });
#else
        Destroy(blockingCanvas.gameObject);
        blockingCanvas = null;
        canvas.enabled = false;

        if (modalQueue.Count != 0)
        {
            ModalObject q = modalQueue.Dequeue() as ModalObject;
            OnShowModal(q, true);
        }
#endif

    }

    public bool IsEmpty()
    {
        return blockingCanvas == null && modalQueue.Count == 0 && waitTimer == null;
    }

    public bool IsStackEmpty()
    {
        return modalStack.Count == 0 && waitTimer == null;
    }

    public ModalObject Peek()
    {
        return modalStack.Count > 0 ? modalStack.Pop() as ModalObject : null;
    }

    public void SetBlockingColor(Color color)
    {
        if (blockingCanvas != null)
        {
            blockingCanvas.GetComponent<Image>().color = color;
        }
    }

    public void DisableBlocking()
    {
        System.IDisposable timer = null;
        timer = Observable.Timer(System.TimeSpan.FromSeconds(0.1f))
            .Subscribe(_ =>
            {
                if (blockingCanvas != null)
                {
                    blockingCanvas.GetComponent<Image>().enabled = false;
                }
                timer = null;
            });
    }
}
