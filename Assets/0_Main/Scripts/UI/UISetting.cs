using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;

    [SerializeField] private Toggle togMusic;
    [SerializeField] private GameObject onMusic;
    [SerializeField] private GameObject offMusic;


    [SerializeField] private Toggle togVibrate;
    [SerializeField] private GameObject onVibrate;
    [SerializeField] private GameObject offVibrate;

    public void CallStart()
    {
        btnExit.SetUpEvent(Action_btnExit);


        togMusic.onValueChanged.RemoveAllListeners();
        togVibrate.onValueChanged.RemoveAllListeners();

        togMusic.onValueChanged.AddListener((x)=>Action_ChangeMusic(x));
        togVibrate.onValueChanged.AddListener((x)=> Action_ChangeVibrate(x));

        togMusic.isOn = Module.musicFx == 1;
        togVibrate.isOn = Module.vibrationFx == 1;
        Action_ChangeMusic(togMusic.isOn);
        Action_ChangeVibrate(togVibrate.isOn);
    }

    public void Action_btnExit()
    {
        gameObject.SetActive(false);
        FirebaseManager.Instance.LogEvent_click_button("close_setting");
    }

    public void Action_ChangeMusic(bool _value)
    {
        onMusic.SetActive(_value);
        offMusic.SetActive(!_value);

        if (_value)
        {
            Module.musicFx = 1;           
        }
        else
        {
            Module.musicFx = 0;
        }

        Module.Action_ChangeMusic();
    }

    public void Action_ChangeVibrate(bool _value)
    {
        onVibrate.SetActive(_value);
        offVibrate.SetActive(!_value);

        if (_value)
        {
            Module.vibrationFx = 1;
        }
        else
        {
            Module.vibrationFx = 0;
        }
    }
}
