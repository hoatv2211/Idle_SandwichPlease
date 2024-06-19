using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxRewardBattle : MonoBehaviour
{
    public Reward_Info reward_Info;
    [SerializeField] private TextMeshProUGUI txtValue;
    [SerializeField] private Sprite[] sprRewards;
    [SerializeField] private Image imgIcon;

    [SerializeField] private UIButton btnClaim;
    [SerializeField] private GameObject objLocked;
    [SerializeField] private GameObject objRewarded;
    
    public void CallStart(Reward_Info _info,bool _isUnLocked =false)
    {
        reward_Info = _info;
        txtValue.text = reward_Info.value.ToString();

        switch (reward_Info.type)
        {
            case ETypeReward.money:
                imgIcon.sprite = sprRewards[0];
                break;
            case ETypeReward.ticket:
                imgIcon.sprite = sprRewards[1];
                break;
            case ETypeReward.skin:
                break;
            default:
                break;
        }

        bool _isRewarded = UserModel.Instance.isRewarded(reward_Info.id);
        objLocked.SetActive(!_isUnLocked);
        objRewarded.SetActive(_isRewarded);

        btnClaim.enabled = !_isRewarded && _isUnLocked;
        btnClaim.SetUpEvent(Action_btnClaim);
    }

    private void Action_btnClaim()
    {
        UserModel.Instance.Action_Reward(reward_Info);
        btnClaim.enabled = false;
        objRewarded.SetActive(true);

        switch (reward_Info.type)
        {
            case ETypeReward.money:
                Module.money_currency += reward_Info.value;
                break;
            case ETypeReward.ticket:
                Module.ticket_currency += reward_Info.value;
                break;
            case ETypeReward.skin:
                break;
            default:
                break;
        }
    }
}
