using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINoInternet : MonoBehaviour
{
    [SerializeField] private UIButton btnRetry;
    private void OnEnable()
    {
        btnRetry.SetUpEvent(Action_btnRetry);
    }

    private void Action_btnRetry()
    {
        if (Module.isNetworking())
        {
            gameObject.SetActive(false);
            FirebaseManager.Instance.LogEvent_InternetClickRetry();
        }
    }
}
