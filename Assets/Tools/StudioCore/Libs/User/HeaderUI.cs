using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using DG.Tweening;
[Prefab(name:"UI/MainUI/HeaderUI", persistent: true)]
public class HeaderUI : SingletonMonoBehaviour<HeaderUI>
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] protected Text txtGem;
    [SerializeField] protected Text txtCoin;
  
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        /*
        UserManager.IData.GoldPoint.Subscribe((val) =>
        {
            var old = UserManager.IData.EssenceLockPoint.OldValue;
            LeanTween.value(txtEnergy.gameObject,
                x => txtEnergy.text = x.FormatNumber(999999),
                old, val, 2);
                
        }).AddTo(txtEnergy.gameObject);
      */
    }

    public void SetActive (bool val){
        canvasGroup.gameObject.SetActive(val);
    }
}
