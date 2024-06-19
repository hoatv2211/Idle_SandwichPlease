using UnityEngine;
using DG.Tweening;

public class SceneTransitionRoot : MonoBehaviour 
{
    private readonly float BASE_TRANSITION = 0.3f;
    private readonly float BASE_X_MOVE = 2100.0f;
    private readonly float BASE_Y_MOVE = 1200.0f;

    public enum TRANSITION_TYPE
    {
        NONE,
        RIGHT_TO_CENTER,
        CENTER_TO_RIGHT,
        LEFT_TO_CENTER,
        CENTER_TO_LEFT,
        BOTTOM_TO_CENTER,
        CENTER_TO_BOTTOM,
        UP_TO_CENTER,
        CENTER_TO_UP,
    }

    private delegate void TransitionFunc();
    private delegate void TransitionFuncWithCB(System.Action cb);
    private System.Collections.Generic.Dictionary<TRANSITION_TYPE, TransitionFunc> sceneIn = new System.Collections.Generic.Dictionary<TRANSITION_TYPE, TransitionFunc>();
    private System.Collections.Generic.Dictionary<TRANSITION_TYPE, TransitionFuncWithCB> sceneOut = new System.Collections.Generic.Dictionary<TRANSITION_TYPE, TransitionFuncWithCB>();

    public void Awake()
    {
       
    }

    public bool SceneIn(TRANSITION_TYPE type)
    {
        TransitionFunc trans = null;
        if (sceneIn.TryGetValue(type, out trans))
        {
            trans();
            return true;
        }

        return false;
    }

    public bool SceneOut(TRANSITION_TYPE type, System.Action cb)
    {
        TransitionFuncWithCB trans = null;
        if (sceneOut.TryGetValue(type, out trans))
        {
            trans(cb);
            return true;
        }

        return false;
    }
    /*

    private void RightToCenter()
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            Vector3 from = new Vector3(base_rect.localPosition.x, base_rect.localPosition.y, base_rect.localPosition.z);
            float to_x = from.x;
            from.x += BASE_X_MOVE;
            base_rect.localPosition = from;
            base_rect.DOMoveX(to_x, BASE_TRANSITION);
        }
    }

    private void CenterToRight(TweenCallback cb)
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            float to_x = BASE_X_MOVE;
            base_rect.DOLocalMoveX(to_x, BASE_TRANSITION).OnComplete(cb);
        }
    }

    private void LeftToCenter()
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            Vector3 from = new Vector3(base_rect.localPosition.x, base_rect.localPosition.y, base_rect.localPosition.z);
            float to_x = from.x;
            from.x -= BASE_X_MOVE;
            base_rect.localPosition = from;
            base_rect.DOMoveX(to_x, BASE_TRANSITION);
        }       
    }

    private void CenterToLeft(TweenCallback cb)
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            float to_x = -BASE_X_MOVE;
            base_rect.DOLocalMoveX(to_x, BASE_TRANSITION).OnComplete(cb);
        }
    }

    private void BottomToCenter()
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            Vector3 from = new Vector3(base_rect.localPosition.x, base_rect.localPosition.y, base_rect.localPosition.z);
            float to_y = from.y;
            from.y += BASE_Y_MOVE;
            base_rect.localPosition = from;
            base_rect.DOMoveY(to_y, BASE_TRANSITION);
        }
    }

    private void CenterToBottom(TweenCallback cb)
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            float to_y = -BASE_Y_MOVE;
            base_rect.DOLocalMoveY(to_y, BASE_TRANSITION).OnComplete(cb);
        }
    }

    private void UpToCenter()
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            Vector3 from = new Vector3(base_rect.localPosition.x, base_rect.localPosition.y, base_rect.localPosition.z);
            float to_y = from.y;
            from.y -= BASE_Y_MOVE;
            base_rect.localPosition = from;
            base_rect.DOMoveY(to_y, BASE_TRANSITION);
        }
    }

    private void CenterToUp(TweenCallback cb)
    {
        var base_rect = GetComponent<RectTransform>();
        if (base_rect != null)
        {
            float to_y = BASE_Y_MOVE;
            base_rect.DOLocalMoveY(to_y, BASE_TRANSITION).OnComplete(cb);
        }
    }
    */
}
