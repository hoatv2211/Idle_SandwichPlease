using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskViewBattle : EnhancedScrollerCellView
{
    public Level_User levelInfos;

    [SerializeField] private TextMeshProUGUI txtLevel;

    [SerializeField] private Sprite[] sprLevels;
    [SerializeField] private Color32[] colors;
    [SerializeField] private Image imgSlider1;
    [SerializeField] private Image imgSlider2;
    [SerializeField] private Image imgLevel;

    [Header("Reward")]
    public BoxRewardBattle box_free;
    public BoxRewardBattle box_manager;
    public BoxRewardBattle box_ceo;

    public void CallStart(Level_User _lvInfos)
    {
        levelInfos = _lvInfos;

        txtLevel.text = levelInfos.level.ToString();

        if (UserModel.Instance.level > levelInfos.level)
        {
            imgSlider1.color = colors[1];
            imgSlider2.color = colors[1];
            imgLevel.sprite = sprLevels[1];
        }
        else if(UserModel.Instance.level == levelInfos.level)
        {
            imgSlider1.color = colors[1];
            imgSlider2.color = colors[0];
            imgLevel.sprite = sprLevels[1];
        }
        else
        {
            imgSlider1.color = colors[0];
            imgSlider2.color = colors[0];
            imgLevel.sprite = sprLevels[0];
        }

        box_free.CallStart(levelInfos.reward_free, levelInfos.level<=UserModel.Instance.level);
        box_manager.CallStart(levelInfos.reward_manager, levelInfos.level <= UserModel.Instance.level && UserModel.Instance.isManagerSub);
        box_ceo.CallStart(levelInfos.reward_ceo, levelInfos.level <= UserModel.Instance.level && UserModel.Instance.isCEOSub);
    }
}
