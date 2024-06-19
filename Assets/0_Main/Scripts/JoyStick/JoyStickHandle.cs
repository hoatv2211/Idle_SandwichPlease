using System;
using System.Collections;
using System.Collections.Generic;
using ThePattern;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickHandle : MonoBehaviour
{
    [Space, Header("UI")]
    [SerializeField] private RectTransform _root;
    [SerializeField] private RectTransform _center;
    [SerializeField] private RectTransform _knob;
    [Space, Header("DATA CONFIG")]
    [SerializeField] private float _range;
    [SerializeField] private bool _isFixedJoystick;

    private bool _isTouchStart = false;
    private Vector3 _touchStartPos;
    private Action<Vector2> _onChangedDir;
    private DataBind<Vector2> _dir = new DataBind<Vector2>(Vector2.zero);
    public bool isTouching => _isTouchStart;

    private void Start()
    {
        SetActiveJoystick(false);
        _dir.RegisterNotify(v =>
        {
            _onChangedDir?.Invoke(v);
        });
    }
    private void Update()
    {

        Vector2 pos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            OnTouchStart(pos);
            return;
        }
        if (Input.GetMouseButton(0))
        {
            OnTouchMove(pos);
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnTouchEnd(pos);
            return;
        }
    }

    #region Joystick Method
    private void OnTouchStart(Vector2 mousePos)
    {
        if (!MapController.Instance.cameraCtrl.IsCanMove)
            return;

        if (IsPointerOverUIObject(mousePos))
            return;

        _isTouchStart = true;
        SetActiveJoystick(true);
        _touchStartPos = mousePos;

        _knob.position = mousePos;
        _center.position = mousePos;

      
    }
    private void OnTouchMove(Vector2 mousePos)
    {
        if (!MapController.Instance.cameraCtrl.IsCanMove)
            return;

        if (Module.isFirstHand == 0)
        {
            Module.isFirstHand = 1;
            UIMainGame.Instance.tut_hand.gameObject.SetActive(false);
        }


        if (!_isTouchStart)
        {
            _dir.Value = Vector2.zero;
            return;
        }
        _knob.position = mousePos;
        _knob.position = _center.position + Vector3.ClampMagnitude(_knob.position - _center.position, _center.sizeDelta.x * _range);

        if (_knob.position != Input.mousePosition && !_isFixedJoystick)
        {
            Vector3 outsideBoundsVector = Input.mousePosition - _knob.position;
            //_center.position += outsideBoundsVector;
        }
        //_dir.Value = (_knob.position - _center.position).normalized;
        _dir.Value = (_knob.position - _center.position).normalized;
    }
    public void OnTouchEnd(Vector2 mousePos)
    {
        _isTouchStart = false;
        SetActiveJoystick(false);
        _dir.Value = Vector2.zero;
    }

    public void StopWhenTut()
    {
        _dir.Value = Vector2.zero;
    }

    public void SetActiveJoystick(bool isActive)
    {
        _root.gameObject.SetActive(isActive);
    }
    #endregion


    private void OnApplicationFocus(bool focus)
    {
        OnTouchEnd(Vector2.zero);
    }

    public void RegisterListenInput(Action<Vector2> onChangedDir)
    {
        _onChangedDir += onChangedDir;
    }

    public void ActiongAdsShow()
    {
        _dir.Value = Vector2.zero;
        SetActiveJoystick(false);
    }

    public bool IsPointerOverUIObject(Vector2 touchPosition)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = touchPosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (!raycastResultList[i].gameObject.activeInHierarchy)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

     
        return raycastResultList.Count > 0;
    }
}