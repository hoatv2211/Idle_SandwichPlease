using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;

    public void CallStart()
    {
        btnExit.SetUpEvent(Action_btnExit);
    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
        FirebaseManager.Instance.LogEvent_click_button("close_shop");
    }


}
