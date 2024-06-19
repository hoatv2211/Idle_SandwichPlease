using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    [SerializeField] private Transform _billBoardUI;

    [SerializeField] private Camera _mainCam;
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

    private Transform billBoadUI
    {
        get
        {
            if (!_billBoardUI)
            {
                _billBoardUI = this.transform;
            }

            return _billBoardUI;
        }
    }


    private void LateUpdate()
    {
        _billBoardUI?.LookAt(new Vector3(MainCam.transform.position.x, MainCam.transform.position.y, MainCam.transform.position.z));
         billBoadUI?.LookAt(MainCam.transform.position);
    }
}