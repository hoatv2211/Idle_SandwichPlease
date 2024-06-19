using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] private float _smoothness;
    [SerializeField] private float _newTargetSmoothness = 1f;
    [SerializeField] private Vector3 _offset;

    private Camera _mainCam;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;
    private Vector3 _velocity;
    public Transform Target => _target;
    private Camera MainCam
    {
        get
        {
            if (!_mainCam)
            {
                _mainCam = Camera.main;
            }
            return _mainCam;
        }
    }
    private float _currentSmoothness;

    void LateUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartCoroutine(IEShowDoor());
        //}

        if (_target!=_player||!isFlow)
            return;

        Vector3 targetPos = GetCamPosOnTarget(_target);
        MainCam.transform.position = Vector3.SmoothDamp(MainCam.transform.position, targetPos, ref _velocity, _currentSmoothness);
    }


    public void SetPlayer(Transform player)
    {
        _player = player;
    }

    public float SetCamTarget(Transform target, float time = 2f)
    {
        _target = target;
        _currentSmoothness = _newTargetSmoothness;
        float timeToMove = Mathf.Abs(_smoothness - _newTargetSmoothness) / time * 2;
        StartCoroutine(IeChangeSmoothness(timeToMove));
        if (_target != _player)
        {
            DOVirtual.DelayedCall(time, () => { SetCamTarget(_player); });
        
        }
        return timeToMove;
    }
    public void SetCamTargetNoAction(Transform target)
    {
        _target = target;
        if (target != null)
        {
            _currentSmoothness = _newTargetSmoothness;
            StartCoroutine(IeChangeSmoothness(Mathf.Abs(_smoothness - _newTargetSmoothness) / 2));
        }
    }

    bool isFlow = true;
    public bool IsCanMove => isFlow;
    bool isFocus = false;
    public void SetCamTargetFocusNoAction(Transform target)
    {
        if (isFocus)
            return;
        isFocus = true;
        // Get the object's position in the viewport.
        Vector3 viewPos = Camera.main.WorldToViewportPoint(target.transform.position);
        // Check if the object is within the view.
        if (viewPos.x > 0 && viewPos.x < 1 && viewPos.y > 0 && viewPos.y < 1 && viewPos.z > 0)
        {
            Debug.Log("Object is within the camera's view.");
            isFocus = false;
        }
        else
        {
            Debug.Log("Object is outside the camera's view.");
            _target = target;
            //MapController.Instance.joyStick.OnTouchEnd(Vector2.zero);
            MapController.Instance.joyStick.StopWhenTut();
            StartCoroutine(IEFocusNewTarget());
        }
     
    }
    IEnumerator IEFocusNewTarget()
    {
        isFlow = false;
        yield return new WaitUntil(() => UIMainGame.Instance.m_UILevelUp.gameObject.activeInHierarchy == false);
        yield return new WaitForSeconds(1f);

        transform.DOMove(GetCamPosOnTarget(_target), 2);

        yield return null;
        yield return new WaitForSeconds(2f);

        _target = _player;
        transform.DOMove(GetCamPosOnTarget(_target), 2);
        yield return new WaitForSeconds(2.1f);

        isFlow = true;
        isFocus = false;
    }

    IEnumerator IeChangeSmoothness(float speed)
    {
        while (_currentSmoothness > _smoothness)
        {
            if (!isFlow)
                break;

            _currentSmoothness -= Time.deltaTime * speed;
            yield return null;
        }
        _currentSmoothness = _smoothness;
    }

    private Vector3 GetCamPosOnTarget(Transform target)
    {
        return target.position + _offset;
    }

    Action callback;
    public void ActionShowCamDoor(Action _callback)
    {
        callback = _callback;
        StartCoroutine(IEShowDoor());
    }

    IEnumerator IEShowDoor()
    {
        isFlow = false;
        Vector3 trBase = this.transform.position;
        Quaternion roBase = this.transform.rotation;
        UIMainGame.Instance.Show_TextTutHelper("Welcome!!!");
        yield return new WaitForSeconds(1f);
        _player.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        DOVirtual.Float(10, 5, 1, (x) => { MainCam.orthographicSize = x; });
        transform.DOMove(new Vector3(-16, 2, -17), 1f);
        transform.DORotate(new Vector3(60, 45, 0), 1f);

        yield return new WaitForSeconds(1.2f);
        transform.DOMove(new Vector3(-16,5, -17), 1f);
        transform.DORotate(new Vector3(30, 0, 0), 1f);
        _player.GetComponent<PlayerCtrl>().ShowAnimWellcome();

        yield return new WaitForSeconds(3f);
        _player.GetComponent<PlayerCtrl>().HideAnimWellcome();
        transform.DOMove(trBase, 1f);
        transform.DORotate(roBase.eulerAngles,1);
        DOVirtual.Float(5, 10, 1, (x) => { MainCam.orthographicSize = x; });
        yield return new WaitForSeconds(1f);
        isFlow = true;
        callback?.Invoke();
    }

}
