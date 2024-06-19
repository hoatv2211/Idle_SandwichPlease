using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupStoreClaim : MonoBehaviour
{
    public int idMap = 1;
    [SerializeField] private UIButton btnExit;
    [SerializeField] private UIButton btnEnter;
    [SerializeField] private UIButton btnAds;

    [SerializeField] private TextMeshProUGUI txtProcess;
    [SerializeField] private TextMeshProUGUI txtMoneyClaim;
    [SerializeField] private TextMeshProUGUI txtEmploy;
    [SerializeField] private TextMeshProUGUI txtIncome;
    [SerializeField] private TextMeshProUGUI txtTitle;

    [SerializeField] private Image imgProcess;
    [SerializeField] private Image imgIcon;
    [SerializeField] private List<Sprite> sprIcons;

    public void CallStart(int _idMap)
    {
        idMap = _idMap;
        switch (_idMap)
        {
            case 1:
                imgIcon.sprite = sprIcons[0];
                txtTitle.text = "Subday Sandwich";
                break;
            case 2:
                imgIcon.sprite = sprIcons[1];
                txtTitle.text = "Which Wich";
                break;
            case 3:
                imgIcon.sprite = sprIcons[2];
                break;
            default:
                imgIcon.sprite = sprIcons[0];
                break;
        }

        MapData mapData = GameManager.Instance.GetMapModel(idMap).mapData;
        txtProcess.text = mapData.crProcess.ToString("00") +"/100";
        txtEmploy.text = mapData.upgradeStaff.crlvEmploy().value.ToString();
        imgProcess.fillAmount = mapData.crProcess / 100f;

        btnExit.SetUpEvent(Action_btnExit);
        btnEnter.SetUpEvent(Action_btnEnter);
    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    private void Action_btnEnter()
    {
        gameObject.SetActive(false);
        Module.LoadScene(idMap);
    }
}
